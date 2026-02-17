//
// NeoKolors Examples: Console ascii table
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Console;

namespace Ascii;

class Program {
    private static readonly string[] UNPRINTABLE = [
        "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS ", "HT ", "LF ", "VT ", "FF ", "CR ", "SO ", "SI ",
        "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB", "CAN", "EN ", "SUB", "ESC", "FS ", "GS ", "RS ", "US ",
        "SPC"
    ];

    private const string DEL = "DEL";
    
    static void Main(string[] args) {
        int layout = 4;
        
        if (args.Length == 1) {
            try {
                layout = int.Parse(args[0]);
            }
            catch (FormatException) {
                NKConsole.WriteLine("Invalid layout number. Please enter a valid integer.", new NKStyle(ConsoleColor.Red));
            }
        }
        
        int rows = 2 << (layout - 1);
        int cols = 2 << (6 - layout);
        
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < cols; x++) {
                int ch = x * rows + y;
                NKConsole.Write($"   {ch:000}", new NKStyle(ConsoleColor.Blue, s: TextStyles.FAINT));
                NKConsole.Write($" {ch:x2}", new NKStyle(ConsoleColor.Magenta, s: TextStyles.FAINT));
                NKConsole.Write($" {(ch < 33 ? UNPRINTABLE[ch] : ch == 127 ? DEL : (char)ch + "  ")}", new NKStyle(s: TextStyles.BOLD));
            }
            
            Console.WriteLine();
        }
    }
}