// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics.CodeAnalysis;

namespace NeoKolors.Tui.Styles.Values;

internal readonly record struct CheckerBckg : IParsableValue<CheckerBckg> {
    public NKColor C1 { get; }
    public NKColor C2 { get; }
    public int Width { get; }
    public int Height { get; }
    public Size2D FieldSize => new(Width, Height);
    
    public bool Enabled { get; }

    public CheckerBckg(NKColor c1, NKColor c2, bool enabled = true, int width = 2, int height = 1) {
        C1 = c1;
        C2 = c2;
        Enabled = enabled;
        Width = width;
        Height = height;
    }


    public CheckerBckg Parse(string s, IFormatProvider? provider) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        var self = this;
        if (((IParsableValue<CheckerBckg>)self).TryParse(s, provider, out var result)) {
            return result;
        }
        throw new FormatException();
    }

    bool IParsableValue<CheckerBckg>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CheckerBckg result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }

        var c = s.Split(' ');
        if (c.Length != 2) {
            result = default;
            return false;
        }

        if (NKColor.TryParse(c[0], null, out var c1) && NKColor.TryParse(c[1], null, out var c2)) {
            result = new CheckerBckg(c1, c2);
            return true;
        }

        result = default;
        return false;
    }
}