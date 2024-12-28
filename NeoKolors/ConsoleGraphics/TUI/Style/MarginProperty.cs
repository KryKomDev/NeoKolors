namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class MarginProperty : IStyleProperty<MarginProperty.MarginData> {
    
    public MarginData Value { get; }
    
    public SizeValue Left => Value.Left;
    public SizeValue Top => Value.Top;
    public SizeValue Bottom => Value.Bottom;
    public SizeValue Right => Value.Right;
    
    public static string GetStaticName() => "margin";
    public string GetName() => GetStaticName();
    public static MarginData GetStaticDefault() => MarginData.Default();
    public MarginData GetDefault() => GetStaticDefault();

    public MarginProperty(MarginData value) {
        Value = value;
    }
    
    public MarginProperty(SizeValue left, SizeValue top, SizeValue right, SizeValue bottom) {
        Value = new MarginData(left, top, right, bottom);
    }

    public class MarginData {
        public SizeValue Left { get; set; }
        public SizeValue Top { get; set; }
        public SizeValue Bottom { get; set; }
        public SizeValue Right { get; set; }

        public static MarginData Default() =>
            new((1, SizeValue.SizeOptions.UNIT_CHAR), 
                (0, SizeValue.SizeOptions.UNIT_CHAR), 
                (1, SizeValue.SizeOptions.UNIT_CHAR), 
                (0, SizeValue.SizeOptions.UNIT_CHAR));

        public MarginData(SizeValue left, SizeValue top, SizeValue right, SizeValue bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}