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

    extension(ConsoleKeyInfo) {
        
        /// <summary>
        /// Converts a character to a ConsoleKeyInfo instance.
        /// </summary>
        /// <param name="c">The character to convert.</param>
        /// <returns>A ConsoleKeyInfo instance representing the character.</returns>
        public static ConsoleKeyInfo FromChar(char c) {
            ConsoleKey key;
            ConsoleModifiers modifiers = 0;
    
            // Handle special characters and keys
            switch (c) {
                case '\b':
                    key = ConsoleKey.Backspace;
                    break;
                case '\t':
                    key = ConsoleKey.Tab;
                    break;
                case '\r':
                case '\n':
                    key = ConsoleKey.Enter;
                    break;
                case '\u001b': // ESC
                    key = ConsoleKey.Escape;
                    break;
                case ' ':
                    key = ConsoleKey.Spacebar;
                    break;
                case '0': key = ConsoleKey.D0; break;
                case '1': key = ConsoleKey.D1; break;
                case '2': key = ConsoleKey.D2; break;
                case '3': key = ConsoleKey.D3; break;
                case '4': key = ConsoleKey.D4; break;
                case '5': key = ConsoleKey.D5; break;
                case '6': key = ConsoleKey.D6; break;
                case '7': key = ConsoleKey.D7; break;
                case '8': key = ConsoleKey.D8; break;
                case '9': key = ConsoleKey.D9; break;
                default:
                    // Handle letters
                    if (char.IsLetter(c)) {
                        char upperC = char.ToUpper(c);
                        if (char.IsUpper(c) && char.IsLower(c) == false) {
                            modifiers = ConsoleModifiers.Shift;
                        }
    
                        key = (ConsoleKey)Enum.Parse(typeof(ConsoleKey), upperC.ToString());
                    }
                    // Handle symbols that might require shift
                    else if (char.IsSymbol(c) || char.IsPunctuation(c)) {
                        var ki = GetKeyForSymbol(c);
                        key = ki.Key;
                        if (ki.Shift) {
                            modifiers = ConsoleModifiers.Shift;
                        }
                    }
                    else {
                        // Default to the character's numeric value if no specific mapping exists
                        key = (ConsoleKey)c;
                    }
    
                    break;
            }
    
            return new ConsoleKeyInfo(c, key, modifiers.HasShift, modifiers.HasAlt, modifiers.HasCtrl);
        }
    }
    
    /// <summary>
    /// Gets the ConsoleKey for common symbols and determines if Shift is required.
    /// </summary>
    /// <param name="symbol">The symbol character.</param>
    /// <returns>The corresponding ConsoleKey and whether shift was used.</returns>
    private static (ConsoleKey Key, bool Shift) GetKeyForSymbol(char symbol) {
        return symbol switch {
            '!' => (ConsoleKey.D1, true),
            '@' => (ConsoleKey.D2, true),
            '#' => (ConsoleKey.D3, true),
            '$' => (ConsoleKey.D4, true),
            '%' => (ConsoleKey.D5, true),
            '^' => (ConsoleKey.D6, true),
            '&' => (ConsoleKey.D7, true),
            '*' => (ConsoleKey.D8, true),
            '(' => (ConsoleKey.D9, true),
            ')' => (ConsoleKey.D0, true),
            '-' => (ConsoleKey.OemMinus, false),
            '_' => (ConsoleKey.OemMinus, true),
            '=' => (ConsoleKey.OemPlus, false),
            '+' => (ConsoleKey.OemPlus, true),
            ':' => (ConsoleKey.Oem1, true),
            ';' => (ConsoleKey.Oem1, false),
            '?' => (ConsoleKey.Oem2, true),
            '/' => (ConsoleKey.Oem2, false),
            '~' => (ConsoleKey.Oem3, true),
            '`' => (ConsoleKey.Oem3, false),
            '{' => (ConsoleKey.Oem4, true),
            '[' => (ConsoleKey.Oem4, false),
            '|' => (ConsoleKey.Oem5, true),
            '\\' => (ConsoleKey.Oem5, false),
            '}' => (ConsoleKey.Oem6, true),
            ']' => (ConsoleKey.Oem6, false),
            '"' => (ConsoleKey.Oem7, true),
            '\'' => (ConsoleKey.Oem7, false),
            ',' => (ConsoleKey.OemComma, false),
            '<' => (ConsoleKey.OemComma, true),
            '.' => (ConsoleKey.OemPeriod, false),
            '>' => (ConsoleKey.OemPeriod, true),
            _ => ((ConsoleKey)symbol, false)
        };
    }
}