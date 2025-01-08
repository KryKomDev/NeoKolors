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
    public object Clone() => MemberwiseClone();

    public string ControlChar =>
        IsPaletteSafe
            ? ((ConsoleColor)ConsoleColor!).ControlChar()
            : ((int)ConsoleColor!).ControlChar();
    
    public string ControlCharB => 
        IsPaletteSafe
            ? ((ConsoleColor)ConsoleColor!).ControlCharB()
            : ((int)ConsoleColor!).ControlCharB();
    
    public string ControlCharEnd =>
        IsPaletteSafe
            ? StringEffects.PALETTE_CONTROL_END
            : StringEffects.CUSTOM_CONTROL_END;

    public string ControlCharEndB => 
        IsPaletteSafe
            ? StringEffects.PALETTE_CONTROL_BACKGROUND_END
            : StringEffects.CUSTOM_CONTROL_BACKGROUND_END;
}