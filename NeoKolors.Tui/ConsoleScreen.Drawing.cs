//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Common.Util;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles;
using SkiaSharp;

namespace NeoKolors.Tui;

public partial class ConsoleScreen : IConsoleScreen {
    
    /// <inheritdoc cref="IConsoleScreen.DrawText(string,int,int,NeoKolors.Common.NKStyle)"/>
    public void DrawText(string s, int x, int y, NKStyle style = default) {
        if (style == default) 
            style = new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);

        s = s.Replace("\n", "");
        
        if (y < 0 || y >= Height) 
            return;
        
        int iter = 0;
        for (int i = Math.Max(x, 0); i < Math.Min(x + s.Length, Width); i++) {
            char c = s[iter++ - Math.Min(x, 0)];
            
            if (c == '\0') continue;
            
            _chars[i, y] = c;
            _styles[i, y] <<= style;
        }
        
        List2D.SafeFill(_changes, true, x, y, x + s.Length - 1, y);
    }
    
    /// <inheritdoc cref="IConsoleScreen.DrawText(string,Rectangle,NKStyle,HorizontalAlign,VerticalAlign)"/>
    public void DrawText(string s, Rectangle border, NKStyle style = default,
        HorizontalAlign hAlign = HorizontalAlign.LEFT,
        VerticalAlign vAlign = VerticalAlign.TOP) 
    {
        if (style == default) 
            style = new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
        
        string[] lines = s.Chop(border.Width);
        int yOffset = vAlign switch {
            VerticalAlign.TOP => 0,
            VerticalAlign.CENTER => Math.Max(border.Height - lines.Length + 1, 0) / 2,
            VerticalAlign.BOTTOM => Math.Max(border.Height - lines.Length + 1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(vAlign), vAlign, null)
        };
        
        for (int i = 0; i < Math.Min(lines.Length, border.Height); i++) {
            int xOffset = hAlign switch {
                HorizontalAlign.LEFT => 0,
                HorizontalAlign.CENTER => Math.Max(border.Width - lines[i].Length, 0) / 2,
                HorizontalAlign.RIGHT => Math.Max(border.Width - lines[i].Length, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(hAlign), hAlign, null)
            };
            
            DrawText(lines[i], border.LowerX + xOffset, border.LowerY + yOffset + i, style);
        }
    }
    
    /// <inheritdoc cref="IConsoleScreen.DrawText(string,int,int,IFont,NeoKolors.Common.NKStyle)"/>
    public void DrawText(string s, int x, int y, IFont font, NKStyle style) {
        s = s.Replace("\0", "");
        s = s.Replace("\n", "");
        s = s.Replace("\r", "");
        
        int xOffset = 0;
        
        for (int i = 0; i < s.Length; i++) {
            if (s[i] == ' ') {
                xOffset += font.WordSpacing;
                continue;
            }
            
            var g = font.GetGlyph(s[i]);
            DrawGlyph(g, x + xOffset, y, style);
            xOffset += g.Width + font.LetterSpacing;
        }
    }

    /// <inheritdoc cref="IConsoleScreen.DrawText(string,Rectangle,IFont,NKStyle,HorizontalAlign,VerticalAlign,bool,bool)"/>
    public void DrawText(string s, Rectangle border, IFont font, NKStyle style = default,
        HorizontalAlign hAlign = HorizontalAlign.LEFT,
        VerticalAlign vAlign = VerticalAlign.TOP, 
        bool enableTopOverflow = true,
        bool enableBottomOverflow = true) 
    {
        s = s.Replace("\0", "");

        var lines = IFont.Chop(s, font, border.Width);

        int totalHeight = lines.Length * font.LineSize + (lines.Length - 1) * font.LineSpacing;
        
        int yOffset = vAlign switch {
            VerticalAlign.TOP => 0,
            VerticalAlign.CENTER => Math.Max(border.Height - totalHeight + 1, 0) / 2,
            VerticalAlign.BOTTOM => Math.Max(border.Height - totalHeight + 1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(vAlign), vAlign, null)
        };
        
        for (int l = 0; l < lines.Length; l++) {
            int lineWidth = 0;
            int chars = 0;
            foreach (var c in lines[l]) {
                if (c == ' ') {
                    lineWidth += font.WordSpacing;
                    continue;
                }
                
                chars++;
                var g = font.GetGlyph(c);
                lineWidth += g.Width;
            }
            
            lineWidth += chars * font.LetterSpacing;
            
            int xOffset = hAlign switch {
                HorizontalAlign.LEFT => 0,
                HorizontalAlign.CENTER => Math.Max(border.Width - lineWidth + 1, 0) / 2,
                HorizontalAlign.RIGHT => Math.Max(border.Width - lineWidth + 1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(hAlign), hAlign, null)
            };
        
            for (int i = 0; i < lines[l].Length; i++) {
                if (lines[l][i] == ' ') {
                    xOffset += font.WordSpacing;
                    continue;
                }
            
                var g = font.GetGlyph(lines[l][i]);
                DrawGlyph(g, border.LowerX + xOffset, 
                    border.LowerY + l * (font.LineSize + font.LineSpacing) + yOffset, 
                    style, border, enableTopOverflow, enableBottomOverflow);
                xOffset += g.Width + font.LetterSpacing;
            }
        }
    }

    private void DrawGlyph(IGlyph g, int x, int y, NKStyle style) {
        var lines = g.GetLines();

        for (int i = 0; i < lines.Length; i++) {
            DrawText(lines[i], x + g.XOffset, y + g.YOffset + i, style);
        }
    }

    private void DrawGlyph(IGlyph g, int x, int y, NKStyle style, Rectangle border, 
        bool enableTopOverflow = true, bool enableBottomOverflow = false) 
    {
        var lines = g.GetLines();

        for (int i = 0; i < lines.Length; i++) {
            int currY = y + g.YOffset + i;
            if ((currY > border.HigherY && !enableBottomOverflow) || (currY < border.LowerY && !enableTopOverflow)) 
                continue;
            DrawText(lines[i], x + g.XOffset, currY, style);
        }
    }

    /// <summary>
    /// tries to set a char at a position
    /// </summary>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    /// <param name="c">the character</param>
    public void TrySetChar(int x, int y, char c) {
        if (x >= 0 && x < Width && y >= 0 && y < Height) _chars[x, y] = c;
    }
    
    /// <summary>
    /// tries to set a style at a position
    /// </summary>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    /// <param name="s">the style</param>
    public void TrySetStyle(int x, int y, NKStyle s) {
        if (x >= 0 && x < Width && y >= 0 && y < Height) _styles[x, y] = s;
    }
    
    /// <summary>
    /// tries to safely set a style at a position
    /// </summary>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    /// <param name="s">the style</param>
    public void TrySafeSetStyle(int x, int y, NKStyle s) {
        if (x >= 0 && x < Width && y >= 0 && y < Height) _styles[x, y].Override(s);
    }

    /// <summary>
    /// Fills the entire console screen with the specified background color.
    /// </summary>
    /// <param name="color">The background color to fill the screen with.</param>
    public void Fill(NKColor color) {
        List2D.Fill(_changes, true);
        
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                _styles[x, y].BColor = color;
            }
        }
    }

    /// <inheritdoc cref="IConsoleScreen.DrawRect(Rectangle,NKColor,BorderStyle)"/>
    public void DrawRect(Rectangle rectangle, NKColor infill, BorderStyle borderStyle = default) {

        SetChangedRect(rectangle);
        
        int startX = Math.Max(rectangle.LowerX, 0);
        int endX = Math.Min(rectangle.HigherX, Width - 1);
        int startY = Math.Max(rectangle.LowerY, 0);
        int endY = Math.Min(rectangle.HigherY, Height - 1);

        // Fill the rectangle
        for (int x = startX; x <= endX; x++) {
            for (int y = startY; y <= endY; y++) {
                _chars[x, y] = ' ';
                _styles[x, y].SafeSetBColor(infill);
            }
        }

        if (borderStyle.IsBorderless) return;

        // Draw horizontal borders
        // top
        if (rectangle.LowerY >= 0 && rectangle.LowerY < Height) {
            for (int x = startX; x <= endX; x++) {
                _chars[x, rectangle.LowerY] = borderStyle.Horizontal;
                _styles[x, rectangle.LowerY].Override(borderStyle.StyleTop);
            }
        }

        // bottom
        if (rectangle.HigherY >= 0 && rectangle.HigherY < Height) {
            for (int x = startX; x <= endX; x++) {
                _chars[x, rectangle.HigherY] = borderStyle.Horizontal;
                _styles[x, rectangle.HigherY].Override(borderStyle.StyleBottom);
            }
        }

        // Draw vertical borders
        // left
        if (rectangle.LowerX >= 0 && rectangle.LowerX < Width) {
            for (int y = startY; y <= endY; y++) {
                _chars[rectangle.LowerX, y] = borderStyle.Vertical;
                _styles[rectangle.LowerX, y].Override(borderStyle.StyleLeft);
            }
        }

        // right
        if (rectangle.HigherX >= 0 && rectangle.HigherX < Width) {
            for (int y = startY; y <= endY; y++) {
                _chars[rectangle.HigherX, y] = borderStyle.Vertical;
                _styles[rectangle.HigherX, y].Override(borderStyle.StyleRight);
            }
        }

        // top left corner
        if (rectangle.LowerX >= 0 && rectangle.LowerX < Width &&
            rectangle.LowerY >= 0 && rectangle.LowerY < Height) 
        {
            _chars[rectangle.LowerX, rectangle.LowerY] = borderStyle.TopLeft;
            _styles[rectangle.LowerX, rectangle.LowerY].Override(borderStyle.StyleTopLeft);
        }
        
        // bottom left corner
        if (rectangle.LowerX >= 0 && rectangle.LowerX < Width &&
            rectangle.HigherY >= 0 && rectangle.HigherY < Height) 
        {
            _chars[rectangle.LowerX, rectangle.HigherY] = borderStyle.BottomLeft;
            _styles[rectangle.LowerX, rectangle.HigherY].Override(borderStyle.StyleBottomLeft);
        }
        
        // top right corner
        if (rectangle.HigherX >= 0 && rectangle.HigherX < Width &&
            rectangle.LowerY >= 0 && rectangle.LowerY < Height) 
        {
            _chars[rectangle.HigherX, rectangle.LowerY] = borderStyle.TopRight;
            _styles[rectangle.HigherX, rectangle.LowerY].Override(borderStyle.StyleTopRight);
        }
        
        // bottom right corner
        if (rectangle.HigherX >= 0 && rectangle.HigherX < Width &&
            rectangle.HigherY >= 0 && rectangle.HigherY < Height) 
        {
            _chars[rectangle.HigherX, rectangle.HigherY] = borderStyle.BottomRight;
            _styles[rectangle.HigherX, rectangle.HigherY].Override(borderStyle.StyleBottomRight);
        }
    }

    private void SetChangedRect(Rectangle rectangle) => List2D.Fill(_changes, true, 
            rectangle.LowerX.Clamp(0, Width - 1), rectangle.LowerY.Clamp(0, Height - 1), 
            rectangle.HigherX.Clamp(0, Width - 1), rectangle.HigherY.Clamp(0, Height - 1));

    public void DrawImage(SKBitmap bitmap, Rectangle border, SKSamplingOptions samplingOptions = default) {
        SetChangedRect(border);
        
        if (bitmap.IsNull || bitmap.IsEmpty) return;
        if (samplingOptions == default) samplingOptions = SKSamplingOptions.Default;
        
        var resized = bitmap.Resize(new SKImageInfo(border.Width, border.Height), samplingOptions);
        
        for (int y = 0; y < Math.Floor(resized.Height / 2f); y++) {
            for (int x = 0; x < resized.Width; x++) {
                _chars[x + border.LowerX, y + border.LowerY] = '▀';
                _styles[x + border.LowerX, y + border.LowerY].SafeSetFColor(resized.GetPixel(x, y * 2).SkiaToNK());
                if (y * 2 + 1 >= resized.Height) continue;
                _styles[x + border.LowerX, y + border.LowerY].SafeSetBColor(resized.GetPixel(x, y * 2 + 1).SkiaToNK());
            }
        }
    }
}