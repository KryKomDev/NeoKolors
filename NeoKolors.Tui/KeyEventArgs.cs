namespace NeoKolors.Tui;

/// <summary>
/// event argument for keyboard interactions
/// </summary>
public class KeyEventArgs : EventArgs {
    public ConsoleKeyInfo PressedKey { get; }

    public KeyEventArgs(ConsoleKeyInfo pressedKey) {
        PressedKey = pressedKey;
    }
}