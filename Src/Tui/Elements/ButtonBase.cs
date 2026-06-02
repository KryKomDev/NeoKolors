// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// An abstract base class for all button-like controls in the NeoKolors TUI framework.
/// </summary>
public abstract class ButtonBase : ContentControl, IMouseInteractableElement<IElement> {
    
    public event Action<MouseButton> OnClick = delegate { };
    public event Action<MouseButton> OnRelease = delegate { };
    public event Action OnHover = delegate { };
    public event Action OnHoverOut = delegate { };

    protected ButtonBase(StyleCollection defaultStyle) : base(defaultStyle) { }
    protected ButtonBase() { }

    public void Click(MouseButton button) => OnClick(button);
    public void Release(MouseButton button) => OnRelease(button);
    public void Hover() => OnHover();
    public void HoverOut() => OnHoverOut();
}
