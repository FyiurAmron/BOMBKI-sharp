using System.Runtime.InteropServices;
using System.Text;

namespace Bombki.Tests;

// Test-side stdin feeder. The game reads fd 0 directly (see Crt), so
// scripted commands reach it through a pipe duped onto fd 0: the same
// bytes a terminal would deliver, echo included. Everything lives here;
// production carries no test hook. Unix only, like the PTY scenarios:
// the Windows driver reads console keys, not stdin.
internal sealed class TestStdin : IDisposable
{
    [DllImport("libc", SetLastError = true)]
    private static extern int pipe(int[] fds);

    [DllImport("libc", SetLastError = true)]
    private static extern int dup(int fd);

    [DllImport("libc", SetLastError = true)]
    private static extern int dup2(int oldfd, int newfd);

    [DllImport("libc", SetLastError = true)]
    private static extern int close(int fd);

    [DllImport("libc", SetLastError = true)]
    private static extern nint write(int fd, byte[] buf, nuint count);

    private readonly int _saved;
    private readonly int _writeEnd;
    private bool _disposed;

    private TestStdin(int saved, int writeEnd)
    {
        _saved = saved;
        _writeEnd = writeEnd;
    }

    // Feeds '|' separated lines (each newline-terminated, like the
    // Pascal unit tests' input files). Restores fd 0 on dispose.
    public static TestStdin Feed(string lines)
    {
        int saved = dup(0);
        int[] fds = new int[2];
        if (pipe(fds) != 0)
            throw new InvalidOperationException("stdin pipe failed");
        if (dup2(fds[0], 0) != 0)
            throw new InvalidOperationException("stdin dup2 failed");
        close(fds[0]);
        byte[] bytes = Encoding.Latin1.GetBytes(
            lines.Replace("|", "\n", StringComparison.Ordinal) + "\n");
        if (write(fds[1], bytes, (nuint)bytes.Length) != (nint)bytes.Length)
            throw new InvalidOperationException("stdin feed failed");
        return new TestStdin(saved, fds[1]);
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        close(_writeEnd);
        if (_saved >= 0)
        {
            dup2(_saved, 0);
            close(_saved);
        }
    }
}
