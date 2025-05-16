//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.Contracts;
using NeoKolors.Common;
using NeoKolors.Tui.Exceptions;
using NeoKolors.Tui.Styles;
using static NeoKolors.Tui.Elements.ApplicableStylesAttribute.Predefined;

namespace NeoKolors.Tui.Elements;

[ElementName("div")]
[ApplicableStyles(CONTAINER | UNIVERSAL)]
public class Div : IElement {
    
    public AlignItems AlignItems => Style["align-items"].Value is AlignItems align ? align : AlignItems.STRETCH;
    public AlignDirection AlignSelf => Style["align-self"].Value is AlignDirection align ? align : new AlignDirection();
    public NKColor BackgroundColor => Style["background-color"].Value is NKColor color ? color : NKColor.Inherit;
    public BorderStyle Border => Style["border"].Value is BorderStyle s ? s : BorderStyle.GetBorderless();
    public DisplayType Display => Style["display"].Value is DisplayType d ? d : DisplayType.BLOCK;
    public PaddingProperty Padding => Style["padding"].Value is PaddingProperty p ? p : new PaddingProperty();
    public MarginProperty Margin => Style["margin"].Value is MarginProperty m ? m : new MarginProperty();
    public FlexDirection FlexDirection => Style["flex-direction"].Value is FlexDirection f ? f : FlexDirection.ROW;
    public OverflowType Overflow => Style["overflow"].Value is OverflowType o ? o : OverflowType.HIDDEN;
    public GridProperty Grid => Style["grid"].Value is GridProperty g ? g : new GridProperty();
    public JustifyContent JustifyContent => Style["justify-content"].Value is JustifyContent j ? j : new JustifyContent(); // Todo : implement justify-content
    
    public List<IElement> Children { get; }

    public string[] Selectors { get; }
    public StyleCollection Style { get; }

    public Div(params IElement[] children) : this([], children) { }
    
    public Div(string[] selectors, params IElement[] children) {
        Children = children.ToList();
        Selectors = selectors;
        Style = new StyleCollection();
    }
    
    public void ApplyStyles(StyleCollection styles) {
        Style.OverrideWith(styles);
    }
    
    public virtual void Render(in IConsoleScreen target, Rectangle rect) {
        if (Display == DisplayType.NONE) return;
        
        var borderRect = IElement.GetBorderRect(rect, Margin);
        var contentRect = IElement.GetContentRect(rect, Margin, Padding, Border);
        
        target.DrawRect(borderRect, BackgroundColor, Border);
        
        switch (Display) {
            case DisplayType.FLEX: 
                if (FlexDirection is FlexDirection.ROW or FlexDirection.ROW_REVERSE)
                    RenderChildrenFlexRow(target, contentRect);
                else
                    RenderChildrenFlexColumn(target, contentRect); 
                break;
            case DisplayType.INLINE:
                break;
            case DisplayType.INLINE_BLOCK:
                break;
            case DisplayType.INLINE_FLEX:
                break;
            case DisplayType.INLINE_GRID:
                break;
            case DisplayType.BLOCK:
                RenderChildrenBlock(target, contentRect);
                break;
            case DisplayType.GRID:
                RenderChildrenGrid(target, contentRect);
                break;
            case DisplayType.NONE:
            default:                
                throw new ArgumentOutOfRangeException();
        }
    }

    protected void RenderChildrenFlexRow(in IConsoleScreen target, Rectangle rect) {
        int total = 0;
        foreach (var child in Children) {
            total += child.GetWidth(rect.Height);
        }

        if (total <= rect.Width) {
            if (FlexDirection is FlexDirection.ROW) {
                int c = rect.LowerX;
                for (int i = 0; i < Children.Count; i++) {
                    var cw = Children[i].GetWidth(rect.Height);
                    var v = ComputeAlignItemRow(Children[i].GetHeight(cw), rect);
                    
                    Children[i].Render(target, new Rectangle(c, v.LowerY, c + cw, v.HigherY));
                    
                    c += cw + 1;
                }
            }
            else if (FlexDirection is FlexDirection.ROW_REVERSE) {
                int c = rect.HigherX;
                for (int i = Children.Count - 1; i >= 0; i--) {
                    var cw = Children[i].GetWidth(rect.Height);
                    var v = ComputeAlignItemRow(Children[i].GetHeight(cw), rect);
                    
                    Children[i].Render(target, new Rectangle(c - cw, v.LowerY, c, v.HigherY));
                    
                    c -= cw + 1;
                }
            }
            else {
                throw new StylePropertyValueOutOfRangeException(typeof(FlexDirection), FlexDirection);
            }
            
            return;
        }
        
        float single = (float)rect.Width / Children.Count;
        
        if (FlexDirection is FlexDirection.ROW) {
            float current = rect.LowerX;
            for (int i = 0; i < Children.Count; i++) {
                var v = ComputeAlignItemRow(Children[i].GetHeight((int)single), rect);
                
                Children[i].Render(target, new Rectangle(
                    (int)current, v.LowerY, (int)(current + single - 1), v.HigherY));
                
                current += single;
            }
        }
        else if (FlexDirection is FlexDirection.ROW_REVERSE) {
            float current = rect.HigherX;
            for (int i = Children.Count - 1; i >= 0; i--) {
                var v = ComputeAlignItemRow(Children[i].GetHeight((int)single), rect);
                
                Children[i].Render(target, new Rectangle(
                    (int)(current - single - 1), v.LowerY, (int)current, v.HigherY));
                
                current += single;
            }
        }
        else {
            throw new StylePropertyValueOutOfRangeException(typeof(FlexDirection), FlexDirection);
        }
    }
    
    [Pure]
    private (int LowerY, int HigherY) ComputeAlignItemRow(int childHeight, Rectangle rect) {
        switch (AlignItems) {
            case AlignItems.START: 
                return (rect.LowerY, childHeight + rect.LowerY);
            case AlignItems.END: 
                return (rect.HigherY - childHeight, rect.HigherY);
            case AlignItems.CENTER:
                float offset = (float)(rect.Height - childHeight - 1) / 2;
                return ((int, int))(Math.Floor(rect.LowerY + offset), Math.Floor(rect.HigherY - offset));
            case AlignItems.STRETCH:
                return (rect.LowerY, rect.HigherY);
            default:
                throw new StylePropertyValueOutOfRangeException(typeof(AlignItems), AlignItems);
        }
    }

    protected void RenderChildrenFlexColumn(in IConsoleScreen target, Rectangle rect) {
        int total = 0;
        foreach (var child in Children) {
            total += child.GetHeight(rect.Width);
        }

        if (total <= rect.Height) {
            if (FlexDirection is FlexDirection.COLUMN) {
                int r = rect.LowerY;
                for (int i = 0; i < Children.Count; i++) {
                    var ch = Children[i].GetHeight(rect.Width);
                    var h = ComputeAlignItemCol(Children[i].GetWidth(ch), rect);
                    
                    Children[i].Render(target, new Rectangle(h.LowerX, r, h.HigherX, r + ch));
                    
                    r += ch + 1;
                }
            }
            else if (FlexDirection is FlexDirection.COLUMN_REVERSE) {
                int r = rect.HigherY;
                for (int i = Children.Count - 1; i >= 0; i--) {
                    var ch = Children[i].GetHeight(rect.Width);
                    var h = ComputeAlignItemCol(Children[i].GetWidth(ch), rect);
                    
                    Children[i].Render(target, new Rectangle(h.LowerX, r - ch, h.HigherX, r));
                    
                    r -= ch + 1;
                }
            }
            else {
                throw new StylePropertyValueOutOfRangeException(typeof(FlexDirection), FlexDirection);
            }
            
            return;
        }
        
        float single = (float)rect.Height / Children.Count;
        
        if (FlexDirection is FlexDirection.COLUMN) {
            float current = rect.LowerY;
            for (int i = 0; i < Children.Count; i++) {
                var h = ComputeAlignItemCol(Children[i].GetWidth((int)single), rect);
                
                Children[i].Render(target, new Rectangle(
                    h.LowerX, (int)current, h.HigherX, (int)(current + single - 1)));
                
                current += single;
            }
        }
        else if (FlexDirection is FlexDirection.COLUMN_REVERSE) {
            float current = rect.HigherY;
            for (int i = Children.Count - 1; i >= 0; i--) {
                var h = ComputeAlignItemCol(Children[i].GetWidth((int)single), rect);
                
                Children[i].Render(target, new Rectangle(
                    h.LowerX, (int)(current - single - 1), h.HigherX, (int)current));
                
                current += single;
            }
        }
        else {
            throw new StylePropertyValueOutOfRangeException(typeof(FlexDirection), FlexDirection);
        }
    }
    
    [Pure]
    private (int LowerX, int HigherX) ComputeAlignItemCol(int childWidth, Rectangle rect) {
        switch (AlignItems) {
            case AlignItems.START: 
                return (rect.LowerX, childWidth + rect.LowerX);
            case AlignItems.END: 
                return (rect.HigherX - childWidth, rect.HigherX);
            case AlignItems.CENTER:
                float offset = (float)(rect.Width - childWidth - 1) / 2;
                return ((int, int))(Math.Floor(rect.LowerX + offset), Math.Floor(rect.HigherX - offset));
            case AlignItems.STRETCH:
                return (rect.LowerX, rect.HigherX);
            default:
                throw new StylePropertyValueOutOfRangeException(typeof(AlignItems), AlignItems);
        }
    }

    protected void RenderChildrenBlock(in IConsoleScreen target, Rectangle rect) {
        int current = rect.LowerY;
        for (int i = 0; i < Children.Count; i++) {
            int ch = Children[i].GetHeight(rect.Width);
            
            Children[i].Render(target, new Rectangle(
                rect.LowerX, current, rect.HigherX, current + ch - 1));
            
            current += ch;
            if (current > rect.HigherY && !Overflow.HasFlag(OverflowType.VISIBLE_BOTTOM)) break;
        }
    }

    protected void RenderChildrenGrid(in IConsoleScreen target, Rectangle rect) {
        var (widths, heights) = ComputeGrid(rect);
        
        foreach (var c in Children) {
            c.Render(target, ComputeGridRect(rect, c.Style["grid-align"].Value 
                is Rectangle align ? align : new Rectangle(0, 0, 0, 0), widths, heights));
        }
    }

    private (int[] Widths, int[] Heights) ComputeGrid(Rectangle rect) {
        var g = Grid;

        int[] widths = new int[g.ColCount];
        int[] heights = new int[g.RowCount];
        
        bool floor = false;
        int sumW = 0;
        int skipped = 0;
        for (int i = 0; i < g.ColCount; i++) {
            if (!g.Cols[i].IsNumber) {
                skipped++;
                continue;
            }

            int w;
            if (floor) w = (int)Math.Floor(g.Cols[i].ToFloatH(rect.Width));
            else w = (int)Math.Ceiling(g.Cols[i].ToFloatH(rect.Width));

            sumW += w;
            widths[i] = w;
            floor = !floor;
        }

        floor = true;
        float singleW = (rect.Width - sumW) / (float)skipped;
        for (int i = 0; i < g.ColCount; i++) {
            if (g.Cols[i].IsNumber) continue;

            if (floor) widths[i] = (int)Math.Floor(singleW);
            else widths[i] = (int)Math.Ceiling(singleW);

            floor = !floor;
        }
        
        floor = false;
        int sumH = 0;
        skipped = 0;
        for (int i = 0; i < g.RowCount; i++) {
            if (!g.Rows[i].IsNumber) {
                skipped++;
                continue;
            }
            
            int h;
            if (floor) h = (int)Math.Floor(g.Cols[i].ToFloatV(rect.Height));
            else h = (int)Math.Ceiling(g.Cols[i].ToFloatV(rect.Height));
            
            sumH += h;
            heights[i] = h;
            floor = !floor;
        }

        floor = true;
        float singleH = (rect.Height - sumH) / (float)skipped;
        for (int i = 0; i < g.RowCount; i++) {
            if (g.Rows[i].IsNumber) continue;

            if (floor) {
                heights[i] = (int)Math.Floor(singleH);
            }
            else {
                heights[i] = (int)Math.Ceiling(singleH);
            }

            floor = !floor;
        }
        
        return (widths, heights);
    }

    private Rectangle ComputeGridRect(Rectangle rect, Rectangle align, int[] widths, int[] heights) {
        int lx = rect.LowerX;
        int ly = rect.LowerY;
        int hx = rect.LowerX - 1;
        int hy = rect.LowerY - 1;

        for (int i = 0; i < Math.Min(align.LowerX, widths.Length); i++) {
            lx += widths[i];
        }

        for (int i = 0; i < Math.Min(align.LowerY, heights.Length); i++) {
            ly += heights[i];
        }

        for (int i = 0; i < Math.Min(align.HigherX + 1, widths.Length); i++) {
            hx += widths[i];
        }

        for (int i = 0; i < Math.Min(align.HigherY + 1, heights.Length); i++) {
            hy += heights[i];
        }
        
        return new Rectangle(lx, ly, hx, hy);
    }
    
    public int GetWidth(int maxHeight) {
        int w = 0;
        
        switch (Display) {
            case DisplayType.NONE:
                return 0;
            case DisplayType.INLINE:
                throw new NotImplementedException();
            case DisplayType.INLINE_BLOCK:
                throw new NotImplementedException();
            case DisplayType.INLINE_FLEX:
                throw new NotImplementedException();
            case DisplayType.INLINE_GRID:
                throw new NotImplementedException();
            case DisplayType.BLOCK:
                w += Children.Sum(c => c.GetWidth(maxHeight));
                break;
            case DisplayType.FLEX:
                switch (FlexDirection) {
                    case FlexDirection.ROW:
                        w += Children.Sum(c => c.GetWidth(maxHeight));
                        break;
                    case FlexDirection.COLUMN:
                        w += Children.Max(e => e.GetWidth(maxHeight));
                        break;
                    case FlexDirection.ROW_REVERSE:
                        w += Children.Sum(c => c.GetWidth(maxHeight));
                        break;
                    case FlexDirection.COLUMN_REVERSE:
                        w += Children.Max(e => e.GetWidth(maxHeight));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case DisplayType.GRID:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
        }

        return w;
    }

    public int GetMinWidth(int maxHeight) {
        return GetWidth(maxHeight);
    }

    public int GetHeight(int maxWidth) {
        int h = 0;
        int contentWidth = IElement.GetContentRect(new Rectangle(0, 0, maxWidth, int.MaxValue), Margin, Padding, Border).Width;
        int marginHeight = Margin.Top.ToIntV(maxWidth) + Margin.Bottom.ToIntV(maxWidth);
        int borderHeight = Border.IsBorderless ? 0 : 2;
        int paddingHeight = Padding.Top.ToIntV(maxWidth) + Padding.Bottom.ToIntV(maxWidth);
        
        switch (Display) {
            case DisplayType.NONE:
                return 0;
            case DisplayType.INLINE:
                throw new NotImplementedException();
            case DisplayType.INLINE_BLOCK:
                throw new NotImplementedException();
            case DisplayType.INLINE_FLEX:
                throw new NotImplementedException();
            case DisplayType.INLINE_GRID:
                throw new NotImplementedException();
            case DisplayType.BLOCK:
                h += Children.Sum(c => c.GetHeight(contentWidth));
                break;
            case DisplayType.FLEX:
                switch (FlexDirection) {
                    case FlexDirection.ROW:
                        h += Children.Max(e => e.GetHeight(contentWidth)) + 1;
                        break;
                    case FlexDirection.COLUMN:
                        h += Children.Sum(c => c.GetHeight(contentWidth));
                        break;
                    case FlexDirection.ROW_REVERSE:
                        h += Children.Max(e => e.GetHeight(contentWidth)) + 1;
                        break;
                    case FlexDirection.COLUMN_REVERSE:
                        h += Children.Sum(c => c.GetHeight(contentWidth));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            case DisplayType.GRID:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException();
        }

        return h + marginHeight + borderHeight + paddingHeight;
    }

    public int GetMinHeight(int maxWidth) {
        return GetHeight(maxWidth);   
    }
}