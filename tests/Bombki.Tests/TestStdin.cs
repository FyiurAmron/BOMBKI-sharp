using System.Runtime.InteropServices;
using System.Text;

namespace Bombki.Tests;

internal class TestStdin : IDisposable {
    struct PipeEnds {
        public int readEnd;
        public int writeEnd;
    }

    [DllImport("libc", SetLastError = true)]
    static extern int pipe(out PipeEnds ends);

    [DllImport("libc", SetLastError = true)]
    static extern int dup(int fd);

    [DllImport("libc", SetLastError = true)]
    static extern int dup2(int oldfd, int newfd);

    [DllImport("libc", SetLastError = true)]
    static extern int close(int fd);

    [DllImport("libc", SetLastError = true)]
    static extern nint write(int fd, byte[] buf, nuint count);

    readonly int _saved;
    readonly int _writeEnd;
    bool _disposed;

    TestStdin(int saved, int writeEnd) {
        _saved = saved;
        _writeEnd = writeEnd;
    }

    public static TestStdin Feed(string lines) {
        int saved = dup(0);
        if (pipe(out PipeEnds ends) != 0)
            throw new InvalidOperationException("stdin pipe failed");
        int readEnd = ends.readEnd;
        int writeEnd = ends.writeEnd;
        if (dup2(readEnd, 0) != 0)
            throw new InvalidOperationException("stdin dup2 failed");
        close(readEnd);
        var bytes = Encoding.Latin1.GetBytes($"{lines.Replace("|", "\n", StringComparison.Ordinal)}\n");
        if (write(writeEnd, bytes, (nuint)bytes.Length) != (nint)bytes.Length)
            throw new InvalidOperationException("stdin feed failed");
        return new(saved, writeEnd);
    }

    public void Dispose() {
        if (_disposed)
            return;
        _disposed = true;
        close(_writeEnd);
        if (_saved >= 0) {
            dup2(_saved, 0);
            close(_saved);
        }
    }
}
