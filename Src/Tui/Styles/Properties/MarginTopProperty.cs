// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct MarginTopProperty : IPartialStyleProperty<Dimension, MarginTopProperty, MarginProperty> {
    public Dimension Value { get; }

    public MarginTopProperty(Dimension value) {
        Value = value;
    }

    public MarginProperty Combine(MarginProperty baseProperty) 
        => new(baseProperty.Left, baseProperty.Right, Value, baseProperty.Bottom);
    
    public MarginTopProperty Extract(MarginProperty baseProperty) => new(baseProperty.Top);
}