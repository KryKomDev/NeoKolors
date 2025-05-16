namespace NeoKolors.Tui.Styles;

[StylePropertyName("max-width")]
public struct MaxWidth : IStyleProperty<MaxWidth, SizeValue> {
    public SizeValue Value { get; }
    
    public MaxWidth(SizeValue value) {
        Value = value;
    }
    
    public MaxWidth() {
        Value = SizeValue.Auto;
    }
}