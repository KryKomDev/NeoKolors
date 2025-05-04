using NeoKolors.Common;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

[ElementName("ol")]
[ApplicableStyles(ApplicableStylesAttribute.Predefined.CONTAINER, "ol-style", "list-point-style")]
public class OrderedList : IElement {
    
    public NKStyle PointStyle => Style["list-point-style"].Value is NKStyle color ? color : new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
    public OListStyleProperty ListStyle => Style["ol-style"].Value is OListStyleProperty s ? s : new OListStyleProperty(); 
    public BorderStyle Border => Style["border"].Value is BorderStyle s ? s : new BorderStyle();
    public PaddingProperty Padding => Style["padding"].Value is PaddingProperty p ? p : new PaddingProperty();
    public MarginProperty Margin => Style["margin"].Value is MarginProperty m ? m : new MarginProperty();
    public DisplayType Display => Style["display"].Value is DisplayType d ? d : DisplayType.BLOCK;
    public NKColor Background => Style["background-color"].Value is NKColor color ? color : NKColor.Inherit;
    
    public string[] Selectors { get; }
    public StyleCollection Style { get; }
    public List<IElement> Children { get; }

    public OrderedList(string[] selectors, params List<IElement> children) {
        Selectors = selectors;
        Children = children;
        Style = new StyleCollection();
    }
    
    public OrderedList(params List<IElement> children) : this([], children) { }
    
    public void ApplyStyles(StyleCollection styles) => Style.OverrideWith(styles);

    public void Render(in IConsoleScreen target, Rectangle rect) {
        if (Display == DisplayType.NONE) return;
        
        var ls = ListStyle;
        string[] points = new string[Children.Count];
        int maxW = 0;
        for (int i = 0; i < Children.Count; i++) {
            points[i] = ls.ToString(i + 1);
            maxW = int.Max(maxW, points[i].Length);
        }
        
        var borderRect = IElement.GetBorderRect(rect, Margin);
        var contentRect = IElement.GetContentRect(rect, Margin, Padding, Border);
        
        target.DrawRect(borderRect, Background, Border);
        
        int offset = maxW + 1;
        int singleW = contentRect.Width - offset;   
        int lx = contentRect.LowerX + offset;
        int y = contentRect.LowerY;
        var s = PointStyle;
        
        for (int i = 0; i < Children.Count; i++) {
            int h = Children[i].GetHeight(singleW);
            
            var p = Children[i].Style["padding"].Value is SizeValue p1 ? p1 : new SizeValue();
            var b = Children[i].Style["border"].Value is BorderStyle b1 ? b1 : new BorderStyle();
            var m = Children[i].Style["margin"].Value is SizeValue m1 ? m1 : new SizeValue();
            
            int o = p.ToIntV(h) + (b.IsBorderless ? 0 : 1) + m.ToIntV(h);
            
            target.DrawText(points[i], contentRect.LowerX, y + o, s);
            Children[i].Render(target, new Rectangle(lx, y, contentRect.HigherX, y + h));
            y += h + 1;
        }
    }
    
    public int GetWidth(int maxHeight) {
        throw new NotImplementedException();
    }
    public int GetMinWidth(int maxHeight) {
        throw new NotImplementedException();
    }
    public int GetHeight(int maxWidth) {
        throw new NotImplementedException();
    }
    public int GetMinHeight(int maxWidth) {
        throw new NotImplementedException();
    }
}