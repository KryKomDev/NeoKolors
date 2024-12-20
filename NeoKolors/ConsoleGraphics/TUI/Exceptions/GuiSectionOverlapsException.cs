namespace NeoKolors.ConsoleGraphics.TUI.Exceptions;

public class GuiSectionOverlapsException : Exception {
    public GuiSectionOverlapsException(string nameA, string nameB) : base($"Gui sections '{nameA}' and '{nameB}' overlap.") { }
}