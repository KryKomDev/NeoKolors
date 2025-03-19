//
// NeoKolors Examples: Console ascii table
// Copyright (c) 2025 KryKom
//

using NeoKolors.Console;

namespace Ascii;

class Program {
    private static readonly string[] UNPRINTABLE = [
        "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS ", "HT ", "LF ", "VT ", "FF ", "CR ", "SO ", "SI ",
        "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB", "CAN", "EN ", "SUB", "ESC", "FS ", "GS ", "RS ", "US "
    ];

    private const string DEL = "DEL";
    
    static void Main(string[] args) {
        for (int y = 0; y < 16; y++) {
            for (int x = 0; x < 8; x++) {
                int ch = x * 16 + y;
                ConsoleColors.Write($"   <f>{ch:000}</f>", ConsoleColor.Blue);
                ConsoleColors.Write($" <f>{ch:x2}</f>", ConsoleColor.Magenta);
                ConsoleColors.Write($" <b>{(ch < 32 ? UNPRINTABLE[ch] : ch == 127 ? DEL : (char)ch + "  ")}</b>", ConsoleColor.White);
            }
            
            Console.WriteLine();
        }
    }
}