// NeoKolors
// Copyright (c) 2025 KryKom

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