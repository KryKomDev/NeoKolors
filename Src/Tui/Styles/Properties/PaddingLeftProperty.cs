// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct PaddingLeftProperty : IPartialStyleProperty<Dimension, PaddingLeftProperty, PaddingProperty> {
    public Dimension Value { get; }

    public PaddingLeftProperty(Dimension value) {
        Value = value;
    }

    public PaddingProperty Combine(PaddingProperty baseProperty) {
        return new PaddingProperty(Value, baseProperty.Right, baseProperty.Top, baseProperty.Bottom);
    }
    
    public PaddingLeftProperty Extract(PaddingProperty baseProperty) => new(baseProperty.Left);
}