// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Dom;

public interface INode {
    public OneOf<IElement[], string> GetChildren();
    public void AddChild(IElement[] child);
    public void SetChildren(OneOf<IElement[], string> children);
}