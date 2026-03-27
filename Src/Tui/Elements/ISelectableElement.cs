// NeoKolors
// Copyright (c) 2026 KryKom

using Implyzer;

namespace NeoKolors.Tui.Elements;

public interface ISelectableElement<T> : IElement<T>, ISelectableElement;

[IndirectImpl(typeof(ISelectableElement<>))]
public interface ISelectableElement : IElement {
    public bool IsSelected { get; }
    public bool IsSelectable { get; }

    public void Select();
    public void Deselect();
}