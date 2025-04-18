//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Events;

/// <summary>
/// event argument for keyboard interactions
/// </summary>
public class KeyEventArgs : EventArgs {
    public ConsoleKeyInfo PressedKey { get; }

    public KeyEventArgs(ConsoleKeyInfo pressedKey) {
        PressedKey = pressedKey;
    }
}