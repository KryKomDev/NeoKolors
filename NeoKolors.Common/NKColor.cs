//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Exceptions;
using OneOf;

namespace NeoKolors.Common;

/// <summary>
/// color structure that can hold every color supported by the console (+ARGB colors) 
/// </summary>
public class NKColor : ICloneable, IEquatable<NKColor> {
    
    public OneOf<int, NKConsoleColor, DefaultColor> Color { get; }

    public byte R {
        get {
            return Color.Match(
                i => (byte)(i >> 16),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom()
            );
        }
    }
    
    public byte G {
        get {
            return Color.Match(
                i => (byte)(i >> 8),
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom()
            );
        }
    }
    
    public byte B {
        get {
            return Color.Match(
                i => (byte)i,
                _ => throw InvalidColorCastException.ConsoleColorToCustom(),
                _ => throw InvalidColorCastException.ConsoleColorToCustom()
            );
        }
    }

    public NKColor(int customColor) => Color = customColor;

    public NKColor(NKConsoleColor consoleColor) => Color = consoleColor;
    public NKColor(ConsoleColor consoleColor) => Color = ColorFormat.SystemToNK(consoleColor);
    public NKColor() => Color = new DefaultColor();

    
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
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public static implicit operator int(NKColor color) =>
        color.Color.Match(
            i => i,
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public static implicit operator UInt24(NKColor color) =>
        color.Color.Match(
            i => (byte)(i >> 24),
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public static implicit operator System.Drawing.Color(NKColor color) =>
        color.Color.Match(
            System.Drawing.Color.FromArgb,
            _ => throw InvalidColorCastException.ConsoleColorToCustom(),
            _ => throw InvalidColorCastException.ConsoleColorToCustom()
        );

    public object Clone() => MemberwiseClone();

    public bool Equals(NKColor? other) {
        return other is not null && Color.Equals(other.Color);
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((NKColor)obj);
    }

    public override int GetHashCode() {
        return Color.GetHashCode();
    }

    public string Text =>
        Color.Match(
            i => i.ControlChar(),
            c => c.ControlChar(),
            _ => EscapeCodes.TEXT_COLOR_END
        );
    
    public string Bckg => 
        Color.Match(
            i => i.ControlCharB(),
            c => c.ControlCharB(),
            _ => EscapeCodes.BACKGROUND_COLOR_END
        );

    public void Write() =>
        Color.Match(
            i => $"{"●".AddColor(i)} #{i:x6}",
            c => $"{"●".AddColor(c)} {Enum.GetName(typeof(NKConsoleColor), c)}",
            _ => "Default"
        );

    public override string ToString() =>
        Color.Match(
            i => $"#{i:x6}",
            c => $"{Enum.GetName(typeof(NKConsoleColor), c)}",
            _ => "Default"
        );
}