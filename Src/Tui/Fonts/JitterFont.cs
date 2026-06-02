// NeoKolors
// Copyright (c) krystof 2026

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Fonts;

public class JitterFont : DefaultFont {
    public override string Name => "Jitter";

    public override void PlaceString(string str, ICharCanvas canvas) {
        str = RandomizeString(str);
        base.PlaceString(str, canvas);
    }
    
    public override void PlaceString(string str, ICharCanvas canvas, int maxWidth) {
        str = RandomizeString(str);
        base.PlaceString(str, canvas, maxWidth);
    }
    
    public override void PlaceString(
        string str,
        ICharCanvas canvas,
        Area2D bounds,
        NKStyle style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP,
        bool overflow = false) 
    {
        str = RandomizeString(str);
        base.PlaceString(str, canvas, bounds, style, horizontalAlign, verticalAlign, overflow);
    }
    
    public override void PlaceString(AnsiString str, ICharCanvas canvas) {
        var text = RandomizeString(str.Plain);
        str = new AnsiString(text, str.Styles.ToList());
        base.PlaceString(str, canvas);
    }
    
    public override void PlaceString(AnsiString str, ICharCanvas canvas, int maxWidth) {
        var text = RandomizeString(str.Plain);
        str = new AnsiString(text, str.Styles.ToList());
        base.PlaceString(str, canvas, maxWidth);
    }
    
    public override void PlaceString(
        AnsiString str,
        ICharCanvas canvas,
        Area2D bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP,
        bool overflow = false) 
    {
        var text = RandomizeString(str.Plain);
        str = new AnsiString(text, str.Styles.ToList());
        base.PlaceString(str, canvas, bounds, horizontalAlign, verticalAlign, overflow);
    }

    private static string RandomizeString(string input) {
        var sb = new StringBuilder();

        for (int i = 0; i < input.Length; i++) {
            var c = input[i];
            sb.Append(char.IsWhiteSpace(c) ? c : GetChar());
        }

        return sb.ToString();
    }

    private static readonly Random RANDOM = new();
    private const           string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXZ0123456789!@#$%^&*()_+|~,./;':[]\\";
    private static readonly int    CHAR_COUNT = CHARS.Length;
    
    private static char GetChar() => CHARS[RANDOM.Next(0, CHAR_COUNT)];
}