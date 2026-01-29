// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.Contracts;
using NeoKolors.Console.Mouse;
using NeoKolors.Tui.Elements.Caching;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Properties;
using static NeoKolors.Tui.Styles.Values.Direction;

namespace NeoKolors.Tui.Elements;

public class Div : ContainerElement, IInteractableElement {
    
    protected readonly LayoutCacher _layoutCacher;
    protected readonly ChildrenLayoutCacher _childrenCacher;
    
    private protected CacheUpdateFlags _updateCache = CacheUpdateFlags.NONE;
    
    // these tell if the cache has been updated for the new content
    protected bool CanUseMaxCache()    => _updateCache.GetHasMax();
    protected bool CanUseMinCache()    => _updateCache.GetHasMin();
    protected bool CanUseRenderCache() => _updateCache.GetHasRender();
    
    // these update the _updateCache
    protected void SetCanUseMaxCache()    => _updateCache |= CacheUpdateFlags.MAX;
    protected void SetCanUseMinCache()    => _updateCache |= CacheUpdateFlags.MIN;
    protected void SetCanUseRenderCache() => _updateCache |= CacheUpdateFlags.RENDER;
    
    protected List<IElement> _children;
    public override event Action? OnElementUpdated;
    
    protected void InvokeElementUpdated() {
        _updateCache = CacheUpdateFlags.NONE;
        OnElementUpdated?.Invoke();
    }
    
    public override ElementInfo Info { get; }
    
    private Rectangle _lastBounds = Rectangle.Zero;
    private bool _hoverEnable;

    private void InvokeMouseAction(MouseEventArgs a) {
        if (!_lastBounds.Contains(a.Position.X, a.Position.Y)) {
            if (!_hoverEnable) 
                return;
            
            OnHoverOut.Invoke();
            _hoverEnable = false;

            return;
        }
        
        if (a.IsPress) {
            OnClick.Invoke(a.Button);
        }
        else if (a.IsRelease) {
            OnRelease.Invoke(a.Button);
        }
        else if (a.IsHover) {
            OnHover.Invoke();
            _hoverEnable = true;
        }
    }
    
    public event Action<MouseButton> OnClick = _ => { };
    public event Action<MouseButton> OnRelease = _ => { };
    public event Action OnHover = () => { };
    public event Action OnHoverOut = () => { };

    private void SubscribeMouseEvents() {
        AppEventBus.SubscribeToMouseEvent(InvokeMouseAction);
    }
    
    public Div(params IElement[] children) {
        _children       = children.ToList();
        Info            = new ElementInfo();
        _style          = new StyleCollection();
        _layoutCacher   = new LayoutCacher(CanUseMinCache, CanUseMaxCache, CanUseRenderCache);
        _childrenCacher = new ChildrenLayoutCacher(CanUseMinCache, CanUseMaxCache, CanUseRenderCache);
        OnStyleAccess  += InvokeElementUpdated;

        foreach (var c in children) {
            c.OnElementUpdated += InvokeElementUpdated;
        }
        
        SubscribeMouseEvents();
    }
    
    public override void Render(ICharCanvas canvas, Rectangle rect) {
        if (!Visible) return;

        #if NK_ENABLE_CACHING
        
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
        
        #else

        var (el, cl) = GetRenderLayout(rect);

        #endif
        
        var pos = new Point(
            Position.AbsoluteX ? Position.X.ToScalar(rect.Width) : rect.LowerX + Position.X.ToScalar(rect.Width), 
            Position.AbsoluteY ? Position.Y.ToScalar(rect.Width) : rect.LowerY + Position.Y.ToScalar(rect.Height)
        );
        
        if (!BackgroundColor.IsInherit) {
            canvas.Fill(el.Border - Size.Two + pos + Point.One, ' ');
        }
        
        if (!Border.IsBorderless) {
            canvas.StyleBackground(el.Border - Size.Two + pos + Point.One, BackgroundColor);
            canvas.PlaceRectangle(el.Border + pos, Border);
        }
        else {
            canvas.StyleBackground(el.Border + pos, BackgroundColor);
        }
        
        var checkerBckg = Style.Get<CheckerBckgProperty>().Value;
        if (checkerBckg.Enabled) {
            canvas.StyleCheckerBckg(el.Border + pos, checkerBckg);
        }
        
        for (int i = 0; i < _children.Count; i++) {
            var child = cl.Children[i];
            
            if (child.Size != Size.Zero && child != Rectangle.Zero)
                _children[i].Render(canvas, child + pos);
        }
    }
    
    
    // --------------------------------- MIN LAYOUT COMP --------------------------------- //

    #region MIN LAYOUT COMP
    
    public override Size GetMinSize(Size parent) {

        #if NK_ENABLE_CACHING
        
        if (CanUseMinCache())
            return _layoutCacher.GetMinLayout().ElementSize;
        
        var (e, c) = ComputeMinLayout(parent);
        _layoutCacher.SetMin(parent, e);
        _childrenCacher.SetMin(parent, c);
        SetCanUseMinCache();
        
        return e.ElementSize;
    
        #else

        return ComputeMinLayout(parent).Element.ElementSize;

        #endif
    }

    [Pure]
    protected (ElementLayout Element, ChildrenLayout Children) ComputeMinLayout(Size parent) {
        return !Visible 
            ? (ElementLayout.Zero, ChildrenLayout.Empty) 
            : EnableGrid 
                ? ComputeMinGridLayout(parent) 
                : ComputeMinNonGridLayout(parent);
    }

    [Pure]
    private (ElementLayout, ChildrenLayout) ComputeMinGridLayout(Size parent) {
        int w = 0;
        int h = 0;
        int[] xs = new int[Grid.Columns.Length];
        int[] xo = new int[Grid.Columns.Length];
        int[] ys = new int[Grid.Rows.Length];
        int[] yo = new int[Grid.Rows.Length];

        for (int i = 0; i < Grid.Columns.Length; i++) {
            var c = Grid.Columns[i];
            
            if (c.IsAuto) continue;

            var scalar = c.ToScalar(parent.Width);
            xs[i] = scalar;
            w += scalar;
            xo[i] = w;
        }

        for (int i = 0; i < Grid.Rows.Length; i++) {
            var r = Grid.Rows[i];
            
            if (r.IsAuto) continue;

            var scalar = r.ToScalar(parent.Height);
            yo[i] = scalar;
            h += scalar;
            yo[i] = h;
        }
        
        var content = new Size(w, h);

        var el = IElement.ComputeLayout(content, parent, Margin, Padding, Border,
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);

        var cl = ComputeMinGridChildren(parent, xo, yo, xs, ys);
        
        return (el, cl);
    }

    private ChildrenLayout ComputeMinGridChildren(Size _, int[] xo, int[] yo, int[] xs, int[] ys) {
        var cl = new Rectangle[_children.Count];

        for (int i = 0; i < _children.Count; i++) {
            var c = _children[i];
            var a = c.Style.Get<GridAlignProperty>().Value;
            cl[i] = new Rectangle(xo[a.LowerX] - xs[a.LowerX], yo[a.LowerY] - ys[a.LowerY],
                    xo[a.HigherX], yo[a.HigherY]);
        }

        return new ChildrenLayout(cl);
    }

    [Pure]
    private (ElementLayout, ChildrenLayout) ComputeMinNonGridLayout(Size parent) {
        return Direction is TOP_TO_BOTTOM or BOTTOM_TO_TOP 
            ? ComputeMinVerticalLayout(parent) 
            : ComputeMinHorizontalLayout(parent);
    }

    [Pure]
    private (ElementLayout, ChildrenLayout) ComputeMinVerticalLayout(Size parent) {
        int height = 0;
        int maxWidth = 0;
        var r = new Rectangle[_children.Count];
        var y = 0;
        
        for (int i = 0; i < _children.Count; i++) {
            var l = _children[i].GetMinSize(parent);
            height += l.Height;
            maxWidth = Math.Max(maxWidth, l.Width);
            r[i] = l + new Point(0, y);
            y += l.Height;
        }
        
        var content = new Size(maxWidth, height);

        var el = IElement.ComputeLayout(content, parent, Margin, Padding, Border,
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);

        var cl = new ChildrenLayout(r);
        
        return (el, cl);
    }
    
    [Pure]
    private (ElementLayout, ChildrenLayout) ComputeMinHorizontalLayout(Size parent) {
        int width = 0;
        int maxHeight = parent.Height;
        var r = new Rectangle[_children.Count];
        int x = 0;

        for (int i = 0; i < _children.Count; i++) {
            var l = _children[i].GetMinSize(parent);
            width += l.Width;
            maxHeight = Math.Max(maxHeight, l.Height);
            r[i] = l + new Point(x, 0);
            x += l.Width;
        }
        
        var content = new Size(width, maxHeight);

        var el = IElement.ComputeLayout(content, parent, Margin, Padding, Border,
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        
        var cl = new ChildrenLayout(r);
        
        return (el, cl);
    }

    #endregion
    

    // --------------------------------- MAX LAYOUT COMP --------------------------------- //

    #region MAX LAYOUT COMP
    
    public override Size GetMaxSize(Size parent) {
        #if NK_ENABLE_CACHING
        
        if (CanUseMaxCache())
            return _layoutCacher.GetMaxLayout().ElementSize;
        
        var layout = ComputeMaxLayout(parent);
        _layoutCacher.SetMax(parent, layout);
        SetCanUseMaxCache();
        
        return layout.ElementSize;
    
        #else

        return ComputeMaxLayout(parent).ElementSize;

        #endif
    }
    
    [Pure]
    protected ElementLayout ComputeMaxLayout(Size parent) {
        return !Visible 
            ? ElementLayout.Zero
            : EnableGrid 
                ? ComputeMaxGridLayout(parent)
                : ComputeMaxNonGridLayout(parent);
    }

    [Pure]
    private ElementLayout ComputeMaxGridLayout(Size parent) {
        int w   = 0;
        int wac = 0;
        int h   = 0;
        int hac = 0;

        for (int i = 0; i < Grid.Columns.Length; i++) {
            var c = Grid.Columns[i];
            
            if (c.IsAuto) {
                wac++;
                continue;
            }
            
            w += c.ToScalar(parent.Width);
        }

        for (int i = 0; i < Grid.Rows.Length; i++) {
            var r = Grid.Rows[i];

            if (r.IsAuto) {
                hac++;
                continue;
            }
            
            h += r.ToScalar(parent.Height);
        }
        
        var winSize = new Size(Stdio.BufferWidth, Stdio.BufferHeight);
        
        var minContent = new Size(w, h);
        var maxContent = IElement.ComputeLayout(winSize, Margin, Padding, Border,
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);

        if (wac == 0 && hac == 0) {
            return IElement.ComputeLayout(minContent, parent, Margin, Padding, Border, 
                Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        }

        if (wac == 0 && hac != 0) {
            var content = new Size(minContent.Width, Math.Max(maxContent.Content.Height, minContent.Height));
            return IElement.ComputeLayout(content, parent, Margin, Padding, Border, 
                Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        }

        if (wac != 0 && hac == 0) {
            var content = new Size(Math.Max(maxContent.Content.Width, minContent.Width), minContent.Height);
            return IElement.ComputeLayout(content, parent, Margin, Padding, Border, 
                Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        }
        
        var cont = Size.Max(maxContent.Content, minContent);
        return IElement.ComputeLayout(cont, parent, Margin, Padding, Border, 
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
    }

    [Pure]
    private ElementLayout ComputeMaxNonGridLayout(Size parent) {
        return Direction is TOP_TO_BOTTOM or BOTTOM_TO_TOP 
            ? ComputeMaxVerticalLayout(parent) 
            : ComputeMaxHorizontalLayout(parent);
    }

    [Pure]
    private ElementLayout ComputeMaxVerticalLayout(Size parent) {
        var el = IElement.ComputeLayout(parent, Margin, Padding, Border, 
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        
        int height = 0;
        int maxWidth = 0;

        for (int i = 0; i < _children.Count; i++) {
            var l = _children[i].GetMaxSize(el.Content);
            height += l.Height;
            maxWidth = Math.Max(maxWidth, l.Width);
        }
        
        var content = new Size(maxWidth, height);
        
        return IElement.ComputeLayout(content, parent, Margin, Padding, Border, 
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
    }
    
    [Pure]
    private ElementLayout ComputeMaxHorizontalLayout(Size parent) {
        var el = IElement.ComputeLayout(parent, Margin, Padding, Border, 
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        
        int width = 0;
        int maxHeight = parent.Height;

        for (int i = 0; i < _children.Count; i++) {
            var l = _children[i].GetMaxSize(el.Content);
            width += l.Width;
            maxHeight = Math.Max(maxHeight, l.Height);
        }
        
        var content = new Size(width, maxHeight);
        
        return IElement.ComputeLayout(content, parent, Margin, Padding, Border, 
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
    }

    #endregion
    
    
    // -------------------------------- RENDER LAYOUT COMP ------------------------------- //

    #region RENDER LAYOUT COMP

    public override Size GetRenderSize(Size parent) => GetRenderLayout(parent).Element.ElementSize;

    protected (ElementLayout Element, ChildrenLayout Children) GetRenderLayout(Size parent) {
        #if NK_ENABLE_CACHING
        
        if (CanUseRenderCache()) 
            return (_layoutCacher.GetRenderLayout(), _childrenCacher.GetRenderLayout());
        
        var layout = ComputeRenderLayout(parent);
        _layoutCacher.SetRender(parent, layout.Element);
        _childrenCacher.SetRender(parent, layout.Children);
        SetCanUseRenderCache();

        return layout;

        #else

        return ComputeRenderLayout(parent);
        
        #endif
    }
    
    [Pure]
    private (ElementLayout Element, ChildrenLayout Children) ComputeRenderLayout(Size parent) {
        return !Visible 
            ? (ElementLayout.Zero, ChildrenLayout.Empty)
            : EnableGrid
                ? ComputeRenderGridLayout(parent)
                : ComputeRenderNonGridLayout(parent);
    }

    [Pure]
    private (ElementLayout Element, ChildrenLayout Chlidren) ComputeRenderGridLayout(Size parent) {
        var el = ComputeRenderGridElementLayout(parent);
        var cl = ComputeRenderGridChildrenLayout(parent, el.Content);
        
        return (el, cl);
    }

    [Pure]
    private ElementLayout ComputeRenderGridElementLayout(Size parent) {
        int w   = 0;
        int wac = 0;
        int h   = 0;
        int hac = 0;

        for (int i = 0; i < Grid.Columns.Length; i++) {
            var c = Grid.Columns[i];
            
            if (c.IsAuto) {
                wac++;
                continue;
            }
            
            w += c.ToScalar(parent.Width);
        }

        for (int i = 0; i < Grid.Rows.Length; i++) {
            var r = Grid.Rows[i];

            if (r.IsAuto) {
                hac++;
                continue;
            }
            
            h += r.ToScalar(parent.Height);
        }
        
        var minContent = new Size(w, h);
        var maxContent = IElement.ComputeLayout(
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);

        if (wac == 0 && hac == 0) {
            return IElement.ComputeLayout(minContent, parent, Margin, Padding, Border, 
                Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        }

        if (wac == 0 && hac != 0) {
            var content = new Size(minContent.Width, Math.Max(maxContent.Content.Height, minContent.Height));
            return IElement.ComputeLayout(content, parent, Margin, Padding, Border, 
                Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        }

        if (wac != 0 && hac == 0) {
            var content = new Size(Math.Max(maxContent.Content.Width, minContent.Width), minContent.Height);
            return IElement.ComputeLayout(content, parent, Margin, Padding, Border, 
                Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        }
        
        var cont = Size.Max(maxContent.Content, minContent);
        return IElement.ComputeLayout(cont, parent, Margin, Padding, Border, 
            Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
    }

    [Pure]
    private ChildrenLayout ComputeRenderGridChildrenLayout(Size parent, Size content) {
        int xAutoCount = Grid.Columns.Count(c => c.IsAuto);
        int yAutoCount = Grid.Rows   .Count(c => c.IsAuto);

        var cc = Grid.Columns.Length;
        var rc = Grid.Rows.Length;

        int[] xSteps = new int[cc];
        int[] ySteps = new int[rc];
        int   width  = 0;
        int   height = 0;
        
        for (int x = 0; x < cc; x++) {
            var c = Grid.Columns[x];
            
            if (c.IsAuto) {
                xSteps[x] = -1;
                continue;
            }

            var scalar = c.ToScalar(parent.Width);
            xSteps[x] = scalar;
            width += scalar;
        }

        for (int y = 0; y < rc; y++) {
            var r = Grid.Rows[y];
            
            if (r.IsAuto) {
                ySteps[y] = -1;
                continue;
            }

            var scalar = r.ToScalar(parent.Height);
            ySteps[y] = scalar;
            height += scalar;
        }
        
        int xAutoSize = Math.Max(0, content.Width  - width)  / xAutoCount;
        int yAutoSize = Math.Max(0, content.Height - height) / yAutoCount;

        int[] xOffsets = new int[cc];
        int[] yOffsets = new int[rc];

        int cx = 0;
        
        for (int x = 0; x < cc; x++) {
            int nx;
            
            if (xSteps[x] == -1) {
                nx        = xAutoSize;
                xSteps[x] = xAutoSize;
            }
            else {
                nx = xSteps[x];    
            }

            cx += nx;
            xOffsets[x] = cx;
        }

        int cy = 0;

        for (int y = 0; y < rc; y++) {
            int ny;
            
            if (ySteps[y] == -1) {
                ny        = yAutoSize;
                ySteps[y] = yAutoSize;
            }
            else {
                ny = ySteps[y];    
            }

            cy += ny;
            yOffsets[y] = cy;
        }
        
        // compute the layout
        
        var childrenCount = _children.Count;
        var childrenLayout = new Rectangle[childrenCount];

        for (int i = 0; i < childrenCount; i++) {
            var child = _children[i];
            var a = child.Style.Get<GridAlignProperty>().Value;
            
            childrenLayout[i] = new Rectangle(
                xOffsets[a.LowerX]  - xSteps[a.LowerX], 
                yOffsets[a.LowerY]  - ySteps[a.LowerY],
                xOffsets[a.HigherX] - 1,
                yOffsets[a.HigherY] - 1);
        }
        
        return new ChildrenLayout(childrenLayout);
    }

    [Pure]
    private (ElementLayout Element, ChildrenLayout Chlidren) ComputeRenderNonGridLayout(Size parent) {
        return Direction is TOP_TO_BOTTOM or BOTTOM_TO_TOP
            ? ComputeRenderVerticalLayout(parent)
            : ComputeRenderHorizontalLayout(parent);
    }

    [Pure]
    private (ElementLayout, ChildrenLayout) ComputeRenderVerticalLayout(Size parent) {
        var el = IElement.ComputeLayout(
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);

        var content = el.Content;
        var cl = new Rectangle[_children.Count];
        int y = 0;
        int maxX = 0;

        for (int i = 0; i < _children.Count; i++) {
            var c =  _children[i];
            var l = c.GetRenderSize(content);
            cl[i] = new Rectangle(new Point(content.LowerX, y + content.LowerY), l);
            y += l.Height;
            maxX = Math.Max(maxX, l.Width);
        }
        
        content = Size.Min(content, new Size(maxX, y));
        el = IElement.ComputeLayout(content, parent, 
            Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        
        return (el, new ChildrenLayout(cl));
    }

    [Pure]
    private (ElementLayout, ChildrenLayout) ComputeRenderHorizontalLayout(Size parent) {
        var el = IElement.ComputeLayout(
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        
        var content = el.Content;
        var cc = _children.Count;
        
        var maxR = new Rectangle[cc];
        int maxXs = 0;
        int maxMaxY = 0;

        for (int i = 0; i < cc; i++) {
            var c =  _children[i];
            var l = c.GetMaxSize(content);
            maxR[i] = new Rectangle(new Point(content.LowerX + maxXs, content.LowerY), l);
            maxXs += l.Width;
            maxMaxY = Math.Max(maxMaxY, l.Height);
        }

        // elements fit with max width
        if (maxXs <= content.Width) {
            var cs = new Size(maxXs, maxMaxY);
            var fel = IElement.ComputeLayout(cs, parent,
                Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);

            return (fel, new ChildrenLayout(maxR));
        }
        
        var minR = new Rectangle[cc];
        int minXs = 0;
        int minMaxY = 0;
         
        for (int i = 0; i < cc; i++) {
            var c =  _children[i];
            var l = c.GetMinSize(content);
            minR[i] = new Rectangle(new Point(content.LowerX + minXs, content.LowerY), l);
            minXs += l.Width;
            minMaxY = Math.Max(minMaxY, l.Height); 
        }

        // overflow
        if (minXs >= content.Width) {
            var cs = new Size(content.Width, minMaxY);
            var fel = IElement.ComputeLayout(cs, parent,
                Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);

            return (fel, new ChildrenLayout(minR));
        }
        
        var rw = new int[cc];
        int msw = 0;
        int rsw = 0;
        bool up = false;
        
        for (int i = 0; i < cc; i++) {
            var w  = (float)maxR[i].Width / maxXs * content.Width;
            int iw = (int)(up ? Math.Ceiling(w) : Math.Floor(w));
            
            if (w < minR[i].Width) {
                var o = minR[i].Width; 
                rw[i] = -o;
                msw += o;
            }
            else {
                rw[i] = iw;
                rsw += iw;
                up = !up;
            }
        }
        
        up = false;
        
        for (int i = 0; i < cc; i++) {
            var f = (float)rw[i] / rsw * (content.Width - msw);
            
            if (rw[i] > 0) {
                rw[i] = (int)(up ? Math.Ceiling(f) : Math.Floor(f));
                up = !up;
            }
            else {
                rw[i] = -rw[i];
            }
        }
        
        var rr = new Rectangle[cc];
        var rx = 0;
        var rMaxY = 0;

        for (int i = 0; i < cc; i++) {
            var c = _children[i];
            var l = c.GetRenderSize(new Size(rw[i], content.Height));
            rr[i] = new Rectangle(new Point(content.LowerX + rx, content.LowerY), l);
            rx += l.Width;
            rMaxY = Math.Max(rMaxY, l.Height); 
        }
        
        var rc = new Size(content.Width, rMaxY);
        el = IElement.ComputeLayout(
            rc, parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);

        return (el, new ChildrenLayout(rr));
    }

    #endregion
    
    
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