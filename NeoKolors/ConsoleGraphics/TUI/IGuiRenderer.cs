namespace NeoKolors.ConsoleGraphics.TUI;

public interface IGuiRenderer {
    public static bool TERMINAL_PALETTE_MODE = true;
    public static abstract void Render(IGuiBuilder gui);
}