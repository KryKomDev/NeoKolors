//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class SettingsBuilderException : Exception {
    private SettingsBuilderException(string message) : base(message) { }

    public static SettingsBuilderException InvalidNodeName(string name) {
        return new SettingsBuilderException($"Could not switch to node of name '{name}', because it doesn't exist.");
    }

    public static SettingsBuilderException NodeIndexOutOfRange(int index, int length) {
        return new SettingsBuilderException($"Could not switch to node of index {index}, because it is out of range. " +
                                            $"Index must be between 0 inclusive and {length}.");
    }
}