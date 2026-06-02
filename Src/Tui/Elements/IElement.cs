// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics.Contracts;
using Implyzer;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

public interface IElement<T> : IElement, INode<T>;

[IndirectImpl(typeof(IElement<>))]
public interface IElement : IRenderable, INode {
    
    /// <summary>
    /// Represents the visual appearance configuration of an element.
    /// </summary>
    /// <remarks>
    /// This property defines various stylistic attributes such as colors, fonts, margins,
    /// and other visual aspects that dictate the look and feel of an element.
    /// Adjusting the Style property allows customization or theming of the element's appearance.
    /// </remarks>
    public StyleCollection Style { get; }

    /// <summary>
    /// Contains descriptive information or metadata related to an object or entity.
    /// </summary>
    /// <remarks>
    /// This property provides details or context about the associated object, which may include
    /// labels, summaries, or other relevant data. It is primarily used to convey additional information
    /// that helps in understanding or processing the object.
    /// </remarks>
    public ElementInfo Info { get; }

    /// <summary>
    /// Renders the element onto the canvas using its pre-calculated layout and bounds.
    /// </summary>
    public new void Render(ICharCanvas canvas);

    /// <summary>
    /// Renders the content onto the specified canvas within the full dimensions of the canvas.
    /// </summary>
    /// <param name="canvas">The drawing surface on which the content will be rendered.</param>
    void IRenderable.Render(ICharCanvas canvas) {
        Measure(new Size(canvas.Width, canvas.Height));
        Arrange(new Rectangle(0, 0, canvas.Width - 1, canvas.Height - 1));
        Render(canvas);
    }

    /// <summary>
    /// Event triggered whenever the state of an element is updated.
    /// </summary>
    /// <remarks>
    /// This event can be used to listen for changes in the element's state or properties,
    /// allowing additional actions to be performed in response to those updates.
    /// </remarks>
    public event Action OnElementUpdated;

    /// <summary>
    /// Gets the size that this element calculated during the measure pass of the layout process.
    /// </summary>
    public Size DesiredSize { get; }

    /// <summary>
    /// Gets the final render bounds of this element relative to its container.
    /// </summary>
    public Rectangle RenderBounds { get; }

    /// <summary>
    /// Gets the computed element layout (margin, border, content boxes) of this element.
    /// </summary>
    public ElementLayout RenderLayout { get; }

    /// <summary>
    /// Updates the DesiredSize of the element based on the available constraints.
    /// </summary>
    public void Measure(Size availableSize);

    /// <summary>
    /// Positions child elements and determines the final rendering size and position.
    /// </summary>
    public void Arrange(Rectangle finalRect);

    /// <summary>
    /// Invalidates the measurement state (layout) for the element.
    /// </summary>
    public void InvalidateMeasure();

    /// <summary>
    /// Invalidates the arrange state (layout) for the element.
    /// </summary>
    public void InvalidateArrange();

    /// <summary>
    /// Gets or sets whether the element is focused.
    /// </summary>
    public bool IsFocused { get; set; }

    /// <summary>
    /// Gets or sets whether the mouse cursor is hovering over the element.
    /// </summary>
    public bool IsHovered { get; set; }

    /// <summary>
    /// Gets or sets the list of style triggers for this element.
    /// </summary>
    public List<TriggerBase> Triggers { get; set; }

    /// <summary>
    /// Evaluates all active triggers and updates element styling.
    /// </summary>
    public void EvaluateTriggers();
    
    // -------------------------------------------------------------- //
    //                       LAYOUT COMPUTATION                       //
    // -------------------------------------------------------------- //

    
    //
    // The following methods can be used to compute the dimensions of an element in 
    // certain scenarios. To see what they do and how to use them correctly, see 
    // NeoKolors.Tui/Docs/Layout/Layout-Computation-Methods.md
    //

    
    /// <summary>
    /// Computes the layout of an element based on the specified content, bounds, spacing, dimensions, and functions
    /// for calculating minimum and maximum content sizes.
    /// </summary>
    /// <param name="bounds">The available size within which the element can be laid out.</param>
    /// <param name="margin">The spacing outside the element to separate it from other elements.</param>
    /// <param name="border">The border style of the element.</param>
    /// <param name="padding">The spacing inside the element between its border and its content.</param>
    /// <param name="width">The width constraint of the element.</param>
    /// <param name="height">The height constraint of the element.</param>
    /// <param name="minWidth">The minimum width constraint of the element.</param>
    /// <param name="maxWidth">The maximum width constraint of the element.</param>
    /// <param name="minHeight">The minimum height constraint of the element.</param>
    /// <param name="maxHeight">The maximum height constraint of the element.</param>
    /// <param name="computeContent">A function to compute the content size of the element for the given width.</param>
    /// <param name="computeMinContent">A function to compute the minimum content size of the element.</param>
    /// <param name="computeMaxContent">A function to compute the maximum content size of the element.</param>
    /// <returns>The computed layout of the element, including its size and positioning adjustments.</returns>
    [Pure]
    public static ElementLayout ComputeLayoutFromBounds(
        Size           bounds,
        Spacing        margin,
        BorderStyle    border,
        Spacing        padding,
        Dimension      width,
        Dimension      height,
        Dimension      minWidth,
        Dimension      maxWidth,
        Dimension      minHeight,
        Dimension      maxHeight,
        Func<int, int> computeContent,
        Func<Size>     computeMinContent,
        Func<Size>     computeMaxContent) 
    {
        // margin
        int ml = margin.Left  .ToScalarX(bounds.Width);
        int mr = margin.Right .ToScalarX(bounds.Width);
        int mt = margin.Top   .ToScalarY(bounds.Height);
        int mb = margin.Bottom.ToScalarY(bounds.Height);

        // padding
        int pl = padding.Left  .ToScalarX(bounds.Width);
        int pr = padding.Right .ToScalarX(bounds.Width);
        int pt = padding.Top   .ToScalarY(bounds.Height);
        int pb = padding.Bottom.ToScalarY(bounds.Height);
        
        var c = ComputeLayoutFromBounds(bounds, (ml, mr, mt, mb), (pl, pr, pt, pb), border).Content.Size;
        
        c = RecomputeContentBoxSize(
            c,
            bounds, 
            margin, border, padding,
            width, height, 
            minWidth, maxWidth,
            minHeight, maxHeight,
            computeContent,
            computeMinContent,
            computeMaxContent
        );
        
        return ComputeLayoutFromContent(c, (ml, mr, mt, mb), (pl, pr, pt, pb), border);
    }
    
    /// <summary>
    /// Computes the layout of an element based on the specified content, bounds, spacing, dimensions, and functions
    /// for calculating minimum and maximum content sizes.
    /// </summary>
    /// <param name="bounds">The available size within which the element can be laid out.</param>
    /// <param name="margin">The spacing outside the element to separate it from other elements.</param>
    /// <param name="border">The border style of the element.</param>
    /// <param name="padding">The spacing inside the element between its border and its content.</param>
    /// <param name="width">The width constraint of the element.</param>
    /// <param name="height">The height constraint of the element.</param>
    /// <param name="minWidth">The minimum width constraint of the element.</param>
    /// <param name="maxWidth">The maximum width constraint of the element.</param>
    /// <param name="minHeight">The minimum height constraint of the element.</param>
    /// <param name="maxHeight">The maximum height constraint of the element.</param>
    /// <returns>The computed layout of the element, including its size and positioning adjustments.</returns>
    [Pure]
    public static ElementLayout ComputeLayoutFromBounds(
        Size           bounds,
        Spacing        margin,
        BorderStyle    border,
        Spacing        padding,
        Dimension      width,
        Dimension      height,
        Dimension      minWidth,
        Dimension      maxWidth,
        Dimension      minHeight,
        Dimension      maxHeight) 
    {
        // margin
        int ml = margin.Left  .ToScalarX(bounds.Width);
        int mr = margin.Right .ToScalarX(bounds.Width);
        int mt = margin.Top   .ToScalarY(bounds.Height);
        int mb = margin.Bottom.ToScalarY(bounds.Height);

        // padding
        int pl = padding.Left  .ToScalarX(bounds.Width);
        int pr = padding.Right .ToScalarX(bounds.Width);
        int pt = padding.Top   .ToScalarY(bounds.Height);
        int pb = padding.Bottom.ToScalarY(bounds.Height);
        
        var c = ComputeLayoutFromBounds(bounds, (ml, mr, mt, mb), (pl, pr, pt, pb), border).Content.Size;
        
        c = RecomputeContentBoxSize(
            c, 
            bounds, 
            margin, border, padding, 
            width, height,
            minWidth, maxWidth,
            minHeight, maxHeight
        );
        
        return ComputeLayoutFromContent(c, (ml, mr, mt, mb), (pl, pr, pt, pb), border);
    }

    /// <summary>
    /// Computes the layout of an element based on the specified content, bounds, spacing, dimensions, and functions
    /// for calculating minimum and maximum content sizes.
    /// </summary>
    /// <param name="content">The size of the content to be laid out within the element.</param>
    /// <param name="parent">The parent element content box size to be used as a reference when recomputing the relative units.</param>
    /// <param name="margin">The spacing outside the element to separate it from other elements.</param>
    /// <param name="border">The border style of the element.</param>
    /// <param name="padding">The spacing inside the element between its border and its content.</param>
    /// <param name="width">The width constraint of the element.</param>
    /// <param name="height">The height constraint of the element.</param>
    /// <param name="minWidth">The minimum width constraint of the element.</param>
    /// <param name="maxWidth">The maximum width constraint of the element.</param>
    /// <param name="minHeight">The minimum height constraint of the element.</param>
    /// <param name="maxHeight">The maximum height constraint of the element.</param>
    /// <param name="computeContent">A function to compute the content size of the element for the given width.</param>
    /// <param name="computeMinContent">A function to compute the minimum content size of the element.</param>
    /// <param name="computeMaxContent">A function to compute the maximum content size of the element.</param>
    /// <returns>The computed layout of the element, including its size and positioning adjustments.</returns>
    [Pure]
    public static ElementLayout ComputeLayoutFromContent(
        Size           content,
        Size           parent,
        Spacing        margin,
        BorderStyle    border,
        Spacing        padding,
        Dimension      width,
        Dimension      height,
        Dimension      minWidth,
        Dimension      maxWidth,
        Dimension      minHeight,
        Dimension      maxHeight,
        Func<int, int> computeContent,
        Func<Size>     computeMinContent,
        Func<Size>     computeMaxContent) 
    {
        var c = content;
        
        c = RecomputeContentBoxSize(
            c,
            parent,
            margin, border, padding,
            width, height,
            minWidth, maxWidth,
            minHeight, maxHeight,
            computeContent, 
            computeMinContent,
            computeMaxContent
        );
        
        return ComputeLayoutFromContent(c, margin, padding, border);
    }


    /// <summary>
    /// Computes the layout of an element based on the specified content, bounds, spacing, dimensions, and functions
    /// for calculating minimum and maximum content sizes.
    /// </summary>
    /// <param name="content">The size of the content to be laid out within the element.</param>
    /// <param name="parent">The parent element content box size to be used as a reference when recomputing the relative units.</param>
    /// <param name="margin">The spacing outside the element to separate it from other elements.</param>
    /// <param name="border">The border style of the element.</param>
    /// <param name="padding">The spacing inside the element between its border and its content.</param>
    /// <param name="width">The width constraint of the element.</param>
    /// <param name="height">The height constraint of the element.</param>
    /// <param name="minWidth">The minimum width constraint of the element.</param>
    /// <param name="maxWidth">The maximum width constraint of the element.</param>
    /// <param name="minHeight">The minimum height constraint of the element.</param>
    /// <param name="maxHeight">The maximum height constraint of the element.</param>
    /// <returns>The computed layout of the element, including its size and positioning adjustments.</returns>
    [Pure]
    public static ElementLayout ComputeLayoutFromContent(
        Size           content,
        Size           parent,
        Spacing        margin,
        BorderStyle    border,
        Spacing        padding,
        Dimension      width,
        Dimension      height,
        Dimension      minWidth,
        Dimension      maxWidth,
        Dimension      minHeight,
        Dimension      maxHeight) 
    {
        var c = content;
        
        c = RecomputeContentBoxSize(
            c,
            parent,
            margin,    border, padding, 
            width,     height,
            minWidth,  maxWidth,
            minHeight, maxHeight
        );
        
        return ComputeLayoutFromContent(c, margin, padding, border);
    }

    [Pure]
    private static Size RecomputeContentBoxSize(
        Size            content, 
        Size            parent,
        Spacing         margin,
        BorderStyle     border,
        Spacing         padding,
        Dimension       width,
        Dimension       height,
        Dimension       minWidth,
        Dimension       maxWidth,
        Dimension       minHeight,
        Dimension       maxHeight,
        Func<int, int>? computeContent    = null,
        Func<Size>?     computeMinContent = null,
        Func<Size>?     computeMaxContent = null) 
    {
        computeMinContent ??= ( ) => new Size(content.Width, content.Height);
        computeMaxContent ??= ( ) => new Size(content.Width, content.Height);
        computeContent    ??= (_) => content.Height;
        
        Size? minC = null;
        Size? maxC = null;
        var c = content;

        // recompute the width of the content box
        if (width.IsMinContent) {
            minC = computeMinContent();
            c = c with { Width = minC.Value.Width };
        }
        else if (width.IsMaxContent) {
            maxC = computeMaxContent();
            c = c with { Width = maxC.Value.Width };
        }
        else if (width.IsStretch) {
            var w = parent.Width 
                - margin .Left.ToScalarX(parent.Width) - margin .Right.ToScalarX(parent.Width) 
                - padding.Left.ToScalarX(parent.Width) - padding.Right.ToScalarX(parent.Width) 
                - (border.IsBorderless ? 0 : 2);
            
            c = c with { Width = w };
        }
        else if (width.IsNumber) {
            int w = width.ToScalarX(parent.Width);

            // if the height is automatically computed, recompute it for the given width
            if (height.IsAuto) {
                int h = computeContent(w);
                c = new Size(width: w, height: h);   
            }
            else {
                c = c with { Width = w };
            }
        }
        else {
            var xw = maxWidth.IsNumber ? maxWidth.ToScalarX(parent.Width) : int.MaxValue;
            var nw = minWidth.IsNumber ? minWidth.ToScalarX(parent.Width) : 0;
            c = c with { Width = Math.DClamp(content.Width, xw, nw) };
        }

        // recompute the height of the content box
        if (height.IsMinContent) {
            minC ??= computeMinContent();
            c = c with { Height = minC.Value.Height };
        }
        else if (height.IsMaxContent) {
            maxC ??= computeMaxContent();
            c = c with { Height = maxC.Value.Height };
        }
        else if (height.IsStretch) {
            var h = parent.Height 
                - margin .Top.ToScalarY(parent.Height) - margin .Bottom.ToScalarY(parent.Height) 
                - padding.Top.ToScalarY(parent.Height) - padding.Bottom.ToScalarY(parent.Height) 
                - (border.IsBorderless ? 0 : 2);
            
            c = c with { Height = h };
        }
        else if (height.IsNumber) {
            c = c with { Height = height.ToScalarY(parent.Height) };
        }
        else {
            var xh = maxHeight.IsNumber ? maxHeight.ToScalarY(parent.Height) : int.MaxValue;
            var nh = minHeight.IsNumber ? minHeight.ToScalarY(parent.Height) : 0;
            c = c with { Height = Math.DClamp(content.Height, xh, nh) };
        }

        return c;
    }

    [Pure]
    private static ElementLayout ComputeLayoutFromContent(
        Size        content,
        Spacing     margin,
        Spacing     padding,
        BorderStyle border) 
    {
        var ml = margin.Left  .ToScalarX(content.Width);
        var mr = margin.Right .ToScalarX(content.Width);
        var mt = margin.Top   .ToScalarY(content.Height);
        var mb = margin.Bottom.ToScalarY(content.Height);
        
        var pl = padding.Left  .ToScalarX(content.Width);
        var pr = padding.Right .ToScalarX(content.Width);
        var pt = padding.Top   .ToScalarY(content.Height);
        var pb = padding.Bottom.ToScalarY(content.Height);

        return ComputeLayoutFromContent(
            content,
            (ml, mr, mt, mb),
            (pl, pr, pt, pb),
            border
        );
    }
    
    [Pure]
    private static ElementLayout ComputeLayoutFromContent(
        Size content,
        (int L, int R, int T, int B) margin,
        (int L, int R, int T, int B) padding,
        BorderStyle border) 
    {
        int bd = border.IsBorderless ? 0 : 1;

        // margin
        int ml = margin.L;
        int mr = margin.R;
        int mt = margin.T;
        int mb = margin.B;

        // padding
        int pl = padding.L;
        int pr = padding.R;
        int pt = padding.T;
        int pb = padding.B;
        
        var e = new Size(content.Width + pl + pr + bd * 2 + ml + mr, content.Height + pt + pb + bd * 2 + mt + mb);
        var c = new Rectangle(new Point(ml + pl + bd, mt + pt + bd), content);

        // no border
        if (bd == 0) return new ElementLayout(e, c, e);

        // yes border :)
        var b = new Rectangle(new Point(ml, mt), new Size(e.Width - ml - mr, e.Height - mt - mb));
        return new ElementLayout(e, c, b);
    }

    [Pure]
    private static ElementLayout ComputeLayoutFromBounds(
        Size bounds,
        (int L, int R, int T, int B) margin,
        (int L, int R, int T, int B) padding,
        BorderStyle border) 
    {
        int bd = border.IsBorderless ? 0 : 1;

        // margin
        int ml = margin.L;
        int mr = margin.R;
        int mt = margin.T;
        int mb = margin.B;

        // padding
        int pl = padding.L;
        int pr = padding.R;
        int pt = padding.T;
        int pb = padding.B;
        
        var e = bounds;
        var c = new Rectangle(
            new Point(ml + pl + bd, mt + pt + bd), 
            new Size(
                bounds.Width  - (ml + mr + pl + pr + 2 * bd), 
                bounds.Height - (mt + mb + pt + pb + 2 * bd)
            )
        );

        // no border
        if (bd == 0) return new ElementLayout(e, c, e);

        // yes border :)
        var b = new Rectangle(new Point(ml, mt), new Size(e.Width - ml - mr, e.Height - mt - mb));
        return new ElementLayout(e, c, b);
    } 
}