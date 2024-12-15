namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class DisplayProperty : IStyleProperty<DisplayProperty.DisplayData> {
    public DisplayData Value { get; }
    public DisplayType Type => Value.Type;
    public static string GetStaticName() => "display";
    public string GetName() => GetStaticName();
    public static DisplayData GetStaticDefault() => new(DisplayType.INLINE);
    public DisplayData GetDefault() => GetStaticDefault();

    public class DisplayData {
        public DisplayType Type { get; }

        public DisplayData(DisplayType type) {
            Type = type;
        }
        
        public static implicit operator DisplayType(DisplayData value) => value.Type;
        public static implicit operator DisplayData(DisplayType type) => new(type);
    }

    public enum DisplayType {
        NONE,
        INLINE,
        BLOCK,
        FLEX,
        GRID,
        TABLE
    }
}