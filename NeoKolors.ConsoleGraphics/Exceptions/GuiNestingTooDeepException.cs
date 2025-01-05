namespace NeoKolors.ConsoleGraphics.Exceptions;

public class GuiNestingTooDeepException : Exception {
    public GuiNestingTooDeepException(string? name) : base($"{(name == null ? "GUI nesting" : $"GUI nesting of element {name}")} is too deep!") { }
}