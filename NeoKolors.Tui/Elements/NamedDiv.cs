// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Elements.Caching;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

public class NamedDiv : Div {

    public string Name {
        get;
        set {
            field = value;
            InvokeElementUpdated();
        }
    }

    
    // ----------------------------- STYLES ----------------------------- //

    public virtual Dimension TitlePadding {
        get => _style.Get(new DivTitlePaddingProperty(DefaultTitlePadding)).Value;
        set => _style.Set(new DivTitlePaddingProperty(value));
    }

    public virtual Dimension DefaultTitlePadding => Dimension.Chars(1);

    public virtual HorizontalAlign TitleAlign {
        get => _style.Get(new DivTitleAlignProperty(DefaultTitleAlign)).Value;
        set => _style.Set(new DivTitleAlignProperty(value));
    }

    public virtual HorizontalAlign DefaultTitleAlign => HorizontalAlign.LEFT;
    
    
    // -------------------------- CONSTRUCTOR -------------------------- // 

    public NamedDiv(string name, params IElement[] children) : base(children) {
        Name = name;
    }
    
    
    // --------------------------- RENDERING --------------------------- // 
    
    public override void Render(ICharCanvas canvas, Rectangle rect) {
        base.Render(canvas, rect);

        ElementLayout el;
        
        if (CanUseRenderCache()) {
            el = _layoutCacher.GetRenderLayout();
        }
        else if (CanUseMinCache() && _updateCache.GetHasRender()) {
            el = _layoutCacher.GetMinLayout();
        }
        else if (CanUseMaxCache() && _updateCache.GetHasRender()) {
            el = _layoutCacher.GetMaxLayout();
        }
        else {
            (el, var cl) = GetRenderLayout(rect.Size);
            _layoutCacher.SetRender(rect.Size, el);
            _childrenCacher.SetRender(rect.Size, cl);
            SetCanUseRenderCache();
        }

        var pos = new Point(
            Position.AbsoluteX ? Position.X.ToScalar(rect.Width) : rect.LowerX + Position.X.ToScalar(rect.Width), 
            Position.AbsoluteY ? Position.Y.ToScalar(rect.Width) : rect.LowerY + Position.Y.ToScalar(rect.Height)
        );
        
        var bw = el.Border.Width;
        int p = TitlePadding.ToScalar(el.Border.Width);
        
        canvas.PlaceString(Name, pos + el.Border.Lower + new Point(p, 0), bw - p * 2, TitleAlign);
    }
}