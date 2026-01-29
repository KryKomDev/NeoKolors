// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct PaddingTopProperty : IPartialStyleProperty<Dimension, PaddingTopProperty, PaddingProperty> {
    public Dimension Value { get; }

    public PaddingTopProperty(Dimension value) {
        Value = value;
    }

    public PaddingProperty Combine(PaddingProperty baseProperty) {
        return new PaddingProperty(baseProperty.Left, baseProperty.Right, Value, baseProperty.Bottom);
    }
}