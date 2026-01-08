//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui;

public struct Dimension : IParsableValue<Dimension> {
    
    public OneOf<UnitDimension, Auto, MinContent, MaxContent, DimensionExpression> Value { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the current <see cref="Dimension"/> instance
    /// represents a numerical value with an associated <see cref="LengthUnit"/>.
    /// </summary>
    /// <remarks>
    /// This property checks if the underlying value of the <see cref="Dimension"/>
    /// is a tuple containing an integer and a <see cref="LengthUnit"/> (e.g., Pixel, Char, or Percent).
    /// If the value is not numerical (e.g., Auto, MinContent, or MaxContent), this property will return false.
    /// </remarks>
    public bool IsNumber => Value.IsT0;

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

    private Dimension(DimensionExpression x) {
        Value = x;
    }
    
    public static Dimension Auto => new(new Auto());
    public static Dimension MinContent => new(new MinContent());
    public static Dimension MaxContent => new(new MaxContent());
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
            x => x.ToString()
        );
    }

    public int ToScalar(int parent) {
        return Value.Match(
            t => t.GetScalar(parent),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            x => x.ToScalar(parent)
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
        s = s.Trim().ToLowerInvariant();
        
        if (s == "auto") return Auto;
        if (s is "min-content" or "mincontent") return MinContent;
        if (s is "max-content" or "maxcontent") return MaxContent;
        
        var ops = s.Split(' ');

        if (ops.Length != 1) return ParseExpression(ops);
        
        if (s.EndsWith("px")) return Pixels(        int.Parse(s.Replace("px", "")));
        if (s.EndsWith("ch")) return Chars(         int.Parse(s.Replace("ch", "")));
        if (s.EndsWith('%'))  return Percent(       int.Parse(s.Replace("%",  "")));
        if (s.EndsWith("vw")) return ViewportWidth( int.Parse(s.Replace("vw", "")));
        if (s.EndsWith("vh")) return ViewportHeight(int.Parse(s.Replace("vh", "")));
        
        if (int.TryParse(s, out int val)) return Chars(val);
        
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
            result = Parse(s, provider);
            return true;
        }
        catch {
            result = default;
            return false;
        }
    }

    bool IParsableValue<Dimension>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Dimension result) => TryParse(s, provider, out result);
    Dimension IParsableValue<Dimension>.Parse(string s, IFormatProvider? provider) => Parse(s, provider);
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