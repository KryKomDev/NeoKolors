//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
    
    
    /// <summary>
    /// Writes a formatted table to the console based on the provided header, data, and row selector.
    /// </summary>
    /// <param name="header">The collection of header strings for the table columns.</param>
    /// <param name="data">The collection of data items to populate the table rows.</param>
    /// <param name="rowSelector">
    /// A function that maps each data item to an array of strings representing the table row values.
    /// </param>
    /// <typeparam name="T">The type of the data items in the collection.</typeparam>
    /// <exception cref="ArgumentException">
    /// Thrown when the length of any row data does not match the length of the header.
    /// </exception>
    public static void WriteTable<T>(
        IEnumerable<string> header, 
        IEnumerable<T> data, 
        Func<T, string[]> rowSelector) 
    {
        var headerArr = header as string[] ?? header.ToArray();
        int cols = headerArr.Length;

        string[][] rows = data.Select(rowSelector).ToArray();

        // check if any row is invalid size
        if (rows.Any(t => t.Length != cols))
            throw new ArgumentException("Row length does not match header length");
        
        WriteTable(headerArr, rows);
    }


    /// <summary>
    /// Writes a tabular representation of the provided data to the console.
    /// </summary>
    /// <typeparam name="T">The type of the data elements to be displayed in the table.</typeparam>
    /// <param name="header">
    /// A collection of strings representing the column headers for the table.
    /// </param>
    /// <param name="data">
    /// A collection of collections, where each inner collection represents a row of data in the table.
    /// </param>
    /// <param name="stringifier">
    /// An optional function to convert elements of type T to their string representation. If not provided, elements'
    /// ToString() method will be used. Throws a NoNullAllowedException if any element is null and no stringifier
    /// is provided.
    /// </param>
    /// <exception cref="NoNullAllowedException">
    /// Thrown when a null element is encountered in the data and no stringifier function is provided.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the length of any row in the data does not match the length of the header.
    /// </exception>
    public static void WriteTable<T>(
        IEnumerable<string> header, 
        IEnumerable<IEnumerable<T>> data,
        Func<T, string>? stringifier = null) 
    {
        stringifier ??= t => {
            if (t is null)
                throw new NoNullAllowedException("Cannot convert null element to string");
            var s = t.ToString();
            if (s is null)
                throw new NoNullAllowedException("ToString returned null");
            return s;
        };

        var headerArr = header as string[] ?? header.ToArray();
        int cols = headerArr.Length;

        string[][] rows = data.Select(t => t.Select(stringifier).ToArray()).ToArray();

        // check if any row is invalid size
        if (rows.Any(t => t.Length != cols))
            throw new ArgumentException("Row length does not match header length");
        
        WriteTable(headerArr, rows);
    }

    private static void WriteTable(string[] header, string[][] rows) {
        var cols = header.Length;
        int[] maxWidths = new int[cols];

        // compute max width for header
        for (int i = 0; i < rows.Length; i++) 
            maxWidths[i] = header[i].Length + 2;

        // compute max width for data
        for (int i0 = 0; i0 < rows.Length; i0++) {
            for (int i1 = 0; i1 < cols; i1++) {
                maxWidths[i1] = Math.Max(maxWidths[i1], rows[i0][i1].Length);
            }
        }

        // print header
        for (int i = 0; i < cols; i++) System.Console.Write($"| {header[i].PadRight(maxWidths[i])} ");
        System.Console.WriteLine("|");

        // print separator
        for (int i = 0; i < cols; i++) System.Console.Write($"|{new string('-', maxWidths[i] + 2)}");
        System.Console.WriteLine("|");
        
        // print data
        for (int i = 0; i < rows.Length; i++) {
            for (int j = 0; j < cols; j++) {
                System.Console.Write($"| {rows[i][j].PadRight(maxWidths[j])} ");
            }
            System.Console.WriteLine("|");
        }
    }


    /// <summary>
    /// Writes a formatted table to the console using the specified header and data.
    /// </summary>
    /// <param name="header">An array of column headers for the table.</param>
    /// <param name="data">An array of data items to populate the table rows.</param>
    /// <typeparam name="T">The type of the data items.</typeparam>
    public static void WriteTable<T>(string[] header, params T[] data) {
        Type t = typeof(T);
        int cols = header.Length;
        var props = t.GetProperties().Where(p => header.Contains(p.Name)).ToArray();
        
        string[][] rows = new string[data.Length][];

        for (int i = 0; i < data.Length; i++) {
            rows[i] = new string[cols];

            for (int j = 0; j < props.Length; j++) {
                rows[i][j] = props[j].GetMethod?.Invoke(data[i], null)?.ToString() ?? "null";
            }
        }
        
        WriteTable(header, rows);
    }
}