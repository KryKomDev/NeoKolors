// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct MarginBottomProperty : IPartialStyleProperty<Dimension, MarginBottomProperty, MarginProperty> {
    public Dimension Value { get; }

    public MarginBottomProperty(Dimension value) {
        Value = value;
    }

    public MarginProperty Combine(MarginProperty baseProperty) 
        => new(baseProperty.Left, baseProperty.Right, baseProperty.Top, Value);

    public MarginBottomProperty Extract(MarginProperty baseProperty) => new(baseProperty.Bottom);
}