//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Common.Util;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles;
using static NeoKolors.Tui.Elements.ApplicableStylesAttribute.Predefined;
using static NeoKolors.Tui.Styles.OverflowType;

namespace NeoKolors.Tui.Elements;

[ElementName("p")]
[ApplicableStyles(UNIVERSAL | TEXT)]
public class Paragraph : IElement {
    
    public NKColor TextColor => Style["color"].Value is NKColor color ? color : NKColor.Inherit;
    public NKColor BackgroundColor => Style["background-color"].Value is NKColor color ? color : NKColor.Inherit;
    public BorderStyle Border => Style["border"].Value is BorderStyle s ? s : BorderStyle.GetSolid(NKColor.Inherit);
    public IFont Font => Style["font"].Value is FontProperty f ? f.Font : IFont.Default;
    public AlignDirection AlignItems => Style["align-items"].Value is AlignDirection a ? a : new AlignDirection();
    public PaddingProperty Padding => Style["padding"].Value is PaddingProperty p ? p : new PaddingProperty();
    public MarginProperty Margin => Style["margin"].Value is MarginProperty m ? m : new MarginProperty();
    public OverflowType Overflow => Style["overflow"].Value is OverflowType o ? o : VISIBLE_ALL;
    public DisplayType Display => Style["display"].Value is DisplayType d ? d : DisplayType.BLOCK;
    public SizeValue MinWidth => Style["min-width"].Value is SizeValue s ? s : new SizeValue();
    public SizeValue MinHeight => Style["min-height"].Value is SizeValue s ? s : new SizeValue();
    public SizeValue MaxWidth => Style["max-width"].Value is SizeValue s ? s : new SizeValue();
    public SizeValue MaxHeight => Style["max-height"].Value is SizeValue s ? s : new SizeValue();
    
    
    /// <summary>
    /// the text content of the paragraph
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Represents the CSS-like selectors associated with the element, allowing the element
    /// to be styled or targeted based on these identifiers.
    /// </summary>
    public string[] Selectors { get; set; }
    
    /// <inheritdoc cref="IElement.Style"/>
    public StyleCollection Style { get; }

    /// <summary>
    /// A delegate property that handles key press events for the Paragraph element.
    /// </summary>
    /// <remarks>
    /// The handler is invoked whenever a key press event occurs. The event arguments,
    /// encapsulated within <see cref="KeyEventArgs"/>, provide information about the pressed key.
    /// </remarks>
    public Action<object?, KeyEventArgs> KeyPressHandler { get; set; } = (_, _) => { };

    /// <summary>
    /// Handles resize events for the paragraph element. This property allows
    /// users to assign custom logic to be executed when the element's size changes.
    /// </summary>
    public Action<ResizeEventArgs> ResizeHandler { get; set; } = _ => { };

    /// <summary>
    /// Represents an action invoked before the render process of the element.
    /// This delegate property can be used to edit the content or additional rendering
    /// behaviors when the element is being rendered.
    /// </summary>
    public Action OnRender { get; set; } = () => { };

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
        
        OnRender();
        
        var borderRect = IElement.GetBorderRect(rect, Margin);
        var contentRect = IElement.GetContentRect(rect, Margin, Padding, Border);
        
        target.DrawRect(borderRect, BackgroundColor, Border);
        
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

    public int GetWidth(int maxHeight) {
        var w = Content.Length +
                Padding.Left.ToIntH(maxHeight) +
                Padding.Right.ToIntH(maxHeight) +
                (Border.IsBorderless ? 0 : 2);
        
        var min = MinWidth.ToIntH(0);
        var max = MaxWidth.ToIntH(Int32.MaxValue);
        return Math.Max(min, Math.Min(w, max));
    }

    public int GetHeight(int maxWidth) {
        int contentWidth = IElement.GetContentRect(new Rectangle(0, 0, maxWidth, int.MaxValue), Margin, Padding, Border).Width;
        var f = Font;
        var lineCount = f == IFont.Default 
            ? Content.Chop(contentWidth).Length 
            : IFont.Chop(Content, f, contentWidth).Length;
        int contentHeight = lineCount * f.LineSize + (lineCount - 1) * f.LineSpacing;
        int marginHeight = Margin.Top.ToIntV(maxWidth) + Margin.Bottom.ToIntV(maxWidth);
        int borderHeight = Border.IsBorderless ? 0 : 2;
        int paddingHeight = Padding.Top.ToIntV(maxWidth) + Padding.Bottom.ToIntV(maxWidth);
        return contentHeight + paddingHeight + borderHeight + marginHeight;
    }

    public int GetMinWidth(int maxHeight) => Content.Split(' ').Select(word => word.Length).Prepend(0).Max();
    public int GetMinHeight(int maxWidth) => Content.Chop(maxWidth).Length;

    /// <summary>
    /// Handles a key press event for the paragraph element by invoking the <see cref="KeyPressHandler"/> delegate..
    /// </summary>
    /// <param name="source">The source object that raised the event.</param>
    /// <param name="args">Event arguments containing details of the key press event.</param>
    public void HandleKeyPress(object? source, KeyEventArgs args) => KeyPressHandler(source, args);

    /// <summary>
    /// Handles the resize event for the paragraph element by invoking the <see cref="ResizeHandler"/> delegate.
    /// </summary>
    /// <param name="args">The resize event arguments containing the new width and height.</param>
    public void HandleResize(ResizeEventArgs args) => ResizeHandler(args);
}