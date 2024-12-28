namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class CheckboxProperty : IStyleProperty<CheckboxProperty.CheckboxData> {
    
    public CheckboxData Value { get; }
    public static string GetStaticName() => "checkbox";
    public string GetName() => GetStaticName();

    public static CheckboxData GetStaticDefault() => new(CheckboxStyle.SWITCH, true);
    public CheckboxData GetDefault() => GetStaticDefault();


    public class CheckboxData {
        public CheckboxStyle Style { get; }
        public bool IsColored { get; }

        public CheckboxData(CheckboxStyle style, bool isColored) {
            Style = style;
            IsColored = isColored;
        }

        public string GetString() {
            throw new NotImplementedException();
        }
    }

    public enum CheckboxStyle {
        SWITCH,
        CHECKBOX_BOX,
        CHECKBOX_RADIO,
        CHECKBOX_TICK,
    }
}