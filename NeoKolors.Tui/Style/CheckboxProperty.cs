using NeoKolors.Common;

namespace NeoKolors.Tui.Style;

public class CheckboxProperty : IStyleProperty<CheckboxProperty.CheckboxData> {
    
    public CheckboxData Value { get; }
    public static string GetStaticName() => "checkbox";
    public string GetName() => GetStaticName();

    public static CheckboxData GetStaticDefault() => new();
    public CheckboxData GetDefault() => GetStaticDefault();

    public CheckboxProperty(CheckboxData value) {
        Value = value;
    }

    public CheckboxProperty(CheckboxStyle style) {
        Value = new CheckboxData(style);
    }

    public class CheckboxData {
        public CheckboxStyle Style { get; }
        public bool IsColored { get; }

        public CheckboxData(CheckboxStyle style = CheckboxStyle.SWITCH, bool isColored = true) {
            Style = style;
            IsColored = isColored;
        }

        public string GetString(bool state) {
            return Style switch {
                CheckboxStyle.SWITCH => IsColored 
                    ? (state ? "[*yy*r] *nn*r " : " *yy*r [*nn*r]").AddColor(
                        ("*y", ConsoleColor.Green), 
                        ("*n", ConsoleColor.Red), 
                        ("*r", ConsoleColor.White)) 
                    : (state ? "[y] n " : " y [n]"),
                CheckboxStyle.CHECKBOX_BOX => state ? "■" : "□",
                CheckboxStyle.CHECKBOX_RADIO => state ? "◉" : "○",
                CheckboxStyle.CHECKBOX_TICK => state ? "[✓]" : "[ ]",
                _ => throw new ArgumentOutOfRangeException(nameof(Style), Style, null)
            };
        }
    }

    public enum CheckboxStyle {
        SWITCH,
        CHECKBOX_BOX,
        CHECKBOX_RADIO,
        CHECKBOX_TICK,
    }
}