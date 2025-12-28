//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.InteropServices;

namespace NeoKolors.Tui;

public struct Dimension {
    public OneOf<UnitDimension, Auto, MinContent, MaxContent> Value { get; set; }
    
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
            _ => "max-content"
        );
    }

    public int ToScalar(int parent) {
        return Value.Match(
            t => t.GetScalar(parent),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value."),
            _ => throw new InvalidOperationException("Cannot convert Auto, MinContent, or MaxContent to scalar value.")
        );
    }

    public int ToIntH(int dim) {
        return Value.Match(
            t => t.GetScalar(dim),
            _ => dim,
            _ => 0,
            _ => 0
        );
    }
    
    public float ToFloatH(int dim) {
        return Value.Match(
            t => t.GetScalar(dim),
            _ => dim,
            _ => 0,
            _ => 0
        );
    }
    
    public int ToIntV(int dim) {
        return Value.Match(
            t => t.GetScalar(dim),
            _ => dim,
            _ => 0,
            _ => 0
        );
    }
    
    public float ToFloatV(int dim) {
        return Value.Match(
            t => t.GetScalar(dim),
            _ => dim,
            _ => 0,
            _ => 0
        );
    }
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