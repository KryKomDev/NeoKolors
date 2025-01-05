namespace NeoKolors.ConsoleGraphics.Style;

public class WidthProperty : IStyleProperty<WidthProperty.SizeData> {
    public SizeData Value { get; }
    public static string GetStaticName() => "width";
    public string GetName() => GetStaticName();
    public static SizeData GetStaticDefault() => new();
    public SizeData GetDefault() => GetStaticDefault();
    
    public WidthProperty(SizeData value) => Value = value;
    public WidthProperty() => Value = new SizeData();

    public class SizeData {
        public SizeValue Value { get; }
        public int ScalarValue => Value.Value;
        public SizeValue.SizeOptions Option => Value.Option;

        public SizeData(SizeValue value) {
            Value = value;
        }

        public SizeData() {
            Value = new SizeValue(SizeValue.SizeOptions.AUTO);
        }
    }
}