// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NeoKolors.Tui.Styles.Values;

public readonly struct GridDimensions : IParsableValue<GridDimensions> {
    public Dimension[] Columns { get; }
    public Dimension[] Rows { get; }
    
    public GridDimensions(Dimension[] columns, Dimension[] rows) {
        Columns = columns;
        Rows = rows;
    }
    
    public GridDimensions() {
        Columns = [Dimension.Auto];
        Rows = [Dimension.Auto];
    }
    
    public static GridDimensions Default => new();

    public static GridDimensions Parse(string s) => Parse(s, CultureInfo.InvariantCulture);
    
    public static GridDimensions Parse(string s, IFormatProvider? provider) {
        s = s.Trim();
        var rc = s.SubstringBetween('[', ']', true, true);
        var rr = s[(rc.Length + 4)..^2];

        var c = rc.Split(',').Select(a => Dimension.Parse(a.Trim()));
        var r = rr.Split(',').Select(a => Dimension.Parse(a.Trim()));

        return new GridDimensions(c.ToArray(), r.ToArray());
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out GridDimensions result) {
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

    GridDimensions IParsableValue<GridDimensions>.Parse(string s, IFormatProvider? provider) => Parse(s, provider);
    bool IParsableValue<GridDimensions>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out GridDimensions result) => TryParse(s, provider, out result);
}