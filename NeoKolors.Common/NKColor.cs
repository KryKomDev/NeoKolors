//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using NeoKolors.Tui.Styles.Values;
using OneOf;

namespace NeoKolors.Common;

/// <summary>
/// color structure that can hold every color supported by the console (+ARGB colors) 
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
public readonly struct NKColor : ICloneable, IEquatable<NKColor>, IFormattable, IParsableValue<NKColor> {
    
    /// <summary>
    /// Represents the underlying 32-bit unsigned integer value used to store the color data,
    /// including the color type and the specific color information.
    /// The color type is encoded in the upper 2 bits of the value.
    /// The lower 24 bits are used to store the actual color information.
    /// 6 bits remain unused...
    /// </summary>
    [FieldOffset(0)]
    private readonly uint _value;
    
    public OneOf<uint, NKConsoleColor, DefaultColor, InheritColor> Value {
        get {
            var type = _value & (0b11 << 30);
            return (ColorType)type switch {
                ColorType.RGB           => _value & 0x00ffffff,
                ColorType.CONSOLE_COLOR => (NKConsoleColor)(_value & 0x000000ff),
                ColorType.DEFAULT       => new DefaultColor(),
                ColorType.INHERIT       => new InheritColor(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public ColorType Type => (ColorType)(_value & (0b11 << 30));
    
    public bool IsRgb => (_value & 0xff000000) == (uint)ColorType.RGB;
    public bool IsPalette => (_value & 0xff000000) == (uint)ColorType.CONSOLE_COLOR;
    public bool IsDefault => (_value & 0xff000000) == (uint)ColorType.DEFAULT;
    public bool IsInherit => (_value & 0xff000000) == (uint)ColorType.INHERIT;
    
    public uint AsRgb => _value & 0x00ffffff;
    public NKConsoleColor AsPalette => (NKConsoleColor)(_value & 0x000000ff);
    
    // ====== CONSTRUCTORS ======

    public NKColor(int rgb) => _value = (uint)rgb & 0x00ffffff | (uint)ColorType.RGB;
    public NKColor(NKConsoleColor consoleColor) => _value = (byte)consoleColor | (uint)ColorType.CONSOLE_COLOR;
    public NKColor(ConsoleColor consoleColor) => _value = (byte)ColorFormat.SystemToNK(consoleColor) | (uint)ColorType.CONSOLE_COLOR;
    public NKColor() => _value = (uint)ColorType.DEFAULT;
    
    
    private NKColor(uint value) => _value = value;

    /// <summary>
    /// Returns a new color with the default console value.
    /// </summary>
    public static NKColor Default => new((uint)ColorType.DEFAULT);
    
    /// <summary>
    /// Returns a new color that indicates that the color should be inherited (not overriden).
    /// </summary>
    public static NKColor Inherit => new((uint)ColorType.INHERIT);
    
    public static NKColor FromRgb(byte r, byte g, byte b) => new((r << 16) | (g << 8) | b);
    public static NKColor FromRgb(uint hex) => new((hex & 0x00ffffff) | (uint)ColorType.RGB);
    
    
    // ====== IMPLICIT CONVERSIONS ======

    public static implicit operator NKColor(NKConsoleColor color) => new(color);
    public static implicit operator NKColor(ConsoleColor color) => new(color);
    public static implicit operator NKColor(uint color) => FromRgb(color);
    public static implicit operator NKColor(int color) => new(color);

    public static implicit operator NKConsoleColor(NKColor color) =>
        color.Value.Match(
            _ => throw InvalidColorCastException.CustomToConsoleColor(),
            c => c,
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public static implicit operator uint(NKColor color) =>
        color.Value.Match(
            i => i,
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );
    
    public static implicit operator int(NKColor color) =>
        color.Value.Match(
            i => (int)(i & 0x00ffffff),
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );
    

    public object Clone() => MemberwiseClone();

    public bool Equals(NKColor? other) => other is not null && Value.Equals(other.Value.Value);

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (Equals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((NKColor)obj);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Represents the ANSI control string or textual representation
    /// of the foreground color associated with the current <see cref="NKColor"/> instance.
    /// </summary>
    public string Text =>
        Value.Match(
            i => i.ControlChar(),
            c => c.ControlChar(),
            _ => EscapeCodes.TEXT_COLOR_RESET,
            _ => "Inherit"
        );

    /// <summary>
    /// Returns the ANSI escape code or control character string representing
    /// the background color associated with the current <see cref="NKColor"/> instance.
    /// </summary>
    public string Bckg =>
        Value.Match(
            i => i.ControlCharB(),
            c => c.ControlCharB(),
            _ => EscapeCodes.BCKG_COLOR_RESET,
            _ => "Inherit"
        );

    public string Underline =>
        Value.Match(
            i => i.ControlCharU(),
            c => c.ControlCharU(),
            _ => EscapeCodes.UNDERLINE_COLOR_RESET,
            _ => "Inherit"
        );

    public void Write() =>
        Console.Write(Value.Match(
            i => $"{"●".AddColor(i)} #{i:x6}",
            c => $"{"●".AddColor(c)} {Enum.GetName(typeof(NKConsoleColor), c)}",
            _ => "Default",
            _ => "Inherit"
        ));

    public override string ToString() =>
        Value.Match(
            i => $"{i:x6}",
            c => $"{Enum.GetName(typeof(NKConsoleColor), c)}",
            _ => "Default",
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

    public static bool operator ==(NKColor left, NKColor right) {
        return left.Equals(right);
    }

    public static bool operator !=(NKColor left, NKColor right) {
        return !left.Equals(right);
    }
    
    public static NKColor Parse(string s) => Parse(s, CultureInfo.InvariantCulture);
    
    public static NKColor Parse(string s, IFormatProvider? provider) {
        s = s.Trim();
        
        if (s.StartsWith('#')) {
            var hex = s[1..];
            if (uint.TryParse(hex, NumberStyles.HexNumber, null, out uint rgb)) {
                return FromRgb(rgb);
            }
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
}

public enum ColorType : uint {
    RGB           = 0,
    CONSOLE_COLOR = 0b01u << 30,
    DEFAULT       = 0b10u << 30,
    INHERIT       = 0b11u << 30
}