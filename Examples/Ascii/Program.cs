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
    
    static void Main() {
        for (int y = 0; y < 16; y++) {
            for (int x = 0; x < 8; x++) {
                int ch = x * 16 + y;
                NKConsole.Write($"   {ch:000}", new NKStyle(ConsoleColor.Blue, TextStyles.FAINT));
                NKConsole.Write($" {ch:x2}", new NKStyle(ConsoleColor.Magenta, TextStyles.FAINT));
                NKConsole.Write($" {(ch < 33 ? UNPRINTABLE[ch] : ch == 127 ? DEL : (char)ch + "  ")}", new NKStyle(ConsoleColor.White, TextStyles.BOLD));
            }
            
            Console.WriteLine();
        }
    }
}