namespace NeoKolors.ConsoleGraphics.Style;

public class HeightProperty : IStyleProperty<HeightProperty.SizeData> {
    public SizeData Value { get; }
    public static string GetStaticName() => "height";
    public string GetName() => GetStaticName();
    public static SizeData GetStaticDefault() => new();
    public SizeData GetDefault() => GetStaticDefault();
    
    public HeightProperty(SizeData value) => Value = value;
    public HeightProperty() => Value = new SizeData();

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