//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("display")]
public struct DisplayProperty : IStyleProperty<DisplayProperty, DisplayType> {
    
    public DisplayType Value { get; }
    
    public DisplayProperty(DisplayType value) {
        Value = value;
    }
    
    public DisplayProperty() {
        Value = DisplayType.BLOCK;
    }
    
    public static implicit operator DisplayProperty(DisplayType value) => new(value);
    public static implicit operator DisplayType(DisplayProperty value) => value.Value;
}