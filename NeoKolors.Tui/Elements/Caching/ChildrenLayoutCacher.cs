// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Elements.Caching;

public sealed class ChildrenLayoutCacher {

    private bool                _min_IsSet;
    private Size                _min_ParentSize;
    private ChildrenLayout      _min_Layout;
    private readonly Func<bool> _min_LayoutValidator;
    
    private bool                _max_IsSet;
    private Size                _max_ParentSize;
    private ChildrenLayout      _max_Layout;
    private readonly Func<bool> _max_LayoutValidator;

    private bool                _render_IsSet;
    private Size                _render_ParentSize;
    private ChildrenLayout      _render_Layout;
    private readonly Func<bool> _render_LayoutValidator;

    public ChildrenLayoutCacher(Func<bool> minLayoutValidator, Func<bool> maxLayoutValidator, Func<bool> renderLayoutValidator) {
        _min_IsSet    = false;
        _max_IsSet    = false;
        _render_IsSet = false;
        
        _min_ParentSize    = default;
        _max_ParentSize    = default;
        _render_ParentSize = default;
        
        _min_Layout    = default;
        _max_Layout    = default;
        _render_Layout = default;
        
        _min_LayoutValidator    = minLayoutValidator;
        _max_LayoutValidator    = maxLayoutValidator;
        _render_LayoutValidator = renderLayoutValidator;
    }

    public bool IsMinValid(Size parent) {
        var res = _min_IsSet && _min_ParentSize == parent && _min_LayoutValidator();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public bool IsMaxValid(Size parent) {
        var res = _max_IsSet && _max_ParentSize == parent && _max_LayoutValidator();

        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public bool IsRenderValid(Size parent) {
        var res = _render_IsSet && _render_ParentSize == parent && _render_LayoutValidator();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }
    
    public void SetMin(Size parent, ChildrenLayout layout) {
        _min_ParentSize = parent;
        _min_Layout     = layout;
        _min_IsSet      = true;
    }
    
    public void SetMax(Size parent, ChildrenLayout layout) {
        _max_ParentSize = parent;
        _max_Layout     = layout;
        _max_IsSet      = true;
    }
    
    public void SetRender(Size parent, ChildrenLayout layout) {
        _render_ParentSize = parent;
        _render_Layout     = layout;
        _render_IsSet      = true;
    }

    public ChildrenLayout GetMaxLayout()    => _max_IsSet    ? _max_Layout    : throw LayoutCacherException.UnsetMax();
    public ChildrenLayout GetMinLayout()    => _min_IsSet    ? _min_Layout    : throw LayoutCacherException.UnsetMin();
    public ChildrenLayout GetRenderLayout() => _render_IsSet ? _render_Layout : throw LayoutCacherException.UnsetRender();
}