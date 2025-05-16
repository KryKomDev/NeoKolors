namespace NeoKolors.Tui.Styles;

[StylePropertyName("max-height")]
public struct MaxHeight : IStyleProperty<MinHeight, SizeValue> {
    public SizeValue Value { get; }
    
    public MaxHeight(SizeValue value) {
        Value = value;
    }
    
    public MaxHeight() {
        Value = SizeValue.Auto;
    }
}