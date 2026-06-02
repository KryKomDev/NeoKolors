// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// An abstract base class for controls that can toggle their checked state.
/// </summary>
public abstract class ToggleButton : ButtonBase {
    
    private bool? _isChecked = false;

    public bool? IsChecked {
        get => _isChecked;
        set {
            if (_isChecked == value) return;
            _isChecked = value;
            OnCheckedChanged();
            UpdateVisualState();
            InvokeElementUpdated();
        }
    }

    public event Action<ToggleButton, bool?>? CheckedStateChanged;

    protected ToggleButton(StyleCollection defaultStyle) : base(defaultStyle) {
        OnClick += HandleClick;
    }

    protected ToggleButton() {
        OnClick += HandleClick;
    }

    private void HandleClick(MouseButton button) {
        if (!IsEnabled) return;
        Toggle();
    }

    protected virtual void Toggle() {
        IsChecked = IsChecked switch {
            true => false,
            false => true,
            _ => true
        };
    }

    protected virtual void OnCheckedChanged() {
        CheckedStateChanged?.Invoke(this, _isChecked);
    }
}
