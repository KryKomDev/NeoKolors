//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Exceptions;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Common;

public class Color : ICloneable {
    public bool IsPaletteSafe { get; }
    public ConsoleColor? ConsoleColor { get; }
    public int? CustomColor { get; }

    public Color(int customColor) {
        IsPaletteSafe = false;
        CustomColor = customColor;
        ConsoleColor = null;
    }

    public Color(ConsoleColor consoleColor = System.ConsoleColor.Gray) {
        IsPaletteSafe = true;
        CustomColor = null;
        ConsoleColor = consoleColor;
    }
    
    public static implicit operator Color(ConsoleColor color) => new(color);
    public static implicit operator Color(int color) => new(color);
    public static implicit operator Color(UInt24 color) => new(color);

    public static implicit operator ConsoleColor(Color color) {
        if (color.IsPaletteSafe) return color.ConsoleColor!.Value;
        throw InvalidColorCastException.CustomToConsoleColor();
    }
    
    public static implicit operator int(Color color) {
        if (!color.IsPaletteSafe) return color.CustomColor!.Value;
        throw InvalidColorCastException.ConsoleColorToCustom();
    }
    
    public static implicit operator UInt24(Color color) {
        if (!color.IsPaletteSafe) return color.CustomColor!.Value;
        throw InvalidColorCastException.ConsoleColorToCustom();
    }
    
    public static implicit operator System.Drawing.Color(Color color) {
        if (!color.IsPaletteSafe) return System.Drawing.Color.FromArgb(color.CustomColor!.Value);
        throw InvalidColorCastException.ConsoleColorToCustom();
    }
    
    public object Clone() => MemberwiseClone();

    public override bool Equals(object? obj) {
        if (obj is Color c) return Equals(c);
        return false;
    }

    private bool Equals(Color other) => 
        IsPaletteSafe == other.IsPaletteSafe && ConsoleColor == other.ConsoleColor && CustomColor == other.CustomColor;

    public override int GetHashCode() {
        unchecked {
            var hashCode = IsPaletteSafe.GetHashCode();
            hashCode = (hashCode * 397) ^ ConsoleColor.GetHashCode();
            hashCode = (hashCode * 397) ^ CustomColor.GetHashCode();
            return hashCode;
        }
    }

    public string ControlChar =>
        IsPaletteSafe
            ? ((ConsoleColor)ConsoleColor!).ControlChar()
            : ((int)CustomColor!).ControlChar();
    
    public string ControlCharB => 
        IsPaletteSafe
            ? ((ConsoleColor)ConsoleColor!).ControlCharB()
            : ((int)CustomColor!).ControlCharB();
    
    public string ControlCharEnd =>
        IsPaletteSafe
            ? PALETTE_COLOR_END
            : CUSTOM_COLOR_END;

    public string ControlCharEndB => 
        IsPaletteSafe
            ? PALETTE_BACKGROUND_COLOR_END
            : CUSTOM_BACKGROUND_COLOR_END;

    public void PrintColor() =>
        Console.WriteLine(IsPaletteSafe
            ? $"{CustomColor:x8} {"●".AddColor(CustomColor!)}"
            : $"{Enum.GetName(typeof(ConsoleColor), ConsoleColor!)} {"●".AddColor(ConsoleColor!)}");
}