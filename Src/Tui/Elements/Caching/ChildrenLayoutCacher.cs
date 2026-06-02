// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements.Caching;

public sealed class ChildrenLayoutCacher {

    private bool                _min_isSet;
    private Size                _min_parentSize;
    private ChildrenLayout      _min_layout;
    private readonly Func<bool> _min_layoutValidator;
    
    private bool                _max_isSet;
    private Size                _max_parentSize;
    private ChildrenLayout      _max_layout;
    private readonly Func<bool> _max_layoutValidator;

    private bool                _render_isSet;
    private Size                _render_parentSize;
    private ChildrenLayout      _render_layout;
    private readonly Func<bool> _render_layoutValidator;

    public ChildrenLayoutCacher(Func<bool> minLayoutValidator, Func<bool> maxLayoutValidator, Func<bool> renderLayoutValidator) {
        _min_isSet    = false;
        _max_isSet    = false;
        _render_isSet = false;
        
        _min_parentSize    = default;
        _max_parentSize    = default;
        _render_parentSize = default;
        
        _min_layout    = default;
        _max_layout    = default;
        _render_layout = default;
        
        _min_layoutValidator    = minLayoutValidator;
        _max_layoutValidator    = maxLayoutValidator;
        _render_layoutValidator = renderLayoutValidator;
    }

    public bool IsMinValid(Size parent) {
        var res = _min_isSet && _min_parentSize == parent && _min_layoutValidator();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public bool IsMaxValid(Size parent) {
        var res = _max_isSet && _max_parentSize == parent && _max_layoutValidator();

        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public bool IsRenderValid(Size parent) {
        var res = _render_isSet && _render_parentSize == parent && _render_layoutValidator();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }
    
    public void SetMin(Size parent, ChildrenLayout layout) {
        _min_parentSize = parent;
        _min_layout     = layout;
        _min_isSet      = true;
    }
    
    public void SetMax(Size parent, ChildrenLayout layout) {
        _max_parentSize = parent;
        _max_layout     = layout;
        _max_isSet      = true;
    }
    
    public void SetRender(Size parent, ChildrenLayout layout) {
        _render_parentSize = parent;
        _render_layout     = layout;
        _render_isSet      = true;
    }

    public ChildrenLayout GetMaxLayout()    => _max_isSet    ? _max_layout    : throw LayoutCacherException.UnsetMax();
    public ChildrenLayout GetMinLayout()    => _min_isSet    ? _min_layout    : throw LayoutCacherException.UnsetMin();
    public ChildrenLayout GetRenderLayout() => _render_isSet ? _render_layout : throw LayoutCacherException.UnsetRender();
}