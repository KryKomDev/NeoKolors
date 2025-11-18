// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Extensions;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Fonts;

public class DefaultFont : IFont {
    
    private static readonly DefaultFontInfo INFO = new();
    
    public IFontInfo Info => INFO;
    
    public void PlaceString(string str, ICharCanvas canvas) {
        string[] lines = str.Split('\n');

        for (int y = 0; y < lines.Length; y++) {
            canvas.PlaceString(0, y, lines[y]);
        }
    }
    
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth) {
        var lines = str.Chop(maxWidth);
        
        for (int y = 0; y < lines.Length; y++) {
            canvas.PlaceString(0, y, lines[y]);
        }
    }

    public void PlaceString(
        string str, ICharCanvas canvas, Size bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP, bool overflow = false) 
    {
        var lines = str.Chop(bounds.Width);

        int yOffset = verticalAlign switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => (bounds.Height - lines.Length) / 2,
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - lines.Length),
            _ => throw new ArgumentOutOfRangeException(nameof(verticalAlign), verticalAlign, null)
        };

        for (int y = 0; y < (overflow ? lines.Length : Math.Min(lines.Length, bounds.Height)); y++) {
            int xOffset = horizontalAlign switch {
                HorizontalAlign.LEFT   => 0,
                HorizontalAlign.CENTER => (bounds.Width - lines[y].Length) / 2,
                HorizontalAlign.RIGHT  => bounds.Width - lines[y].Length,
                _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlign), horizontalAlign, null)
            };
            
            canvas.PlaceString(xOffset, y + yOffset, lines[y]);
        }
    }
}