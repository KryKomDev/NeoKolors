﻿
using NeoKolors.Settings.Argument;

namespace NeoKolors.Tui.Elements.Interactive;

/// <summary>
/// interface for interactive elements
/// </summary>
public interface IInteractiveElement<out T> : IElement where T : IArgument {
    public T Argument { get; }
    public void Interact(ConsoleKeyInfo keyInfo);
}