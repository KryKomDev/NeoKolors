// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a control with a single piece of content. Content controls are designed to wrap
/// arbitrary contents (strings, custom models, or individual elements) under structural templates.
/// </summary>
public abstract class ContentControl : Control<IElement>, INode<IElement> {
    
    private object? _content;

    public object? Content {
        get => _content;
        set {
            if (ReferenceEquals(_content, value)) return;
            var old = _content;
            _content = value;
            OnContentChanged(old, value);
            InvokeElementUpdated();
        }
    }

    public event Action<ContentControl, object?, object?>? ContentChanged;

    protected ContentControl(StyleCollection defaultStyle) : base(defaultStyle) { }
    protected ContentControl() { }

    private TextBlock? _fallbackTextBlock;

    protected virtual void OnContentChanged(object? oldContent, object? newContent) {
        if (_fallbackTextBlock != null) {
            _fallbackTextBlock.OnElementUpdated -= OnContentUpdated;
            _fallbackTextBlock = null;
        }
        if (oldContent is IElement oldElement) {
            oldElement.OnElementUpdated -= OnContentUpdated;
        }
        if (newContent is IElement newElement) {
            newElement.OnElementUpdated += OnContentUpdated;
        }
        ContentChanged?.Invoke(this, oldContent, newContent);
    }

    private void OnContentUpdated() {
        InvokeElementUpdated();
    }

    public override IElement GetChildNode() {
        if (Content is IElement element) {
            if (_fallbackTextBlock != null) {
                _fallbackTextBlock.OnElementUpdated -= OnContentUpdated;
                _fallbackTextBlock = null;
            }
            return element;
        }
        
        var text = Content?.ToString() ?? string.Empty;
        if (_fallbackTextBlock == null || _fallbackTextBlock.Content.ToString() != text) {
            if (_fallbackTextBlock != null) {
                _fallbackTextBlock.OnElementUpdated -= OnContentUpdated;
            }
            _fallbackTextBlock = new TextBlock(text);
            _fallbackTextBlock.OnElementUpdated += OnContentUpdated;
        }
        
        return _fallbackTextBlock;
    }

    public override void SetChildNode(IElement childNode) {
        Content = childNode;
    }

    // Explicitly implement INode.SetChildNode to support both IElement and plain content parsing from XML/DOM loaders
    void INode.SetChildNode(object? child) {
        if (child is IElement element) {
            Content = element;
        }
        else {
            Content = child;
        }
    }
}
