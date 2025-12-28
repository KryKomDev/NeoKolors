// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Elements.Caching;

/// <summary>
/// Provides caching mechanisms for various layout information of elements.
/// </summary>
/// <remarks>
/// The <see cref="LayoutCacher"/> class is used to store and retrieve cached layout information
/// such as compute, minimum, and render layouts for UI elements. Caching requires validation
/// against a specified parent size to ensure correctness.
/// </remarks>
public sealed class LayoutCacher {
    
    private Size          _max_Parent;
    private ElementLayout _max_Layout;
    private bool          _max_IsSet;
    
    private Size          _min_Parent;
    private ElementLayout _min_Layout;
    private bool          _min_IsSet;
    
    private Size          _render_Parent;
    private ElementLayout _render_Layout;
    private bool          _render_IsSet;

    /// <summary>
    /// Gets or sets a delegate function used to validate the compute layout cache.
    /// </summary>
    /// <remarks>
    /// The <c>ValidateMax</c> property stores a function executed during
    /// cache validation to determine if the computed layout is still valid. This function
    /// is called as part of the validation conditions in methods like <c>IsMaxValid</c>.
    /// </remarks>
    public Func<bool> ValidateMax { get; set; }

    /// <summary>
    /// Gets or sets a delegate function used to validate the minimum layout cache.
    /// </summary>
    /// <remarks>
    /// The <c>ValidateMin</c> property stores a function executed during
    /// cache validation to determine if the minimum layout is still valid. This function
    /// is called as part of the validation conditions in methods like <c>IsMinValid</c>.
    /// </remarks>
    public Func<bool> ValidateMin { get; set; }

    /// <summary>
    /// Gets or sets a delegate function used to validate the render layout cache.
    /// </summary>
    /// <remarks>
    /// The <c>ValidateRender</c> property stores a function executed during
    /// cache validation to determine if the rendered layout is still valid.
    /// This function is invoked as part of the validation process in methods like <c>IsRenderValid</c>.
    /// </remarks>
    public Func<bool> ValidateRender { get; set; }

    public bool IsMaxValid(Size parent) {
        var res = _max_IsSet && _max_Parent == parent && ValidateMax();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public bool IsMinValid(Size parent) {
        var res = _min_IsSet && _min_Parent == parent && ValidateMin();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public bool IsRenderValid(Size parent) {
        var res = _render_IsSet && _render_Parent == parent && ValidateRender();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public void SetMax(Size parent, ElementLayout layout) {
        _max_Parent = parent;
        _max_Layout = layout;
        _max_IsSet = true;
    }

    public void SetMin(Size parent, ElementLayout layout) {
        _min_Parent = parent;
        _min_Layout = layout;
        _min_IsSet = true;
    }

    public void SetRender(Size parent, ElementLayout layout) {
        _render_Parent = parent;
        _render_Layout = layout;
        _render_IsSet = true;
    }

    public ElementLayout GetMaxLayout()    => _max_IsSet    ? _max_Layout    : throw LayoutCacherException.UnsetMax();
    public ElementLayout GetMinLayout()    => _min_IsSet    ? _min_Layout    : throw LayoutCacherException.UnsetMin();
    public ElementLayout GetRenderLayout() => _render_IsSet ? _render_Layout : throw LayoutCacherException.UnsetRender();

    public LayoutCacher(Func<bool> validateMin, Func<bool> validateMax, Func<bool> validateRender) {
        ValidateMin    = validateMin;
        ValidateMax    = validateMax;
        ValidateRender = validateRender;
    }
    
    public LayoutCacher() : this(() => true, () => true, () => true) { }
}