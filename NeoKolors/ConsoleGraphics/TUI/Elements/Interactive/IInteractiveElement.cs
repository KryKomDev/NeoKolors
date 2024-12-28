using NeoKolors.ConsoleGraphics.Settings.ArgumentType;

namespace NeoKolors.ConsoleGraphics.TUI.Elements.Interactive;

/// <summary>
/// interface for interactive elements
/// </summary>
public interface IInteractiveElement<out T> : IElement where T : IArgumentType {
    public T Argument { get; }
    public void Interact(ConsoleKeyInfo keyInfo);
}