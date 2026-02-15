// NeoKolors
// Copyright (c) 2026 KryKom

using Implyzer;
using NeoKolors.Console.Mouse;

namespace NeoKolors.Tui.Elements;

public interface IMouseInteractableElement<T> : IInteractableElement, IElement<T> { }

[IndirectImpl(typeof(IMouseInteractableElement<>))]
public interface IInteractableElement : IElement {
    public event Action<MouseButton> OnClick;
    public event Action<MouseButton> OnRelease;
    public event Action OnHover;
    public event Action OnHoverOut;
}