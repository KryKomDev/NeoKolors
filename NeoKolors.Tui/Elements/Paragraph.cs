//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Common.Util;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles;
using static NeoKolors.Tui.Styles.OverflowType;

namespace NeoKolors.Tui.Elements;

[ElementName("p")]
[ApplicableStyles("color", "background-color", "border", "font", "align-items", "padding", "margin", "overflow", 
    "display", "grid-align")]
public class Paragraph : IElement {
    
    public NKColor TextColor => Style["color"].Value is NKColor color ? color : NKColor.Inherit;
    public NKColor BackgroundColor => Style["background-color"].Value is NKColor color ? color : NKColor.Inherit;
    public BorderStyle Border => Style["border"].Value is BorderStyle s ? s : BorderStyle.GetSolid(NKColor.Inherit);
    public IFont Font => Style["font"].Value as IFont ?? IFont.Default;
    public AlignDirection AlignItems => Style["align-items"].Value is AlignDirection a ? a : new AlignDirection();
    public PaddingProperty Padding => Style["padding"].Value is PaddingProperty p ? p : new PaddingProperty();
    public MarginProperty Margin => Style["margin"].Value is MarginProperty m ? m : new MarginProperty();
    public OverflowType Overflow => Style["overflow"].Value is OverflowType o ? o : VISIBLE_ALL;
    public DisplayType Display => Style["display"].Value is DisplayType d ? d : DisplayType.BLOCK;
    
    
    /// <summary>
    /// the text content of the paragraph
    /// </summary>
    public string Content { get; private set; }
    public string[] Selectors { get; }
    public StyleCollection Style { get; }

    public Paragraph(string content, StyleCollection style, string[]? selectors = null) {
        Content = content;
        Style = style;
        Selectors = selectors ?? [];
    }
    
    public Paragraph(string content) {
        Content = content;
        Style = new StyleCollection();
        Selectors = [];
    }

    public void ApplyStyles(StyleCollection styles) {
        Style.OverrideWith(styles);
    }

    /// <summary>
    /// renders the paragraph in the given region
    /// </summary>
    /// <param name="target">the target console screen</param>
    /// <param name="rect">the region</param>
    public void Render(in IConsoleScreen target, Rectangle rect) {
        if (Display == DisplayType.NONE) return;
        
        var borderRect = IElement.GetBorderRect(rect, Margin);
        var contentRect = IElement.GetContentRect(rect, Margin, Padding, Border);
        
        target.DrawRect(borderRect, BackgroundColor, Border);
        
        target.Render();
        
        if (Font != IFont.Default) {
            target.DrawText(Content, contentRect, Font,
                new NKStyle(TextColor, BackgroundColor),
                AlignItems.Horizontal, AlignItems.Vertical,
                Overflow is VISIBLE_ALL or VISIBLE_TOP,
                Overflow is VISIBLE_ALL or VISIBLE_BOTTOM);
        }
        else {
            target.DrawText(Content, contentRect,
                new NKStyle(TextColor, BackgroundColor),
                AlignItems.Horizontal, AlignItems.Vertical);
        }
    }

    public int GetWidth(int maxHeight) => 
        Content.Length +
        Padding.Left.ToIntH(maxHeight) + 
        Padding.Right.ToIntH(maxHeight) + 
        (Border.IsBorderless ? 0 : 2);

    public int GetHeight(int maxWidth) => 
        Content.Chop(IElement.GetContentRect(
            new Rectangle(0, 0, maxWidth, int.MaxValue), Margin, Padding, Border).Width).Length;

    public int GetMinWidth(int maxHeight) => Content.Split(' ').Select(word => word.Length).Prepend(0).Max();
    public int GetMinHeight(int maxWidth) => Content.Chop(maxWidth).Length;
}