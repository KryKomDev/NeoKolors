// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct WidthProperty : IStyleProperty<Dimension, WidthProperty> {
    public Dimension Value { get; }
    
    public WidthProperty(Dimension value) {
        Value = value;
    }

    public WidthProperty() {
        Value = Dimension.Auto;
    }
}