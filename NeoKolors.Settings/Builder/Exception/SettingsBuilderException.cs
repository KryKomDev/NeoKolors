//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Builder.Exception;

public class SettingsBuilderException : System.Exception {
    internal SettingsBuilderException(string message) : base(message) { }

    public static SettingsBuilderException SwitchInvalidNodeName(string name) => 
        new($"Could not switch to node of name '{name}', because it doesn't exist.");

    public static SettingsBuilderException SwitchNodeIndexOutOfRange(int index, int length) =>
        new($"Could not switch to node of index {index}. Index out of range. " +
            $"Index must be between 0 inclusive and {length}.");

    public static SettingsBuilderException InvalidNodeName(string name) => 
        new($"Could not return node of name '{name}', because it doesn't exist.");

    public static SettingsBuilderException NodeIndexOutOfRange(int index, int length) =>
        new($"Could not return node of index {index}. Index out of range. " +
            $"Index must be between 0 inclusive and {length}.");
    
    public static SettingsBuilderException DuplicateNode(string name) =>
        new($"Could not add node '{name}'. Node with the same name already exists.");
    
    public static SettingsBuilderException DuplicateElement(string name) =>
        new($"Could not add element '{name}'. Element with the same name already exists.");
    
    public static SettingsBuilderException DuplicateOption(string name) =>
        new($"Could not add option '{name}'. Option with the same name already exists.");
    
    public static SettingsBuilderException InvalidNodeType() => 
        new($"Could not add node. Node is of invalid type.");
}