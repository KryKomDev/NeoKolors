//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Text;
using OneOf;

namespace NeoKolors.Common;

/// <summary>
/// color structure that can hold every color supported by the console (+ARGB colors) 
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
public readonly record struct NKColor : IFormattable, IParsableValue<NKColor> {
    
    /// <summary>
    /// Represents the underlying 32-bit unsigned integer value used to store the color data,
    /// including the color type and the specific color information.
    /// The color type is encoded in the upper 2 bits of the value.
    /// The lower 24 bits are used to store the actual color information.
    /// 6 bits remain unused...
    /// </summary>
    [FieldOffset(0)]
    private readonly uint _value;

    [FieldOffset(3)]
    private readonly byte _type;
    
    public OneOf<uint, NKConsoleColor, DefaultColor, InheritColor> Value {
        get {
            return (ColorType)_type switch {
                ColorType.RGB           => _value & 0x00ffffff,
                ColorType.CONSOLE_COLOR => (NKConsoleColor)(_value & 0x000000ff),
                ColorType.DEFAULT       => new DefaultColor(),
                ColorType.INHERIT       => new InheritColor(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    /// <summary>
    /// Executes specific actions based on the type of the color.
    /// </summary>
    /// <param name="default">Action to execute when the color is of type DefaultColor.</param>
    /// <param name="rgb">Action to execute when the color is an RGB value.</param>
    /// <param name="palette">Action to execute when the color is a console palette color.</param>
    /// <param name="inherit">Action to execute when the color is of type InheritColor.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the color type is invalid.</exception>
    public void Switch(
        Action<DefaultColor>   @default,
        Action<uint>           rgb,
        Action<NKConsoleColor> palette,
        Action<InheritColor>   inherit) 
    {
        switch ((ColorType)_type) {
            case ColorType.DEFAULT:       @default(new DefaultColor()); break;
            case ColorType.CONSOLE_COLOR: palette(AsPalette);           break;
            case ColorType.RGB:           rgb(AsRgb);                   break;
            case ColorType.INHERIT:       inherit(new InheritColor());  break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Applies different processing logic based on the type of color.
    /// </summary>
    /// <typeparam name="T">The return type of the processing logic.</typeparam>
    /// <param name="default">Function to execute when the color is a default color.</param>
    /// <param name="rgb">Function to execute when the color is an RGB value.</param>
    /// <param name="palette">Function to execute when the color is a console palette color.</param>
    /// <param name="inherit">Function to execute when the color is inherited.</param>
    /// <returns>The result of the function corresponding to the color type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the color type is invalid.</exception>
    public T Match<T>(
        Func<DefaultColor,   T> @default,
        Func<uint,           T> rgb,
        Func<NKConsoleColor, T> palette,
        Func<InheritColor,   T> inherit)
    {
        return (ColorType)_type switch {
            ColorType.DEFAULT       => @default(new DefaultColor()),
            ColorType.RGB           => rgb(AsRgb),
            ColorType.CONSOLE_COLOR => palette(AsPalette),
            ColorType.INHERIT       => inherit(new InheritColor()),
            _                       => throw new ArgumentOutOfRangeException()
        };
    }

    public ColorType Type => (ColorType)(_type);
    
    public bool IsRgb     => _type == (byte)ColorType.RGB;
    public bool IsPalette => _type == (byte)ColorType.CONSOLE_COLOR;
    public bool IsDefault => _type == (byte)ColorType.DEFAULT;
    public bool IsInherit => _type == (byte)ColorType.INHERIT;
    
    public uint AsRgb => _value & 0x00ffffff;
    public NKConsoleColor AsPalette => (NKConsoleColor)(_value & 0x000000ff);
    
    // ====== CONSTRUCTORS ======

    public NKColor(int rgb) {
        _value = (uint)(rgb & 0x00ffffff);
        _type  = (byte)ColorType.RGB;
    }

    public NKColor(NKConsoleColor consoleColor) {
        _value = (byte)consoleColor;
        _type  = (byte)ColorType.CONSOLE_COLOR;
    }

    public NKColor(ConsoleColor consoleColor) {
        _value = (byte)ColorFormat.SystemToNK(consoleColor);
        _type  = (byte)ColorType.CONSOLE_COLOR;
    }

    public NKColor() {
        _value = 0;
        _type  = (byte)ColorType.DEFAULT;
    }

    private NKColor(ColorType type, uint value = 0) {
        _value = value & 0x00ffffff;
        _type  = (byte)type;
    }
    
    /// <summary>
    /// Returns a new color with the default console value.
    /// </summary>
    public static NKColor Default => new(ColorType.DEFAULT);
    
    /// <summary>
    /// Returns a new color that indicates that the color should be inherited (not overriden).
    /// </summary>
    public static NKColor Inherit => new(ColorType.INHERIT);
    
    public static NKColor FromRgb(byte r, byte g, byte b) => new((r << 16) | (g << 8) | b);
    public static NKColor FromRgb(uint hex) => new(ColorType.RGB, hex);
    public static NKColor FromRgb(int hex) => new(hex);
    
    
    // ====== IMPLICIT CONVERSIONS ======

    public static implicit operator NKColor(NKConsoleColor color) => new(color);
    public static implicit operator NKColor(ConsoleColor color) => new(color);
    public static implicit operator NKColor(uint color) => FromRgb(color);

    public static implicit operator NKColor(int color) => FromRgb(color);

    public static implicit operator NKConsoleColor(NKColor color) =>
        color.Match(
            _ => throw InvalidColorCastException.DefaultToConsole(),
            _ => throw InvalidColorCastException.RgbToConsole(),
            c => c,
            _ => throw InvalidColorCastException.InheritToConsole()
        );

    public static implicit operator uint(NKColor color) =>
        color.Match(
            _ => throw InvalidColorCastException.DefaultToRgb(),
            i => i,
            _ => throw InvalidColorCastException.ConsoleToRgb(),
            _ => throw InvalidColorCastException.InheritToConsole()
        );
    
    public static implicit operator int(NKColor color) =>
        color.Match(
            _ => throw InvalidColorCastException.DefaultToRgb(),
            i => (int)(i & 0x00ffffff),
            _ => throw InvalidColorCastException.ConsoleToRgb(),
            _ => throw InvalidColorCastException.InheritToConsole()
        );
    
    public bool Equals(NKColor? other) => other.HasValue && _value == other.Value._value;

    public override int GetHashCode() {
        return _value.GetHashCode();
    }

    /// <summary>
    /// Represents the ANSI control string or textual representation
    /// of the foreground color associated with the current <see cref="NKColor"/> instance.
    /// </summary>
    public string Text =>
        Match(
            _ => EscapeCodes.TEXT_COLOR_RESET,
            i => i.ControlChar(),
            c => c.ControlChar(),
            _ => "Inherit"
        );

    /// <summary>
    /// Returns the ANSI escape code or control character string representing
    /// the background color associated with the current <see cref="NKColor"/> instance.
    /// </summary>
    public string Bckg =>
        Match(
            _ => EscapeCodes.BCKG_COLOR_RESET,
            i => i.ControlCharB(),
            c => c.ControlCharB(),
            _ => "Inherit"
        );

    public string Underline =>
        Match(
            _ => EscapeCodes.UNDERLINE_COLOR_RESET,
            i => i.ControlCharU(),
            c => c.ControlCharU(),
            _ => "Inherit"
        );

    public void Write() =>
        Console.Write(Match(
            _ => "Default",
            i => $"{"●".AddColor(i)} #{i:x6}",
            c => $"{"●".AddColor(c)} {Enum.GetName(typeof(NKConsoleColor), c)}",
            _ => "Inherit"
        ));

    public override string ToString() =>
        Match(
            _ => "Default",
            i => $"{i:x6}",
            c => $"{Enum.GetName(typeof(NKConsoleColor), c)}",
            _ => "Inherit"
        );

    public string ToString(string format) => 
        ToString(format, CultureInfo.InvariantCulture);
    
    public string ToString(string? format, IFormatProvider? formatProvider) {
        if (string.IsNullOrEmpty(format)) format = "T";
        return format switch {
            "#p" or "#P" or "#Plain" or "#r" or "#R" or "#Raw" => "#" + ToString(),
            "p" or "P" or "Plain" or "r" or "R" or "Raw" => ToString(),
            "t" or "T" or "Text" or "f" or "F" or "Forg" => Text,
            "b" or "B" or "Bckg" => Bckg,
            "u" or "U" or "Underline" => Underline,
            _ => Text
        };
    }


    public static void PrintColorCube() {
        for (int z = 0; z < 6; z++) {
            for (int y = 0; y < 6; y++) {
                for (int x = 0; x < 6; x++) {
                    Console.Write($"{z * 36 + y * 6 + x + 16:x2}"
                        .AddColorB((byte)(z * 255 / 5), (byte)(y * 255 / 5), (byte)(x * 255 / 5))
                        .AddColor(
                            FromRgb((byte)(z * 255 / 5), (byte)(y * 255 / 5), (byte)(x * 255 / 5)).GetInverse()));
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }

    public NKColor GetInverse() {
        switch (Type) {
            case ColorType.RGB:
                byte r = (byte)(_value >> 16);
                byte g = (byte)(_value >> 08);
                byte b = (byte) _value;
                
                return FromRgb((byte)(255 - r), (byte)(255 - g), (byte)(255 - b));
            case ColorType.CONSOLE_COLOR:
                byte c = (byte)(_value & 0x000000ff);
                return new NKColor((NKConsoleColor)((c + 8) % 16));
            case ColorType.DEFAULT:
                return Default;
            case ColorType.INHERIT:
                return Inherit;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool Equals(NKColor other) => _value == other._value;
    
    public static NKColor Parse(string s) => Parse(s, CultureInfo.InvariantCulture);
    
    public static NKColor Parse(string s, IFormatProvider? provider) {
        s = s.Trim();
        
        if (s.StartsWith('#')) {
            var hex = s[1..];
            
            return uint.TryParse(hex, NumberStyles.HexNumber, null, out uint rgb) 
                ? FromRgb(rgb)
                : throw new FormatException($"Invalid color: {s}");
        }
        
        if (string.Equals(s, "Default", StringComparison.OrdinalIgnoreCase)) return Default;
        if (string.Equals(s, "Inherit", StringComparison.OrdinalIgnoreCase)) return Inherit;

        return Enum.TryParse<NKConsoleColor>(s, true, out var nkc) 
            ? new NKColor(nkc)
            : throw new FormatException($"Invalid color: {s}");
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out NKColor result) {
        if (string.IsNullOrEmpty(s)) {
            result = Default;
            return true;
        }

        try {
            result = Parse(s, provider);
            return true;
        }
        catch {
            result = Default;
            return false;
        }
    }
    
    NKColor IParsableValue<NKColor>.Parse(string s, IFormatProvider? provider) => Parse(s, provider);
    bool IParsableValue<NKColor>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out NKColor result) => TryParse(s, provider, out result);

    /// <summary>
    /// Performs linear interpolation (Lerp) between two RGB colors based on a specified fraction.
    /// </summary>
    /// <param name="start">The starting color as an NKColor in RGB format.</param>
    /// <param name="end">The ending color as an NKColor in RGB format.</param>
    /// <param name="fraction">The interpolation factor, where 0 represents the start color and 1
    /// represents the end color. Values outside the range [0, 1] will be clamped.</param>
    /// <returns>The interpolated NKColor as an RGB value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when either the start or end color (or both) are
    /// not in RGB format.</exception>
    public static NKColor Lerp(NKColor start, NKColor end, float fraction) {
        if (!start.IsRgb && !end.IsRgb)
            throw new InvalidOperationException("Both colors must be RGB to use Linear Interpolation (Lerp).");

        var s = start.AsRgb;
        var e = end  .AsRgb;
        
        fraction = Math.Clamp(fraction, 0f, 1f);
        byte r = (byte)(s.R + (e.R - s.R) * fraction);
        byte g = (byte)(s.G + (e.G - s.G) * fraction);
        byte b = (byte)(s.B + (e.B - s.B) * fraction);
        return FromRgb(r, g, b);
    }
    
    public static NKColor GetMultiStopColor(NKColor[] colors, float fraction) {
        switch (colors.Length) {
            case 0: return Default;
            case 1: return colors[0];
        }

        fraction = Math.Clamp(fraction, 0f, 1f);

        // If at the absolute end, return the last color
        if (fraction >= 1f) return colors[^1];

        // Determine which segment we are in
        float segmentValue = fraction * (colors.Length - 1);
        int segmentIndex = (int)Math.Floor(segmentValue);
        
        // Determine how far we are into that specific segment
        float localFraction = segmentValue - segmentIndex;

        return Lerp(colors[segmentIndex], colors[segmentIndex + 1], localFraction);
    }
    

    internal static void AppendInnerF(StringBuilder sb, NKColor prev, NKColor next) => AppendInner(sb, prev, next, 3);
    internal static void AppendInnerB(StringBuilder sb, NKColor prev, NKColor next) => AppendInner(sb, prev, next, 4);
    internal static void AppendInnerU(StringBuilder sb, NKColor prev, NKColor next) => AppendInner(sb, prev, next, 5);

    private static void AppendInner(StringBuilder sb, NKColor prev, NKColor next, int mode) {
        if ((prev.IsInherit && next.IsInherit) || prev == next) return;

        if (next.IsInherit) {
            sb.Append(prev.IsDefault 
                ? $"{mode}9;" 
                : prev.IsPalette 
                    ? $"{mode}8;5;{(byte)prev.AsPalette};" 
                    : $"{mode}8;2;{prev.AsRgb.R};{prev.AsRgb.G};{prev.AsRgb.B};"
            );

            return;
        }

        sb.Append(next.IsDefault 
            ? $"{mode}9;" 
            : next.IsPalette 
                ? $"{mode}8;5;{(byte)next.AsPalette};" 
                : $"{mode}8;2;{next.AsRgb.R};{next.AsRgb.G};{next.AsRgb.B};"
        );
    }
    
    public enum ColorType : byte {
        DEFAULT       = 0,
        CONSOLE_COLOR = 0b01,
        RGB           = 0b10,
        INHERIT       = 0b11
    }
}