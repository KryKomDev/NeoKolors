// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct MarginLeftProperty : IPartialStyleProperty<Dimension, MarginLeftProperty, MarginProperty> {
    public Dimension Value { get; }

    public MarginLeftProperty(Dimension value) {
        Value = value;
    }

    public MarginProperty Combine(MarginProperty baseProperty) 
        => new(Value, baseProperty.Right, baseProperty.Top, baseProperty.Bottom);
    
    public MarginLeftProperty Extract(MarginProperty baseProperty) => new(baseProperty.Left);
}