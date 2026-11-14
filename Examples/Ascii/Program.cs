//
// NeoKolors Examples: Console ascii table
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Console;

namespace Ascii;

internal class Program {
    private static readonly string[] UNPRINTABLE = [
        "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS ", "HT ", "LF ", "VT ", "FF ", "CR ", "SO ", "SI ",
        "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB", "CAN", "EN ", "SUB", "ESC", "FS ", "GS ", "RS ", "US ",
        "SPC"
    ];

    private const string DEL = "DEL";

    private static void Main(string[] args) {
        var layout = 4;

        if (args.Length == 1)
            try {
                layout = int.Parse(args[0]);
            }
            catch (FormatException) {
                NKConsole.WriteLine("Invalid layout number. Please enter a valid integer.", new NKStyle(ConsoleColor.Red));
            }

        var rows = 2 << (layout - 1);
        var cols = 2 << (6      - layout);

        for (var y = 0; y < rows; y++) {
            for (var x = 0; x < cols; x++) {
                var ch = x * rows + y;
                NKConsole.Write($"   {ch:000}",                                                        new NKStyle(ConsoleColor.Blue,    styles: NKTextStyles.FAINT));
                NKConsole.Write($" {ch:x2}",                                                           new NKStyle(ConsoleColor.Magenta, styles: NKTextStyles.FAINT));
                NKConsole.Write($" {(ch < 33 ? UNPRINTABLE[ch] : ch == 127 ? DEL : (char)ch + "  ")}", new NKStyle(styles: NKTextStyles.BOLD));
            }

            Console.WriteLine();
        }
    }
}