//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[StylePropertyName("padding")]
public struct PaddingProperty : IStyleProperty<PaddingProperty, PaddingProperty> {
    
    public SizeValue Left { get; set; }
    public SizeValue Top { get; set; }
    public SizeValue Right { get; set; }
    public SizeValue Bottom { get; set; }
    
    public PaddingProperty Value => this;

    public PaddingProperty(
        SizeValue left = default, 
        SizeValue top = default, 
        SizeValue right = default, 
        SizeValue bottom = default) 
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public PaddingProperty() {
        Left = new SizeValue();
        Top = new SizeValue();
        Right = new SizeValue();
        Bottom = new SizeValue();       
    }

    public static PaddingProperty Zero => new(0, 0, 0, 0);
}