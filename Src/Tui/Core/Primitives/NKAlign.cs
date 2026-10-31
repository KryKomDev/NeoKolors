// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics.CodeAnalysis;
using H = NeoKolors.Tui.Core.HorizontalAlign;
using V = NeoKolors.Tui.Core.VerticalAlign;

namespace NeoKolors.Tui.Core;

public readonly record struct NKAlign : IParsable<NKAlign> {
    public H Horizontal { get; }
    public V Vertical   { get; }

    public NKAlign(H horizontal, V vertical) {
        Horizontal = horizontal;
        Vertical   = vertical;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out NKAlign result) => TryParse(s, null, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out NKAlign result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;

            return false;
        }

        (result, bool success) = s switch {
            "left"         => (new NKAlign(H.LEFT,   V.CENTER), true),
            "top"          => (new NKAlign(H.CENTER, V.TOP),    true),
            "right"        => (new NKAlign(H.RIGHT,  V.CENTER), true),
            "bottom"       => (new NKAlign(H.CENTER, V.BOTTOM), true),
            "center"       => (new NKAlign(H.CENTER, V.CENTER), true),
            "top-left"     => (new NKAlign(H.LEFT,   V.TOP),    true),
            "top-right"    => (new NKAlign(H.RIGHT,  V.TOP),    true),
            "bottom-left"  => (new NKAlign(H.LEFT,   V.BOTTOM), true),
            "bottom-right" => (new NKAlign(H.RIGHT,  V.BOTTOM), true),
            _              => (default, false)
        };
        
        return success;
    }

    public static NKAlign Parse(string? s) =>
        TryParse(s, null, out var result)
            ? result
            : throw new FormatException($"Alignment '{s}' not in correct format.");

    public static NKAlign Parse(string? s, IFormatProvider? provider) {
        ArgumentNullException.ThrowIfNull(s);

        return TryParse(s, provider, out var result) 
            ? result 
            : throw new FormatException($"Alignment '{s}' not in correct format.");
    }

    public static NKAlign Center => new(H.CENTER, V.CENTER);

    public void Deconstruct(out H horizontal, out V vertical) {
        horizontal = Horizontal;
        vertical   = Vertical;
    }
}