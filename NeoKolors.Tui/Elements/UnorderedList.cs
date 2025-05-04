//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

[ElementName("ul")]
[ApplicableStyles(ApplicableStylesAttribute.Predefined.CONTAINER, "ul-style", "list-point-style")]
public class UnorderedList : IElement {
    
    public NKStyle PointStyle => Style["list-point-style"].Value is NKStyle color ? color : new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.NONE);
    public string ListStyle => Style["ul-style"].Value as string ?? new UListStyleProperty().Value; 
    public BorderStyle Border => Style["border"].Value is BorderStyle s ? s : new BorderStyle();
    public PaddingProperty Padding => Style["padding"].Value is PaddingProperty p ? p : new PaddingProperty();
    public MarginProperty Margin => Style["margin"].Value is MarginProperty m ? m : new MarginProperty();
    public DisplayType Display => Style["display"].Value is DisplayType d ? d : DisplayType.BLOCK;
    public NKColor Background => Style["background-color"].Value is NKColor color ? color : NKColor.Inherit;
    
    public string[] Selectors { get; }
    public StyleCollection Style { get; }
    public List<IElement> Children { get; }

    public UnorderedList(string[] selectors, params List<IElement> children) {
        Selectors = selectors;
        Children = children;
        Style = new StyleCollection();
    }
    
    public UnorderedList(params List<IElement> children) : this([], children) { }
    
    public void ApplyStyles(StyleCollection styles) {
        Style.OverrideWith(styles);
    }
    
    public void Render(in IConsoleScreen target, Rectangle rect) {
        if (Display == DisplayType.NONE) return;
        
        var borderRect = IElement.GetBorderRect(rect, Margin);
        var contentRect = IElement.GetContentRect(rect, Margin, Padding, Border);
        
        target.DrawRect(borderRect, Background, Border);
        
        string bullet = ListStyle;
        int offset = bullet.Length + 1;
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
            
            target.DrawText(bullet, contentRect.LowerX, y + o, s);
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