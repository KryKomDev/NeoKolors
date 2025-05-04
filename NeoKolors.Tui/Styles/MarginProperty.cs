//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("margin")]
public struct MarginProperty : IStyleProperty<MarginProperty, MarginProperty> {
    public SizeValue Left { get; set; }
    public SizeValue Top { get; set; }
    public SizeValue Right { get; set; }
    public SizeValue Bottom { get; set; }
    
    public MarginProperty Value => this;

    public MarginProperty(
        SizeValue? left = null,
        SizeValue? top = null,
        SizeValue? right = null, 
        SizeValue? bottom = null) 
    {
        Left = left ?? 0;
        Top = top ?? 0;
        Right = right ?? 0;
        Bottom = bottom ?? 0;
    }

    public MarginProperty() {
        Left = 0;
        Top = 0;
        Right = 0;
        Bottom = 0;       
    }
}