namespace NeoKolors.Tui.Styles;

[StylePropertyName("min-height")]
public struct MinHeight : IStyleProperty<MinHeight, SizeValue> {
    public SizeValue Value { get; }
    
    public MinHeight(SizeValue value) {
        Value = value;
    }
    
    public MinHeight() {
        Value = SizeValue.Auto;
    }
}