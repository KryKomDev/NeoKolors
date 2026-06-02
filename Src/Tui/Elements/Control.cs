// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles;
#if NK_ENABLE_CACHING
using NeoKolors.Tui.Elements.Caching;
#endif

namespace NeoKolors.Tui.Elements;

/// <summary>
/// The fundamental element for all interactive controls in the UWP-inspired NeoKolors TUI framework.
/// It provides focus state, interaction enablement, and drives visual updates matching active states.
/// </summary>
public abstract class Control<T> : AbstractElement<T> {
    
    private bool _isEnabled = true;
    private bool _isTabStop = true;
    private int _tabIndex;

    public bool IsEnabled {
        get => _isEnabled;
        set {
            if (_isEnabled == value) return;
            _isEnabled = value;
            IsEnabledChanged?.Invoke(this, value);
            UpdateVisualState();
            InvokeElementUpdated();
        }
    }

    public override bool IsFocused {
        get => base.IsFocused;
        set {
            if (base.IsFocused == value) return;
            base.IsFocused = value;
            UpdateVisualState();
        }
    }

    public bool IsTabStop {
        get => _isTabStop;
        set => _isTabStop = value;
    }

    public int TabIndex {
        get => _tabIndex;
        set => _tabIndex = value;
    }

    public event Action<Control<T>>? GotFocus;
    public event Action<Control<T>>? LostFocus;
    public event Action<Control<T>, bool>? IsEnabledChanged;

    protected Control(StyleCollection defaultStyle) : base(defaultStyle) { }
    protected Control() { }

    public virtual void Focus() {
        if (!IsEnabled || !IsTabStop || IsFocused) return;
        IsFocused = true;
        GotFocus?.Invoke(this);
    }

    public virtual void Unfocus() {
        if (!IsFocused) return;
        IsFocused = false;
        LostFocus?.Invoke(this);
    }

    protected virtual void UpdateVisualState() {
        // Subclasses override this to dynamically modify Style properties matching state changes
    }
}
