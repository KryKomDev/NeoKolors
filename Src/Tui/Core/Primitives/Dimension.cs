// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

public readonly union Dimension(Dimension.Fixed, Dimension.Grow, Dimension.Fit) : IEquatable<Dimension> {

    /// <summary>
    ///     Represents a flexible dimension type that adapts to fit within a specified range,
    ///     constrained by a minimum and maximum size.
    /// </summary>
    public readonly record struct Fit {
        public Fixed Min { get; init; }
        public Fixed Max { get; init; }

        public Fit(Fixed min, Fixed max) {
            Min = min;
            Max = max;
        }

        public (int Min, int Max) ToSize(int parent, Size2D screen) {
            return (Min.ToSize(parent, screen), Max.ToSize(parent, screen));
        }
    }

    /// <summary>
    ///     Represents a fixed dimension type defined by a specific value and unit,
    ///     without any adaptability to available space or proportional constraints.
    /// </summary>
    public readonly record struct Fixed {
        public float Value { get; init; }
        public Units Unit  { get; init; }

        public Fixed(float value, Units unit) {
            Value = value;
            Unit  = unit;
        }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public static Fixed Zero { get; } = new(0, Units.CHARS);
        public static Fixed Max  { get; } = new(int.MaxValue, Units.CHARS);

        public int ToSize(int parent, Size2D screen) {
            return Unit switch {
                Units.CHARS   => (int)MathF.Round(Value),
                Units.PERCENT => (int)MathF.Round(parent   / 100f * Value),
                Units.VW      => (int)MathF.Round(screen.X / 100f * Value),
                Units.VH      => (int)MathF.Round(screen.Y / 100f * Value),
                _             => throw new ArgumentOutOfRangeException()
            };
        }

        public enum Units {

            /// <summary>
            ///     Represents a unit of measurement based on character width.
            /// </summary>
            /// <remarks>
            ///     This unit is typically used to define dimensions where character width is the primary
            ///     reference point, such as in text-based user interfaces or terminal-based layouts.
            /// </remarks>
            CHARS,

            /// <summary>
            ///     Represents a unit of measurement defined as a percentage.
            /// </summary>
            /// <remarks>
            ///     This unit is commonly used to specify dimensions as a proportion relative to a parent
            ///     container or context, such as in responsive layouts or scaling calculations.
            /// </remarks>
            PERCENT,

            /// <summary>
            ///     Represents a unit of measurement based on the viewport width.
            /// </summary>
            /// <remarks>
            ///     This unit is commonly used to define dimensions as a percentage of the total width of
            ///     the display area or viewport, often employed in responsive layouts or scaling user
            ///     interfaces.
            /// </remarks>
            VW,

            /// <summary>
            ///     Represents a unit of measurement relative to the viewport height.
            /// </summary>
            /// <remarks>
            ///     This unit is often used for defining dimensions based on the height of the visible
            ///     display area, making it ideal for responsive layouts that adjust dynamically to the
            ///     viewport's height.
            /// </remarks>
            VH
        }
    }

    /// <summary>
    ///     Represents a dynamic dimension type that grows proportionally based on its weight.
    /// </summary>
    public readonly record struct Grow {
        public int   Weight { get; init; }
        public Fixed Min    { get; init; }
        public Fixed Max    { get; init; }

        public Grow(int weight = 1, Fixed? min = null, Fixed? max = null) {
            Weight = weight;
            Min    = min ?? Fixed.Zero;
            Max    = max ?? Fixed.Max;
        }
    }

    public int Resolve(int parent, Size2D screen, int contentSize = 0, bool ungrown = false) {
        return Value switch {
            Fixed f => f.ToSize(parent, screen),
            Fit fit => Math.Clamp(
                contentSize,
                fit.Min.ToSize(parent, screen),
                fit.Max.ToSize(parent, screen)
            ),
            Grow g => ungrown
                ? Math.Max(contentSize, g.Min.ToSize(parent, screen))
                : Math.Clamp(
                    parent,
                    Math.Max(contentSize, g.Min.ToSize(parent, screen)),
                    Math.Max(Math.Max(contentSize, g.Min.ToSize(parent, screen)), g.Max.ToSize(parent, screen))
                ),
            _ => 0
        };
    }

    public static Dimension Auto => new(new Grow(1));
    public static Dimension Zero => Chars(0);

    public static Dimension Chars(float   value) => new(new Fixed(value, Fixed.Units.CHARS));
    public static Dimension Percent(float value) => new(new Fixed(value, Fixed.Units.PERCENT));
    public static Dimension Vw(float      value) => new(new Fixed(value, Fixed.Units.VW));
    public static Dimension Vh(float      value) => new(new Fixed(value, Fixed.Units.VH));

    public static Dimension FitContent() {
        return new Dimension(new Fit(Fixed.Zero, Fixed.Max));
    }

    public static Dimension FitContent(Fixed min, Fixed max) {
        return new Dimension(new Fit(min, max));
    }

    public static Dimension FitContent(float minChars, float maxChars = int.MaxValue) {
        return new Dimension(
            new Fit(new Fixed(minChars, Fixed.Units.CHARS), new Fixed(maxChars, Fixed.Units.CHARS))
        );
    }

    public static Dimension Fr(int weight = 1, Fixed? min = null, Fixed? max = null) {
        return new Dimension(new Grow(weight, min, max));
    }

    public static Dimension Fr(int weight, float minChars, float maxChars = int.MaxValue) {
        return new Dimension(
            new Grow(
                weight,
                new Fixed(minChars, Fixed.Units.CHARS),
                new Fixed(maxChars, Fixed.Units.CHARS)
            )
        );
    }

    public static Dimension GrowTrack(
        int   weight   = 1,
        float minChars = 0,
        float maxChars = int.MaxValue
    ) {
        return new Dimension(
            new Grow(
                weight,
                new Fixed(minChars, Fixed.Units.CHARS),
                new Fixed(maxChars, Fixed.Units.CHARS)
            )
        );
    }

    public static implicit operator Dimension(int chars) => Chars(chars);

    public bool Equals(Dimension other) => Equals(Value, other.Value);

    public override bool Equals(object? obj) => obj is Dimension other && Equals(other);

    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

    public static bool operator ==(Dimension left, Dimension right) => left.Equals(right);

    public static bool operator !=(Dimension left, Dimension right) => !left.Equals(right);
}
