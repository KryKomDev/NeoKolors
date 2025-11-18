//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;
using NeoKolors.Settings.Command.Exception;

namespace NeoKolors.Settings.Command;

public readonly struct NodeInfo {
    
    /// <summary>
    /// Gets the name of the node, which serves as an identifier within the command parsing structure.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the command node associated with this instance, which represents a specific part
    /// of the command parsing structure and allows interaction with its configuration and behavior.
    /// </summary>
    public ICommandNode Node { get; }
    
    public NodeInfo(string name, ICommandNode node) {
        if (!name.IsValidIdentifier()) 
            throw CommandBuilderException.InvalidNodeName();
        
        Name = name;
        Node = node;
    }
    
    public override string ToString() => Name;
}