// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.CodeAnalysis;

namespace NeoKolors.Tui.Styles.Values;

public readonly struct Position : IParsableValue<Position> {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<Position>();
    
    public Dimension X { get; }
    public Dimension Y { get; }
    public bool RelativeX { get; }
    public bool AbsoluteX => !RelativeX;
    public bool RelativeY { get; }
    public bool AbsoluteY => !RelativeY;

    public Position(Dimension x, Dimension y, bool relativeX = false, bool relativeY = false) {
        X = x;
        Y = y;
        RelativeX = relativeX;
        RelativeY = relativeY;
    }

    public Position() {
        X = Dimension.Zero;
        Y = Dimension.Zero;
        RelativeX = true;
        RelativeY = true;
    }

    public static Position Parse(string s) => Parse(s, null);
    
    public static Position Parse(string s, IFormatProvider? formatProvider) {
        var a = s.Split(',').Select(p => p.Trim()).ToArray();

        if (a.Length != 2) {
            LOGGER.Error("Invalid position: {0}", s);
            return new Position(Dimension.Zero, Dimension.Zero, true, true);
        }
        
        var rx = a[0];
        var ry = a[1];

        bool xa = rx[0] == '^';
        bool ya = ry[0] == '^';

        Dimension x = 0;
        Dimension y = 0;
        
        try {
            x = Dimension.Parse(xa ? rx[1..] : rx);
        }
        catch {
            LOGGER.Error("Invalid x-position: {0}", rx);
        }
        
        try {
            y = Dimension.Parse(ya ? ry[1..] : ry);
        }
        catch {
            LOGGER.Error("Invalid y-position: {0}", ry);
        }
        
        return new Position(x, y, !xa, !ya);
    }

    public static bool TryParse(string? s, IFormatProvider? formatProvider, out Position result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }

        try {
            result = Parse(s, formatProvider);
            return true;
        }
        catch {
            result = default;
            return false;
        }
    }
    
    
    Position IParsableValue<Position>.Parse(string s, IFormatProvider? provider) => Parse(s, provider);
    bool IParsableValue<Position>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Position result) => TryParse(s, provider, out result);
}