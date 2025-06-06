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

    /// <summary>
    /// Represents the margin property used in styling, allowing to specify the space surrounding an element.
    /// </summary>
    /// <remarks>
    /// The margin property is defined by four sides: left, top, right, and bottom.
    /// Each side can have its value set individually or collectively.
    /// </remarks>
    public MarginProperty(
        SizeValue left,
        SizeValue top,
        SizeValue right,
        SizeValue bottom) 
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Represents the margin property used in styling, allowing the specification of the space surrounding an element.
    /// </summary>
    /// <param name="value">a margin value applied to all four sides</param>
    /// <remarks>
    /// The margin property is defined by four sides: left, top, right, and bottom.
    /// Each side can have its value set individually or collectively.
    /// </remarks>
    public MarginProperty(SizeValue value) {
        Left = value;
        Top = value;
        Right = value;
        Bottom = value;
    }

    /// <summary>
    /// Represents the margin property in styling, allowing the specification of the space surrounding an element.
    /// </summary>
    /// <param name="horizontal">a margin value applied to left and right</param>
    /// <param name="vertical">a margin value applied to top and bottom</param>
    /// <remarks>
    /// The margin property is defined by four sides: left, top, right, and bottom.
    /// Each side can have its value set individually or collectively.
    /// </remarks>
    public MarginProperty(SizeValue horizontal, SizeValue vertical) {
        Left = horizontal;
        Top = vertical;
        Right = horizontal;
        Bottom = vertical;   
    }
    
    /// <summary>
    /// Represents the margin property in styling, allowing the specification of the space surrounding an element.
    /// The default margin value is 0.
    /// </summary>
    /// <remarks>
    /// The margin property is defined by four sides: left, top, right, and bottom.
    /// Each side can have its value set individually or collectively.
    /// </remarks>
    public MarginProperty() {
        Left = 0;
        Top = 0;
        Right = 0;
        Bottom = 0;       
    }
    
    public static MarginProperty Zero => new(0, 0, 0, 0);
}