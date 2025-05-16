//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui.Styles;

[StylePropertyName("ndiv-title")]
public readonly struct NDivTitleProperty : IStyleProperty<NDivTitleProperty, NDivTitleProperty> {
    
    public NKStyle Style { get; }
    public HorizontalAlign Align { get; }
    public bool Padding { get; }
    
    public NDivTitleProperty Value => this;
    
    public NDivTitleProperty(NKStyle style, HorizontalAlign align = HorizontalAlign.LEFT, bool padding = true) {
        Style = style;
        Align = align;
        Padding = padding;
    }
    
    public NDivTitleProperty() {
        Style = new NKStyle(NKColor.Inherit, NKColor.Inherit, TextStyles.BOLD);
        Align = HorizontalAlign.LEFT;
        Padding = true;
    }
}