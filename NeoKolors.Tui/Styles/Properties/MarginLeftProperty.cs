// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct MarginLeftProperty : IPartialStyleProperty<Dimension, MarginLeftProperty, MarginProperty> {
    public Dimension Value { get; }

    public MarginLeftProperty(Dimension value) {
        Value = value;
    }

    public MarginProperty Combine(MarginProperty baseProperty) 
        => new(Value, baseProperty.Right, baseProperty.Top, baseProperty.Bottom);
}