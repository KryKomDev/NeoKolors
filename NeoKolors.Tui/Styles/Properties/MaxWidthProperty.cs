// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct MaxWidthProperty : IStyleProperty<Dimension, MaxWidthProperty> {
    public Dimension Value { get; }
    
    public MaxWidthProperty(Dimension value) {
        Value = value;
    }

    public MaxWidthProperty() {
        Value = Dimension.Auto;
    }
}