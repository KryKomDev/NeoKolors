// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions;

public static class ConsoleKeyInfoExtensions {
    extension(ConsoleKeyInfo key) {
        public string ToString() => $"{(key.HasCtrl ? "Ctrl + " : "")}" +
                                    $"{(key.HasAlt ? "Alt + " : "")}" +
                                    $"{(key.HasShift ? "Shift + " : "")}" + 
                                    $"{key.Key.ToString()} => '{key.KeyChar}'";
        
        public bool HasShift => key.Modifiers.HasFlag(ConsoleModifiers.Shift);
        public bool HasAlt => key.Modifiers.HasFlag(ConsoleModifiers.Alt);
        public bool HasCtrl => key.Modifiers.HasFlag(ConsoleModifiers.Control);
    }
    
    extension(ConsoleModifiers mods) {
        public bool HasShift => mods.HasFlag(ConsoleModifiers.Shift);
        public bool HasAlt => mods.HasFlag(ConsoleModifiers.Alt);
        public bool HasCtrl => mods.HasFlag(ConsoleModifiers.Control);
    }
}