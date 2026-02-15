// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct MarginRightProperty : IPartialStyleProperty<Dimension, MarginRightProperty, MarginProperty> {
    public Dimension Value { get; }

    public MarginRightProperty(Dimension value) {
        Value = value;
    }

    public MarginProperty Combine(MarginProperty baseProperty) 
        => new(baseProperty.Left, Value, baseProperty.Top, baseProperty.Bottom);
    
    public MarginRightProperty Extract(MarginProperty baseProperty) => new(baseProperty.Right);
}