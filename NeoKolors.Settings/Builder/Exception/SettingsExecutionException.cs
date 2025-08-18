// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder.Exception;

public class SettingsExecutionException : System.Exception {
    private SettingsExecutionException(string message) : base(message) { }
    
    public static SettingsExecutionException NoResultConstructor() => 
        new($"Could not return result of settings execution. Result builder is not initialized.");
    
    public static SettingsExecutionException InvalidResultType() => 
        new($"Could not build result. Result is of invalid type.");
    
    public static SettingsExecutionException NoOptionSelected() =>
        new($"Could not merge context. No option was selected.");

    public static SettingsExecutionException NoContextMerger() => 
        new($"Could not merge context. No context merger was set.");
    
    public static SettingsExecutionException InvalidOptionName(string optionName) => 
        new($"Could not return option of name '{optionName}'. Option with that name doesn't exist.");
    
    public static SettingsExecutionException InvalidNodeName(string name) => 
        new($"Could not choose node '{name}'. Node with that name doesn't exist.");
    
    public static SettingsExecutionException UnknownSettingsElement(string name) => 
        new($"Could not parse node: element named '{name}' is of a unknown type.");
}