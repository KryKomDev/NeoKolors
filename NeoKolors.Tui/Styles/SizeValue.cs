//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.InteropServices;
using OneOf;

namespace NeoKolors.Tui.Styles;

public struct SizeValue {
    public OneOf<(int Value, SizeUnit Unit), Auto, MinContent, MaxContent> Value { get; set; }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="SizeValue"/> instance
    /// represents a numerical value with an associated <see cref="SizeUnit"/>.
    /// </summary>
    /// <remarks>
    /// This property checks if the underlying value of the <see cref="SizeValue"/>
    /// is a tuple containing an integer and a <see cref="SizeUnit"/> (e.g., Pixel, Char, or Percent).
    /// If the value is not numerical (e.g., Auto, MinContent, or MaxContent), this property will return false.
    /// </remarks>
    public bool IsNumber => Value.IsT0;

    public SizeValue(int value, SizeUnit unit) {
        Value = (Value: value, Unit: unit);
    }

    private SizeValue(Auto auto) {
        Value = auto;
    }
    
    private SizeValue(MinContent minContent) {
        Value = minContent;
    }
    
    private SizeValue(MaxContent maxContent) {
        Value = maxContent;
    }
    
    public static SizeValue Auto => new(new Auto());
    public static SizeValue MinContent => new(new MinContent());
    public static SizeValue MaxContent => new(new MaxContent());
    public static SizeValue Zero => new(0, SizeUnit.CHAR);
    public static SizeValue Chars(int value) => new(value, SizeUnit.CHAR);
    public static SizeValue Pixels(int value) => new(value, SizeUnit.PIXEL);
    public static SizeValue Percent(int value) => new(value, SizeUnit.PERCENT);
    
    public static implicit operator SizeValue(int value) => new(value, SizeUnit.CHAR);

    public override string ToString() {
        return Value.Match(
            t => $"{t.Value} {t.Unit.ToString().ToLowerInvariant()}",
            _ => "auto",
            _ => "min-content",
            _ => "max-content"
        );
    }

    public int ToIntH(int dim) {
        return Value.Match(
            t => t.Unit switch {
                SizeUnit.PIXEL => t.Value * 2,
                SizeUnit.CHAR => t.Value,
                SizeUnit.PERCENT => t.Value * dim / 100,
                _ => 0
            },
            _ => dim,
            _ => 0,
            _ => 0
        );
    }
    
    public float ToFloatH(int dim) {
        return Value.Match(
            t => t.Unit switch {
                SizeUnit.PIXEL => t.Value * 2,
                SizeUnit.CHAR => t.Value,
                SizeUnit.PERCENT => t.Value * dim / 100f,
                _ => 0
            },
            _ => dim,
            _ => 0,
            _ => 0
        );
    }
    
    public int ToIntV(int dim) {
        return Value.Match(
            t => t.Unit switch {
                SizeUnit.PIXEL => t.Value,
                SizeUnit.CHAR => t.Value,
                SizeUnit.PERCENT => t.Value * dim / 100,
                _ => 0
            },
            _ => dim,
            _ => 0,
            _ => 0
        );
    }
    
    public float ToFloatV(int dim) {
        return Value.Match(
            t => t.Unit switch {
                SizeUnit.PIXEL => t.Value,
                SizeUnit.CHAR => t.Value,
                SizeUnit.PERCENT => t.Value * dim / 100f,
                _ => 0
            },
            _ => dim,
            _ => 0,
            _ => 0
        );
    }
}

public enum SizeUnit {
    
    /// <summary>
    /// 2x1 characters
    /// </summary>
    PIXEL,
    
    /// <summary>
    /// a single character
    /// </summary>
    CHAR,
    
    /// <summary>
    /// percentage of the (parent) element's size
    /// </summary>
    PERCENT
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