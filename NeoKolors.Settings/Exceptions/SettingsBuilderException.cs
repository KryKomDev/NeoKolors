//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class SettingsBuilderException : Exception {
    private SettingsBuilderException(string message) : base(message) { }

    public static SettingsBuilderException SwitchInvalidNodeName(string name) => new($"Could not switch to node of name '{name}', because it doesn't exist.");

    public static SettingsBuilderException SwitchNodeIndexOutOfRange(int index, int length) =>
        new($"Could not switch to node of index {index}. Index out of range. " +
            $"Index must be between 0 inclusive and {length}.");

    public static SettingsBuilderException InvalidNodeName(string name) => new($"Could not return node of name '{name}', because it doesn't exist.");

    public static SettingsBuilderException NodeIndexOutOfRange(int index, int length) =>
        new($"Could not return node of index {index}. Index out of range. " +
            $"Index must be between 0 inclusive and {length}.");
}