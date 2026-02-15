// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Elements.Caching;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

public class List : AbstractElement<IElement[]> {
    
    
    private readonly LayoutCacher _layoutCacher;
    private readonly ChildrenLayoutCacher _childrenCacher;
    
    private CacheUpdateFlags _updateCache = CacheUpdateFlags.NONE;
    
    // these tell if the cache has been updated for the new content
    private bool CanUseMaxCache()    => _updateCache.GetHasMax();
    private bool CanUseMinCache()    => _updateCache.GetHasMin();
    private bool CanUseRenderCache() => _updateCache.GetHasRender();
    
    // these update the _updateCache
    private void SetCanUseMaxCache()    => _updateCache |= CacheUpdateFlags.MAX;
    private void SetCanUseMinCache()    => _updateCache |= CacheUpdateFlags.MIN;
    private void SetCanUseRenderCache() => _updateCache |= CacheUpdateFlags.RENDER;
    
    private List<IElement> _children;
    public override event Action? OnElementUpdated;
    
    private void InvokeElementUpdated() {
        _updateCache = CacheUpdateFlags.NONE;
        OnElementUpdated?.Invoke();
    }

    public override ElementInfo Info { get; }
    
    // ------------------------------------ STYLE -------------------------------------- //

    public static StyleCollection DefaultStyle { get; } = new(AbstractContainerElement.DefaultStyle) {
        ListPoint       = _ => "*",
        ListPointStyle  = NKStyle.Default,
        ListPointGap    = Dimension.Chars(1),
        ListPointAlign  = HorizontalAlign.RIGHT,
        ListPointOffset = 0,
    };
    
    
    // ------------------------------------ CONSTRUCTOR ---------------------------------- //
    
    public List(params IElement[] children) : base(DefaultStyle) {
        _children       = new List<IElement>(children);
        _style          = new StyleCollection();
        Info            = new ElementInfo();
        _layoutCacher   = new LayoutCacher        (CanUseMinCache, CanUseMaxCache, CanUseRenderCache);
        _childrenCacher = new ChildrenLayoutCacher(CanUseMinCache, CanUseMaxCache, CanUseRenderCache);
    }
    
    // ------------------------------------ RENDER ------------------------------------- //
    
    public override void Render(ICharCanvas canvas, Rectangle rect) {
        if (!_style.Visible) return;

        ElementLayout  el;
        ChildrenLayout cl;

        if (CanUseRenderCache()) {
            el = _layoutCacher.GetRenderLayout();
            cl = _childrenCacher.GetRenderLayout();
        }
        else if (CanUseMinCache() && _updateCache.GetHasRender()) {
            el = _layoutCacher.GetMinLayout();
            cl = _childrenCacher.GetMinLayout();
        }
        else if (CanUseMaxCache() && _updateCache.GetHasRender()) {
            el = _layoutCacher.GetMaxLayout();
            cl = _childrenCacher.GetMaxLayout();
        }
        else {
            (el, cl) = GetRenderLayout(rect);
            _layoutCacher.SetRender(rect, el);
            _childrenCacher.SetRender(rect, cl);
            SetCanUseRenderCache();
        }
        
        if (!_style.Border.IsBorderless) {
            canvas.StyleBackground(el.Border + rect.Lower, _style.BackgroundColor);
            canvas.PlaceRectangle(el.Border + rect.Lower, _style.Border);
        }
        else {
            canvas.StyleBackground(el.Border + rect.Lower, _style.BackgroundColor);
        }

        var sp = _style.Position;
        var pos = new Point(
            sp.AbsoluteX ? sp.X.ToScalar(rect.Width) : rect.LowerX + sp.X.ToScalar(rect.Width), 
            sp.AbsoluteY ? sp.Y.ToScalar(rect.Width) : rect.LowerY + sp.Y.ToScalar(rect.Height)
        );
        
        var pg = _style.ListPoint;
        int mpw = 0;

        for (int i = 1; i < _children.Count + 1; i++) {
            var p = pg(i);
            mpw = Math.Max(mpw, p.Length);
        }
        
        var co = new Point(mpw + _style.ListPointGap.ToScalar(rect.Width), 0);
        
        for (int i = 0; i < _children.Count; i++) {
            var child = cl.Children[i];

            if (child.Size == Size.Zero || child == Rectangle.Zero) continue;
            
            var p = pg(i + 1);
            canvas.PlaceString(
                p, 
                new Point(
                    child.LowerX + pos.X + el.Content.LowerX, 
                    child.LowerY + pos.Y + el.Content.LowerY + _style.ListPointOffset
                ), 
                mpw, 
                _style.ListPointAlign
            );
                
            _children[i].Render(canvas, child + pos + co + el.Content.Lower);
        }
    }
    
    
    // --------------------------------- MIN LAYOUT COMP --------------------------------- //
    
    public override Size GetMinSize(Size parent) {
        if (CanUseMinCache())
            return _layoutCacher.GetMinLayout().ElementSize;
        
        var (e, c) = ComputeMinLayout(parent);
        _layoutCacher.SetMin(parent, e);
        _childrenCacher.SetMin(parent, c);
        SetCanUseMinCache();
        
        return e.ElementSize;
    }

    private (ElementLayout, ChildrenLayout) ComputeMinLayout(Size parent) {
        if (!_style.Visible)
            return (ElementLayout.Zero, ChildrenLayout.Empty);

        int maxPointWidth = 0;
        var pg = _style.ListPoint;
        
        for (int i = 1; i < _children.Count + 1; i++) {
            var p = pg(i);
            maxPointWidth = Math.Max(maxPointWidth, p.Length);
        }

        int y = 0;
        var r = new Rectangle[_children.Count];
        int maxW = 0;

        for (int i = 0; i < _children.Count; i++) { 
            var l = _children[i].GetMinSize(parent);
            y += l.Height;
            r[i] = l + new Point(0, y);
            maxW = Math.Max(l.Width, maxW);
        }
        
        var content = new Size(maxPointWidth + maxW + _style.ListPointGap.ToScalar(parent.Width), y);
        var el = IElement.ComputeLayoutFromContent(
            content, 
            parent,
            _style.Margin,    _style.Border, _style.Padding, 
            _style.Width,     _style.Height, 
            _style.MinWidth,  _style.MaxWidth,
            _style.MinHeight, _style.MaxHeight
        );

        return (el, new ChildrenLayout(r));
    }
    
    
    // --------------------------------- MAX LAYOUT COMP --------------------------------- //
    
    public override Size GetMaxSize(Size parent) {
        if (CanUseMaxCache())
            return _layoutCacher.GetMaxLayout().ElementSize;
        
        var (e, c) = ComputeMaxLayout(parent);
        _layoutCacher.SetMax(parent, e);
        _childrenCacher.SetMin(parent, c);
        SetCanUseMaxCache();
        
        return e.ElementSize;
    }

    private (ElementLayout, ChildrenLayout) ComputeMaxLayout(Size parent) {
        if (!_style.Visible)
            return (ElementLayout.Zero, ChildrenLayout.Empty);

        int maxPointWidth = 0;
        var pg = _style.ListPoint;
        
        for (int i = 1; i < _children.Count + 1; i++) {
            var p = pg(i);
            maxPointWidth = Math.Max(maxPointWidth, p.Length);
        }

        int y = 0;
        var r = new Rectangle[_children.Count];
        int maxW = 0;

        for (int i = 0; i < _children.Count; i++) { 
            var l = _children[i].GetMaxSize(parent);
            y += l.Height;
            r[i] = l + new Point(0, y);
            maxW = Math.Max(l.Width, maxW);
        }
        
        var content = new Size(maxPointWidth + maxW + _style.ListPointGap.ToScalar(parent.Width), y);
        var el = IElement.ComputeLayoutFromContent(
            content, 
            parent,
            _style.Margin,    _style.Border, _style.Padding, 
            _style.Width,     _style.Height, 
            _style.MinWidth,  _style.MaxWidth,
            _style.MinHeight, _style.MaxHeight
        );

        return (el, new ChildrenLayout(r));
    }
    
    
    // -------------------------------- RENDER LAYOUT COMP ------------------------------- //
    
    public override Size GetRenderSize(Size parent) {
        if (CanUseRenderCache())
            return _layoutCacher.GetRenderLayout().ElementSize;
        
        var (e, c) = GetRenderLayout(parent);
        _layoutCacher.SetRender(parent, e);
        _childrenCacher.SetRender(parent, c);
        SetCanUseRenderCache();

        return e.ElementSize;
    }

    private (ElementLayout Element, ChildrenLayout Children) GetRenderLayout(Size rect) {
        if (!_style.Visible)
            return (ElementLayout.Zero, ChildrenLayout.Empty);

        var c = IElement.ComputeLayoutFromBounds(
            rect, _style.Margin, _style.Border, _style.Padding, 
            _style.Width, _style.Height, _style.MinWidth, _style.MaxWidth, _style.MinHeight, _style.MaxHeight
        );
        
        int mpw = 0;
        var pg = _style.ListPoint;
        
        for (int i = 1; i < _children.Count + 1; i++) {
            var p = pg(i);
            mpw = Math.Max(mpw, p.Length);
        }

        int y = 0;
        var r = new Rectangle[_children.Count];
        int maxW = 0;
        int cw = c.Content.Width - mpw - _style.ListPointGap.ToScalar(rect.Width);

        for (int i = 0; i < _children.Count; i++) { 
            var l = _children[i].GetRenderSize(new Size(cw, c.Content.Height - y));
            r[i] = l + new Point(0, y);
            y += l.Height;
            maxW = Math.Max(l.Width, maxW);
        }
        
        var content = new Size(mpw + maxW + _style.ListPointGap.ToScalar(rect.Width), y);
        var el = IElement.ComputeLayoutFromContent(
            content, rect,
            _style.Margin,    _style.Border, _style.Padding, 
            _style.Width,     _style.Height, 
            _style.MinWidth,  _style.MaxWidth,
            _style.MinHeight, _style.MaxHeight
        );

        return (el, new ChildrenLayout(r));
    }
    
    
    // ------------------------------------ CHILDREN ------------------------------------- //
    
    public override IElement[] GetChildNode() => _children.ToArray();

    public void AddChild(IElement[] child) {
        foreach (var c in child.Except(_children).Distinct()) {
            c.OnElementUpdated += InvokeElementUpdated;
        }

        _children.AddRange(child);
        
        InvokeElementUpdated();
    }
    
    public override void SetChildNode(IElement[] childNode) {

        // unsubscribe from old children
        foreach (var c in _children) {
            c.OnElementUpdated -= InvokeElementUpdated;
        }

        // subscribe to new children
        foreach (var c in childNode) {
            c.OnElementUpdated += InvokeElementUpdated;
        }
        
        _children = childNode.ToList();
        
        // invoke update
        InvokeElementUpdated();
    }
}