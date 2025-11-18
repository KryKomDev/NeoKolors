//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Command.Exception;

public class CommandBuilderException : System.Exception {
    public CommandBuilderException(string message) : base(message) { }

    public static CommandBuilderException DuplicateNode(string node) => 
        new($"A node with the name '{node}' already exists.");
    
    public static CommandBuilderException DuplicateDefaultNode() => 
        new($"A default node already exists.");

    public static CommandBuilderException InvalidNodeName() =>
        new($"Could not create node. Node name is invalid.");
    
    public static CommandBuilderException InvalidNodeType() =>
        new($"Could not create node. Node is of invalid type.");
    
    public static CommandBuilderException InvalidNodeCollectionType() =>
        new($"Could not create node. Node collection is of invalid type.");
    
    public static CommandBuilderException InvalidResultType() =>
        new($"Could not build result. Result is of invalid type.");
    
    public static CommandBuilderException DefaultNotAllowed() =>
        new($"Could not add default node. Default node is not allowed here.");
    
    public static CommandBuilderException InvalidFlagShortcut(char shortcut) =>
        new($"Could not set flag shortcut. Shortcut '{shortcut}' is invalid.");
    
    public static CommandBuilderException InvalidFlagName(string flag) =>
        new($"Could not set flag name. Flag name '{flag}' is invalid.");
}