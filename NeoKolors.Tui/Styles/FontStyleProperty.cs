//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui.Styles;

[StylePropertyName("font-style")]
public struct FontStyleProperty : IStyleProperty<FontStyleProperty, TextStyles> {
    public TextStyles Value { get; }

    public FontStyleProperty() {
        Value = TextStyles.NONE;
    }
    
    public FontStyleProperty(TextStyles value) {
        Value = value;
    }
}