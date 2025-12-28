// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct MinWidthProperty : IStyleProperty<Dimension, MinWidthProperty> {
    public Dimension Value { get; }
    
    public MinWidthProperty(Dimension value) {
        Value = value;
    }

    public MinWidthProperty() {
        Value = Dimension.Auto;
    }
}