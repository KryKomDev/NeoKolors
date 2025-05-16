//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Reflection;
using System.Runtime.CompilerServices;
using NeoKolors.Common.Util;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

public interface IElement : IRenderable {
    
    /// <summary>
    /// The classes and ids of the element
    /// </summary>
    public string[] Selectors { get; }

    /// <summary>
    /// Represents the collection of styles applied to the element.
    /// </summary>
    public StyleCollection Style { get; }

    /// <summary>
    /// Applies the styles to the element
    /// </summary>
    public void ApplyStyles(StyleCollection styles) => Style.OverrideWith(styles);

    /// <summary>
    /// Renders the element in the given region
    /// </summary>
    /// <param name="target">the target console screen</param>
    /// <param name="rect">the region where the element shall be rendered</param>
    public void Render(in IConsoleScreen target, Rectangle rect);
    
    void IRenderable.Render(in IConsoleScreen target) => 
        Render(target, new Rectangle(0, 0, target.Width, target.Height));

    /// <summary>
    /// returns the name of the element (e.g. "p" for paragraph, "div" for div)
    /// </summary>
    /// <param name="element">the element to get its name</param>
    /// <returns>the name of the element</returns>
    public static string GetName(IElement element) {
        var type = element.GetType();
        var attribute = type.GetCustomAttribute<ElementNameAttribute>();
        return attribute is not null ? attribute.Name : type.Name.PascalToKebab();
    }

    /// <summary>
    /// returns the name of the element (e.g. "p" for the Paragraph, "div" for Div)
    /// </summary>
    /// <param name="type">type of the element</param>
    /// <returns>the name of the element</returns>
    public static string GetName(Type type) {
        var attribute = type.GetCustomAttribute<ElementNameAttribute>();
        return attribute is not null ? attribute.Name : type.Name.PascalToKebab();
    }

    /// <summary>
    /// computes the width of the element in the given maximal height
    /// </summary>
    /// <param name="maxHeight">the maximal height the element can take in the computed situation</param>
    /// <returns>the width the element will have in characters</returns>
    public int GetWidth(int maxHeight);

    /// <summary>
    /// computes the minimal width of the element in the given maximal height
    /// </summary>
    /// <param name="maxHeight">the maximal height the element can take in the computed situation</param>
    /// <returns>the width the element will have in characters</returns>
    public int GetMinWidth(int maxHeight);
    
    /// <summary>
    /// computes the height of the element in the given maximal width
    /// </summary>
    /// <param name="maxWidth">the maximal width the element can take in the computed situation</param>
    /// <returns>the height the element will have in characters</returns>
    public int GetHeight(int maxWidth);
    
    /// <summary>
    /// computes the minimal height of the element in the given maximal width
    /// </summary>
    /// <param name="maxWidth">the maximal width the element can take in the computed situation</param>
    /// <returns>the height the element will have in characters</returns>
    public int GetMinHeight(int maxWidth);

    /// <summary>
    /// computes the border rectangle of the element
    /// </summary>
    public static Rectangle GetBorderRect(Rectangle rect, MarginProperty margin) => ApplyMargin(margin, rect);

    /// <summary>
    /// Calculates the content rectangle of an element based on the provided dimensions and styles.
    /// </summary>
    /// <param name="rect">The original rectangle representing the dimensions of the element.</param>
    /// <param name="margin">The margin property to apply, affecting the outer spacing of the element.</param>
    /// <param name="padding">The padding property to apply, affecting the inner spacing within the element.</param>
    /// <param name="border">The border style to apply, affecting the final position of the content area.</param>
    /// <returns>The adjusted rectangle representing the content area.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when an invalid margin or padding unit is encountered.
    /// </exception>
    /// <exception cref="NotImplementedException">
    /// Thrown when a specific padding or margin unit type (min-content and max-content) is not implemented.
    /// </exception>
    public static Rectangle GetContentRect(
        Rectangle rect,
        MarginProperty margin,
        PaddingProperty padding,
        BorderStyle border) 
    {
        var mr = ApplyMargin(margin, rect);

        int lx = padding.Left.Value.Match(
            u => mr.LowerX + u.Unit switch {
                SizeUnit.CHAR => u.Value,
                SizeUnit.PERCENT => rect.Width * u.Value / 100,
                SizeUnit.PIXEL => u.Value * 2,
                _ => throw new ArgumentOutOfRangeException(nameof(margin))
            },
            _ => mr.LowerX,
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        );

        int hx = padding.Right.Value.Match(
            u => mr.HigherX - u.Unit switch {
                SizeUnit.CHAR => u.Value,
                SizeUnit.PERCENT => rect.Width * u.Value / 100,
                SizeUnit.PIXEL => u.Value * 2,
                _ => throw new ArgumentOutOfRangeException(nameof(margin))
            },
            _ => mr.HigherX,
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        );

        int ly = padding.Top.Value.Match(
            u => mr.LowerY + u.Unit switch {
                SizeUnit.CHAR => u.Value,
                SizeUnit.PERCENT => rect.Height * u.Value / 100,
                SizeUnit.PIXEL => u.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(margin))
            },
            _ => mr.LowerY,
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        );

        int hy = padding.Bottom.Value.Match(
            u => mr.HigherY - u.Unit switch {
                SizeUnit.CHAR => u.Value,
                SizeUnit.PERCENT => rect.Height * u.Value / 100,
                SizeUnit.PIXEL => u.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(margin))
            },
            _ => mr.HigherY,
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        );
        
        return ApplyBorder(border, new Rectangle(lx, ly, hx, hy));
    }
    
    /// <summary>
    /// applies the margin to the given rectangle
    /// </summary>
    /// <param name="margin">margin to be applied</param>
    /// <param name="rect">the base rectangle</param>
    /// <returns>the rectangle with the applied margin</returns>
    /// <exception cref="ArgumentOutOfRangeException">invalid uint in a margin</exception>
    /// <exception cref="NotImplementedException">min and max content margins are not supported right now</exception>
    private static Rectangle ApplyMargin(MarginProperty margin, Rectangle rect) { // TODO min and max content margins? maybe get rid of them
        int lx = margin.Left.Value.Match(
            u => rect.LowerX + u.Unit switch {
                SizeUnit.CHAR => u.Value,
                SizeUnit.PERCENT => rect.Width * u.Value / 100,
                SizeUnit.PIXEL => u.Value * 2,
                _ => throw new ArgumentOutOfRangeException(nameof(margin))
            },
            _ => rect.LowerX,
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        );

        int hx = margin.Right.Value.Match(
            u => rect.HigherX - u.Unit switch {
                SizeUnit.CHAR => u.Value,
                SizeUnit.PERCENT => rect.Width * u.Value / 100,
                SizeUnit.PIXEL => u.Value * 2,
                _ => throw new ArgumentOutOfRangeException(nameof(margin))
            },
            _ => rect.HigherX,
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        );

        int ly = margin.Top.Value.Match(
            u => rect.LowerY + u.Unit switch {
                SizeUnit.CHAR => u.Value,
                SizeUnit.PERCENT => rect.Height * u.Value / 100,
                SizeUnit.PIXEL => u.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(margin))
            },
            _ => rect.LowerY,
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        );

        int hy = margin.Bottom.Value.Match(
            u => rect.HigherY - u.Unit switch {
                SizeUnit.CHAR => u.Value,
                SizeUnit.PERCENT => rect.Height * u.Value / 100,
                SizeUnit.PIXEL => u.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(margin))
            },
            _ => rect.HigherY,
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        );

        return new Rectangle(lx, ly, hx, hy);
    }

    /// <summary>
    /// applies the border padding to the given rectangle
    /// </summary>
    /// <param name="border">the border style</param>
    /// <param name="rect">the source rectangle</param>
    /// <returns>the rectangle with the border applied</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Rectangle ApplyBorder(BorderStyle border, Rectangle rect) => 
        border.IsBorderless 
            ? rect 
            : new Rectangle(rect.LowerX + 1, rect.LowerY + 1, rect.HigherX - 1, rect.HigherY - 1);
    
    /// <summary>
    /// applies the padding to the given rectangle
    /// </summary>
    /// <param name="padding">padding to be applied</param>
    /// <param name="rect">the base rectangle</param>
    /// <returns>the rectangle with the applied padding</returns>
    /// <exception cref="ArgumentOutOfRangeException">invalid uint in a padding</exception>
    /// <exception cref="NotImplementedException">min and max content padding are not supported right now</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Rectangle ApplyPadding(PaddingProperty padding, Rectangle rect) => 
        ApplyMargin(new MarginProperty(padding.Left, padding.Top, padding.Right, padding.Bottom), rect);

    /// <summary>
    /// returns whether the given type is an element type or not
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsElement(Type t) => 
        t is { IsInterface: false, IsAbstract: false } && typeof(IElement).IsAssignableFrom(t);
}