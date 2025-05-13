//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NeoKolors.Common;

namespace NeoKolors.Console;

public static partial class NKConsole {
    
    /// <summary>
    /// prints a colored string in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int hex) => 
        System.Console.Write(s.AddColor(hex));

    
    /// <summary>
    /// prints a colored string in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, byte r, byte g, byte b) => 
        System.Console.Write(s.AddColor(r, g, b));
    
    
    /// <summary>
    /// prints a string colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, System.Drawing.Color c) =>
        System.Console.Write(s.AddColor(c));

    
    /// <summary>
    /// prints a string colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, ConsoleColor c) => 
        System.Console.Write(s.AddColor(c));

    
    /// <summary>
    /// prints a string colored by the <see cref="NKColor"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKColor c) => 
        System.Console.Write(s.AddColor(c));


    /// <summary>
    /// Writes a string to the console with the specified color applied.
    /// </summary>
    /// <param name="s">The string to write to the console.</param>
    /// <param name="colors">The colors to be applied to the string.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteF(string s, params NKColor[] colors) =>
        System.Console.Write(s, colors.Cast<object?>().ToArray());


    // --- --- let there begin WriteLine implementations --- ---
    
    /// <summary>
    /// prints a colored string in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int hex) => 
        System.Console.WriteLine(s.AddColor(hex));

    
    /// <summary>
    /// prints a colored string in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, byte r, byte g, byte b) =>
        System.Console.WriteLine(s.AddColor(r, g, b));
    
    
    /// <summary>
    /// prints a string colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, ConsoleColor c) => 
        System.Console.WriteLine(s.AddColor(c));
    
    
    /// <summary>
    /// prints a string colored by the <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, System.Drawing.Color c) => 
        System.Console.WriteLine(s.AddColor(c));
    
    
    /// <summary>
    /// prints a string colored by the universal <see cref="NKColor"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKColor c) => 
        System.Console.WriteLine(s.AddColor(c));


    /// <summary>
    /// prints a string with its background colored using string symbols
    /// </summary>
    /// <param name="s">source string</param>
    /// <param name="colors">
    /// tuple of symbol string and color,
    /// with which will the symbol be replaced,
    /// if a color is -1, the colors will be reset
    /// </param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineF(string s, params NKColor[] colors) => 
        System.Console.WriteLine(s, colors.Cast<object?>().ToArray());

    
    /// <summary>
    /// prints a string with a colored background in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, int hex) => 
        System.Console.Write(s.AddColorB(hex));

    
    /// <summary>
    /// prints a string with a colored background in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, byte r, byte g, byte b) => 
        System.Console.Write(s.AddColorB(r, g, b));

    
    /// <summary>
    /// prints a string with the background colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, ConsoleColor c) => 
        System.Console.Write(s.AddColorB(c));
    
    
    /// <summary>
    /// prints a string with the background colored by the <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, System.Drawing.Color c) => 
        System.Console.Write(s.AddColorB(c));
    
    
    /// <summary>
    /// prints a string with the background colored by the universal <see cref="NKColor"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, NKColor c) => 
        System.Console.Write(s.AddColorB(c));
    
    
    /// <summary>
    /// Writes a formatted string with specified colors to the console.
    /// </summary>
    /// <param name="s">The string to format and write.</param>
    /// <param name="colors">An array of colors to format the string with.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void WriteBF(string s, params NKColor[] colors) =>
        System.Console.Write(s, colors.Cast<object?>().ToArray());


    /// <summary>
    /// prints a string with a colored background in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, int hex) => 
        System.Console.WriteLine(s.AddColor(hex));
    
    
    /// <summary>
    /// prints a string with a colored background in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, byte r, byte g, byte b) => 
        System.Console.WriteLine(s.AddColorB(r, g, b));
    
    
    /// <summary>
    /// prints a string with the background colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, ConsoleColor c) =>
        System.Console.WriteLine(s.AddColorB(c));

    
    /// <summary>
    /// prints a string with the background colored by the <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, System.Drawing.Color c) => 
        System.Console.WriteLine(s.AddColorB(c));

    
    /// <summary>
    /// prints a string with the background colored by the universal <see cref="NKColor"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, NKColor c) => 
        System.Console.WriteLine(s.AddColorB(c));


    /// <summary>
    /// Writes a formatted string with specified colors to the console, followed by a newline.
    /// </summary>
    /// <param name="s">The string to format and write.</param>
    /// <param name="colors">An array of colors to format the string with.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void WriteLineBF(string s, params NKColor[] colors) =>
        System.Console.WriteLine(s, colors.Cast<object?>().ToArray());


    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int text, int background) =>
        System.Console.Write(s.AddColor(text).AddColorB(background));

    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="tr">red value of text color</param>
    /// <param name="tg">green value of text color</param>
    /// <param name="tb">blue value of text color</param>
    /// <param name="br">red value of background color</param>
    /// <param name="bg">green value of background color</param>
    /// <param name="bb">blue value of background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, byte tr, byte tg, byte tb, byte br, byte bg, byte bb) =>
        System.Console.Write(s.AddColor(tr, tg, tb).AddColorB(br, bg, bb));
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int text, ConsoleColor background)  =>
        System.Console.Write(s.AddColor(text).AddColorB(background));
    
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, System.Drawing.Color text, System.Drawing.Color background)  =>
        System.Console.Write(s.AddColor(text).AddColorB(background));


    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKColor text, NKColor background)  =>
        System.Console.Write(s.AddColor(text).AddColorB(background));
    
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int text, int background) =>
        System.Console.WriteLine(s.AddColor(text).AddColorB(background));

    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="tr">red value of text color</param>
    /// <param name="tg">green value of text color</param>
    /// <param name="tb">blue value of text color</param>
    /// <param name="br">red value of background color</param>
    /// <param name="bg">green value of background color</param>
    /// <param name="bb">blue value of background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, byte tr, byte tg, byte tb, byte br, byte bg, byte bb) =>
        System.Console.WriteLine(s.AddColor(tr, tg, tb).AddColorB(br, bg, bb));
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int text, ConsoleColor background)  =>
        System.Console.WriteLine(s.AddColor(text).AddColorB(background));
    
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, System.Drawing.Color text, System.Drawing.Color background)  =>
        System.Console.WriteLine(s.AddColor(text).AddColorB(background));


    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKColor text, NKColor background)  =>
        System.Console.WriteLine(s.AddColor(text).AddColorB(background));


    /// <summary>
    /// prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKStyle st) =>
        System.Console.Write(s.AddStyle(st));
    
    
    /// <summary>
    /// prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKStyle st) =>
        System.Console.WriteLine(s.AddStyle(st));
    
    
    /// <summary>
    /// prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(char s, NKStyle st) =>
        System.Console.Write(s.AddStyle(st));
    
    
    /// <summary>
    /// prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(char s, NKStyle st) =>
        System.Console.WriteLine(s.AddStyle(st));
}