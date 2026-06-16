// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct GridAlignProperty : IStyleProperty<Area2D, GridAlignProperty> {
    public Area2D Value { get; }
    
    public GridAlignProperty(Area2D value) {
        Value = value;
    }
    
    public GridAlignProperty() {
        Value = new Area2D(0, 0, 0, 0);
    }
}