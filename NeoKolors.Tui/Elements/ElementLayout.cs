// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Elements;

public struct ElementLayout {
    
    /// <summary>
    /// Gets or sets the overall size of an elementOld.
    /// The ElementSize defines the complete dimensions of the elementOld, including
    /// its content and any additional padding, decoration, or external boundaries.
    /// </summary>
    public Size ElementSize { get; set; }

    /// <summary>
    /// Gets or sets the rectangle representing the content area of the elementOld.
    /// The .Point returns the relative offset of the content area from the top-left corner of the elementOld.
    /// .Size returns the dimensions of the content area.
    /// </summary>
    public Rectangle Content { get; set; }

    /// <summary>
    /// Gets or sets an optional border for the elementOld.
    /// The Border represents the rectangular boundary surrounding the content area.
    /// The .Point returns the relative offset of the border area from the top-left corner of the elementOld.
    /// .Size returns the dimensions of the border area.
    /// If defined, it specifies the dimensions and position of the border relative to the elementOld.
    /// If not defined, the border is not rendered.
    /// </summary>
    public Rectangle? Border { get; set; }

    public ElementLayout(Size elementSize, Rectangle content, Rectangle? border) {
        ElementSize = elementSize;
        Content = content;
        Border = border;
    }

    public static ElementLayout Zero => new(Size.Zero, Size.Zero, null);
}