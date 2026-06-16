// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics.CodeAnalysis;
using NeoKolors.Common;
using H = NeoKolors.Tui.Core.HorizontalAlign;
using V = NeoKolors.Tui.Core.VerticalAlign;

namespace NeoKolors.Tui.Core;

public readonly record struct Align : IParsableValue<Align> {
    public H Horizontal { get; }
    public V Vertical   { get; }
    
    public Align(H horizontal, V vertical) {
        Horizontal = horizontal;
        Vertical   = vertical;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Align result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }
        switch (s) {
            case "left":         result = new Align(H.LEFT,   V.CENTER); return true;
            case "top":          result = new Align(H.CENTER, V.TOP); return true;
            case "right":        result = new Align(H.RIGHT,  V.CENTER); return true;
            case "bottom":       result = new Align(H.CENTER, V.BOTTOM); return true;
            case "center":       result = new Align(H.CENTER, V.CENTER); return true;
            case "top-left":     result = new Align(H.LEFT,   V.TOP); return true;
            case "top-right":    result = new Align(H.RIGHT,  V.TOP); return true;
            case "bottom-left":  result = new Align(H.LEFT,   V.BOTTOM); return true;
            case "bottom-right": result = new Align(H.RIGHT,  V.BOTTOM); return true;
            default:
                result = default;
                return false;
        }
    }

    public static Align Parse(string s, IFormatProvider? provider) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (TryParse(s, provider, out var result)) return result;
        throw new FormatException($"Alignment '{s}' not in correct format.");
    }

    bool IParsableValue<Align>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Align result) 
        => TryParse(s, provider, out result);
    
    public static Align Center => new(H.CENTER, V.CENTER);

    public void Deconstruct(out H horizontal, out V vertical) {
        horizontal = Horizontal;
        vertical   = Vertical;
    }
}