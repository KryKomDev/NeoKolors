//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Extensions;

namespace NeoKolors.Console.Input;

/// <summary>
/// Represents information about a keyboard event, including the pressed key, associated character,
/// and any active modifiers.
/// </summary>
public readonly record struct KeyEventArgs {
    public KeyModifiers Modifiers { get; }
    public KeyCode      Key       { get; }
    public char         Char      { get; }
    public bool         Down      { get; }

    public KeyEventArgs(KeyCode key, KeyModifiers modifiers, char c, bool down = true) {
        Modifiers = modifiers;
        Key       = key;
        Char      = c;
        Down      = down;
    }

    public KeyEventArgs(ConsoleKey key, ConsoleModifiers modifiers, char c) {
        Modifiers = KeyModifiers.NONE 
            | (modifiers.HasAlt   ? KeyModifiers.LEFT_ALT  : KeyModifiers.NONE)
            | (modifiers.HasShift ? KeyModifiers.SHIFT     : KeyModifiers.NONE)
            | (modifiers.HasCtrl  ? KeyModifiers.LEFT_CTRL : KeyModifiers.NONE);
        
        Key  = (KeyCode)key;
        Char = c;
    }
    
    public KeyEventArgs(ConsoleKeyInfo keyInfo) : this(keyInfo.Key, keyInfo.Modifiers, keyInfo.KeyChar) { }
    
    
    
    public override string ToString() =>
        $"{(Modifiers != KeyModifiers.NONE ? $"{Modifiers.ToString()} + " : "")}{Key} -> {Char}";
}