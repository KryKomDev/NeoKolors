// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// Represents a font that does not convert strings to any glyph form but simply places the string.
/// </summary>
public class DefaultFont : IFont {
    
    private static readonly DefaultFontInfo INFO = new();
    
    public IFontInfo Info => INFO;
    
    public void PlaceString(string str, ICharCanvas canvas) {
        string[] lines = str.Split('\n');

        for (int y = 0; y < lines.Length; y++) {
            canvas.Place(lines[y], new Point2D(0, y));
        }
    }
    
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth) {
        var lines = str.Chop(maxWidth);
        
        for (int y = 0; y < lines.Length; y++) {
            canvas.Place(lines[y], new Point2D(0, y));
        }
    }

    public void PlaceString(
        string str, ICharCanvas canvas, Rectangle bounds, NKStyle style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP, bool overflow = false) 
    {
        var lines = str.Chop(bounds.Width);

        int y = verticalAlign switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => Math.Max(0, bounds.Height - lines.Length) / 2,
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - lines.Length),
            _                    => throw new ArgumentOutOfRangeException(nameof(verticalAlign), verticalAlign, null)
        } + bounds.LowerY;
        
        Func<int, int, int> computeXOffset = horizontalAlign switch {
            HorizontalAlign.LEFT   => static (_ ,  _) => 0,
            HorizontalAlign.CENTER => static (tw, lw) => Math.Max(0, tw - lw) / 2,
            HorizontalAlign.RIGHT  => static (tw, lw) => Math.Max(0, tw - lw),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlign), horizontalAlign, null)
        };
        
        for (int l = 0; l < lines.Length; l++) {
            if (!overflow && y != bounds.YRange)
                break;
            
            var s = lines[l];
            var x = computeXOffset(bounds.Width, s.Length) + bounds.LowerX;
            
            canvas.Place(s, new Point2D(x, y));
            canvas.Style(new Rectangle(new Point(x, y), new Size(s.Length, 1)), style);

            y++;
        }
    }

    public void PlaceString(AnsiString str, ICharCanvas canvas) {
        int x = 0, y = 0;
        var chars = str.ToArray();
        
        for (int i = 0; i < str.Length; i++) {
            if (chars[i] == '\n') {
                x = 0;
                y++;
            }
            else {
                canvas.Place(chars[i], new Point2D(x, y));
            }
        }
    }
    
    public void PlaceString(AnsiString str, ICharCanvas canvas, int maxWidth) {
        var lines = str.String.Chop(maxWidth);
        var chars = str.ToArray();

        int i = 0;
        int y = 0;
        for (int l = 0; l < lines.Length; l++) {
            for (int x = 0; x < lines[l].Length; x++) {
                canvas.Place(chars[i], new Point2D(x, y));
                i++;
            }

            y++;
        }
    }

    public void PlaceString(
        AnsiString str, ICharCanvas canvas, Rectangle bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool overflow = false) 
    {
        var lines = str.Chop(bounds.Width);

        int y = verticalAlign switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => Math.Max(0, bounds.Height - lines.Length) / 2,
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - lines.Length),
            _ => throw new ArgumentOutOfRangeException(nameof(verticalAlign), verticalAlign, null)
        } + bounds.LowerY;
        
        Func<int, int, int> computeXOffset = horizontalAlign switch {
            HorizontalAlign.LEFT   => static (_ ,  _) => 0,
            HorizontalAlign.CENTER => static (tw, lw) => Math.Max(0, tw - lw) / 2,
            HorizontalAlign.RIGHT  => static (tw, lw) => Math.Max(0, tw - lw),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlign), horizontalAlign, null)
        };
        
        for (int l = 0; l < lines.Length; l++) {
            if (!overflow && y != bounds.YRange)
                break;
            
            var s = lines[l];
            var x = computeXOffset(bounds.Width, s.Length) + bounds.LowerX;
            
            canvas.Place(s, new Point2D(x, y));

            y++;
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

    public Size GetMinSize(AnsiString str) => GetMinSize(str.String);

    public Size GetSize(AnsiString str) => GetSize(str.String);

    public Size GetSize(AnsiString str, int maxWidth) => GetSize(str.String, maxWidth);
}