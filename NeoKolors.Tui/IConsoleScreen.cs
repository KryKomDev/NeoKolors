//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Events;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles.Values;
using SkiaSharp;

namespace NeoKolors.Tui;

public interface IConsoleScreen {
    
    /// <summary>
    /// width of the screen 
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// height of the screen 
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// writes the screen to the console
    /// </summary>
    public void Render();

    /// <summary>
    /// writes a string to the console screen
    /// </summary>
    /// <param name="s">string to write, should be plain with no escape characters</param>
    /// <param name="x">x starting pos</param>
    /// <param name="y">y starting pos</param>
    /// <param name="style">the style of the string</param>
    public void DrawText(string s, int x, int y, NKStyle style);

    /// <summary>
    /// writes a string to the console screen
    /// </summary>
    /// <param name="s">string to write, should be plain with no escape characters</param>
    /// <param name="border">the border</param>
    /// <param name="style">the style of the string</param>
    /// <param name="hAlign">the horizontal alignment of the string</param>
    /// <param name="vAlign">the vertical alignment of the string</param>
    public void DrawText(string s, Rectangle border, NKStyle style = default,
        HorizontalAlign hAlign = HorizontalAlign.LEFT,
        VerticalAlign vAlign = VerticalAlign.TOP);
    
    /// <summary>
    /// writes a string to the console screen
    /// </summary>
    /// <param name="s">string to write, should be plain with no escape characters</param>
    /// <param name="x">x starting pos</param>
    /// <param name="y">y starting pos</param>
    /// <param name="font">the font of the string</param>
    /// <param name="style">the style of the string</param>
    public void DrawText(string s, int x, int y, IFont font, NKStyle style);

    /// <summary>
    /// writes a string to the console screen
    /// </summary>
    /// <param name="s">string to write, should be plain with no escape characters</param>
    /// <param name="border">the border</param>
    /// <param name="font">the font to be applied</param>
    /// <param name="style">the style of the string</param>
    /// <param name="hAlign">the horizontal alignment of the string</param>
    /// <param name="vAlign">the vertical alignment of the string</param>
    /// <param name="enableTopOverflow">if true, renders the text that overflows at the top</param>
    /// <param name="enableBottomOverflow">if true, renders the text that overflows at the bottom</param>
    public void DrawText(string s, Rectangle border, IFont font, NKStyle style = default,
        HorizontalAlign hAlign = HorizontalAlign.LEFT,
        VerticalAlign vAlign = VerticalAlign.TOP, 
        bool enableTopOverflow = true,
        bool enableBottomOverflow = true);
    
    /// <summary>
    /// writes a rectangle to the console screen
    /// </summary>
    public void DrawRect(Rectangle rectangle, NKColor infill, BorderStyle borderStyle = default);

    /// <summary>
    /// writes an image to the console screen
    /// </summary>
    /// <param name="bitmap">the bitmap to be drawn</param>
    /// <param name="rectangle">the perimeter of the drawn image</param>
    /// <param name="samplingOptions">sampling options for SkiaSharp</param>
    public void DrawImage(SKBitmap bitmap, Rectangle rectangle, SKSamplingOptions samplingOptions = default);

    public void Resize(ResizeEventArgs args);
}