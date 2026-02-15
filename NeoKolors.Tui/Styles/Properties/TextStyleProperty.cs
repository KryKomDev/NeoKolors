// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public struct TextStyleProperty : IStyleProperty<TextStyles, TextStyleProperty> {
    public TextStyles Value { get; }
    
    public TextStyleProperty(TextStyles value) => Value = value;
    public TextStyleProperty() => Value = TextStyles.NONE;
}