//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NeoKolors.Common;
using NeoKolors.Common.Util;
using ConsoleColor = System.ConsoleColor;

#if NET8_0_OR_GREATER
using SkiaSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixPix;
#endif

using Std = System.Console;

namespace NeoKolors.Console;

public static partial class NKConsole {
    
    /// <summary>
    /// prints a colored string in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int hex) => 
        Std.Write(s.AddColor(hex));

    
    /// <summary>
    /// prints a colored string in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, byte r, byte g, byte b) => 
        Std.Write(s.AddColor(r, g, b));
    
    
    /// <summary>
    /// prints a string colored by the <see cref="System.ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, System.Drawing.Color c) =>
        Std.Write(s.AddColor(c));

    
    /// <summary>
    /// prints a string colored by the <see cref="System.ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, ConsoleColor c) => 
        Std.Write(s.AddColor(c));

    
    /// <summary>
    /// prints a string colored by the <see cref="NKColor"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKColor c) => 
        Std.Write(s.AddColor(c));


    /// <summary>
    /// Writes a string to the console with the specified color applied.
    /// </summary>
    /// <param name="s">The string to write to the console.</param>
    /// <param name="colors">The colors to be applied to the string.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteF(string s, params NKColor[] colors) =>
        Std.Write(s, colors.Cast<object?>().ToArray());


    // --- --- let there begin WriteLine implementations --- ---
    
    /// <summary>
    /// prints a colored string in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int hex) => 
        Std.WriteLine(s.AddColor(hex));

    
    /// <summary>
    /// prints a colored string in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, byte r, byte g, byte b) =>
        Std.WriteLine(s.AddColor(r, g, b));
    
    
    /// <summary>
    /// prints a string colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, ConsoleColor c) => 
        Std.WriteLine(s.AddColor(c));
    
    
    /// <summary>
    /// prints a string colored by the <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, System.Drawing.Color c) => 
        Std.WriteLine(s.AddColor(c));
    
    
    /// <summary>
    /// prints a string colored by the universal <see cref="NKColor"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKColor c) => 
        Std.WriteLine(s.AddColor(c));


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
        Std.WriteLine(s, colors.Cast<object?>().ToArray());

    
    /// <summary>
    /// prints a string with a colored background in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, int hex) => 
        Std.Write(s.AddColorB(hex));

    
    /// <summary>
    /// prints a string with a colored background in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, byte r, byte g, byte b) => 
        Std.Write(s.AddColorB(r, g, b));

    
    /// <summary>
    /// prints a string with the background colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, ConsoleColor c) => 
        Std.Write(s.AddColorB(c));
    
    
    /// <summary>
    /// prints a string with the background colored by the <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, System.Drawing.Color c) => 
        Std.Write(s.AddColorB(c));
    
    
    /// <summary>
    /// prints a string with the background colored by the universal <see cref="NKColor"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, NKColor c) => 
        Std.Write(s.AddColorB(c));
    
    
    /// <summary>
    /// Writes a formatted string with specified colors to the console.
    /// </summary>
    /// <param name="s">The string to format and write.</param>
    /// <param name="colors">An array of colors to format the string with.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void WriteBF(string s, params NKColor[] colors) =>
        Std.Write(s, colors.Cast<object?>().ToArray());


    /// <summary>
    /// prints a string with a colored background in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, int hex) => 
        Std.WriteLine(s.AddColor(hex));
    
    
    /// <summary>
    /// prints a string with a colored background in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, byte r, byte g, byte b) => 
        Std.WriteLine(s.AddColorB(r, g, b));
    
    
    /// <summary>
    /// prints a string with the background colored by the <see cref="ConsoleColor"/> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, ConsoleColor c) =>
        Std.WriteLine(s.AddColorB(c));

    
    /// <summary>
    /// prints a string with the background colored by the <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, System.Drawing.Color c) => 
        Std.WriteLine(s.AddColorB(c));

    
    /// <summary>
    /// prints a string with the background colored by the universal <see cref="NKColor"/>
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, NKColor c) => 
        Std.WriteLine(s.AddColorB(c));


    /// <summary>
    /// Writes a formatted string with specified colors to the console, followed by a newline.
    /// </summary>
    /// <param name="s">The string to format and write.</param>
    /// <param name="colors">An array of colors to format the string with.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void WriteLineBF(string s, params NKColor[] colors) =>
        Std.WriteLine(s, colors.Cast<object?>().ToArray());


    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int text, int background) =>
        Std.Write(s.AddColor(text).AddColorB(background));

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
        Std.Write(s.AddColor(tr, tg, tb).AddColorB(br, bg, bb));
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int text, ConsoleColor background)  =>
        Std.Write(s.AddColor(text).AddColorB(background));
    
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, System.Drawing.Color text, System.Drawing.Color background)  =>
        Std.Write(s.AddColor(text).AddColorB(background));


    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKColor text, NKColor background)  =>
        Std.Write(s.AddColor(text).AddColorB(background));
    
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int text, int background) =>
        Std.WriteLine(s.AddColor(text).AddColorB(background));

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
        Std.WriteLine(s.AddColor(tr, tg, tb).AddColorB(br, bg, bb));
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int text, ConsoleColor background)  =>
        Std.WriteLine(s.AddColor(text).AddColorB(background));
    
    
    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, System.Drawing.Color text, System.Drawing.Color background)  =>
        Std.WriteLine(s.AddColor(text).AddColorB(background));


    /// <summary>
    /// prints a colored string with a colored background  
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKColor text, NKColor background)  =>
        Std.WriteLine(s.AddColor(text).AddColorB(background));


    /// <summary>
    /// prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKStyle st) =>
        Std.Write(s.AddStyle(st));
    
    
    /// <summary>
    /// prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKStyle st) =>
        Std.WriteLine(s.AddStyle(st));
    
    
    /// <summary>
    /// prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(char s, NKStyle st) =>
        Std.Write(s.AddStyle(st));
    
    
    /// <summary>
    /// prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(char s, NKStyle st) =>
        Std.WriteLine(s.AddStyle(st));
    
    
    /// <summary>
    /// Writes a formatted table to the console based on the provided header, data, and row selector.
    /// </summary>
    /// <param name="header">The collection of header strings for the table columns.</param>
    /// <param name="data">The collection of data items to populate the table rows.</param>
    /// <param name="rowSelector">
    /// A function that maps each data item to an array of strings representing the table row values.
    /// </param>
    /// <param name="style">The style of the printed table.</param>
    /// <typeparam name="T">The type of the data items in the collection.</typeparam>
    /// <exception cref="ArgumentException">
    /// Thrown when the length of any row data does not match the length of the header.
    /// </exception>
    public static void WriteTable<T>(
        IEnumerable<string> header, 
        IEnumerable<T> data, 
        Func<T, string[]> rowSelector,
        NKTableStyle style = NKTableStyle.ASCII) 
    {
        var headerArr = header as string[] ?? header.ToArray();
        int cols = headerArr.Length;

        string[][] rows = data.Select(rowSelector).ToArray();
        
        WriteTable(headerArr, rows, style);
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
    /// <param name="style">The style of the printed table.</param>
    /// <exception cref="NoNullAllowedException">
    /// Thrown when a null element is encountered in the data and no stringifier function is provided.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the length of any row in the data does not match the length of the header.
    /// </exception>
    public static void WriteTable<T>(
        IEnumerable<string> header, 
        IEnumerable<IEnumerable<T>> data,
        Func<T, string>? stringifier = null, 
        NKTableStyle style = NKTableStyle.ASCII) 
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

        string[][] rows = data.Select(t => t.Select(stringifier).ToArray()).ToArray();
        
        WriteTable(headerArr, rows, style);
    }


    /// <summary>
    /// Writes a table to the console with the provided headers and rows.
    /// </summary>
    /// <param name="header">An array of header names for the table columns.</param>
    /// <param name="rows">
    /// A jagged array representing the rows of the table, with each inner array corresponding to a row of data.
    /// </param>
    /// <param name="style">The style of the printed table.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the length of a row does not match the length of the headers.
    /// </exception>
    public static void WriteTable(string[] header, string[][] rows, NKTableStyle style = NKTableStyle.ASCII) {
        var cols = header.Length;

        // check if any row is invalid size
        if (rows.Any(r => r.Length != cols))
            throw new ArgumentException("Row length does not match header length");
        
        int[] maxWidths = new int[cols];

        // compute max width for header
        for (int i = 0; i < cols; i++) 
            maxWidths[i] = header[i].Length;

        // compute max width for data
        for (int i0 = 0; i0 < rows.Length; i0++) {
            for (int i1 = 0; i1 < cols; i1++) {
                maxWidths[i1] = Math.Max(maxWidths[i1], rows[i0][i1].VisibleLength());
            }
        }

        char vSeparator = style switch {
            NKTableStyle.BORDERLESS => ' ',
            NKTableStyle.ASCII => '|',
            NKTableStyle.NORMAL => '│',
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };

        // print header
        for (int i = 0; i < cols; i++) 
            Std.Write($"{vSeparator} {header[i].AddStyle(TextStyles.BOLD).VisiblePadRight(maxWidths[i])} ");
        Std.WriteLine(vSeparator);

        // print separator
        switch (style) {
            case NKTableStyle.BORDERLESS:
                for (int i = 0; i < cols; i++) 
                    Std.Write($"  {new string('─', header[i].Length).VisiblePadRight(maxWidths[i])} ");
                Std.WriteLine();
                break;
            case NKTableStyle.ASCII:
                for (int i = 0; i < cols; i++) 
                    Std.Write($"{vSeparator}{new string('-', maxWidths[i] + 2)}");
                Std.WriteLine(vSeparator);
                break;
            case NKTableStyle.NORMAL:
                Std.Write('├');
                for (int i = 0; i < cols; i++) 
                    Std.Write((i == 0 ? "" : "┼") + new string('─', maxWidths[i] + 2));
                Std.WriteLine("┤");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
        
        // print data
        for (int i = 0; i < rows.Length; i++) {
            for (int j = 0; j < cols; j++) {
                Std.Write($"{vSeparator} {rows[i][j].VisiblePadRight(maxWidths[j])} ");
            }
            Std.WriteLine(vSeparator);
        }
    }


    /// <summary>
    /// Writes a formatted table to the console, with a specified header and rows.
    /// </summary>
    /// <param name="header">An array of strings representing the header of the table.</param>
    /// <param name="rows">
    /// A two-dimensional array of strings representing the rows of the table, converted to a jagged array.
    /// </param>
    /// <param name="style">The style of the printed table.</param>
    public static void WriteTable(string[] header, string[,] rows, NKTableStyle style = NKTableStyle.ASCII) => 
        WriteTable(header, List2D.ToJagged(rows), style);


    /// <summary>
    /// Writes a formatted table to the console using the specified header and data.
    /// </summary>
    /// <param name="header">An array of column headers for the table.</param>
    /// <param name="data">An array of data items to populate the table rows.</param>
    /// <param name="style">The style of the printed table.</param>
    /// <typeparam name="T">The type of the data items.</typeparam>
    public static void WriteTable<T>(string[] header, NKTableStyle style = NKTableStyle.ASCII, params T[] data) {
        var t = typeof(T);
        int cols = header.Length;
        
        // get properties that match the header
        var props = t.GetProperties().Where(p => header.Contains(p.Name)).ToArray();
        
        // check if all properties were found
        if (props.Length != cols) 
            throw new ArgumentException("Some properties from header are not valid.");
        
        string[][] rows = new string[data.Length][];

        // get values for each property
        for (int i = 0; i < data.Length; i++) {
            rows[i] = new string[cols];

            for (int j = 0; j < props.Length; j++) {
                var m = props[j].GetMethod;

                if (m is null) {
                    rows[i][j] = "null";
                    continue;
                }
                
                var o = m.Invoke(data[i], null);

                if (o is null) {
                    rows[i][j] = "null";
                    continue;
                }
                
                rows[i][j] = o.ToString() ?? "null";
            }
        }
        
        WriteTable(header, rows, style);
    }

    /// <summary>
    /// Automatically collects all properties of type <see cref="T"/> from a type specified by <see cref="TSource"/>
    /// and lists their values in a table.
    /// </summary>
    /// <param name="header">Properties of <see cref="T"/> to be listed in the table</param>
    /// <param name="displayName">Whether to add a column for the names of the properties.</param>
    /// <param name="style">The style of the table</param>
    /// <typeparam name="T">The type of a single row item</typeparam>
    /// <typeparam name="TSource">The type to collect the properties from</typeparam>
    public static void WriteTable<TSource, T>(
        string[] header, 
        bool displayName = true, 
        NKTableStyle style = NKTableStyle.ASCII)
    {
        WriteTable<T>(typeof(TSource), header, displayName, style);
    }

    /// <summary>
    /// Automatically collects all properties of type <see cref="TTarget"/> from a type specified by <see cref="source"/>
    /// and lists their values in a table.
    /// </summary>
    /// <param name="source">The type to collect the properties from</param>
    /// <param name="header">Properties of <see cref="TTarget"/> to be listed in the table</param>
    /// <param name="displayName">Whether to add a column for the names of the properties.</param>
    /// <param name="style">The style of the table</param>
    /// <typeparam name="TTarget">The type of a single row item</typeparam>
    public static void WriteTable<TTarget>(
        Type source,
        string[] header, 
        bool displayName = true, 
        NKTableStyle style = NKTableStyle.ASCII)
    {
        var t = typeof(TTarget);
        
        // load static properties
        var fields = source.GetProperties();
        Dictionary<string, TTarget> objects = new();
        
        foreach (var field in fields) {
            if (field.PropertyType != t) continue;
            
            var palette = field.CanRead ? field.GetValue(null) : null;
            if (palette is not TTarget p) continue;
                
            objects.Add(field.Name, p);
        }
        
        int cols = header.Length;
        
        // get properties that match the header
        var props = t.GetProperties().Where(p => header.Contains(p.Name)).ToArray();
        
        // check if all properties were found
        if (props.Length != cols) 
            throw new ArgumentException("Some properties from header are not valid.");
        
        string[][] rows = new string[objects.Count][];

        // get values for each property
        for (int i = 0; i < objects.Count; i++) {
            rows[i] = new string[cols + 1];
            rows[i][0] = objects.ElementAt(i).Key;
            
            for (int j = 1; j < props.Length + 1; j++) {
                var m = props[j - 1].GetMethod;

                if (m is null) {
                    rows[i][j] = "null";
                    continue;
                }
                
                var o = m.Invoke(objects.ElementAt(i).Value, null);

                if (o is null) {
                    rows[i][j] = "null";
                    continue;
                }
                
                rows[i][j] = o.ToString() ?? "null";
            }
        }

        var h = header.ToList();
        if (displayName) h.Insert(0, "Name");
        
        WriteTable(h.ToArray(), rows, style);
    }
    
    /// <summary>
    /// Moves the cursor to the specified position relatively to the current position.
    /// </summary>
    /// <param name="x">x offset from the current position</param>
    /// <param name="y">y offset from the current position</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MoveCursor(int x, int y) =>
        Std.SetCursorPosition(Std.CursorLeft + x, Std.CursorTop + y);
    
    #if NET8_0_OR_GREATER

    /// <summary>
    /// Moves the console cursor to a specified position based on the provided indices.
    /// </summary>
    /// <param name="x">
    /// The horizontal offset of the cursor.
    /// Can be relative (from-start index) or absolute (from-end index).
    /// </param>
    /// <param name="y">
    /// The vertical offset of the cursor.
    /// Can be relative (from-start index) or absolute (from-end index).
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
    public static void MoveCursor(Index x, Index y) => 
        Std.SetCursorPosition(
            Math.Clamp(x.IsFromEnd ? x.Value : Std.CursorLeft + x.Value, 0, Std.BufferWidth - 1),
            Math.Clamp(y.IsFromEnd ? y.Value : Std.CursorTop + y.Value, 0, Std.BufferHeight - 1));
    
    /// <summary>
    /// Sets the cursor position in the console window.
    /// </summary>
    /// <param name="x">The horizontal position of the cursor, represented as an Index.</param>
    /// <param name="y">The vertical position of the cursor, represented as an Index.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
    public static void SetCursorPosition(Index x, Index y) =>
        Std.SetCursorPosition(
            Math.Clamp(x.IsFromEnd ? Std.BufferWidth - x.Value : x.Value, 0, Std.BufferWidth - 1),
            Math.Clamp(y.IsFromEnd ? Std.BufferHeight - y.Value : y.Value, 0, Std.BufferWidth - 1));
    

    /// <summary>
    /// Writes an image to the console in Sixel format from the specified file path.
    /// </summary>
    /// <param name="path">The path to the image file to be displayed in Sixel format.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSixel(string path) => WriteSixel(Image.Load<Rgba32>(new DecoderOptions(), path));

    /// <summary>
    /// Renders a sixel image onto the console at a specified position, loading the image from the provided file path.
    /// </summary>
    /// <param name="path">The file path of the image to be rendered.</param>
    /// <param name="x">The horizontal position in the console where the image will be rendered.</param>
    /// <param name="y">The vertical position in the console where the image will be rendered.</param>
    public static void WriteSixel(string path, Index x, Index y) =>
        WriteSixel(Image.Load<Rgba32>(new DecoderOptions(), path), x, y);

    /// <summary>
    /// Writes a SIXEL-encoded representation of an image to the console.
    /// </summary>
    /// <param name="image">The image to encode and display in SIXEL format.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSixel(Image<Rgba32> image) => Std.WriteLine(Sixel.Encode(image).ToArray());

    /// <summary>
    /// Writes an image to the console using the sixel encoding at a specified cursor position.
    /// </summary>
    /// <param name="image">The image to display in the console.</param>
    /// <param name="x">The horizontal position in the console where the image will be displayed.</param>
    /// <param name="y">The vertical position in the console where the image will be displayed.</param>
    public static void WriteSixel(Image<Rgba32> image, Index x, Index y) {
        SetCursorPosition(x, y);
        WriteSixel(image);
    }

    /// <summary>
    /// Writes a Sixel-encoded representation of the provided image at a specific position in the console,
    /// resizing the image to the specified dimensions before rendering.
    /// </summary>
    /// <param name="image">The image to be written to the console.</param>
    /// <param name="x">The horizontal position of the image start as an <see cref="Index"/>.</param>
    /// <param name="y">The vertical position of the image start as an <see cref="Index"/>.</param>
    /// <param name="width">The width to which the image should be resized.</param>
    /// <param name="height">The height to which the image should be resized.</param>
    public static void WriteSixel(Image<Rgba32> image, Index x, Index y, int width, int height) {
        SetCursorPosition(x, y);
        image.Mutate(context => context.Resize(width, height));
        WriteSixel(image);           
    }
    
    /// <summary>
    /// Writes a SIXEL-encoded representation of an image to the console.
    /// </summary>
    /// <param name="image">The image to encode and display in SIXEL format.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSixel(SKImage image) => Std.WriteLine(Sixel.Encode(image.Encode().AsStream()).ToArray());

    /// <summary>
    /// Writes an image to the console using the sixel encoding at a specified cursor position.
    /// </summary>
    /// <param name="image">The image to display in the console.</param>
    /// <param name="x">The horizontal position in the console where the image will be displayed.</param>
    /// <param name="y">The vertical position in the console where the image will be displayed.</param>
    public static void WriteSixel(SKImage image, Index x, Index y) {
        SetCursorPosition(x, y);
        WriteSixel(image);
    }

    /// <summary>
    /// Writes a Sixel-encoded representation of the provided image at a specific position in the console,
    /// resizing the image to the specified dimensions before rendering.
    /// </summary>
    /// <param name="image">The image to be written to the console.</param>
    /// <param name="x">The horizontal position of the image start as an <see cref="Index"/>.</param>
    /// <param name="y">The vertical position of the image start as an <see cref="Index"/>.</param>
    /// <param name="width">The width to which the image should be resized.</param>
    /// <param name="height">The height to which the image should be resized.</param>
    /// <param name="sampling">The options for resizing the image.</param>
    public static void WriteSixel(
        SKImage image, Index x, Index y, int width, int height, SKSamplingOptions? sampling = null) => 
            WriteSixel(SKBitmap.FromImage(image));

    /// <summary>
    /// Writes the given bitmap to the console in Sixel format.
    /// </summary>
    /// <param name="bmp">The bitmap image to be encoded and displayed in Sixel format.</param>
    public static void WriteSixel(SKBitmap bmp) => 
        Std.WriteLine(Sixel.Encode(bmp.Encode(SKEncodedImageFormat.Png, 100).AsStream()).ToArray());

    /// <summary>
    /// Writes the provided SKBitmap object as a sixel graphic to the console.
    /// </summary>
    /// <param name="bmp">The bitmap image to encode and write as sixel data.</param>
    /// <param name="x">The horizontal cursor position where the sixel begins.</param>
    /// <param name="y">The vertical cursor position where the sixel begins.</param>
    public static void WriteSixel(SKBitmap bmp, Index x, Index y) {
        SetCursorPosition(x, y);
        WriteSixel(bmp);
    }

    /// <summary>
    /// Writes a sixel-encoded representation of the given image to the console.
    /// </summary>
    /// <param name="bmp">The bitmap image to encode and write.</param>
    /// <param name="x">The horizontal position in the console where the output starts.</param>
    /// <param name="y">The vertical position in the console where the output starts.</param>
    /// <param name="width">The desired width of the image in the console.</param>
    /// <param name="height">The desired height of the image in the console.</param>
    /// <param name="sampling">The sampling options used for resizing the image, or null to use default options.</param>
    public static void WriteSixel(SKBitmap bmp, Index x, Index y, int width, int height,
        SKSamplingOptions? sampling = null) {
        SetCursorPosition(x, y);
        bmp = bmp.Resize(new SKImageInfo(width, height), sampling ?? new SKSamplingOptions(SKFilterMode.Nearest));
        WriteSixel(bmp);
    }

    /// <summary>
    /// Writes a sixel graphic to the console at the specified location and dimensions.
    /// </summary>
    /// <param name="path">The file path of the image to render as a sixel graphic.</param>
    /// <param name="x">The horizontal index position where the sixel graphic will begin.</param>
    /// <param name="y">The vertical index position where the sixel graphic will begin.</param>
    /// <param name="width">The display width of the sixel graphic.</param>
    /// <param name="height">The display height of the sixel graphic.</param>
    /// <param name="sampling">The sampling options used for resizing the image, or null to use default options.</param>
    public static void WriteSixel(string path, Index x, Index y, int width, int height, SKSamplingOptions? sampling = null) =>
        WriteSixel(SKBitmap.Decode(path), x, y, width, height, sampling);

    #endif
}