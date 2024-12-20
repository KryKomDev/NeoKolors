namespace NeoKolors.ConsoleGraphics.TUI;

public interface IGuiBuilder {
    public IGuiSection[] GetRenderedSections();
    public void Build(params IGuiSection[] sections);
}