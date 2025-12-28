// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Styles.Properties;

public struct FontProperty : IStyleProperty<IFont, FontProperty> {
    public IFont Value { get; }
    
    public FontProperty(IFont value) {
        Value = value;
    }

    public FontProperty() {
        Value = IFont.Default;
    }
}