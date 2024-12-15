namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class PaddingProperty : IStyleProperty<PaddingProperty.PaddingData> {

    private PaddingData value;
    public PaddingData Value { get => value; set => this.value = value; }
    public SizeValue Left { get => value.Left; set => this.value.Left = value; }
    public SizeValue Top { get => value.Top; set => this.value.Top = value; }
    public SizeValue Right { get => value.Right; set => this.value.Right = value; }
    public SizeValue Bottom { get => value.Bottom; set => this.value.Bottom = value; }
    public static string GetStaticName() => "padding";
    public string GetName() => GetStaticName();

    public static PaddingData GetStaticDefault() => PaddingData.Default();
    public PaddingData GetDefault() => GetStaticDefault();

    public PaddingProperty(PaddingData value) => this.value = value;
    public PaddingProperty() => value = GetStaticDefault();

    public class PaddingData {
        public SizeValue Left { get; set; }
        public SizeValue Top { get; set; }
        public SizeValue Bottom { get; set; }
        public SizeValue Right { get; set; }

        public static PaddingData Default() =>
            new((1, SizeValue.UnitType.CHAR), 
                (0, SizeValue.UnitType.CHAR), 
                (1, SizeValue.UnitType.CHAR), 
                (0, SizeValue.UnitType.CHAR));

        public PaddingData(SizeValue left, SizeValue top, SizeValue right, SizeValue bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}