// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public readonly struct PaddingBottomProperty : IPartialStyleProperty<Dimension, PaddingBottomProperty, PaddingProperty> {
    public Dimension Value { get; }

    public PaddingBottomProperty(Dimension value) {
        Value = value;
    }

    public PaddingProperty Combine(PaddingProperty baseProperty) {
        return new PaddingProperty(baseProperty.Left, baseProperty.Right, baseProperty.Top, Value);
    }
    
    public PaddingBottomProperty Extract(PaddingProperty baseProperty) => new(baseProperty.Bottom);
}