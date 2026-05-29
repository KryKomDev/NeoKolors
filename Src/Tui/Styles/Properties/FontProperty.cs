// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Styles.Properties;

public struct FontProperty : IStyleProperty<IAsciiFont, FontProperty> {
    public IAsciiFont Value { get; }
    
    public FontProperty(IAsciiFont value) {
        Value = value;
    }

    public FontProperty() {
        Value = IAsciiFont.Default;
    }
}