// NeoKolors
// Copyright (c) 2026 KryKom

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
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (TryParse(s, formatProvider, out var result)) return result;
        throw new FormatException($"Invalid position: '{s}'");
    }

    public static bool TryParse(string? s, IFormatProvider? formatProvider, out Position result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }

        try {
            var a = s.Split(',').Select(p => p.Trim()).ToArray();

            if (a.Length != 2) {
                LOGGER.Error("Invalid position: {0}", s);
                result = new Position(Dimension.Zero, Dimension.Zero, true, true);
                return false;
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
                result = default;
                return false;
            }
            
            try {
                y = Dimension.Parse(ya ? ry[1..] : ry);
            }
            catch {
                LOGGER.Error("Invalid y-position: {0}", ry);
                result = default;
                return false;
            }
            
            result = new Position(x, y, !xa, !ya);
            return true;
        }
        catch {
            result = default;
            return false;
        }
    }
    
    bool IParsableValue<Position>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Position result) => TryParse(s, provider, out result);
}