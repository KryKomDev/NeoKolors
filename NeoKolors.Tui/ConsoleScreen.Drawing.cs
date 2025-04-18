//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Style;
using SkiaSharp;

// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable LoopCanBeConvertedToQuery

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
        for (int i = int.Max(x, 0); i < int.Min(x + s.Length, Width); i++) {
            char c = s[iter++ - int.Min(x, 0)];
            
            if (c == '\0') continue;
            
            Chars[i, y] = c;
            Styles[i, y].SafeSet(style);
        }
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
        
        for (int i = 0; i < int.Min(lines.Length, border.Height); i++) {
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
            
            IGlyph g = font.GetGlyph(s[i]);
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
            
                IGlyph g = font.GetGlyph(lines[l][i]);
                DrawGlyph(g, border.LowerX + xOffset, 
                    border.LowerY + l * (font.LineSize + font.LineSpacing) + yOffset, 
                    style, border, enableTopOverflow, enableBottomOverflow);
                xOffset += g.Width + font.LetterSpacing;
            }
        }
    }

    public void DrawGlyph(IGlyph g, int x, int y, NKStyle style) {
        var lines = g.GetLines();

        for (int i = 0; i < lines.Length; i++) {
            DrawText(lines[i], x + g.XOffset, y + g.YOffset + i, style);
        }
    }
    
    public void DrawGlyph(IGlyph g, int x, int y, NKStyle style, Rectangle border,
        bool enableTopOverflow = true, bool enableBottomOverflow = false) {
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
        if (x >= 0 && x < Width && y >= 0 && y < Height) Chars[x, y] = c;
    }
    
    /// <summary>
    /// tries to set a style at a position
    /// </summary>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    /// <param name="s">the style</param>
    public void TrySetStyle(int x, int y, NKStyle s) {
        if (x >= 0 && x < Width && y >= 0 && y < Height) Styles[x, y] = s;
    }
    
    /// <summary>
    /// tries to safely set a style at a position
    /// </summary>
    /// <param name="x">horizontal coordinate</param>
    /// <param name="y">vertical coordinate</param>
    /// <param name="s">the style</param>
    public void TrySafeSetStyle(int x, int y, NKStyle s) {
        if (x >= 0 && x < Width && y >= 0 && y < Height) Styles[x, y].SafeSet(s);
    }

    public void Fill(NKColor color) {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Styles[x, y].BColor = color;
            }
        }
    }

    /// <inheritdoc cref="IConsoleScreen.DrawRect(Rectangle,NKColor,BorderStyle)"/>
    public void DrawRect(Rectangle rectangle, NKColor infill, BorderStyle borderStyle = default) {
        int startX = Math.Max(rectangle.LowerX, 0);
        int endX = Math.Min(rectangle.HigherX, Width - 1);
        int startY = Math.Max(rectangle.LowerY, 0);
        int endY = Math.Min(rectangle.HigherY, Height - 1);

        // Fill the rectangle
        for (int x = startX; x <= endX; x++) {
            for (int y = startY; y <= endY; y++) {
                Styles[x, y].SafeSetBColor(infill);
                PixelChanges[x, y] = true;
            }
        }

        // Draw horizontal borders
        if (rectangle.LowerY >= 0 && rectangle.LowerY < Height) {
            for (int x = startX; x <= endX; x++) {
                Chars[x, rectangle.LowerY] = borderStyle.Horizontal;
                Styles[x, rectangle.LowerY].SafeSetBColor(borderStyle.BColor);
                Styles[x, rectangle.LowerY].SetFColor(borderStyle.FColor);
            }
        }

        if (rectangle.HigherY >= 0 && rectangle.HigherY < Height) {
            for (int x = startX; x <= endX; x++) {
                Chars[x, rectangle.HigherY] = borderStyle.Horizontal;
                Styles[x, rectangle.HigherY].SafeSetBColor(borderStyle.BColor);
                Styles[x, rectangle.HigherY].SetFColor(borderStyle.FColor);
            }
        }

        // Draw vertical borders
        if (rectangle.LowerX >= 0 && rectangle.LowerX < Width) {
            for (int y = startY; y <= endY; y++) {
                Chars[rectangle.LowerX, y] = borderStyle.Vertical;
                Styles[rectangle.LowerX, y].SafeSetBColor(borderStyle.BColor);
                Styles[rectangle.LowerX, y].SetFColor(borderStyle.FColor);
            }
        }

        if (rectangle.HigherX >= 0 && rectangle.HigherX < Width) {
            for (int y = startY; y <= endY; y++) {
                Chars[rectangle.HigherX, y] = borderStyle.Vertical;
                Styles[rectangle.HigherX, y].SafeSetBColor(borderStyle.BColor);
                Styles[rectangle.HigherX, y].SetFColor(borderStyle.FColor);
            }
        }

        if (rectangle.LowerX >= 0 && rectangle.LowerX < Width &&
            rectangle.LowerY >= 0 && rectangle.LowerY < Height) 
        {
            Chars[rectangle.LowerX, rectangle.LowerY] = borderStyle.TopLeft;
        }
        
        if (rectangle.LowerX >= 0 && rectangle.LowerX < Width &&
            rectangle.HigherY >= 0 && rectangle.HigherY < Height) 
        {
            Chars[rectangle.LowerX, rectangle.HigherY] = borderStyle.BottomLeft;
        }
        
        if (rectangle.HigherX >= 0 && rectangle.HigherX < Width &&
            rectangle.LowerY >= 0 && rectangle.LowerY < Height) 
        {
            Chars[rectangle.HigherX, rectangle.LowerY] = borderStyle.TopRight;
        }
        
        if (rectangle.HigherX >= 0 && rectangle.HigherX < Width &&
            rectangle.HigherY >= 0 && rectangle.HigherY < Height) 
        {
            Chars[rectangle.HigherX, rectangle.HigherY] = borderStyle.BottomRight;
        }
    }

    public void DrawImage(SKBitmap bitmap, Rectangle border, SKSamplingOptions samplingOptions = default) {
        if (bitmap.IsNull || bitmap.IsEmpty) return;
        if (samplingOptions == default) samplingOptions = SKSamplingOptions.Default;
        
        SKBitmap resized = bitmap.Resize(new SKImageInfo(border.Width, border.Height), samplingOptions);
        
        for (int y = 0; y < Math.Floor(resized.Height / 2f); y++) {
            for (int x = 0; x < resized.Width; x++) {
                Chars[x + border.LowerX, y + border.LowerY] = '▀';
                Styles[x + border.LowerX, y + border.LowerY].SafeSetFColor(resized.GetPixel(x, y * 2).SkiaToNK());
                if (y * 2 + 1 >= resized.Height) continue;
                Styles[x + border.LowerX, y + border.LowerY].SafeSetBColor(resized.GetPixel(x, y * 2 + 1).SkiaToNK());
            }
        }
    }
}