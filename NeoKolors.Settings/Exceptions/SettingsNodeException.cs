//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class SettingsNodeException : Exception {
    private SettingsNodeException(string message) : base(message) { }

    public static SettingsNodeException NoResultConstructor(string nodeName) => 
        new($"Could not return result of settings node '{nodeName}'. Result builder is not initialized.");

    public static SettingsNodeException InvalidResultType(string nodeName, Type nodeResulType, Type actualResultType) => 
        new($"Could not set result constructor for node '{nodeName}'. " +
            $"Result type '{actualResultType.FullName}' does not match expected type '{nodeResulType.FullName}'.");
    
    public static SettingsNodeException InvalidResultType(string nodeName, Type nodeResultType) => 
        new($"Could not set result constructor for  node '{nodeName}'. " +
            $"Type checking failed. Constructor must return object of type '{nodeResultType.FullName}'."); 
    
    public static SettingsNodeException InvalidGroupName(string nodeName, string groupName) =>
        new($"Could not return group named '{groupName}' in node '{nodeName}'. Group of name '{groupName}' does not exist.");
}