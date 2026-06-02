// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

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
    
    private Size          _max_parent;
    private ElementLayout _max_layout;
    private bool          _max_isSet;
    
    private Size          _min_parent;
    private ElementLayout _min_layout;
    private bool          _min_isSet;
    
    private Size          _render_parent;
    private ElementLayout _render_layout;
    private bool          _render_isSet;

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
        var res = _max_isSet && _max_parent == parent && ValidateMax();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public bool IsMinValid(Size parent) {
        var res = _min_isSet && _min_parent == parent && ValidateMin();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public bool IsRenderValid(Size parent) {
        var res = _render_isSet && _render_parent == parent && ValidateRender();
        
        #if NK_ENABLE_CACHE_ANALYSIS
        if (res) CacheAnalyzer.Hit();
        else     CacheAnalyzer.Miss();
        #endif
        
        return res;
    }

    public void SetMax(Size parent, ElementLayout layout) {
        _max_parent = parent;
        _max_layout = layout;
        _max_isSet = true;
    }

    public void SetMin(Size parent, ElementLayout layout) {
        _min_parent = parent;
        _min_layout = layout;
        _min_isSet = true;
    }

    public void SetRender(Size parent, ElementLayout layout) {
        _render_parent = parent;
        _render_layout = layout;
        _render_isSet = true;
    }

    public ElementLayout GetMaxLayout()    => _max_isSet    ? _max_layout    : throw LayoutCacherException.UnsetMax();
    public ElementLayout GetMinLayout()    => _min_isSet    ? _min_layout    : throw LayoutCacherException.UnsetMin();
    public ElementLayout GetRenderLayout() => _render_isSet ? _render_layout : throw LayoutCacherException.UnsetRender();

    public LayoutCacher(Func<bool> validateMin, Func<bool> validateMax, Func<bool> validateRender) {
        ValidateMin    = validateMin;
        ValidateMax    = validateMax;
        ValidateRender = validateRender;
    }
    
    public LayoutCacher() : this(() => true, () => true, () => true) { }
}