using NeoKolors.Common;
using NeoKolors.Console;

namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class BorderProperty : IStyleProperty<BorderProperty.BorderData> {
    
    public BorderData Value { get; }
    public static string GetStaticName() => "border";
    public string GetName() => GetStaticName();
    public static BorderData GetStaticDefault() => new(new Color(ConsoleColor.Gray), BorderStyle.NONE);
    public BorderData GetDefault() => GetStaticDefault();

    public class BorderData {
        public Color Color { get; }
        public BorderStyle Style { get; }

        public BorderData(Color color, BorderStyle style) {
            Color = color;
            Style = style;
        }
    }

    public BorderProperty(BorderData value) {
        Value = value;
    }

    public BorderProperty(BorderStyle style, Color? color = null) {
        Value = new BorderData(color ?? ConsoleColor.Gray, style);
    }
    
    public enum BorderStyle {
        NONE,
        ASCII,
        NORMAL,
        ROUNDED,
        THICK,
        DOUBLE
    }

    public static void WriteTopLeftCorner(BorderData borderData, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (borderData.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("+", borderData.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("┌", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("╭", borderData.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┏", borderData.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("╔", borderData.Color, backgroundColor);
                break;
        }
    }
    
    public static void WriteTopRightCorner(BorderData borderData, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (borderData.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("+", borderData.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("┐", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("╮", borderData.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┓", borderData.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("╗", borderData.Color, backgroundColor);
                break;
        }
    }
    
    public static void WriteBottomLeftCorner(BorderData borderData, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (borderData.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("+", borderData.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("└", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("╰", borderData.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┗", borderData.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("╚", borderData.Color, backgroundColor);
                break;
        }
    }
    
    public static void WriteBottomRightCorner(BorderData borderData, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (borderData.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("+", borderData.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("┘", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("╯", borderData.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┛", borderData.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("╝", borderData.Color, backgroundColor);
                break;
        }
    }

    public static void WriteHorizontal(BorderData borderData, Color? backgroundColor = null, int count = 1) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (borderData.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(new string(' ', count), borderData.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored(new string('-', count), borderData.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored(new string('─', count), borderData.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored(new string('─', count), borderData.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored(new string('━', count), borderData.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored(new string('═', count), borderData.Color, backgroundColor);
                break;
        }
    }

    public static void WriteVertical(BorderData borderData, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (borderData.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("|", borderData.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("│", borderData.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("│", borderData.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┃", borderData.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("║", borderData.Color, backgroundColor);
                break;
        }
    }

    public static void WriteBorder(Rectangle r, BorderData borderData, Color? backgroundColor = null) {
        if (r.Height <= 1) return;
        
        backgroundColor ??= new Color(ConsoleColor.Black);
        
        System.Console.SetCursorPosition(r.LowerX, r.HigherY);
        WriteBottomLeftCorner(borderData, backgroundColor);
        WriteHorizontal(borderData, backgroundColor, r.Width - 1);
        WriteBottomRightCorner(borderData, backgroundColor);

        for (int i = 1; i < r.Height; i++) {
            System.Console.SetCursorPosition(r.LowerX, r.LowerY + i);
            WriteVertical(borderData, backgroundColor);
            ConsoleColors.PrintColoredB(new string(' ', r.Width - 1), backgroundColor);
            WriteVertical(borderData, backgroundColor);
        }
        
        System.Console.SetCursorPosition(r.LowerX, r.LowerY);
        WriteTopLeftCorner(borderData, backgroundColor);
        WriteHorizontal(borderData, backgroundColor, r.Width - 1);
        WriteTopRightCorner(borderData, backgroundColor);
    }
}
