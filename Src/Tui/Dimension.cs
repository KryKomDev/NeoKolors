//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace NeoKolors.Tui;

public struct Dimension : IParsableValue<Dimension> {
    
    public OneOf<UnitDimension, Auto, MinContent, MaxContent, DimensionExpression, Stretch> Value { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the current <see cref="Dimension"/> instance
    /// represents a numerical value with an associated <see cref="LengthUnit"/>.
    /// </summary>
    /// <remarks>
    /// This property checks if the underlying value of the <see cref="Dimension"/>
    /// is a tuple containing an integer and a <see cref="LengthUnit"/> (e.g., Pixel, Char, or Percent).
    /// If the value is not numerical (e.g., Auto, MinContent, or MaxContent), this property will return false.
    /// </remarks>
    public bool IsNumber => Value.IsT0 || Value.IsT4;

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Dimension"/> instance
    /// represents an automatic dimension, defined by the <see cref="Auto"/> type.
    /// </summary>
    /// <remarks>
    /// This property checks if the underlying value of the <see cref="Dimension"/>
    /// is of type <see cref="Auto"/>. If the value is not automatic (e.g., numerical, MinContent, or MaxContent),
    /// this property will return false.
    /// </remarks>
    public bool IsAuto => Value.IsT1;

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Dimension"/> instance
    /// represents the "min-content" intrinsic sizing behavior.
    /// </summary>
    /// <remarks>
    /// This property checks if the underlying value of the <see cref="Dimension"/>
    /// is of type <see cref="MinContent"/>, which signifies that the dimension should
    /// shrink to the minimum size required to fit its content. If the value is not
    /// of type <see cref="MinContent"/>, this property will return false.
    /// </remarks>
    public bool IsMinContent => Value.IsT2;

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Dimension"/> instance
    /// represents a <see cref="MaxContent"/> value.
    /// </summary>
    /// <remarks>
    /// This property checks if the underlying value of the <see cref="Dimension"/>
    /// is associated with the <see cref="MaxContent"/> type. If the value is numerical,
    /// <see cref="Auto"/>, or <see cref="MinContent"/>, this property will return false.
    /// </remarks>
    public bool IsMaxContent => Value.IsT3;

    /// <summary>
    /// Gets a value indicating whether the current <see cref="Dimension"/> instance
    /// represents a stretch value.
    /// </summary>
    /// <remarks>
    /// This property checks if the underlying value of the <see cref="Dimension"/>
    /// is categorized as a <see cref="Stretch"/> type. A stretch value is typically
    /// used to represent flexible or proportional sizing, where the dimension can
    /// adjust dynamically based on the available space.
    /// </remarks>
    public bool IsStretch => Value.IsT5;

    public Dimension(int value, LengthUnit unit) {
        Value = new UnitDimension(value, unit);
    }

    private Dimension(Auto auto) {
        Value = auto;
    }
    
    private Dimension(MinContent minContent) {
        Value = minContent;
    }
    
    private Dimension(MaxContent maxContent) {
        Value = maxContent;
    }

    private Dimension(Stretch stretch) {
        Value = stretch;
    }

    private Dimension(DimensionExpression x) {
        Value = x;
    }
    
    public static Dimension Auto => new(new Auto());
    public static Dimension MinContent => new(new MinContent());
    public static Dimension MaxContent => new(new MaxContent());
    public static Dimension Stretch => new(new Stretch());
    public static Dimension Zero => new(0, LengthUnit.CHAR);
    public static Dimension Chars(int value) => new(value, LengthUnit.CHAR);
    public static Dimension Pixels(int value) => new(value, LengthUnit.PIXEL);
    public static Dimension Percent(int value) => new(value, LengthUnit.PERCENT);
    public static Dimension ViewportWidth(int value) => new(value, LengthUnit.VIEWPORT_WIDTH);
    public static Dimension ViewportHeight(int value) => new(value, LengthUnit.VIEWPORT_HEIGHT);
    
    public static implicit operator Dimension(int value) => new(value, LengthUnit.CHAR);

    public override string ToString() {
        return Value.Match(
            t => $"{t.Value} {t.Unit.ToString().ToLowerInvariant()}",
            _ => "auto",
            _ => "min-content",
            _ => "max-content",
            x => x.ToString(),
            _ => "stretch"
        );
    }

    public int ToScalar(int parent) {
        return Value.Match(
            t => t.GetScalar(parent),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            x => x.ToScalar(parent),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to Scalar value.")
        );
    }
    
    public int ToScalarX(int parent) {
        return Value.Match(
            t => t.GetScalarX(parent),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            x => x.ToScalarX(parent),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to Scalar value.")
        );
    }
    
    public int ToScalarY(int parent) {
        return Value.Match(
            t => t.GetScalarY(parent),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            x => x.ToScalarY(parent),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to Scalar value.")
        );
    }

    public static implicit operator Dimension(DimensionExpression x) => new(x);

    public static DimensionExpression operator +(Dimension left, Dimension right) {
        if (!left.IsNumber || !right.IsNumber)
            throw new InvalidOperationException($"Cannot add non-number dimensions.");
        
        return new DimensionExpression(left, right);
    }

    public static DimensionExpression operator -(Dimension left, Dimension right) {
        if (!left.IsNumber || !right.IsNumber)
            throw new InvalidOperationException($"Cannot subtract non-number dimensions.");
        
        return new DimensionExpression(left, new Dimension(right.Value.AsT0.Value * -1, right.Value.AsT0.Unit));
    }
    
    public static DimensionExpression operator +(DimensionExpression left, Dimension right) {
        if (!right.IsNumber)
            throw new InvalidOperationException($"Cannot add non-number dimensions.");
        
        left.AddOperand(right);
        
        return left;
    }

    public static DimensionExpression operator -(DimensionExpression left, Dimension right) {
        if (!right.IsNumber)
            throw new InvalidOperationException($"Cannot subtract non-number dimensions.");

        left.AddOperand(new Dimension(right.Value.AsT0.Value * -1, right.Value.AsT0.Unit));
        
        return left;
    }

    public static Dimension Parse(string s) => Parse(s, CultureInfo.InvariantCulture);

    public static Dimension Parse(string s, IFormatProvider? provider) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (TryParse(s, provider, out var result)) return result;
        throw new FormatException($"Invalid dimension: {s}");
    }

    private static Dimension ParseExpression(string[] ops) {
        var e = new DimensionExpression();
        
        bool flip = false;
        foreach (var o in ops) {
            var t = o.Trim();

            if (t == "-") {
                flip = true;
            }
            else if (t == "+") {
                flip = false;
            }
            else {
                var d = Parse(t);
                
                if (flip && d.IsNumber) {
                    d = new Dimension(d.Value.AsT0.Value * -1, d.Value.AsT0.Unit);
                }
                
                e.AddOperand(d);
            }
        }
        
        return e;
    }
    
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Dimension result) {
        if (string.IsNullOrEmpty(s)) {
            result = default;
            return false;
        }

        try {
            var trimmed = s.Trim().ToLowerInvariant();
            
            if (trimmed == "auto") { result = Auto; return true; }
            if (trimmed is "min-content" or "mincontent") { result = MinContent; return true; }
            if (trimmed is "max-content" or "maxcontent") { result = MaxContent; return true; }
            if (trimmed is "stretch") { result = Stretch; return true; }
            
            var ops = trimmed.Split(' ');

            if (ops.Length != 1) {
                result = ParseExpression(ops);
                return true;
            }
            
            if (trimmed.EndsWith("px")) { result = Pixels(        int.Parse(trimmed.Replace("px", ""))); return true; }
            if (trimmed.EndsWith("ch")) { result = Chars(         int.Parse(trimmed.Replace("ch", ""))); return true; }
            if (trimmed.EndsWith('%'))  { result = Percent(       int.Parse(trimmed.Replace("%",  ""))); return true; }
            if (trimmed.EndsWith("vw")) { result = ViewportWidth( int.Parse(trimmed.Replace("vw", ""))); return true; }
            if (trimmed.EndsWith("vh")) { result = ViewportHeight(int.Parse(trimmed.Replace("vh", ""))); return true; }
            
            if (int.TryParse(trimmed, out int val)) { result = Chars(val); return true; }
            
            result = default;
            return false;
        }
        catch {
            result = default;
            return false;
        }
    }

    bool IParsableValue<Dimension>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Dimension result) => TryParse(s, provider, out result);
}

/// <summary>
/// works as a value for automatic sizing
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 0)]
public record struct Auto;

/// <summary>
/// works as a value for minimum content size
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 0)]
public record struct MinContent;

/// <summary>
/// works as a value for maximum content size
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 0)]
public record struct MaxContent;

/// <summary>
/// Represents a value that makes the margin box stretch to fill available space.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 0)]
public record struct Stretch;