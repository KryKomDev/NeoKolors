// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.Contracts;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;
namespace NeoKolors.Tui.Elements;

public interface IElement : IRenderable, INode {
    public Styles.StyleCollection Style { get; }
    public ElementInfo Info { get; }
    
    public void Render(ICharCanvas canvas, Rectangle rect);
    void IRenderable.Render(ICharCanvas canvas) => Render(canvas, new Rectangle(0, 0, canvas.Width, canvas.Height));

    /// <summary>
    /// Calculates the minimum layout size required by the element based on the provided parent size.
    /// </summary>
    /// <param name="parent">The size of the parent container, used as a basis for determining relative sizes.</param>
    /// <returns>A <see cref="Size"/> struct representing the minimum width and its corresponding height required
    /// by the element.</returns>
    public Size GetMinSize(Size parent);

    /// <summary>
    /// Calculates the maximum layout size required by the element based on the provided parent size.
    /// </summary>
    /// <param name="parent">The size of the parent container, used as a basis for determining relative sizes.</param>
    /// <returns>A <see cref="Size"/> struct representing the maximum width and its corresponding height required
    /// by the element.</returns>
    public Size GetMaxSize(Size parent);

    /// <summary>
    /// Computes the render layout size of an element based on the given bounding size.
    /// </summary>
    /// <param name="bounds">The bounding size within which the element's render layout is calculated.</param>
    /// <returns>A <see cref="Size"/> struct representing the computed width and height of the render layout.</returns>
    public Size GetRenderSize(Size bounds);

    /// <summary>
    /// Event triggered whenever the state of an element is updated.
    /// </summary>
    /// <remarks>
    /// This event can be used to listen for changes in the element's state or properties,
    /// allowing additional actions to be performed in response to those updates.
    /// </remarks>
    public event Action OnElementUpdated;
    
    
    // -------------------------------------------------------------- //
    //                       LAYOUT COMPUTATION                       //
    // -------------------------------------------------------------- //

    /// <summary>
    /// Computes the layout of an element based on its content size, parent container size, and style properties such as margin, padding, and border.
    /// </summary>
    /// <param name="content">The size of the content to be laid out.</param>
    /// <param name="parent">The size of the parent container in which the element is being laid out.</param>
    /// <param name="margin">The margin style properties defining the spacing outside the element.</param>
    /// <param name="padding">The padding style properties defining the spacing inside the element, around the content.</param>
    /// <param name="border">The border style properties, including whether the element's border is present or absent.</param>
    /// <returns>An <c>ElementLayout</c> struct containing computed sizes and positions for the element's overall size, content area, and border area.</returns>
    [Pure]
    public static ElementLayout ComputeLayout(
        Size content, Size parent,
        MarginProperty margin,
        PaddingProperty padding,
        BorderStyle border) 
    {
        int bd = border.IsBorderless ? 0 : 1;

        // margin
        int ml = margin.Left  .ToScalar(parent.Width);
        int mr = margin.Right .ToScalar(parent.Width);
        int mt = margin.Top   .ToScalar(parent.Height);
        int mb = margin.Bottom.ToScalar(parent.Height);

        // padding
        int pl = padding.Left  .ToScalar(parent.Width);
        int pr = padding.Right .ToScalar(parent.Width);
        int pt = padding.Top   .ToScalar(parent.Height);
        int pb = padding.Bottom.ToScalar(parent.Height);
        
        var e = new Size(content.Width + pl + pr + bd * 2 + ml + mr, content.Height + pt + pb + bd * 2 + mt + mb);
        var c = new Rectangle(new Point(ml + pl + bd, mt + pt + bd), content);

        // no border
        if (bd == 0) return new ElementLayout(e, c, e);

        // yes border :)
        var b = new Rectangle(new Point(ml, mt), new Size(e.Width - ml - mr, e.Height - mt - mb));
        return new ElementLayout(e, c, b);
    }

    /// <summary>
    /// Computes the layout settings for an element based on its content, parent dimensions, and style properties.
    /// </summary>
    /// <param name="content">The size of the content within the element.</param>
    /// <param name="parent">The size of the parent container, used for calculating relative dimensions.</param>
    /// <param name="margin">The margin property that defines space outside the element.</param>
    /// <param name="padding">The padding property that defines space between content and the element's edges.</param>
    /// <param name="border">The border style of the element, affecting its dimensions.</param>
    /// <param name="width">The width property of the element, defining its horizontal sizing behavior.</param>
    /// <param name="height">The height property of the element, defining its vertical sizing behavior.</param>
    /// <param name="minWidth">The minimum width property of the element, setting a lower bound on the width.</param>
    /// <param name="maxWidth">The maximum width property of the element, setting an upper bound on the width.</param>
    /// <param name="minHeight">The minimum height property of the element, setting a lower bound on the height.</param>
    /// <param name="maxHeight">The maximum height property of the element, setting an upper bound on the height.</param>
    /// <returns>An <see cref="ElementLayout"/> struct representing the computed layout settings for the element.</returns>
    [Pure]
    public static ElementLayout ComputeLayout(
        Size            content, 
        Size            parent,
        MarginProperty  margin,
        PaddingProperty padding,
        BorderStyle     border,
        Dimension       width,
        Dimension       height,
        Dimension       minWidth,
        Dimension       maxWidth,
        Dimension       minHeight,
        Dimension       maxHeight) 
    {
        var l = ComputeLayout(content, parent, margin, padding, border);

        var e = l.ElementSize;
        
        if (width.IsNumber) {
            e = e with { Width = width.ToScalar(parent.Width) };
        }
        else {
            var xw = maxWidth.IsNumber ? maxWidth.ToScalar(parent.Width) : int.MaxValue;
            var nw = minWidth.IsNumber ? minWidth.ToScalar(parent.Width) : 0;
            e = e with { Width = Math.DClamp(e.Width, xw, nw) };
        }

        if (height.IsNumber) {
            e = e with { Height = height.ToScalar(parent.Height) };
        }
        else {
            var xh = maxHeight.IsNumber ? maxHeight.ToScalar(parent.Height) : int.MaxValue;
            var nh = minHeight.IsNumber ? minHeight.ToScalar(parent.Height) : 0;
            e = e with { Height = Math.DClamp(e.Height, xh, nh) };
        }
        
        int dx = l.ElementSize.Width  - e.Width;
        int dy = l.ElementSize.Height - e.Height;

        return new ElementLayout(
            new Size(e.Width, e.Height), 
            new Size(l.Content.Width - dx, l.Content.Height - dy) + l.Content.Lower,
            new Size(l.Border!.Value.Width - dx, l.Border.Value.Height - dy) + l.Border.Value.Lower 
        );
    }

    /// <summary>
    /// Computes the layout of an element based on its size constraints, margins, padding, and border styles.
    /// </summary>
    /// <param name="bounds">The available size of the parent container.</param>
    /// <param name="margin">The margin properties of the element, defining the space outside the border.</param>
    /// <param name="padding">The padding properties of the element, defining the space inside the border.</param>
    /// <param name="border">The border style applied to the element, including whether it is borderless.</param>
    /// <param name="width">The desired width of the element, if specified.</param>
    /// <param name="height">The desired height of the element, if specified.</param>
    /// <param name="minWidth">The minimum width constraint of the element.</param>
    /// <param name="maxWidth">The maximum width constraint of the element.</param>
    /// <param name="minHeight">The minimum height constraint of the element.</param>
    /// <param name="maxHeight">The maximum height constraint of the element.</param>
    /// <returns>An <c>ElementLayout</c> struct representing the computed layout size of the element.</returns>
    [Pure]
    public static ElementLayout ComputeLayout(
        Size            bounds,
        MarginProperty  margin,
        PaddingProperty padding,
        BorderStyle     border,
        Dimension       width,
        Dimension       height,
        Dimension       minWidth,
        Dimension       maxWidth,
        Dimension       minHeight,
        Dimension       maxHeight) 
    {
        int bd = border.IsBorderless ? 0 : 2;

        // margin
        int ml = margin.Left  .ToScalar(bounds.Width);
        int mr = margin.Right .ToScalar(bounds.Width);
        int mt = margin.Top   .ToScalar(bounds.Height);
        int mb = margin.Bottom.ToScalar(bounds.Height);

        // padding
        int pl = padding.Left  .ToScalar(bounds.Width);
        int pr = padding.Right .ToScalar(bounds.Width);
        int pt = padding.Top   .ToScalar(bounds.Height);
        int pb = padding.Bottom.ToScalar(bounds.Height);

        var c = new Size(bounds.Width - ml - mr - bd - pl - pr, bounds.Height - mt - mb - bd - pt - pb);
        
        return ComputeLayout(c, bounds, margin, padding, border, width, height, minWidth, maxWidth, minHeight, maxHeight);
    }
}