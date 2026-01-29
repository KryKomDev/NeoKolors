// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct PaddingRightProperty : IPartialStyleProperty<Dimension, PaddingRightProperty, PaddingProperty> {
    public Dimension Value { get; }

    public PaddingRightProperty(Dimension value) {
        Value = value;
    }

    public PaddingProperty Combine(PaddingProperty baseProperty) {
        return new PaddingProperty(baseProperty.Left, Value, baseProperty.Top, baseProperty.Bottom);
    }
}