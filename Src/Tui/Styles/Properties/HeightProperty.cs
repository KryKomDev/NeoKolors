// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct HeightProperty : IStyleProperty<Dimension, HeightProperty> {
    public Dimension Value { get; }
    
    public HeightProperty(Dimension value) {
        Value = value;
    }

    public HeightProperty() {
        Value = Dimension.Auto;
    }
}