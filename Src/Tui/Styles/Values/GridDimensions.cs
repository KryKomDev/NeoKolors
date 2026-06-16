// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NeoKolors.Tui.Styles.Values;

public readonly struct GridDimensions : IParsableValue<GridDimensions> {
    public Dimension[] Columns { get; }
    public Dimension[] Rows { get; }
    
    public GridDimensions(Dimension[] columns, Dimension[] rows) {
        Columns = columns;
        Rows    = rows;
    }
    
    public GridDimensions() {
        Columns = [Dimension.Auto];
        Rows    = [Dimension.Auto];
    }
    
    public static GridDimensions Default => new();

    public static GridDimensions Parse(string s) => Parse(s, CultureInfo.InvariantCulture);

    /// <summary>
    /// Parses a string representation of grid dimensions into a <see cref="GridDimensions"/> instance.
    /// </summary>
    /// <param name="s">The string containing the grid dimensions to parse.</param>
    /// <param name="provider">An optional format provider to interpret the input string.</param>
    /// <returns>A new <see cref="GridDimensions"/> instance parsed from the specified string.</returns>
    /// <exception cref="ArgumentException">Thrown if the input string format is invalid.</exception>
    /// <exception cref="FormatException">Thrown if parsing a dimension fails.</exception>
    public static GridDimensions Parse(string s, IFormatProvider? provider) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (TryParse(s, provider, out var result)) return result;
        throw new FormatException($"Invalid grid dimensions: {s}");
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out GridDimensions result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }

        try {
            var trimmed = s.Trim();
            var rc = trimmed.SubstringBetween('[', ']', true, true);
            var rr = trimmed[(rc.Length + 4)..^2];

            var c = rc.Split(',').Select(a => Dimension.Parse(a.Trim()));
            var r = rr.Split(',').Select(a => Dimension.Parse(a.Trim()));

            result = new GridDimensions(c.ToArray(), r.ToArray());
            return true;
        }
        catch {
            result = default;
            return false;
        }
    }

    bool IParsableValue<GridDimensions>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out GridDimensions result) => TryParse(s, provider, out result);
}