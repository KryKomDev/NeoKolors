// NeoKolors
// Copyright (c) KryKom 2026

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Fonts;

public class JitterFont : DefaultFont {
    public override string Name => "Jitter";

    public override void DrawString(string str, ICharDrawer drawer, Point2D position = default, int z = 0) {
        str = RandomizeString(str);
        base.DrawString(str, drawer, position, z);
    }

    public override void DrawString(string str, ICharDrawer drawer, int maxWidth, Point2D position = default, int z = 0) {
        str = RandomizeString(str);
        base.DrawString(str, drawer, maxWidth, position, z);
    }

    public override void DrawString(
        string          str,
        ICharDrawer     drawer,
        Rect2D          bounds,
        NKStyle         style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool            overflow        = false,
        int             z               = 0
    ) {
        str = RandomizeString(str);
        base.DrawString(str, drawer, bounds, style, horizontalAlign, verticalAlign, overflow, z);
    }

    public override void DrawString(AnsiString str, ICharDrawer drawer, Point2D position = default, int z = 0) {
        var text = RandomizeString(str.Plain);
        str = new AnsiString(text, str.Styles.ToList());
        base.DrawString(str, drawer, position, z);
    }

    public override void DrawString(AnsiString str, ICharDrawer drawer, int maxWidth, Point2D position = default, int z = 0) {
        var text = RandomizeString(str.Plain);
        str = new AnsiString(text, str.Styles.ToList());
        base.DrawString(str, drawer, maxWidth, position, z);
    }

    public override void DrawString(
        AnsiString      str,
        ICharDrawer     drawer,
        Rect2D          bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool            overflow        = false,
        int             z               = 0
    ) {
        var text = RandomizeString(str.Plain);
        str = new AnsiString(text, str.Styles.ToList());
        base.DrawString(str, drawer, bounds, horizontalAlign, verticalAlign, overflow, z);
    }

    public override void DrawString(string str, ICharDrawer drawer, Rect2D bounds, TextRenderingOptions options, int z = 0) {
        str = RandomizeString(str);
        base.DrawString(str, drawer, bounds, options, z);
    }

    public override void DrawString(AnsiString str, ICharDrawer drawer, Rect2D bounds, TextRenderingOptions options, int z = 0) {
        var text = RandomizeString(str.Plain);
        str = new AnsiString(text, str.Styles.ToList());
        base.DrawString(str, drawer, bounds, options, z);
    }

    private static string RandomizeString(string input) {
        var sb = new StringBuilder();

        for (int i = 0; i < input.Length; i++) {
            var c = input[i];
            sb.Append(char.IsWhiteSpace(c) ? c : GetChar());
        }

        return sb.ToString();
    }

    private static readonly Random RANDOM     = new();
    private const           string CHARS      = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXZ0123456789!@#$%^&*()_+|~,./;':[]\\";
    private static readonly int    CHAR_COUNT = CHARS.Length;

    private static char GetChar() => CHARS[RANDOM.Next(0, CHAR_COUNT)];
}