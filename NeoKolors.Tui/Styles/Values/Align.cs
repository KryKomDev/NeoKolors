// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.CodeAnalysis;
using H = NeoKolors.Tui.Styles.Values.HorizontalAlign;
using V = NeoKolors.Tui.Styles.Values.VerticalAlign;

namespace NeoKolors.Tui.Styles.Values;

public struct Align : IParsableValue<Align> {
    public H Horizontal { get; }
    public V   Vertical   { get; }
    
    public Align(H horizontal, V vertical) {
        Horizontal = horizontal;
        Vertical   = vertical;
    }

    public static Align Parse(string s, IFormatProvider? provider) {
        return s switch {
            "left"         => new Align(H.LEFT,   V.CENTER),
            "top"          => new Align(H.CENTER, V.TOP),
            "right"        => new Align(H.RIGHT,  V.CENTER),
            "bottom"       => new Align(H.CENTER, V.BOTTOM),
            "center"       => new Align(H.CENTER, V.CENTER),
            "top-left"     => new Align(H.LEFT,   V.TOP),
            "top-right"    => new Align(H.RIGHT,  V.TOP),
            "bottom-left"  => new Align(H.LEFT,   V.BOTTOM),
            "bottom-right" => new Align(H.RIGHT,  V.BOTTOM),
            _ => throw new FormatException($"Alignment '{s}' not in correct format.")
        };
    }
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Align result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }
        
        try {
            result = Parse(s, provider);
            return true;
        }
        catch (FormatException) {
            result = default;
            return false;
        }
    }

    Align IParsableValue<Align>.Parse(string s, IFormatProvider? provider) 
        => Parse(s, provider);
    bool IParsableValue<Align>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Align result) 
        => TryParse(s, provider, out result);
    
    public static Align Center => new(H.CENTER, V.CENTER);
}

public enum HorizontalAlign {
    LEFT,
    CENTER,
    RIGHT
}

public enum VerticalAlign {
    TOP,
    CENTER,
    BOTTOM
}