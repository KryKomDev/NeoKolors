//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Command;

public readonly struct SwitchInfo {
    
    /// <summary>
    /// Gets the name of the switch.
    /// </summary>
    /// <remarks>
    /// The name serves as the primary identifier for the switch in the command parser.
    /// </remarks>
    public string Name { get; }

    /// <summary>
    /// Gets the shortcut character for the switch.
    /// </summary>
    /// <remarks>
    /// The shortcut provides a single-character alias for the switch, allowing for concise command inputs.
    /// If the switch does not have a shortcut, this property is null.
    /// </remarks>
    public char? Shortcut { get; }

    /// <summary>
    /// Gets the description of the switch.
    /// </summary>
    /// <remarks>
    /// The description provides additional information about the purpose
    /// or functionality of the switch in the command parser.
    /// </remarks>
    public string Description { get; }
    
    public SwitchInfo(string name, string description = "", char? shortcut = null) {
        Name = name;
        Description = description;
        Shortcut = shortcut;
    }
    
    public override string ToString() => Shortcut == null ? $"--{Name}" : $"[--{Name} | -{Shortcut}]";
}