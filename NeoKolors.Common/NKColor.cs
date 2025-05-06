//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Globalization;
using NeoKolors.Common.Exceptions;
using NeoKolors.Common.Util;
using OneOf;

namespace NeoKolors.Common;

/// <summary>
/// color structure that can hold every color supported by the console (+ARGB colors) 
/// </summary>
public struct NKColor : ICloneable, IEquatable<NKColor>, IFormattable {
    
    public OneOf<int, NKConsoleColor, DefaultColor, InheritColor> Color { get; private set; }

    public byte A {
        get {
            return Color.Match(
                i => (byte)(i >> 24),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom()
            );
        }
        set {
            if (!Color.IsT0)
                throw InvalidColorCastException.ConsoleColorToCustom();
            
            Color = Color.AsT0 & 0x00ffffff | value << 24;
        }
    }
    
    public byte R {
        get {
            return Color.Match(
                i => (byte)(i >> 16),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom()
            );
        }
        set {
            if (!Color.IsT0)
                throw InvalidColorCastException.ConsoleColorToCustom();
            
            Color = (int)(Color.AsT0 & 0xff00ffff | (uint)(value << 16));
        }
    }
    
    public byte G {
        get {
            return Color.Match(
                i => (byte)(i >> 8),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom()
            );
        }
        set {
            if (!Color.IsT0)
                throw InvalidColorCastException.ConsoleColorToCustom();
            
            Color = (int)(Color.AsT0 & 0xffff00ff | (uint)(value << 8));
        }
    }
    
    public byte B {
        get {
            return Color.Match(
                i => (byte)i,
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom()
            );
        }
        set {
            if (!Color.IsT0)
                throw InvalidColorCastException.ConsoleColorToCustom();
            
            Color = (int)(Color.AsT0 & 0xffffff00 | value);
        }
    }
    
    public void Set(int argb) => Color = argb;
    public void Set(NKConsoleColor consoleColor) => Color = consoleColor;
    public void Set(ConsoleColor consoleColor) => Color = ColorFormat.SystemToNK(consoleColor);

    public NKColor(int customColor) => Color = customColor;

    public NKColor(NKConsoleColor consoleColor) => Color = consoleColor;
    public NKColor(ConsoleColor consoleColor) => Color = ColorFormat.SystemToNK(consoleColor);
    public NKColor() => Color = new DefaultColor();
    
    // ReSharper disable UnusedParameter.Local
    private NKColor(DefaultColor _) => Color = new DefaultColor();
    private NKColor(InheritColor _) => Color = new InheritColor();
    // ReSharper restore UnusedParameter.Local

    /// <summary>
    /// returns a new color with the default console value
    /// </summary>
    public static NKColor Default => new(new DefaultColor());
    
    /// <summary>
    /// returns a new color that indicates that the color should be inherited (not overriden) 
    /// </summary>
    public static NKColor Inherit => new(new InheritColor());
    
    public static NKColor FromArgb(int color) => new(color);
    public static NKColor FromArgb(byte r, byte g, byte b) => new((r << 16) | (g << 8) | b);
    public static NKColor FromArgb(byte a, byte r, byte g, byte b) => new((a << 24) | (r << 16) | (g << 8) | b);
    public static NKColor FromArgb(UInt24 color) => new(color);
    

    public static implicit operator NKColor(NKConsoleColor color) => new(color);
    public static implicit operator NKColor(ConsoleColor color) => new(color);
    public static implicit operator NKColor(int color) => new(color);
    public static implicit operator NKColor(UInt24 color) => new(color);

    public static implicit operator NKConsoleColor(NKColor color) =>
        color.Color.Match(
            _ => throw InvalidColorCastException.CustomToConsoleColor(),
            c => c,
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public static implicit operator int(NKColor color) =>
        color.Color.Match(
            i => i,
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public static implicit operator UInt24(NKColor color) =>
        color.Color.Match(
            i => (byte)(i >> 24),
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public static implicit operator System.Drawing.Color(NKColor color) =>
        color.Color.Match(
            System.Drawing.Color.FromArgb,
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public object Clone() => MemberwiseClone();

    public bool Equals(NKColor? other) {
        return other is not null && Color.Equals(other.Value.Color);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (Equals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((NKColor)obj);
    }

    public override int GetHashCode() {
        return Color.GetHashCode();
    }

    /// <summary>
    /// Represents the ANSI control string or textual representation
    /// of the foreground color associated with the current <see cref="NKColor"/> instance.
    /// </summary>
    public string Text =>
        Color.Match(
            i => i.ControlChar(),
            c => c.ControlChar(),
            _ => EscapeCodes.TEXT_COLOR_END,
            _ => "Inherit"
        );

    /// <summary>
    /// Returns the ANSI escape code or control character string representing
    /// the background color associated with the current <see cref="NKColor"/> instance.
    /// </summary>
    public string Bckg =>
        Color.Match(
            i => i.ControlCharB(),
            c => c.ControlCharB(),
            _ => EscapeCodes.BACKGROUND_COLOR_END,
            _ => "Inherit"
        );

    public void Write() =>
        Color.Match(
            i => $"{"●".AddColor(i)} #{i:x6}",
            c => $"{"●".AddColor(c)} {Enum.GetName(typeof(NKConsoleColor), c)}",
            _ => "Default",
            _ => "Inherit"
        );

    public override string ToString() =>
        Color.Match(
            i => $"#{i:x6}",
            c => $"{Enum.GetName(typeof(NKConsoleColor), c)}",
            _ => "Default",
            _ => "Inherit"
        );

    public string ToString(string format) => 
        ToString(format, CultureInfo.InvariantCulture);
    
    public string ToString(string? format, IFormatProvider? formatProvider) {
        if (string.IsNullOrEmpty(format)) format = "T";
        return format switch {
            "p" or "P" => ToString(),
            "t" or "T" or "Text" or "f" or "F" or "Forg" => Text,
            "b" or "B" or "Bckg" => Bckg,
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
                            FromArgb((byte)(z * 255 / 5), (byte)(y * 255 / 5), (byte)(x * 255 / 5)).GetInverse()));
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }

    public NKColor GetInverse() {
        int r = R;
        int g = G;
        int b = B;
        return Color.Match(
            _ => FromArgb((byte)(255 - r), (byte)(255 - g), (byte)(255 - b)),
            c => new NKColor((NKConsoleColor)(((int)c + 8) % 16)),
            _ => Default,
            _ => Inherit
        );
    }

    public bool Equals(NKColor other) => Color.Equals(other.Color);

    public static bool operator ==(NKColor left, NKColor right) {
        return left.Equals(right);
    }

    public static bool operator !=(NKColor left, NKColor right) {
        return !left.Equals(right);
    }
}