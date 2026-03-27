// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics.Contracts;
using NeoKolors.Console.Input;
using NeoKolors.Tui.Elements.Caching;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Properties;
using StyleCollection = NeoKolors.Tui.Styles.StyleCollection;

namespace NeoKolors.Tui.Elements;

public class Text : AbstractTextElement, INotifyOnRender, IMouseInteractableElement<AnsiString> {

    #region Content
    
    private AnsiString _text;
    
    public virtual AnsiString Content {
        get => _text.Clone();
        set {
            if (value.First().Style != NKStyle.Default) {
                _text = value;
            }
            else {
                var v = value.Style(new NKStyle(_style.TextColor, _style.BackgroundColor, _style.TextStyle));

                if (v == _text) 
                    return;

                _text = v;
            }

            InvokeElementUpdated();
        }
    }
    
    public override ElementInfo Info { get; }

    #endregion

    #region Cache
    
    private readonly LayoutCacher _layoutCacher;
    
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

    #endregion

    #region Events
    
    protected event Action _onElementUpdated = delegate { };
    
    public event OnRenderEventHandler OnRender;
    public override event Action? OnElementUpdated {
        add => _onElementUpdated += value;
        remove => _onElementUpdated += value;
    }

    protected void InvokeElementUpdated() {
        _updateCache = CacheUpdateFlags.NONE;
        _onElementUpdated.Invoke();
    }
    
    private Rectangle _lastBounds = Rectangle.Zero;
    private bool _hoverEnable;

    private void InvokeMouseAction(MouseEventArgs a) {
        if (!_lastBounds.Contains(a.Position.X, a.Position.Y)) {
            if (!_hoverEnable) 
                return;
            
            _onHoverOut.Invoke();
            _hoverEnable = false;

            return;
        }
        
        if (a.IsPress) {
            _onClick.Invoke(a.Button);
        }
        else if (a.IsRelease) {
            _onRelease.Invoke(a.Button);
        }
        else if (a.IsHover) {
            _onHover.Invoke();
            _hoverEnable = true;
        }
    }
    
    private event Action<MouseButton> _onClick = delegate { }; 
    private event Action<MouseButton> _onRelease = delegate { }; 
    private event Action _onHover = delegate { }; 
    private event Action _onHoverOut = delegate { }; 
    private int _onClickCount = 0;
    private int _onReleaseCount = 0;
    private int _onHoverCount = 0;
    private int _onHoverOutCount = 0;
    private bool Unsubscribed => 
        _onClickCount == 0 && _onReleaseCount == 0 && _onHoverCount == 0 && _onHoverOutCount == 0;
    
    public event Action<MouseButton> OnClick {
        add {
            TrySubMouse();
            
            _onClick += value;
            _onClickCount++;
        }
        remove {
            _onClick -= value;
            _onClickCount--;
            
            TryUnsubMouse();
        }
    }

    public event Action<MouseButton> OnRelease {
        add {
            TrySubMouse();
            
            _onRelease += value;
            _onReleaseCount++;
        }
        remove {
            _onRelease -= value;
            _onReleaseCount--;
            
            TryUnsubMouse();
        }
    }
    
    public event Action OnHover {
        add {
            TrySubMouse();
            
            _onHover += value;
            _onHoverCount++;
        }
        remove {
            _onHover -= value;
            _onHoverCount--;
            
            TryUnsubMouse();
        }
    }
    
    public event Action OnHoverOut {
        add {
            TrySubMouse();
            
            _onHoverOut += value;
            _onHoverCount++;
        }
        remove {
            _onHoverOut -= value;
            _onHoverOutCount--;
            
            TryUnsubMouse();
        }
    }

    private void TrySubMouse() {
        if (Unsubscribed) 
            AppEventBus.SubscribeToMouseEvent(InvokeMouseAction);
    }

    private void TryUnsubMouse() {
        if (Unsubscribed)
            AppEventBus.UnsubscribeFromMouseEvent(InvokeMouseAction);
    }

    private void OnStyleChanged(IStyleProperty changedProp) {
        switch (changedProp) {
            case TextColorProperty tc:
                _text = _text.Restyle(new NKStyle(tc.Value, _style.BackgroundColor, _style.TextStyle));
                break;
            case BackgroundColorProperty bc:
                _text = _text.Restyle(new NKStyle(_style.TextColor, bc.Value, _style.TextStyle));
                break;
            case TextStyleProperty ts:
                _text = _text.Restyle(new NKStyle(_style.TextColor, _style.BackgroundColor, ts.Value));
                break;
        }
        
        InvokeElementUpdated();
    }
    
    #endregion

    protected new static StyleCollection DefaultStyles { get; } = new(AbstractTextElement.DefaultStyles) {
        ReadOnly = true
    };
    
    public Text(string text) : this(text, DefaultStyles) { }
    
    protected Text(string text, StyleCollection defaultStyles) : base(defaultStyles) {
        _text          = text;
        _layoutCacher  = new LayoutCacher(CanUseMinCache, CanUseMaxCache, CanUseRenderCache);
        Info           = new ElementInfo();
        OnRender            += () => {};
        _style.StyleChanged += OnStyleChanged;
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

        var sp = _style.Position;
        var pos = new Point(
            sp.AbsoluteX ? sp.X.ToScalarX(rect.Width)  : rect.LowerX + sp.X.ToScalarX(rect.Width), 
            sp.AbsoluteY ? sp.Y.ToScalarY(rect.Height) : rect.LowerY + sp.Y.ToScalarY(rect.Height)
        );

        _lastBounds = layout.Border + pos;
        
        if (!_style.BackgroundColor.IsInherit) {
            var clearRegion = _style.Border.IsBorderless 
                ? layout.Border + pos 
                : layout.Border - Size.Two + pos + Point.One;
            
            canvas.Fill(clearRegion, ' ');
            canvas.Style(clearRegion, new NKStyle(_style.TextColor, _style.BackgroundColor, _style.TextStyle));
        }
        
        if (!_style.Border.IsBorderless) {
            canvas.PlaceRectangle(layout.Border + pos, _style.Border);
        }
        
        _style.Font.PlaceString(Content, canvas, layout.Content + pos, 
            _style.TextAlign.Horizontal, _style.TextAlign.Vertical, _style.Overflow);
        
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

        return ComputeMinLayout(parent).Margin;

        #endif
    }

    [Pure]
    protected ElementLayout ComputeMinLayout(Size bounds) {
        var compute = _style.Font.GetMinSize(Content);
        return IElement.ComputeLayoutFromContent(
            compute,
            bounds,
            _style.Margin,
            _style.Border,
            _style.Padding,
            _style.Width,
            _style.Height,
            _style.MinWidth,
            _style.MaxWidth,
            _style.MinHeight,
            _style.MaxHeight,
            (w) => _style.Font.GetSize(_text, w).Height,
            ( ) => _style.Font.GetMinSize(_text),
            ( ) => _style.Font.GetSize(_text)
        );
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
        
        return ComputeMaxLayout(parent).Margin;
        
        #endif
    }
    
    [Pure]
    protected ElementLayout ComputeMaxLayout(Size bounds) {
        var compute = _style.Font.GetSize(Content);
        return IElement.ComputeLayoutFromContent(
            compute,
            bounds,
            _style.Margin,
            _style.Border,
            _style.Padding,
            _style.Width,
            _style.Height,
            _style.MinWidth,
            _style.MaxWidth,
            _style.MinHeight,
            _style.MaxHeight,
            (w) => _style.Font.GetSize(_text, w).Height,
            ( ) => _style.Font.GetMinSize(_text),
            ( ) => _style.Font.GetSize(_text)
        );
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
        
        return ComputeRenderLayout(parent).Margin;
        
        #endif
    }
    
    [Pure]
    protected ElementLayout ComputeRenderLayout(Size bounds) {
        var inner = IElement.ComputeLayoutFromBounds(
            bounds,
            _style.Margin,    _style.Border, _style.Padding,
            _style.Width,     _style.Height,
            _style.MinWidth,  _style.MaxWidth,
            _style.MinHeight, _style.MaxHeight
        );
        
        var content = _style.Font.GetSize(Content, inner.Content.Width);
        
        return IElement.ComputeLayoutFromContent(
            content,
            bounds,
            _style.Margin,    _style.Border, _style.Padding,
            _style.Width,     _style.Height,
            _style.MinWidth,  _style.MaxWidth,
            _style.MinHeight, _style.MaxHeight,
            (w) => _style.Font.GetSize(_text, w).Height,
            ( ) => _style.Font.GetMinSize(_text),
            ( ) => _style.Font.GetSize(_text)
        );
    }
    
    #endregion

    
    // -------------------------------- INODE IMPL -------------------------------- //
    
    #region INode impl
    
    public override AnsiString GetChildNode() => Content;

    public override void SetChildNode(AnsiString childNode) {
        if (childNode == Content) return;
        Content = childNode;
        InvokeElementUpdated();
    }

    #endregion
}