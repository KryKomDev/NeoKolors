//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using static System.Console;

namespace NeoKolors.Console;

public static class ConsoleColors {
    
    /// <summary>
    /// prints a colored string in the console without newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    /// <param name="continuousColoring">replaces <c>\e[0m</c> characters with the <c>hex</c> color</param>
    public static void PrintColored(string s, int hex, bool continuousColoring = true) {
        s = s.AddTextStyles();
        if (continuousColoring) s = s.Replace("\e[0m", $"\e[38;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m");
        Write($"\e[38;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m{s}\e[0m");
    }

    /// <summary>
    /// prints a string with its characters colored using string symbols
    /// </summary>
    /// <param name="s">source string</param>
    /// <param name="colors">
    /// tuple of symbol string and hexadecimal color representation,
    /// with which will the symbol be replaced,
    /// if a color is -1 the colors will be reset
    /// </param>
    public static void PrintComplexColored(string s, params (string symbol, int hex)[] colors) {
        s = s.AddTextStyles();
        foreach (var c in colors) {
            
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (c.hex == -1) {
                s = s.Replace(c.symbol, "\e[0m");
            }
            else {
                s = s.Replace(c.symbol, $"\e[38;2;{(byte)(c.hex >> 16)};{(byte)(c.hex >> 8)};{(byte)c.hex}m");
            }
        }
        
        Write(s);
    }
    
    /// <summary>
    /// prints a colored string in the console with newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    /// <param name="continuousColoring">replaces <c>\e[0m</c> characters with the <c>hex</c> color</param>
    public static void PrintlnColored(string s, int hex, bool continuousColoring = true) {
        s = s.AddTextStyles();
        if (continuousColoring) s = s.Replace("\e[0m", $"\e[38;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m");
        Write($"\e[38;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m{s}\e[0m\n");
    }
    
    /// <summary>
    /// prints a string with its background colored using string symbols
    /// </summary>
    /// <param name="s">source string</param>
    /// <param name="colors">
    /// tuple of symbol string and hexadecimal color representation,
    /// with which will the symbol be replaced,
    /// if a color is -1 the colors will be reset
    /// </param>
    public static void PrintlnComplexColored(string s, params (string symbol, int hex)[] colors) {
        s = s.AddTextStyles();
        foreach (var c in colors) {
            
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (c.hex == -1) {
                s = s.Replace(c.symbol, "\e[0m");
            }
            else {
                s = s.Replace(c.symbol, $"\e[38;2;{(byte)(c.hex >> 16)};{(byte)(c.hex >> 8)};{(byte)c.hex}m");
            }
        }
        
        WriteLine(s);
    }
    
    /// <summary>
    /// prints a colored string in the console without newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    /// <param name="continuousColoring">replaces <c>\e[0m</c> characters with the <c>hex</c> color</param>
    public static void PrintColored(string s, byte r, byte g, byte b, bool continuousColoring = true) {
        s = s.AddTextStyles();
        if (continuousColoring) s = s.Replace("\e[0m", $"\e[38;2;{r};{g};{b}m");
        Write($"\e[38;2;{r};{g};{b}m{s}\e[0m");
    }
    
    /// <summary>
    /// prints a colored string in the console with newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    /// <param name="continuousColoring">replaces <c>\e[0m</c> characters with the <c>hex</c> color</param>
    public static void PrintlnColored(string s, byte r, byte g, byte b, bool continuousColoring = true) {
        s = s.AddTextStyles();
        if (continuousColoring) s = s.Replace("\e[0m", $"\e[38;2;{r};{g};{b}m");
        Write($"\e[38;2;{r};{g};{b}m{s}\e[0m\n");
    }
    
    /// <summary>
    /// prints a string with colored background in the console without newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    /// <param name="continuousColoring">replaces <c>\e[0m</c> characters with the <c>hex</c> color</param>
    public static void PrintColoredB(string s, int hex, bool continuousColoring = true) {
        s = s.AddTextStyles();
        if (continuousColoring) s = s.Replace("\e[0m", $"\e[48;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m");
        Write($"\e[48;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m{s}\e[0m");
    }
    
    /// <summary>
    /// prints a string with colored background in the console with newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    /// <param name="continuousColoring">replaces <c>\e[0m</c> characters with the <c>hex</c> color</param>
    public static void PrintlnColoredB(string s, int hex, bool continuousColoring = true) {
        s = s.AddTextStyles();
        if (continuousColoring) s = s.Replace("\e[0m", $"\e[48;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m");
        Write($"\e[48;2;{(byte)(hex >> 16)};{(byte)(hex >> 8)};{(byte)hex}m{s}\e[0m\n");
    }
    
    /// <summary>
    /// prints a string with colored background in the console without newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    /// <param name="continuousColoring">replaces <c>\e[0m</c> characters with the <c>hex</c> color</param>
    public static void PrintColoredB(string s, byte r, byte g, byte b, bool continuousColoring = true) {
        s = s.AddTextStyles();
        if (continuousColoring) s = s.Replace("\e[0m", $"\e[48;2;{r};{g};{b}m");
        Write($"\e[48;2;{r};{g};{b}m{s}\e[0m");
    }
    
    /// <summary>
    /// prints a string with colored background in the console with newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    /// <param name="continuousColoring">replaces <c>\e[0m</c> characters with the <c>hex</c> color</param>
    public static void PrintlnColoredB(string s, byte r, byte g, byte b, bool continuousColoring = true) {
        s = s.AddTextStyles();
        if (continuousColoring) s = s.Replace("\e[0m", $"\e[48;2;{r};{g};{b}m");
        Write($"\e[48;2;{r};{g};{b}m{s}\e[0m\n");
    }

    /// <summary>
    /// prints a string colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    public static void PrintColored(string s, ConsoleColor c) {
        s = s.AddTextStyles();
        ForegroundColor = c;
        Write(s);
        ForegroundColor = ConsoleColor.Gray;
    }
    
    /// <summary>
    /// prints a string colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    public static void PrintlnColored(string s, ConsoleColor c) {
        s = s.AddTextStyles();
        ForegroundColor = c;
        WriteLine(s);
        ForegroundColor = ConsoleColor.Gray;
    }
    
    /// <summary>
    /// prints a string with background colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    public static void PrintColoredB(string s, ConsoleColor c) {
        s = s.AddTextStyles();
        BackgroundColor = c;
        Write(s);
        BackgroundColor = ConsoleColor.Black;
    }
    
    /// <summary>
    /// prints a string with background colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    public static void PrintlnColoredB(string s, ConsoleColor c) {
        s = s.AddTextStyles();
        BackgroundColor = c;
        WriteLine(s);
        BackgroundColor = ConsoleColor.Black;
    }

    /// <summary>
    /// prints a colored string with colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    public static void PrintColored(string s, ConsoleColor text, ConsoleColor background) {
        s = s.AddTextStyles();
        BackgroundColor = background;
        ForegroundColor = text;
        Write(s);
        ForegroundColor = ConsoleColor.Gray;
        BackgroundColor = ConsoleColor.Black;
    }
    
    /// <summary>
    /// prints a colored string with colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    public static void PrintlnColored(string s, ConsoleColor text, ConsoleColor background) {
        s = s.AddTextStyles();
        BackgroundColor = background;
        ForegroundColor = text;
        WriteLine(s);
        ForegroundColor = ConsoleColor.Gray;
        BackgroundColor = ConsoleColor.Black;
    }

    public static void PrintColored(string s, Color c) {
        if (c.IsPaletteSafe) {
            PrintColored(s, (ConsoleColor)c.ConsoleColor!);
        }
        else {
            PrintColored(s, (int)c.ConsoleColor!);
        }
    }
    
    public static void PrintlnColored(string s, Color c) {
        if (c.IsPaletteSafe) {
            PrintlnColored(s, (ConsoleColor)c.ConsoleColor!);
        }
        else {
            PrintlnColored(s, (int)c.ConsoleColor!);
        }
    }
    
    public static void PrintColoredB(string s, Color c) {
        if (c.IsPaletteSafe) {
            PrintColoredB(s, (ConsoleColor)c.ConsoleColor!);
        }
        else {
            PrintColoredB(s, (int)c.ConsoleColor!);
        }
    }
    
    public static void PrintlnColoredB(string s, Color c) {
        if (c.IsPaletteSafe) {
            PrintlnColoredB(s, (ConsoleColor)c.ConsoleColor!);
        }
        else {
            PrintlnColoredB(s, (int)c.ConsoleColor!);
        }
    }

    public static void PrintColored(string s, ConsoleColor text, int background) {
        ForegroundColor = text;
        PrintColoredB(s, background);
        ForegroundColor = ConsoleColor.Black;
    }

    public static void PrintlnColored(string s, ConsoleColor text, int background) {
        PrintColored(s + "\n", text, background);
    }
    
    public static void PrintColored(string s, int text, ConsoleColor background) {
        BackgroundColor = background;
        PrintColored(s, text);
        BackgroundColor = ConsoleColor.Black;
    }

    public static void PrintlnColored(string s, int text, ConsoleColor background) {
        PrintColored(s + "\n", text, background);
    }

    public static void PrintColored(string s, int text, int background) {
        s = s.AddTextStyles();
        Write($"\e[38;2;{(byte)(text >> 16)};{(byte)(text >> 8)};{(byte)text}m\e[48;2;{(byte)(background >> 16)};{(byte)(background >> 8)};{(byte)background}m{s}\e[0m");
    }
    
    public static void PrintColored(string s, Color text, Color background) {
        if (text.IsPaletteSafe) {
            if (background.IsPaletteSafe) {
                PrintColored(s, (ConsoleColor)text.ConsoleColor!, (ConsoleColor)background.ConsoleColor!);
            }
            else {
                PrintColored(s, (ConsoleColor)text.ConsoleColor!, (int)background.ConsoleColor!);
            }
        }
        else {
            if (background.IsPaletteSafe) {
                PrintColored(s, (int)text.ConsoleColor!, (ConsoleColor)background.ConsoleColor!);
            }
            else {
                PrintColored(s, (int)text.ConsoleColor!, (int)background.ConsoleColor!);
            }
        }
    }
    
    public static void PrintlnColored(string s, Color text, Color background) {
        PrintColored(s + "\n", text, background);
    }
}