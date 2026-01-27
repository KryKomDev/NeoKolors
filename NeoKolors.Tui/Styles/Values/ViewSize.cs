// NeoKolors
// Copyright (c) 2025 KryKom

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

    public static ViewSize Parse(string s) {
        var args = s.Split(',').Select(a => a.Trim()).ToArray();

        if (args.Length is > 2 or 0)
            throw new FormatException();

        return args.Length == 2 
            ? new ViewSize(Dimension.Parse(args[0]), Dimension.Parse(args[1])) 
            : new ViewSize(Dimension.Parse(args[0]));
    }
    
    public ViewSize Parse(string s, IFormatProvider? provider) => Parse(s);

    public bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ViewSize result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }
        
        try {
            result = Parse(s);
            return true;
        }
        catch (FormatException) {
            result = default;
            return false;
        }
    }
}