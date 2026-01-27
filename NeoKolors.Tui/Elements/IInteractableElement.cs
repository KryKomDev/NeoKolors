// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Mouse;

namespace NeoKolors.Tui.Elements;

public interface IInteractableElement<T> : IInteractableElement, IElement<T> { }

public interface IInteractableElement : IElement {
    public event Action<MouseButton> OnClick;
    public event Action<MouseButton> OnRelease;
    public event Action OnHover;
    public event Action OnHoverOut;
}