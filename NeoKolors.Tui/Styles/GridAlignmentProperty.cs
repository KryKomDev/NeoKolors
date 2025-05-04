//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("grid-align")]
public struct GridAlignmentProperty : IStyleProperty<GridAlignmentProperty, Rectangle> {
    public Rectangle Value { get; }
    
    public GridAlignmentProperty(Rectangle value) {
        Value = value;
    }
    
    public GridAlignmentProperty() {
        Value = new Rectangle(0, 0, 0, 0);
    }
    
    public static implicit operator GridAlignmentProperty(Rectangle value) => new(value);
}