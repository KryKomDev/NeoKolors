//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument;

namespace NeoKolors.Settings.Command;

public readonly struct ArgumentInfo {
    
    /// <summary>
    /// Specifies the name of the command-line flag. This value is typically used as an identifier
    /// for the flag when parsing command-line arguments.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Provides a human-readable description of the purpose or functionality
    /// of the command-line flag. This value is intended to be displayed in usage
    /// instructions or help documentation.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Represents an argument associated with a command-line flag. This argument encapsulates
    /// logic for parsing, setting, and retrieving values from a string input. It is used
    /// to store and manipulate the value linked to a specific flag.
    /// </summary>
    public IParsableArgument Argument { get; }

    public ArgumentInfo(string name, string description, IParsableArgument argument) {
        Name = name;
        Description = description;
        Argument = argument;
    }
    
    public override string ToString() => 
        $"<{Name}: {Argument.GetType().GetDisplayName()}>";
}