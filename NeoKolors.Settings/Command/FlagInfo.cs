//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Command.Exception;

namespace NeoKolors.Settings.Command;

/// <summary>
/// Represents information about a command-line flag, including its name, description,
/// whether it is required, and its associated argument.
/// </summary>
public readonly struct FlagInfo {
    
    /// <summary>
    /// Specifies the name of the command-line flag. This value is typically used as an identifier
    /// for the flag when parsing command-line arguments.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Specifies an optional single-character shortcut for the command-line flag.
    /// This allows the flag to be identified and used with a shorter alias
    /// in addition to its full name when parsing command-line arguments.
    /// </summary>
    public char? Shortcut { get; }
    
    /// <summary>
    /// Represents an argument associated with a command-line flag. This argument encapsulates
    /// logic for parsing, setting, and retrieving values from a string input. It is used
    /// to store and manipulate the value linked to a specific flag.
    /// </summary>
    public IParsableArgument Argument { get; }

    /// <summary>
    /// Provides a human-readable description of the purpose or functionality
    /// of the command-line flag. This value is intended to be displayed in usage
    /// instructions or help documentation.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Indicates whether the command-line flag is mandatory. If set to true, the flag must be
    /// provided when the application runs; otherwise, the absence of this flag may cause
    /// a parsing error or trigger invalid usage feedback.
    /// </summary>
    public bool IsRequired { get; }
    
    /// <summary>
    /// Represents information regarding a single command-line flag. This includes the flag's
    /// name, description, whether it is required, and its associated argument.
    /// </summary>
    public FlagInfo(string name,
        IParsableArgument argument,
        string description = "",
        bool isRequired = false,
        char? shortcut = null) 
    {
        Name = name;
        if (!Name.IsValidIdentifier())
            throw CommandBuilderException.InvalidFlagName(Name);
        
        Shortcut = shortcut;
        if (Shortcut is not null && !char.IsLetterOrDigit(Shortcut ?? ' '))
            throw CommandBuilderException.InvalidFlagShortcut(Shortcut ?? ' ');

        IsRequired = isRequired;
        Argument = argument;
        Description = description;
    }
    
    public override string ToString() {
        string flag = Shortcut == null ? $"--{Name}" : $"--{Name} | -{Shortcut}";
        return $"{(IsRequired ? "" : "[")}{flag} <{Name}: {Argument.GetType().GetDisplayName()}>{(IsRequired ? "" : "]")}";
    }
}