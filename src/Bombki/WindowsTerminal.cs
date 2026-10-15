using System.Text;

// Windows terminal driver: plain System.Console, no P/Invoke.
// Key-based (not byte-based) input with the same CRT echo semantics
// as the Unix driver: printable keys echo, Backspace/Delete erase,
// Enter terminates, other control keys are ignored. Output bytes go
// through the shared Out writer, so game text matches Unix exactly;
// the console renders them per its own code page and VT support.

public static class WindowsTerminal
{
    public static bool KeyPressed() => Console.KeyAvailable;

    public static char ReadKeyRaw() => Console.ReadKey(intercept: true).KeyChar;

    public static string ReadLine()
    {
        StringBuilder sb = new();
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter || key.KeyChar == '\n')
            {
                Crt.EchoNewLine();
                return sb.ToString();
            }
            if (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Delete || key.KeyChar == '\b')
            {
                if (sb.Length > 0)
                {
                    sb.Length--;
                    Crt.EchoErase();
                }
                continue;
            }
            if (key.KeyChar == '\0')
                continue;
            sb.Append(key.KeyChar);
            Crt.EchoChar(key.KeyChar);
        }
    }
}
