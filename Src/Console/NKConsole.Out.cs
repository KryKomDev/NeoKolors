//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Extensions;
using SkiaSharp;

namespace NeoKolors.Console;

public static partial class NKConsole {

    #region OUTPUT DRIVER DELEGATING METHODS

    /// <summary>
    ///     Writes the specified span of characters to the output.
    /// </summary>
    /// <param name="value">The span of characters to write.</param>
    public static void Write(ReadOnlySpan<char> value) => OutputDriver.Write(value);


    /// <summary>
    ///     Writes the specified string value to the output.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public static void Write(string value) => OutputDriver.Write(value);


    /// <summary>
    ///     Writes the specified ANSI string to the output.
    /// </summary>
    /// <param name="value">The ANSI string to write to the console output.</param>
    public static void Write(AnsiString value) => OutputDriver.Write(value.ToString());


    /// <summary>
    ///     Writes the specified formatted string along with its arguments to the output.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">An array of objects to write using the format string.</param>
    public static void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object[] args) => OutputDriver.Write(format, args);

    /// <summary>
    ///     Writes a newline to the output.
    /// </summary>
    public static void WriteLine() => OutputDriver.Write("\n");

    /// <summary>
    ///     Writes the specified span of characters followed by a new line to the output.
    /// </summary>
    /// <param name="value">The span of characters to write followed by a new line.</param>
    public static void WriteLine(ReadOnlySpan<char> value) => OutputDriver.WriteLine(value);


    /// <summary>
    ///     Writes the specified string value followed by a new line to the output.
    /// </summary>
    /// <param name="value">The string to write followed by a new line.</param>
    public static void WriteLine(string value) => OutputDriver.WriteLine(value);


    /// <summary>
    ///     Writes the specified ANSI string to the output and a new line.
    /// </summary>
    /// <param name="value">The ANSI string to write to the console output.</param>
    public static void WriteLine(AnsiString value) => OutputDriver.WriteLine(value.ToString());


    /// <summary>
    ///     Writes the specified string value followed by a new line to the output.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">An array of objects to write using the format string.</param>
    public static void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args) => OutputDriver.WriteLine(format, args);


    /// <summary>
    ///     Writes the specified object value followed by a new line to the output.
    /// </summary>
    /// <param name="value">
    ///     The object to write followed by a new line.
    ///     If the value is null, an empty string is written.
    /// </param>
    public static void WriteLine(object? value) => OutputDriver.WriteLine(value);

    #endregion


    /// <summary>
    ///     prints a colored string in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int hex) => OutputDriver.Write(s.AddColor(hex));


    /// <summary>
    ///     prints a colored string in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, byte r, byte g, byte b) => OutputDriver.Write(s.AddColor(r, g, b));


    /// <summary>
    ///     prints a string colored by the <see cref="System.ConsoleColor" /> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, System.Drawing.Color c) => OutputDriver.Write(s.AddColor(c));


    /// <summary>
    ///     prints a string colored by the <see cref="System.ConsoleColor" /> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, ConsoleColor c) => OutputDriver.Write(s.AddColor(c));


    /// <summary>
    ///     prints a string colored by the <see cref="NKColor" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKColor c) => OutputDriver.Write(s.AddColor(c));


    /// <summary>
    ///     prints a string colored by the universal <see cref="NKColor" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="t">style</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKTextStyles t) => OutputDriver.Write(s.AddStyle(t));


    /// <summary>
    ///     Writes a string to the console with the specified color applied.
    /// </summary>
    /// <param name="s">The string to write to the console.</param>
    /// <param name="colors">The colors to be applied to the string.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteF(string s, params NKColor[] colors) => OutputDriver.Write(s, colors.Cast<object?>().ToArray());


    // --- --- let there begin WriteLine implementations --- ---

    /// <summary>
    ///     prints a colored string in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int hex) => OutputDriver.WriteLine(s.AddColor(hex));


    /// <summary>
    ///     prints a colored string in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, byte r, byte g, byte b) => OutputDriver.WriteLine(s.AddColor(r, g, b));


    /// <summary>
    ///     prints a string colored by the <see cref="ConsoleColor" /> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, ConsoleColor c) => OutputDriver.WriteLine(s.AddColor(c));


    /// <summary>
    ///     prints a string colored by the <see cref="System.Drawing.Color" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, System.Drawing.Color c) => OutputDriver.WriteLine(s.AddColor(c));


    /// <summary>
    ///     prints a string colored by the universal <see cref="NKColor" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKColor c) => OutputDriver.WriteLine(s.AddColor(c));

    /// <summary>
    ///     prints a string colored by the universal <see cref="NKColor" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="t">style</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKTextStyles t) => OutputDriver.WriteLine(s.AddStyle(t));


    /// <summary>
    ///     prints a string with its background colored using string symbols
    /// </summary>
    /// <param name="s">source string</param>
    /// <param name="colors">
    ///     tuple of symbol string and color,
    ///     with which will the symbol be replaced,
    ///     if a color is -1, the colors will be reset
    /// </param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineF(string s, params NKColor[] colors) => OutputDriver.WriteLine(s, colors.Cast<object?>().ToArray());


    /// <summary>
    ///     prints a string with a colored background in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, int hex) => OutputDriver.Write(s.AddColorB(hex));


    /// <summary>
    ///     prints a string with a colored background in the console without a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, byte r, byte g, byte b) => OutputDriver.Write(s.AddColorB(r, g, b));


    /// <summary>
    ///     prints a string with the background colored by the <see cref="ConsoleColor" /> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, ConsoleColor c) => OutputDriver.Write(s.AddColorB(c));


    /// <summary>
    ///     prints a string with the background colored by the <see cref="System.Drawing.Color" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, System.Drawing.Color c) => OutputDriver.Write(s.AddColorB(c));


    /// <summary>
    ///     prints a string with the background colored by the universal <see cref="NKColor" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteB(string s, NKColor c) => OutputDriver.Write(s.AddColorB(c));


    /// <summary>
    ///     Writes a formatted string with specified colors to the console.
    /// </summary>
    /// <param name="s">The string to format and write.</param>
    /// <param name="colors">An array of colors to format the string with.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void WriteBF(string s, params NKColor[] colors) => OutputDriver.Write(s, colors.Cast<object?>().ToArray());


    /// <summary>
    ///     prints a string with a colored background in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="hex">hexadecimal value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, int hex) => OutputDriver.WriteLine(s.AddColor(hex));


    /// <summary>
    ///     prints a string with a colored background in the console with a newline
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="r">red value of the color</param>
    /// <param name="g">green value of the color</param>
    /// <param name="b">blue value of the color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, byte r, byte g, byte b) => OutputDriver.WriteLine(s.AddColorB(r, g, b));


    /// <summary>
    ///     prints a string with the background colored by the <see cref="ConsoleColor" /> value
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, ConsoleColor c) => OutputDriver.WriteLine(s.AddColorB(c));


    /// <summary>
    ///     prints a string with the background colored by the <see cref="System.Drawing.Color" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, System.Drawing.Color c) => OutputDriver.WriteLine(s.AddColorB(c));


    /// <summary>
    ///     prints a string with the background colored by the universal <see cref="NKColor" />
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="c">color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineB(string s, NKColor c) => OutputDriver.WriteLine(s.AddColorB(c));


    /// <summary>
    ///     Writes a formatted string with specified colors to the console, followed by a newline.
    /// </summary>
    /// <param name="s">The string to format and write.</param>
    /// <param name="colors">An array of colors to format the string with.</param>
    [StringFormatMethod(nameof(s))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void WriteLineBF(string s, params NKColor[] colors) => OutputDriver.WriteLine(s, colors.Cast<object?>().ToArray());


    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int text, int background) => OutputDriver.Write(s.AddColor(text).AddColorB(background));

    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="tr">red value of text color</param>
    /// <param name="tg">green value of text color</param>
    /// <param name="tb">blue value of text color</param>
    /// <param name="br">red value of background color</param>
    /// <param name="bg">green value of background color</param>
    /// <param name="bb">blue value of background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, byte tr, byte tg, byte tb, byte br, byte bg, byte bb) => OutputDriver.Write(s.AddColor(tr, tg, tb).AddColorB(br, bg, bb));

    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, int text, ConsoleColor background) => OutputDriver.Write(s.AddColor(text).AddColorB(background));


    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, System.Drawing.Color text, System.Drawing.Color background) => OutputDriver.Write(s.AddColor(text).AddColorB(background));


    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKColor text, NKColor background) => OutputDriver.Write(s.AddColor(text).AddColorB(background));


    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int text, int background) => OutputDriver.WriteLine(s.AddColor(text).AddColorB(background));

    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="tr">red value of text color</param>
    /// <param name="tg">green value of text color</param>
    /// <param name="tb">blue value of text color</param>
    /// <param name="br">red value of background color</param>
    /// <param name="bg">green value of background color</param>
    /// <param name="bb">blue value of background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, byte tr, byte tg, byte tb, byte br, byte bg, byte bb) => OutputDriver.WriteLine(s.AddColor(tr, tg, tb).AddColorB(br, bg, bb));

    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, int text, ConsoleColor background) => OutputDriver.WriteLine(s.AddColor(text).AddColorB(background));


    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, System.Drawing.Color text, System.Drawing.Color background) => OutputDriver.WriteLine(s.AddColor(text).AddColorB(background));


    /// <summary>
    ///     prints a colored string with a colored background
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="text">text color</param>
    /// <param name="background">background color</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKColor text, NKColor background) => OutputDriver.WriteLine(s.AddColor(text).AddColorB(background));


    /// <summary>
    ///     prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(string s, NKStyle st) => OutputDriver.Write(s.AddStyle(st));


    /// <summary>
    ///     prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(string s, NKStyle st) => OutputDriver.WriteLine(s.AddStyle(st));


    /// <summary>
    ///     prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(char s, NKStyle st) => OutputDriver.Write(s.AddStyle(st));


    /// <summary>
    ///     prints a string with the applied style
    /// </summary>
    /// <param name="s">string to print</param>
    /// <param name="st">style to be applied</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLine(char s, NKStyle st) => OutputDriver.WriteLine(s.AddStyle(st));
    

    /// <summary>
    ///     Moves the cursor to the specified position relatively to the current position.
    /// </summary>
    /// <param name="x">x offset from the current position</param>
    /// <param name="y">y offset from the current position</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MoveCursor(int x, int y) => Stdio.SetCursorPosition(Stdio.CursorLeft + x, Stdio.CursorTop + y);


    /// <summary>
    ///     Moves the console cursor to a specified position based on the provided indices.
    /// </summary>
    /// <param name="x">
    ///     The horizontal offset of the cursor.
    ///     Can be relative (from-start index) or absolute (from-end index).
    /// </param>
    /// <param name="y">
    ///     The vertical offset of the cursor.
    ///     Can be relative (from-start index) or absolute (from-end index).
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
    public static void MoveCursor(Index x, Index y) =>
        Stdio.SetCursorPosition(
            Math.Clamp(x.IsFromEnd ? x.Value : Stdio.CursorLeft + x.Value, 0, Stdio.BufferWidth  - 1),
            Math.Clamp(y.IsFromEnd ? y.Value : Stdio.CursorTop  + y.Value, 0, Stdio.BufferHeight - 1)
        );

    /// <summary>
    ///     Sets the cursor position in the console window.
    /// </summary>
    /// <param name="x">The horizontal position of the cursor, represented as an Index.</param>
    /// <param name="y">The vertical position of the cursor, represented as an Index.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
    public static void SetCursorPosition(Index x, Index y) =>
        Stdio.SetCursorPosition(
            Math.Clamp(x.IsFromEnd ? Stdio.BufferWidth  - x.Value : x.Value, 0, Stdio.BufferWidth  - 1),
            Math.Clamp(y.IsFromEnd ? Stdio.BufferHeight - y.Value : y.Value, 0, Stdio.BufferHeight - 1)
        );


    // ===================================================================================== //
    //                              ---  GRAPHICS WRITING  ---                               //
    // ===================================================================================== //


    // TODO: use SKBitmap or SKImage?

    public static void WriteImage(SKImage image, Point2D offset, Size2D charSize, SKSamplingOptions samplingOptions) {
        throw new NotImplementedException();
    }

    public static void WriteImage(SKBitmap image, Point2D offset, Size2D charSize, SKSamplingOptions samplingOptions) {
        throw new NotImplementedException();
    }

    #region SIXEL

    /// <summary>
    ///     Writes the given image to the console in the Sixel graphics format.
    /// </summary>
    /// <param name="image">
    ///     The bitmap image to be converted and written to the console in Sixel
    ///     format.
    /// </param>
    public static void WriteSixel(SKBitmap image) => OutputDriver.Write(image.ToSixel());

    /// <summary>
    ///     Renders a bitmap image in sixel format to the console output at the specified position.
    /// </summary>
    /// <param name="image">The bitmap image to render.</param>
    /// <param name="x">The x-coordinate of the starting position.</param>
    /// <param name="y">The y-coordinate of the starting position.</param>
    public static void WriteSixel(SKBitmap image, int x, int y) {
        if (x < 0 || y < 0)
            return;

        WriteSixel(image, ^x, ^y);
    }

    /// <summary>
    ///     Renders a given bitmap image as a sixel graphic at a specified position in the console.
    /// </summary>
    /// <param name="image">The bitmap image to be converted and displayed as sixel graphics.</param>
    /// <param name="x">
    ///     The horizontal position in the console buffer where the sixel graphic will
    ///     be rendered.
    /// </param>
    /// <param name="y">
    ///     The vertical position in the console buffer where the sixel graphic will be
    ///     rendered.
    /// </param>
    public static void WriteSixel(SKBitmap image, Index x, Index y) {
        SaveCursor();
        MoveCursor(x, y);
        OutputDriver.Write(image.ToSixel());
        RestoreCursor();
    }

    /// <summary>
    ///     Renders an SKBitmap image using the Sixel protocol at a specified position and size in
    ///     the console.
    /// </summary>
    /// <param name="image">The SKBitmap image to render.</param>
    /// <param name="x">The horizontal position (in columns) of the image's starting point.</param>
    /// <param name="y">The vertical position (in rows) of the image's starting point.</param>
    /// <param name="width">The width (in pixels) of the image to render.</param>
    /// <param name="height">The height (in pixels) of the image to render.</param>
    /// <param name="sampling">The SKSamplingOptions to apply for scaling the image, if specified.</param>
    public static void WriteSixel(
        SKBitmap           image,
        int                x,
        int                y,
        int                width,
        int                height,
        SKSamplingOptions? sampling = null
    ) =>
        WriteSixel(image, x < 0 ? 0 : ^x, y < 0 ? 0 : ^y, width, height, sampling);

    /// <summary>
    ///     Renders a given image as a sixel graphic at the specified position in the console.
    /// </summary>
    /// <param name="image">The bitmap image to render as a sixel graphic.</param>
    /// <param name="x">
    ///     The horizontal position in the console to render the image. Supports
    ///     relative or absolute indexing.
    /// </param>
    /// <param name="y">
    ///     The vertical position in the console to render the image. Supports relative
    ///     or absolute indexing.
    /// </param>
    /// <param name="width">The desired width for the rendered image in pixels.</param>
    /// <param name="height">The desired height for the rendered image in pixels.</param>
    /// <param name="sampling">
    ///     Optional sampling options for scaling the image. Defaults to
    ///     <see cref="SKSamplingOptions.Default" /> if not provided.
    /// </param>
    public static void WriteSixel(
        SKBitmap           image,
        Index              x,
        Index              y,
        int                width,
        int                height,
        SKSamplingOptions? sampling = null
    ) {
        if (width <= 0 || height <= 0)
            return;

        var bmp = new SKBitmap(width, height);
        image.ScalePixels(bmp, sampling ?? SKSamplingOptions.Default);

        SaveCursor();
        MoveCursor(x, y);
        OutputDriver.Write(bmp.ToSixel());
        RestoreCursor();
    }

    #endregion

}