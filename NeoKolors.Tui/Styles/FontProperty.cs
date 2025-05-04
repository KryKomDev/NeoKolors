//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Styles;

[StylePropertyName("font")]
public struct FontProperty : IStyleProperty<FontProperty, FontProperty> {
    
    public IFont Font { get; set; }
    public FontProperty Value => this;
    
    public FontProperty(IFont value) {
        Font = value;
    }
    
    public FontProperty() {
        Font = IFont.Default;
    }
}
