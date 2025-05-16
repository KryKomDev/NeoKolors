namespace NeoKolors.Tui.Styles;

[StylePropertyName("min-width")]
public struct MinWidth : IStyleProperty<MinWidth, SizeValue> {
    public SizeValue Value { get; }
    
    public MinWidth(SizeValue value) {
        Value = value;
    }
    
    public MinWidth() {
        Value = SizeValue.Auto;
    }
}