// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a control that contains a single piece of content and a header.
/// </summary>
public abstract class HeaderedContentControl : ContentControl {
    
    private object? _header;

    public object? Header {
        get => _header;
        set {
            if (ReferenceEquals(_header, value)) return;
            var old = _header;
            _header = value;
            OnHeaderChanged(old, value);
            InvokeElementUpdated();
        }
    }

    public event Action<HeaderedContentControl, object?, object?>? HeaderChanged;

    protected HeaderedContentControl(StyleCollection defaultStyle) : base(defaultStyle) { }
    protected HeaderedContentControl() { }

    protected virtual void OnHeaderChanged(object? oldHeader, object? newHeader) {
        HeaderChanged?.Invoke(this, oldHeader, newHeader);
    }
}
