//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("overflow")]
public struct OverflowProperty : IStyleProperty<OverflowProperty, OverflowType> {
    public OverflowType Value { get; }
    
    public OverflowProperty(OverflowType value) {
        Value = value;
    }
    
    public OverflowProperty() {
        Value = OverflowType.HIDDEN;
    }
}

[Flags]
public enum OverflowType {
    HIDDEN =         0x0,
    VISIBLE_TOP =    0x1,
    VISIBLE_BOTTOM = 0x2,
    VISIBLE_LEFT =   0x4,
    VISIBLE_RIGHT =  0x8,
    VISIBLE_ALL =    0xF
}