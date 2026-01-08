// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.CodeAnalysis;

namespace NeoKolors.Tui.Styles.Values;

public struct Spacing : IParsableValue<Spacing> {
    public Dimension Left { get; }
    public Dimension Right { get; }
    public Dimension Top { get; }
    public Dimension Bottom { get; }
    
    public Spacing(Dimension left, Dimension right, Dimension top, Dimension bottom) {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }
    
    public Spacing(Dimension value) : this(value, value, value, value) { }
    
    public Spacing(Dimension horizontal, Dimension vertical) : this(horizontal, horizontal, vertical, vertical) { }
    
    public Spacing() : this(Dimension.Zero) { }
    public static Spacing Zero => new();

    public static Spacing Parse(string s) => Parse(s, null);
    
    public static Spacing Parse(string s, IFormatProvider? provider) {
        var args = s.Split(' ');

        return args.Length switch {
            1 => new Spacing(Dimension.Parse(args[0])),
            2 => new Spacing(Dimension.Parse(args[0]), Dimension.Parse(args[1])),
            3 => new Spacing(Dimension.Parse(args[0]), Dimension.Parse(args[1]), Dimension.Parse(args[2]), Dimension.Zero),
            _ => new Spacing(Dimension.Parse(args[0]), Dimension.Parse(args[1]), Dimension.Parse(args[2]), Dimension.Parse(args[3]))
        };
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Spacing result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }

        try {
            result = Parse(s, provider);
            return true;
        }
        catch {
            result = default;
            return false;
        }
    }

    Spacing IParsableValue<Spacing>.Parse(string s, IFormatProvider? provider) => Parse(s, provider);
    bool IParsableValue<Spacing>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Spacing result) => TryParse(s, provider, out result);
}