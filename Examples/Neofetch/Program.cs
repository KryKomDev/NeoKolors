using System.Runtime.InteropServices;
using System.Text;
using NeoKolors.Common;
using NeoKolors.Console;
using static NeoKolors.Common.NKConsoleColor;

namespace Neofetch;

class Program {

    public const string LOGO = "{0}⠀⠀⠀⣤⣴⣾⣿⣿⣿⣿⣿⣶⡄{1}⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⡄\n" +
                               "{0}⠀⠀⢀⣿⣿⣿⣿⣿⣿⣿⣿⣿⠀{1}⠀⢰⣦⣄⣀⣀⣠⣴⣾⣿⠃\n" +
                               "{0}⠀⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⡏⠀{1}⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⠀\n" +
                               "{0}⠀⠀⣼⣿⡿⠿⠛⠻⠿⣿⣿⡇⠀{1}⠀⣿⣿⣿⣿⣿⣿⣿⣿⡿⠀\n" +
                               "{0}⠀⠀⠉⠀⠀⠀ ⠀⠀⠀⠈⠁{1}⠀⢰⣿⣿⣿⣿⣿⣿⣿⣿⠇⠀\n" +
                               "{2}⠀⠀⣠⣴⣶⣿⣿⣿⣷⣶⣤⠀⠀{1}⠀⠈⠉⠛⠛⠛⠉⠉⠀⠀⠀\n" +
                               "{2}⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⡇{3}⠀⠀⣶⣦⣄⣀⣀⣀⣤⣤⣶⠀⠀\n" +
                               "{2}⠀⣾⣿⣿⣿⣿⣿⣿⣿⣿⡇{3}⠀⢀⣿⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀\n" +
                               "{2}⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⠁{3}⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀\n" +
                               "{2}⢠⣿⡿⠿⠛⠉⠉⠉⠛⠿⠀{3}⠀⢸⣿⣿⣿⣿⣿⣿⣿⣿⠁⠀⠀\n" +
                               "{2}⠘⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀ {3}⠻⢿⣿⣿⣿⣿⣿⠿⠛⠀⠀⠀{4}";

    static void Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;
        Console.OpenStandardOutput();
        Console.WriteLine();
        NKConsole.WriteLineF(LOGO, RED, GREEN, BLUE, YELLOW, NKColor.Default);
        NKConsole.MoveCursor(28, -11);
        string h = $"{Environment.UserDomainName}@{Environment.UserName}";
        NKConsole.Write(h, ConsoleColor.Green);
        NKConsole.MoveCursor(^28, 1);
        NKConsole.Write(new string('─', h.Length), ConsoleColor.White);
        NKConsole.MoveCursor(^28, 1);
        NKConsole.WriteF($"{{0}}OS{{1}}: {Environment.OSVersion}", BLUE, NKColor.Default);
        NKConsole.MoveCursor(^28, 1);
        NKConsole.WriteF($"{{0}}Kernel{{1}}: {Environment.OSVersion.Platform}", BLUE, NKColor.Default);
        NKConsole.MoveCursor(^28, 1);
        NKConsole.WriteF($"{{0}}DotNet{{1}}: {Environment.Version}", BLUE, NKColor.Default);
        NKConsole.MoveCursor(^28, 1);
        NKConsole.WriteF($"{{0}}Arch{{1}}: {RuntimeInformation.ProcessArchitecture}", BLUE, NKColor.Default);
        NKConsole.MoveCursor(^28, 1);
        NKConsole.WriteF($"{{0}}CPU{{1}}: 12th Gen Intel(R) Core(TM) i5-12400F", BLUE, NKColor.Default);
        NKConsole.MoveCursor(^28, 1);
        NKConsole.WriteF($"{{0}}GPU{{1}}: Nvidia GeForce 3060", BLUE, NKColor.Default);
        NKConsole.MoveCursor(^28, 1);
        NKConsole.WriteF($"{{0}}Terminal{{1}}: Windows Terminal", BLUE, NKColor.Default);
        NKConsole.MoveCursor(^0, 3);
        Console.WriteLine();
    }
}