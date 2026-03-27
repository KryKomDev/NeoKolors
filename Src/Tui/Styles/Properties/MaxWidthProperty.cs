// NeoKolors
// Copyright (c) 2026 KryKom

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