// NeoKolors
// Copyright (c) 2025 KryKom

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
}

public interface INode {
    public object? GetChildNode();
    public void SetChildNode(object? childNode);
}