using NeoKolors.Common;
using NeoKolors.Console;

namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class BorderProperty : IStyleProperty<BorderProperty.Border> {
    
    public Border Value { get; }
    public static string GetStaticName() => "border";
    public string GetName() => GetStaticName();
    public static Border GetStaticDefault() => new(new Color(ConsoleColor.Gray), BorderStyle.NORMAL);
    public Border GetDefault() => GetStaticDefault();

    public class Border {
        public Color Color { get; }
        public BorderStyle Style { get; }

        public Border(Color color, BorderStyle style) {
            Color = color;
            Style = style;
        }
    }

    public BorderProperty(Border value) {
        Value = value;
    }

    public BorderProperty(BorderStyle style, Color? color = null) {
        Value = new Border(color ?? ConsoleColor.Gray, style);
    }
    
    public enum BorderStyle {
        NONE,
        ASCII,
        NORMAL,
        ROUNDED,
        THICK,
        DOUBLE
    }

    public static void WriteTopLeftCorner(Border border, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (border.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", border.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("+", border.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("┌", border.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("╭", border.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┏", border.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("╔", border.Color, backgroundColor);
                break;
        }
    }
    
    public static void WriteTopRightCorner(Border border, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (border.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", border.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("+", border.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("┐", border.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("╮", border.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┓", border.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("╗", border.Color, backgroundColor);
                break;
        }
    }
    
    public static void WriteBottomLeftCorner(Border border, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (border.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", border.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("+", border.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("└", border.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("╰", border.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┗", border.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("╚", border.Color, backgroundColor);
                break;
        }
    }
    
    public static void WriteBottomRightCorner(Border border, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (border.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", border.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("+", border.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("┘", border.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("╯", border.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┛", border.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("╝", border.Color, backgroundColor);
                break;
        }
    }

    public static void WriteHorizontal(Border border, Color? backgroundColor = null, int count = 1) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (border.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(new string(' ', count), border.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored(new string('-', count), border.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored(new string('─', count), border.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored(new string('─', count), border.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored(new string('━', count), border.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored(new string('═', count), border.Color, backgroundColor);
                break;
        }
    }

    public static void WriteVertical(Border border, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);

        switch (border.Style) {
            case BorderStyle.NONE:
                ConsoleColors.PrintColored(" ", border.Color, backgroundColor);
                break;
            case BorderStyle.ASCII:
                ConsoleColors.PrintColored("|", border.Color, backgroundColor);
                break;
            case BorderStyle.NORMAL:
                ConsoleColors.PrintColored("│", border.Color, backgroundColor);
                break;
            case BorderStyle.ROUNDED:
                ConsoleColors.PrintColored("│", border.Color, backgroundColor);
                break;
            case BorderStyle.THICK:
                ConsoleColors.PrintColored("┃", border.Color, backgroundColor);
                break;
            case BorderStyle.DOUBLE:
                ConsoleColors.PrintColored("║", border.Color, backgroundColor);
                break;
        }
    }

    public static void WriteBorder(Rectangle r, Border border, Color? backgroundColor = null) {
        backgroundColor ??= new Color(ConsoleColor.Black);
        
        System.Console.SetCursorPosition(r.LowerX, r.HigherY);
        WriteBottomLeftCorner(border, backgroundColor);
        WriteHorizontal(border, backgroundColor, r.Width - 1);
        WriteBottomRightCorner(border, backgroundColor);

        for (int i = 1; i < r.Height; i++) {
            System.Console.SetCursorPosition(r.LowerX, r.LowerY + i);
            WriteVertical(border, backgroundColor);
            ConsoleColors.PrintColoredB(new string(' ', r.Width - 1), backgroundColor);
            WriteVertical(border, backgroundColor);
        }
        
        System.Console.SetCursorPosition(r.LowerX, r.LowerY);
        WriteTopLeftCorner(border, backgroundColor);
        WriteHorizontal(border, backgroundColor, r.Width - 1);
        WriteTopRightCorner(border, backgroundColor);
    }
}