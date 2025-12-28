// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct GridAlignProperty : IStyleProperty<Rectangle, GridAlignProperty> {
    public Rectangle Value { get; }
    
    public GridAlignProperty(Rectangle value) {
        Value = value;
    }
    
    public GridAlignProperty() {
        Value = new Rectangle(0, 0, 0, 0);
    }
}