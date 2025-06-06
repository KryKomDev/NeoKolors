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
public class Paragraph : TextElement, IElement {
    
    /// <summary>
    /// the text content of the paragraph
    /// </summary>
    public string Content { get; set; }

    public string[] Selectors { get; set; }
    
    /// <inheritdoc cref="IElement.Style"/>
    public override StyleCollection Style { get; }

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
                new NKStyle(Color, BackgroundColor),
                TextAlign.Horizontal, TextAlign.Vertical,
                Overflow is VISIBLE_ALL or VISIBLE_TOP,
                Overflow is VISIBLE_ALL or VISIBLE_BOTTOM);
        }
        else {
            target.DrawText(Content, contentRect,
                new NKStyle(Color, BackgroundColor),
                TextAlign.Horizontal, TextAlign.Vertical);
        }
    }

    public int GetWidth(int maxHeight) {
        var w = Content.Length +
                Padding.Left.ToIntH(maxHeight) +
                Padding.Right.ToIntH(maxHeight) +
                (Border.IsBorderless ? 0 : 2);
        
        var min = MinWidth.ToIntH(0);
        var max = MaxWidth.ToIntH(maxHeight);
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
    /// Handles a key press event for the paragraph element by invoking the <see cref="KeyPressHandler"/> delegate.
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