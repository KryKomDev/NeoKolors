namespace NeoKolors.ConsoleGraphics.TUI.Style;

public struct SizeValue {
    public int Value { get; set; }
    public UnitType Unit { get; set; }
    
    public static implicit operator (int Value, UnitType Unit)(SizeValue v) => (v.Value, v.Unit);
    public static implicit operator SizeValue((int Value, UnitType Unit) v) => new(v.Value, v.Unit);

    public SizeValue(int value, UnitType unit) {
        Value = value;
        Unit = unit;
    }
    
    public enum UnitType {
        CHAR,
        PIXEL,
        PERCENT
    }
}