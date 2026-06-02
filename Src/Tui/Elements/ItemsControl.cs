// NeoKolors
// Copyright (c) 2026 KryKom

using System.Collections;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a control that can display a collection of items.
/// </summary>
public abstract class ItemsControl : Control<IReadOnlyList<IElement>>, INode<IReadOnlyList<IElement>> {
    
    private IEnumerable? _itemsSource;
 
    public IEnumerable? ItemsSource {
        get => _itemsSource;
        set {
            if (ReferenceEquals(_itemsSource, value)) return;
            _itemsSource = value;
            OnItemsSourceChanged();
        }
    }
 
    public Panel? ItemsPanel { get; set; }
 
    protected ItemsControl(StyleCollection defaultStyle) : base(defaultStyle) { }
    protected ItemsControl() { }
 
    protected virtual void OnItemsSourceChanged() {
        // Subclasses override this to rebuild child elements when ItemsSource changes
    }
 
    public override IReadOnlyList<IElement> GetChildNode() {
        return ItemsPanel?.GetChildNode() ?? Array.Empty<IElement>();
    }
 
    public override void SetChildNode(IReadOnlyList<IElement> childNode) {
        if (ItemsPanel != null) {
            ItemsPanel.SetChildNode(childNode);
            InvokeElementUpdated();
        }
    }

    void INode.SetChildNode(object? child) {
        if (ItemsPanel != null) {
            ((INode)ItemsPanel).SetChildNode(child);
            InvokeElementUpdated();
        }
    }
}
