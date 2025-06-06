//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("grid-align")]
public struct GridAlignProperty : IStyleProperty<GridAlignProperty, Rectangle> {
    public Rectangle Value { get; }
    
    public GridAlignProperty(Rectangle value) {
        Value = value;
    }
    
    public GridAlignProperty() {
        Value = new Rectangle(0, 0, 0, 0);
    }
    
    public static implicit operator GridAlignProperty(Rectangle value) => new(value);
}