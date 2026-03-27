// NeoKolors
// Copyright (c) 2026 KryKom

using Implyzer;
using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Dom;

public interface INode<T> : INode {
    public new T GetChildNode();
    public void SetChildNode(T child);

    object? INode.GetChildNode() => GetChildNode();
    void INode.SetChildNode(object? child) {
        if (child is T c) 
            SetChildNode(c);
        else 
            throw new ArgumentException("Invalid child node type.");
    }
    
    public bool CanHaveChildren => typeof(IEnumerable<IElement>).IsAssignableFrom(typeof(T));
}

[IndirectImpl(typeof(INode<>))]
public interface INode {
    public object? GetChildNode();
    public void SetChildNode(object? childNode);
}