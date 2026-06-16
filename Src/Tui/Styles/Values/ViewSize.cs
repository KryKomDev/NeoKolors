// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics.CodeAnalysis;

namespace NeoKolors.Tui.Styles.Values;

public struct ViewSize : IParsableValue<ViewSize> {
    public Dimension Horizontal { get; }
    public Dimension Vertical   { get; }

    public ViewSize(Dimension horizontal, Dimension vertical) {
        Horizontal = horizontal;
        Vertical   = vertical;
    }

    public ViewSize(Dimension dim) {
        Horizontal = dim;
        Vertical   = dim;
    }

    public ViewSize() {
        Horizontal = Dimension.Auto;
        Vertical   = Dimension.Auto;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ViewSize result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }

        try {
            var args = s.Split(',').Select(a => a.Trim()).ToArray();

            if (args.Length is > 2 or 0) {
                result = default;
                return false;
            }

            if (args.Length == 2) {
                if (Dimension.TryParse(args[0], null, out var h) && Dimension.TryParse(args[1], null, out var v)) {
                    result = new ViewSize(h, v);
                    return true;
                }
            }
            else {
                if (Dimension.TryParse(args[0], null, out var d)) {
                    result = new ViewSize(d);
                    return true;
                }
            }

            result = default;
            return false;
        }
        catch {
            result = default;
            return false;
        }
    }

    public static ViewSize Parse(string s) => Parse(s, null);

    public static ViewSize Parse(string s, IFormatProvider? provider) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (TryParse(s, provider, out var result)) return result;
        throw new FormatException($"Invalid ViewSize: '{s}'");
    }

    bool IParsableValue<ViewSize>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ViewSize result) => TryParse(s, provider, out result);
}