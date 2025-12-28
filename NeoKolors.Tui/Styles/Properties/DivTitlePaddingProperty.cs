// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct DivTitlePaddingProperty : IStyleProperty<Dimension, DivTitlePaddingProperty> {
    public Dimension Value { get; }
    
    public DivTitlePaddingProperty(Dimension value) => Value = value;
    public DivTitlePaddingProperty() => Value = Dimension.Chars(1);
}