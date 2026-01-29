// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.Contracts;
using NeoKolors.Console.Mouse;
using NeoKolors.Tui.Elements.Caching;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Rendering;
using StyleCollection = NeoKolors.Tui.Styles.StyleCollection;

namespace NeoKolors.Tui.Elements;

public class Text : TextElement, INotifyOnRender, IInteractableElement {
    
    private readonly LayoutCacher _layoutCacher;
    private string _text;
    
    /// <summary>
    /// Tracks whether the paragraph content has been modified,
    /// used to determine if layout recalculations are necessary.
    /// The least significant bit is for compute cache, next for
    /// min cache and last for render cache.
    /// </summary>
    private CacheUpdateFlags _updateCache = CacheUpdateFlags.NONE;
    
    // these tell if the cache has been updated for the new content
    private bool CanUseMaxCache()    => _updateCache.GetHasMax();
    private bool CanUseMinCache()    => _updateCache.GetHasMin();
    private bool CanUseRenderCache() => _updateCache.GetHasRender();
    
    // these update the _updateCache
    private void SetCanUseMaxCache()    => _updateCache |= CacheUpdateFlags.MAX;
    private void SetCanUseMinCache()    => _updateCache |= CacheUpdateFlags.MIN;
    private void SetCanUseRenderCache() => _updateCache |= CacheUpdateFlags.RENDER;

    public event OnRenderEventHandler OnRender;
    public override event Action? OnElementUpdated;
    
    private void InvokeElementUpdated() {
        _updateCache = CacheUpdateFlags.NONE;
        OnElementUpdated?.Invoke();
    }

    public string Content {
        get => _text;
        set {
            if (value == _text) return;
            
            _text = value;
            
            InvokeElementUpdated();
        }
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
    
    public Text(string text) {
        _text          = text;
        _layoutCacher  = new LayoutCacher(CanUseMinCache, CanUseMaxCache, CanUseRenderCache);
        _style         = new StyleCollection();
        Info           = new ElementInfo();
        OnRender      += () => {};
        OnStyleAccess += InvokeElementUpdated;
        SubscribeMouseEvents();
    }
    
    public override void Render(ICharCanvas canvas, Rectangle rect) {

        var onRender = Task.Run(OnRender.Invoke);

        #if NK_ENABLE_CACHING

        ElementLayout layout;
        
        if (_layoutCacher.IsRenderValid(rect.Size)) {
            layout = _layoutCacher.GetRenderLayout();
        }
        else if (_layoutCacher.IsMinValid(rect.Size) && _updateCache.GetHasRender()) {
            layout = _layoutCacher.GetMinLayout();
        }
        else if (_layoutCacher.IsMaxValid(rect.Size) && _updateCache.GetHasRender()) {
            layout = _layoutCacher.GetMaxLayout();
        }
        else {
            layout = ComputeRenderLayout(rect.Size);
            _layoutCacher.SetRender(rect.Size, layout);
            SetCanUseRenderCache();
        }
        
        #else
        
        var layout = ComputeRenderLayout(rect.Size);
        
        #endif
        
        var pos = new Point(
            Position.AbsoluteX ? Position.X.ToScalar(rect.Width) : rect.LowerX + Position.X.ToScalar(rect.Width), 
            Position.AbsoluteY ? Position.Y.ToScalar(rect.Width) : rect.LowerY + Position.Y.ToScalar(rect.Height)
        );

        _lastBounds = layout.Border + pos;
        
        if (!BackgroundColor.IsInherit) {
            canvas.Fill(layout.Border - Size.Two + pos + Point.One, ' ');
        }
        
        if (!Border.IsBorderless) {
            canvas.StyleBackground(layout.Border - Size.Two + pos + Point.One, BackgroundColor);
            canvas.PlaceRectangle(layout.Border + pos, Border);
        }
        else {
            canvas.StyleBackground(layout.Border + pos, BackgroundColor);
        }
        
        Font.PlaceString(_text, canvas, layout.Content + pos, new NKStyle(Color, BackgroundColor),
            TextAlign.Horizontal, TextAlign.Vertical);
        
        onRender.Wait();
    }
    
    
    // --------------------------------- LAYOUT COMP --------------------------------- //

    #region LAYOUT COMP
    
    public override Size GetMinSize(Size parent) {
        
        #if NK_ENABLE_CACHING

        if (_layoutCacher.IsMinValid(parent)) 
            return _layoutCacher.GetMinLayout().ElementSize;

        var layout = ComputeMinLayout(parent);
        _layoutCacher.SetMin(parent, layout);
        SetCanUseMinCache();
        
        return layout.ElementSize;
        
        #else

        return ComputeMinLayout(parent).ElementSize;

        #endif
    }

    [Pure]
    protected ElementLayout ComputeMinLayout(Size parent) {
        var t = Font.GetMinSize(_text);
        return IElement.ComputeLayout(
            t, parent, Margin, Padding, Border);
    }
    
    public override Size GetMaxSize(Size parent) {
        
        #if NK_ENABLE_CACHING
        
        if (_layoutCacher.IsMaxValid(parent))
            return _layoutCacher.GetMaxLayout().ElementSize;

        var layout = ComputeMaxLayout(parent);
        _layoutCacher.SetMin(parent, layout);
        SetCanUseMaxCache();
        
        return layout.ElementSize;

        #else
        
        return ComputeMaxLayout(parent).ElementSize;
        
        #endif
    }
    
    [Pure]
    protected ElementLayout ComputeMaxLayout(Size parent) {
        var compute = Font.GetSize(_text);
        return IElement.ComputeLayout(
            compute, parent, Margin, Padding, Border);
    }

    public override Size GetRenderSize(Size parent) {
        
        #if NK_ENABLE_CACHING
        
        if (_layoutCacher.IsRenderValid(parent)) 
            return _layoutCacher.GetRenderLayout().ElementSize;
        
        var layout = ComputeRenderLayout(parent);
        _layoutCacher.SetRender(parent, layout);
        SetCanUseRenderCache();
        
        return layout.ElementSize;
    
        #else
        
        return ComputeRenderLayout(parent).ElementSize;
        
        #endif
    }
    
    [Pure]
    protected ElementLayout ComputeRenderLayout(Size bounds) {
        var p = Padding.Left.ToScalar(bounds.Width) + Padding.Right.ToScalar(bounds.Width);
        var m = Margin .Left.ToScalar(bounds.Width) + Margin .Right.ToScalar(bounds.Width);
        var b = Border.IsBorderless ? 0 : 2;
        
        var content = Font.GetSize(_text, bounds.Width - p - m - b);
        return IElement.ComputeLayout(
            content, bounds, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
    }
    
    #endregion

    
    // -------------------------------- INODE IMPL -------------------------------- //
    
    #region INode impl
    
    public override string GetChildNode() => _text;

    public override void SetChildNode(string childNode) {
        if (childNode == _text) return;
        _text = childNode;
        InvokeElementUpdated();
    }

    #endregion
}