// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Fonts;

public class DefaultFont : IFont {
    
    private static readonly DefaultFontInfo INFO = new();
    
    public IFontInfo Info => INFO;
    
    public void PlaceString(string str, ICharCanvas canvas) {
        string[] lines = str.Split('\n');

        for (int y = 0; y < lines.Length; y++) {
            canvas.PlaceString(lines[y], new Point2D(0, y));
        }
    }
    
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth) {
        var lines = str.Chop(maxWidth);
        
        for (int y = 0; y < lines.Length; y++) {
            canvas.PlaceString(lines[y], new Point2D(0, y));
        }
    }

    public void PlaceString(
        string str, ICharCanvas canvas, Rectangle bounds, NKStyle style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP, bool overflow = false) 
    {
        var lines = str.Chop(bounds.Width);

        int yOffset = verticalAlign switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => (bounds.Height - lines.Length) / 2,
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - lines.Length),
            _ => throw new ArgumentOutOfRangeException(nameof(verticalAlign), verticalAlign, null)
        } + bounds.LowerY;

        for (int y = 0; y < (overflow ? lines.Length : Math.Min(lines.Length, bounds.Height)); y++) {
            int xOffset = horizontalAlign switch {
                HorizontalAlign.LEFT   => 0,
                HorizontalAlign.CENTER => (bounds.Width - lines[y].Length) / 2,
                HorizontalAlign.RIGHT  => bounds.Width - lines[y].Length,
                _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlign), horizontalAlign, null)
            } + bounds.LowerX;
            
            canvas.PlaceString(lines[y], new Point2D(xOffset, y + yOffset));
            canvas.Style(new Rectangle(xOffset, y + yOffset, xOffset + lines[y].Length, y + yOffset), style);
        }
    }

    public Size GetMinSize(string str) {
        int maxLength = str.Split(' ', '\n').Max(s => s.Length);
        int lineCount = str.Chop(maxLength).Length;
        
        return new Size(maxLength, lineCount);
    }
    
    public Size GetSize(string str) => new(str.Length, 1);
    public Size GetSize(string str, int maxWidth) {
        var l = str.Chop(maxWidth);
        var w = l.Select(s => s.Length).Max();
        return new Size(w, l.Length);
    }
}