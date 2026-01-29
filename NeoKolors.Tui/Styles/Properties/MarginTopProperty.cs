// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct MarginTopProperty : IPartialStyleProperty<Dimension, MarginTopProperty, MarginProperty> {
    public Dimension Value { get; }

    public MarginTopProperty(Dimension value) {
        Value = value;
    }

    public MarginProperty Combine(MarginProperty baseProperty) 
        => new(baseProperty.Left, baseProperty.Right, Value, baseProperty.Bottom);
}