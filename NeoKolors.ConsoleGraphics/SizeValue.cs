namespace NeoKolors.ConsoleGraphics;

public struct SizeValue {
    public int Value { get; set; }
    public SizeOptions Option { get; set; }
    
    public static implicit operator (int value, SizeOptions option)(SizeValue v) => (v.Value, v.Option);
    public static implicit operator SizeValue((int value, SizeOptions option) v) => new(v.value, v.option);
    public static implicit operator SizeValue(int value) => new(value, SizeOptions.UNIT_CHAR);

    public SizeValue(int value, SizeOptions option) {
        Value = value;
        Option = option;
    }

    public SizeValue(SizeOptions option) {
        if (option is SizeOptions.UNIT_CHAR or SizeOptions.UNIT_PIXEL or SizeOptions.UNIT_PERCENT)
            throw new ArgumentException(
                "Parameter 'option' must not be a unit in the constructor 'SizeValue(SizeOptions)'."
                , nameof(option)
            );
        
        Value = 0;
        Option = SizeOptions.UNIT_CHAR;
    }
    
    public enum SizeOptions {
        UNIT_CHAR,
        UNIT_PIXEL,
        UNIT_PERCENT,
        AUTO,
        MAX_CONTENT,
        MIN_CONTENT
    }

    public int ToChars(int total, bool isVertical = false) {
        return Option switch {
            SizeOptions.UNIT_CHAR => Value,
            SizeOptions.UNIT_PIXEL => isVertical ? Value : Value * 3,
            SizeOptions.UNIT_PERCENT => total / 100 * Value,
            _ => throw new ArgumentOutOfRangeException(nameof(total))
        };
    }
    
    public bool IsStatic => Option is SizeOptions.UNIT_CHAR or SizeOptions.UNIT_PIXEL or SizeOptions.UNIT_PERCENT;
}