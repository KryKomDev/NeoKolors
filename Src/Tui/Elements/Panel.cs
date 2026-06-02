using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Provides a base class for all layout containers that arrange multiple child controls.
/// </summary>
public abstract class Panel : Control<IReadOnlyList<IElement>>, INode<IReadOnlyList<IElement>> {
    
    protected readonly List<IElement> _children = new();
    
    public IReadOnlyList<IElement> Children => _children;

    protected Panel(StyleCollection defaultStyle) : base(defaultStyle) { }
    protected Panel() { }

    public virtual void AddChild(IElement child) {
        if (child == null) throw new ArgumentNullException(nameof(child));
        if (_children.Contains(child)) return;

        _children.Add(child);
        child.OnElementUpdated += OnChildUpdated;
        InvokeElementUpdated();
    }

    public virtual void RemoveChild(IElement child) {
        if (child == null) throw new ArgumentNullException(nameof(child));
        if (!_children.Contains(child)) return;

        _children.Remove(child);
        child.OnElementUpdated -= OnChildUpdated;
        InvokeElementUpdated();
    }

    public virtual void ClearChildren() {
        foreach (var child in _children) {
            child.OnElementUpdated -= OnChildUpdated;
        }
        _children.Clear();
        InvokeElementUpdated();
    }

    public override IReadOnlyList<IElement> GetChildNode() => _children;

    public override void SetChildNode(IReadOnlyList<IElement> childNode) {
        ClearChildren();
        if (childNode == null) return;
        foreach (var child in childNode) {
            AddChild(child);
        }
    }

    void INode.SetChildNode(object? child) {
        ClearChildren();
        if (child == null) return;
        if (child is IElement el) {
            AddChild(el);
        }
        else if (child is IEnumerable<IElement> children) {
            foreach (var c in children) {
                if (c != null) AddChild(c);
            }
        }
    }

    protected void OnChildUpdated() => InvokeElementUpdated();
}
