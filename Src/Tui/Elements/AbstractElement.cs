//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Reflection;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a universal element in the NeoKolors TUI framework. It provides common styling and layout properties
/// shared by all elements and defines the fundamental structure and behavior for custom elements.
/// Every property is overridable, so any element can define its defaults.
/// </summary>
public abstract class AbstractElement<T> : IElement<T> {
    protected StyleCollection _style;

    public StyleCollection Style {
        get => _style;
        init => _style.Apply(value);
    }

    private bool _isFocused;

    public virtual bool IsFocused {
        get => _isFocused;
        set {
            if (_isFocused == value)
                return;

            _isFocused = value;
            EvaluateTriggers();
            InvokeElementUpdated();
        }
    }

    private bool _isHovered;

    public virtual bool IsHovered {
        get => _isHovered;
        set {
            if (_isHovered == value)
                return;

            _isHovered = value;
            EvaluateTriggers();
            InvokeElementUpdated();
        }
    }

    public List<TriggerBase> Triggers { get; set; } = new();

    private readonly Dictionary<string, object?> _originalValues          = new();
    private readonly HashSet<string>             _activeTriggerProperties = new();

    public void EvaluateTriggers() {
        _activeTriggerProperties.Clear();
        var settersToApply = new Dictionary<string, object>();

        if (Triggers != null) {
            foreach (var trigger in Triggers) {
                if (!trigger.IsActive(this))
                    continue;

                foreach (var setter in trigger.Setters) {
                    if (string.IsNullOrEmpty(setter.Property))
                        continue;

                    settersToApply[setter.Property] = setter.Value;
                    _activeTriggerProperties.Add(setter.Property);
                }
            }
        }

        // Save original values for properties about to be modified by triggers
        foreach (var propName in settersToApply.Keys) {
            if (!_originalValues.ContainsKey(propName)) {
                _originalValues[propName] = GetPropertyValue(propName);
            }
        }

        // Restore original values for properties no longer modified by active triggers
        var propertiesToRestore = new List<string>();

        foreach (var key in _originalValues.Keys) {
            if (!_activeTriggerProperties.Contains(key)) {
                propertiesToRestore.Add(key);
            }
        }

        foreach (var propName in propertiesToRestore) {
            var origValue = _originalValues[propName];
            SetPropertyValue(propName, origValue);
            _originalValues.Remove(propName);
        }

        // Apply active trigger setters
        foreach (var kvp in settersToApply) {
            SetPropertyValue(kvp.Key, kvp.Value);
        }
    }

    private object? GetPropertyValue(string name) {
        var prop = GetType()
            .GetProperty(
                name,
                BindingFlags.IgnoreCase |
                BindingFlags.Public     |
                BindingFlags.Instance
            );

        if (prop != null && prop.CanRead) {
            return prop.GetValue(this);
        }

        if (!Styles.Properties.IStyleProperty.TryGetByName(name, out var type))
            return null;

        var styleProp = Style[type];

        return styleProp.Value;
    }

    private void SetPropertyValue(string name, object? value) {
        if (value == null)
            return;

        // 1. Try public C# property on the element class
        var prop = GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop != null && prop.CanWrite) {
            try {
                if (value is string str) {
                    var converted = Dom.XamlElementLoader.ConvertValue(str, prop.PropertyType);
                    prop.SetValue(this, converted);
                }
                else {
                    prop.SetValue(this, value);
                }

                return;
            }
            catch {
                // fall through
            }
        }

        // 2. Try style property inside element.Style
        if (Styles.Properties.IStyleProperty.TryGetByName(name, out var type)) {
            try {
                object? targetValue = value;

                if (value is string strVal) {
                    Type targetType;

                    if (Styles.Properties.IStyleProperty.IsPartial(type)) {
                        var partial = (Styles.Properties.IPartialStyleProperty?)Activator.CreateInstance(type);
                        targetType = partial!.ValueType;
                    }
                    else {
                        var full = (Styles.Properties.IStyleProperty?)Activator.CreateInstance(type);
                        targetType = full!.ValueType;
                    }

                    targetValue = Dom.XamlElementLoader.ConvertValue(strVal, targetType);
                }

                var property = (Styles.Properties.IStyleProperty)Activator.CreateInstance(type, targetValue)!;
                Style.Set(property);
            }
            catch {
                // fall through
            }
        }
    }

    protected AbstractElement(StyleCollection defaultStyle) {
        _style              =  new StyleCollection(defaultStyle);
        _style.StyleChanged += _ => InvokeElementUpdated();
    }

    protected AbstractElement() {
        _style              =  new StyleCollection(AbstractElement.DefaultStyle);
        _style.StyleChanged += _ => InvokeElementUpdated();
    }

    // Cache for layout computations
    private Size2D?        _cachedMinParent;
    private Size2D?        _cachedMinContent;
    private ElementLayout? _cachedMinLayout;

    private Size2D?        _cachedRenderLayoutParent;
    private ElementLayout? _cachedRenderLayout;

    public Size2D DesiredSize  { get; protected set; } = Size2D.Zero;
    public Area2D RenderBounds { get; protected set; } = default;

    public ElementLayout RenderLayout {
        get {
            var bounds = RenderBounds != default ? RenderBounds.Size : Size2D.Zero;

            return GetLayout(bounds);
        }
    }

    public virtual event Action? OnElementUpdated;

    protected void InvokeElementUpdated() {
        InvalidateMeasure();
        OnElementUpdated?.Invoke();
    }

    public void InvalidateMeasure() {
        _cachedMinParent          = null;
        _cachedMinContent         = null;
        _cachedMinLayout          = null;
        _cachedRenderLayoutParent = null;
        _cachedRenderLayout       = null;

        _isMeasureValid = false;
        _isArrangeValid = false;
    }

    public void InvalidateArrange() {
        _cachedRenderLayoutParent = null;
        _cachedRenderLayout       = null;
        _isArrangeValid           = false;
    }

    private bool   _isMeasureValid;
    private Size2D _lastAvailableSize;

    private bool _isMeasuring;

    public void Measure(Size2D availableSize) {
        if (!_style.Visible) {
            DesiredSize     = Size2D.Zero;
            _isMeasureValid = true;

            return;
        }

        if (_isMeasuring) {
            return;
        }

        if (_isMeasureValid && _lastAvailableSize == availableSize) {
            return;
        }

        _isMeasuring = true;

        try {
            _lastAvailableSize = availableSize;
            var contentSize = MeasureOverride(availableSize);
            var minLayout   = GetMinLayout(availableSize, contentSize);
            DesiredSize     = minLayout.Margin;
            _isMeasureValid = true;
        }
        finally {
            _isMeasuring = false;
        }
    }

    private bool   _isArrangeValid;
    private Area2D _lastFinalRect;

    public void Arrange(Area2D finalRect) {
        if (!_style.Visible) {
            RenderBounds    = default;
            _isArrangeValid = true;

            return;
        }

        if (_isArrangeValid && _lastFinalRect == finalRect) {
            return;
        }

        RenderBounds   = finalRect;
        _lastFinalRect = finalRect;

        _cachedRenderLayout = IElement.ComputeLayoutFromBounds(
            finalRect.Size,
            _style.Margin,
            _style.Border,
            _style.Padding,
            _style.Width,
            _style.Height,
            _style.MinWidth,
            _style.MaxWidth,
            _style.MinHeight,
            _style.MaxHeight
        );

        _cachedRenderLayoutParent = finalRect.Size;

        ArrangeOverride(_cachedRenderLayout.Value.Content.Size);

        _isArrangeValid = true;
    }

    protected ElementLayout GetLayout(Size2D bounds) {
        if (_cachedRenderLayoutParent == bounds && _cachedRenderLayout != null) {
            return _cachedRenderLayout.Value;
        }

        var layout = IElement.ComputeLayoutFromBounds(
            bounds,
            _style.Margin,
            _style.Border,
            _style.Padding,
            _style.Width,
            _style.Height,
            _style.MinWidth,
            _style.MaxWidth,
            _style.MinHeight,
            _style.MaxHeight
        );

        _cachedRenderLayoutParent = bounds;
        _cachedRenderLayout       = layout;

        return layout;
    }

    protected ElementLayout GetMinLayout(Size2D parent, Size2D content) {
        if (_cachedMinParent == parent && _cachedMinContent == content && _cachedMinLayout != null) {
            return _cachedMinLayout.Value;
        }

        var layout = IElement.ComputeLayoutFromContent(
            content,
            parent,
            _style.Margin,
            _style.Border,
            _style.Padding,
            _style.Width,
            _style.Height,
            _style.MinWidth,
            _style.MaxWidth,
            _style.MinHeight,
            _style.MaxHeight
        );

        _cachedMinParent  = parent;
        _cachedMinContent = content;
        _cachedMinLayout  = layout;

        return layout;
    }

    public virtual void Render(ICharCanvas canvas) {
        if (!_style.Visible)
            return;

        // Auto-measure and auto-arrange if not already completed (e.g. root elements)
        if (!_isMeasureValid) {
            Measure(new Size2D(canvas.Width, canvas.Height));
        }

        if (!_isArrangeValid) {
            Arrange(RenderBounds != default ? RenderBounds : new Area2D(0, 0, canvas.Width, canvas.Height));
        }

        var pos = RenderBounds.Lower;

        if (!_style.BackgroundColor.IsInherit) {
            canvas.StyleBackground(RenderLayout.Border + pos, _style.BackgroundColor);
            canvas.Fill(RenderLayout.Content           + pos, ' ');
        }

        if (!_style.TextColor.IsInherit) {
            canvas.StyleTextColor(RenderLayout.Border + pos, _style.TextColor);
        }

        if (!_style.Border.IsBorderless) {
            canvas.PlaceRectangle(RenderLayout.Border + pos, _style.Border);
        }

        RenderCore(canvas);
    }

    protected virtual Size2D MeasureOverride(Size2D availableSize) {
        if (GetChildNode() is IElement childElement) {
            childElement.Measure(availableSize);

            return childElement.DesiredSize;
        }

        return Size2D.Zero;
    }

    protected virtual Size2D ArrangeOverride(Size2D finalSize) {
        if (GetChildNode() is IElement childElement) {
            childElement.Arrange(RenderLayout.Content + RenderBounds.Lower);
        }

        return finalSize;
    }

    protected virtual void RenderCore(ICharCanvas canvas) { }

    // =========================== IELEMENT IMPLEMENTATION =========================== // 

    public abstract ElementInfo Info { get; }
    public abstract T           GetChildNode();
    public abstract void        SetChildNode(T childNode);
}

public static class AbstractElement {
    public static StyleCollection DefaultStyle { get; } = new() {
        Visible         = true,
        Padding         = Spacing.Zero,
        Margin          = Spacing.Zero,
        Position        = new Position(),
        Overflow        = false,
        GridAlign       = new Area2D(0, 0, 0, 0),
        ZIndex          = 0,
        BackgroundColor = NKColor.Inherit,
        Border          = BorderStyle.Borderless,
        Width           = Dimension.Auto,
        Height          = Dimension.Auto,
        MinWidth        = Dimension.Auto,
        MinHeight       = Dimension.Auto,
        MaxWidth        = Dimension.Auto,
        MaxHeight       = Dimension.Auto,

        ReadOnly = true
    };
}