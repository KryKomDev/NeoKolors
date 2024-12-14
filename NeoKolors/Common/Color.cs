namespace NeoKolors.Common;

public class Color {
    public bool IsPaletteSafe { get; }
    public ConsoleColor? ConsoleColor { get; }
    public int? CustomColor { get; }

    public Color(int customColor) {
        IsPaletteSafe = false;
        CustomColor = customColor;
        ConsoleColor = null;
    }

    public Color(ConsoleColor consoleColor) {
        IsPaletteSafe = true;
        CustomColor = null;
        ConsoleColor = consoleColor;
    }
}