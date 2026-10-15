using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

// Minimal host for the Pascal `crt` unit habits used by the game.
// All I/O goes straight to fds 0/1: System.Console would emit its own
// terminal init escapes on first use, which the reference never has.
// Input stays in the inherited canonical+echo mode, whose observable
// echo matches the reference transcript byte for byte.

static class Crt
{
    private static Stream? _stdin;
    private static Stream Stdin
    {
        get { return _stdin ??= new FileStream(new SafeFileHandle(0, false), FileAccess.Read, 1); }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PollFd
    {
        public int fd;
        public short events;
        public short revents;
    }

    [DllImport("libc", SetLastError = true)]
    private static extern int poll([In, Out] PollFd[] fds, uint nfds, int timeout);

    [StructLayout(LayoutKind.Sequential)]
    private struct Termios
    {
        public uint c_iflag;
        public uint c_oflag;
        public uint c_cflag;
        public uint c_lflag;
        public byte c_line;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] c_cc;
        public uint c_ispeed;
        public uint c_ospeed;
    }

    [DllImport("libc", SetLastError = true)]
    private static extern int tcgetattr(int fd, out Termios termios);

    [DllImport("libc", SetLastError = true)]
    private static extern int tcsetattr(int fd, int action, ref Termios termios);

    private const uint ICANON = 0x0002;
    private const uint ECHO = 0x0008;
    private const uint ISIG = 0x0001;
    private const uint OPOST = 0x0001;

    internal static readonly bool IsWindows = OperatingSystem.IsWindows();

    // Test knob, default 1 (real Pascal timing). BOMBKI_DELAY_MULT scales
    // every Delay; 0 skips sleeping entirely, like the FPC dev build's
    // Delay stub. Game logic never sees it.
    private static readonly double DelayMult = ReadDelayMult();

    private static double ReadDelayMult()
    {
        string? raw = Environment.GetEnvironmentVariable("BOMBKI_DELAY_MULT");
        if (raw is not null && double.TryParse(raw,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out double mult)
            && double.IsFinite(mult) && mult >= 0)
            return mult;
        return 1;
    }

    // Like the CRT unit init: raw byte reads, no kernel echo, no output
    // postprocessing (newlines are emitted literally). Line echo is
    // done by ReadLn itself, exactly like the reference. Unix only:
    // the Windows driver needs no terminal setup.
    static Crt()
    {
        if (IsWindows)
            return;
        if (tcgetattr(0, out Termios saved) == 0)
        {
            Termios raw = saved;
            raw.c_lflag &= ~(ICANON | ECHO | ISIG);
            raw.c_oflag &= ~OPOST;
            raw.c_cc[6] = 1;
            raw.c_cc[5] = 0;
            if (tcsetattr(0, 0, ref raw) == 0)
            {
                Termios restore = saved;
                AppDomain.CurrentDomain.ProcessExit += (_, _) => tcsetattr(0, 0, ref restore);
            }
        }
    }

    // Probed FPC bytes on a terminal (dotnet Console.Clear emits a
    // longer, different sequence plus a newline, so emit raw here).
    public static void ClrScr() => Out.Write("\x1b[H\x1b[2J");
    public static void Sound(int tone) { }
    public static void NoSound() { }

    // FPC CRT TextColor emits state-dependent ANSI deltas. The table below
    // is the probed behavior for every color pair the game uses (4, 14, 15
    // from the default state); state 7 resets to default.
    private static int _colorState;
    private static readonly Dictionary<(int, int), string> ColorDeltas = new()
    {
        [(0, 4)] = "\x1b[31m",
        [(0, 14)] = "\x1b[1;33m",
        [(0, 15)] = "\x1b[1m",
        [(4, 0)] = "\x1b[m",
        [(4, 14)] = "\x1b[0;1;33m",
        [(4, 15)] = "\x1b[0;1m",
        [(14, 0)] = "\x1b[m",
        [(14, 4)] = "\x1b[0;31m",
        [(14, 15)] = "\x1b[0;1m",
        [(15, 0)] = "\x1b[m",
        [(15, 4)] = "\x1b[0;31m",
        [(15, 14)] = "\x1b[0;1;33m",
    };

    public static void TextColor(int color)
    {
        if (color == 7)
            color = 0;
        if (color == _colorState)
            return;
        if (!ColorDeltas.TryGetValue((_colorState, color), out string? delta))
            throw new InvalidOperationException($"unprobed color transition {_colorState}->{color}");
        _colorState = color;
        Out.Write(delta);
    }

    // Probed FPC mapping for inputs 0..23 (no scenario sends anything
    // else): 0 and 16 emit nothing, 8 resets, the rest select
    // background N mod 8. Stateless: repeats re-emit.
    public static void TextBackground(int color)
    {
        int bits = color & 31;
        if (bits == 0 || bits > 23)
            return;
        if (bits == 8)
            Out.Write("\x1b[m");
        else
            Out.Write($"\x1b[0;4{bits % 8}m");
    }

    // FPC ReadLn(integer): skips blank space, reads an optional sign and
    // digits. Non-numeric input halts the reference with an I/O error;
    // no scenario covers that, so this yields 0 there (audit later).
    public static short ReadShort()
    {
        string line = ReadLn().Trim();
        if (short.TryParse(line, out short value))
            return value;
        return 0;
    }
    public static void Delay(int ms)
    {
        int scaled = (int)(ms * DelayMult);
        if (scaled > 0)
            Thread.Sleep(scaled);
    }

    public static bool KeyPressed() =>
        IsWindows ? WindowsTerminal.KeyPressed() : UnixKeyPressed();

    private static bool UnixKeyPressed()
    {
        PollFd[] fds = { new() { fd = 0, events = 1 } };
        return poll(fds, 1, 0) > 0;
    }

    public static char ReadKey() =>
        IsWindows ? WindowsTerminal.ReadKeyRaw() : UnixReadKey();

    private static char UnixReadKey()
    {
        int b = Stdin.ReadByte();
        if (b == 27)
        {
            ConsumeEscape();
            return '\0';
        }
        return b < 0 ? '\0' : (char)b;
    }

    private static int TryReadByte(int timeoutMs)
    {
        PollFd[] fds = { new() { fd = 0, events = 1 } };
        if (poll(fds, 1, timeoutMs) <= 0)
            return -1;
        return Stdin.ReadByte();
    }

    // Swallow one terminal escape sequence or Alt combo, echoing and
    // buffering nothing - the keyboard layer feeds those to ReadLn as
    // unknowns, which drops them the same way.
    private static void ConsumeEscape()
    {
        int second = TryReadByte(50);
        if (second < 0)
            return;
        if (second == '[')
        {
            while (true)
            {
                int c = TryReadByte(50);
                if (c < 0)
                    return;
                if (c >= 0x40 && c <= 0x7E)
                    return;
            }
        }
    }

    // Shared echo primitives (both drivers echo like the CRT does).
    public static void EchoChar(char c) => Out.Write(c.ToString());
    public static void EchoNewLine() => Out.Write("\r\n");
    public static void EchoErase() => Out.WriteRaw("\b \b");

    public static string ReadLn() =>
        IsWindows ? WindowsTerminal.ReadLine() : UnixReadLn();

    private static string UnixReadLn()
    {
        StringBuilder sb = new();
        int b;
        while ((b = Stdin.ReadByte()) >= 0 && b != '\n')
        {
            if (b == 27)
            {
                ConsumeEscape();
            }
            else if (b == 8 || b == 127)
            {
                if (sb.Length > 0)
                {
                    sb.Length--;
                    EchoErase();
                }
            }
            else
            {
                sb.Append((char)b);
                EchoChar((char)b);
            }
        }
        EchoNewLine();
        return sb.ToString();
    }
}

// Plain terminal output. The game prints text lines terminated with
// CR LF and a few real console actions (clear, colors); cursor motion
// for line advancement is the terminal's own business, not ours.

static class Out
{
    private static readonly Encoding Latin1 = Encoding.Latin1;
    private static readonly Stream Stdout = Crt.IsWindows
        ? Console.OpenStandardOutput()
        : new FileStream(new SafeFileHandle(1, false), FileAccess.Write);

    // Direct emit, no interpretation: colors, erase echo, control bytes.
    public static void WriteRaw(string value)
    {
        byte[] bytes = Latin1.GetBytes(value);
        Stdout.Write(bytes, 0, bytes.Length);
        Stdout.Flush();
    }

    public static void Write(string value) => WriteRaw(value);

    public static void WriteLine(string value) => WriteRaw(value + "\r\n");

    public static void WriteLine() => WriteRaw("\r\n");
    public static void Write(params object?[] parts) => WriteRaw(string.Concat(parts));
    public static void WriteLine(params object?[] parts) => WriteRaw(string.Concat(parts) + "\r\n");
}
