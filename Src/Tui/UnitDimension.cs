// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui;

public readonly struct UnitDimension {

    #if NK_3_WIDE_PIXELS
    private const int PIXEL_WIDTH = 3;
    #else
    private const int PIXEL_WIDTH = 2;
    #endif
    
    public int Value { get; }
    public LengthUnit Unit { get; }
    
    public UnitDimension(int value, LengthUnit unit) {
        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// Calculates the scalar value of the dimension based on the parent value
    /// and the current unit type.
    /// </summary>
    /// <param name="parent">The parent value used for calculations when percentages are involved.</param>
    /// <returns>The computed scalar value as an integer.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the unit type is not
    /// a recognized <see cref="LengthUnit"/> value.</exception>
    public int GetScalar(int parent) => Unit switch {
        LengthUnit.PIXEL           => Value,
        LengthUnit.CHAR            => Value,
        LengthUnit.PERCENT         => (int)(parent             / 100f * Value),
        LengthUnit.VIEWPORT_WIDTH  => (int)(Stdio.BufferWidth  / 100f * Value),
        LengthUnit.VIEWPORT_HEIGHT => (int)(Stdio.BufferHeight / 100f * Value),
        _ => throw new ArgumentOutOfRangeException()
    };
    
    public int GetScalarX(int parent) => Unit switch {
        LengthUnit.PIXEL           => Value * PIXEL_WIDTH,
        LengthUnit.CHAR            => Value,
        LengthUnit.PERCENT         => (int)(parent             / 100f * Value),
        LengthUnit.VIEWPORT_WIDTH  => (int)(Stdio.BufferWidth  / 100f * Value),
        LengthUnit.VIEWPORT_HEIGHT => (int)(Stdio.BufferHeight / 100f * Value),
        _                          => throw new ArgumentOutOfRangeException()
    };
    
    public int GetScalarY(int parent) => Unit switch {
        LengthUnit.PIXEL           => Value,
        LengthUnit.CHAR            => Value,
        LengthUnit.PERCENT         => (int)(parent             / 100f * Value),
        LengthUnit.VIEWPORT_WIDTH  => (int)(Stdio.BufferWidth  / 100f * Value),
        LengthUnit.VIEWPORT_HEIGHT => (int)(Stdio.BufferHeight / 100f * Value),
        _                          => throw new ArgumentOutOfRangeException()
    };

    public override string ToString() => $"{(Value < 0 ? "- " : "")}{Math.Abs(Value)}{Unit.String}";
}