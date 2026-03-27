//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Collections;
using System.Runtime.CompilerServices;
using NeoKolors.Settings.Command.Exception;

namespace NeoKolors.Settings.Command;

public class CommandNodeCollection<TResult> : CommandNodeCollection {
    
    public CommandNodeCollection(bool allowDefaultNode) : base(allowDefaultNode) { }
    public CommandNodeCollection() : this(true) { }
    
    /// <summary>
    /// Adds a new command node to the collection with the specified name and supplier.
    /// </summary>
    /// <param name="name">The name of the command node to be added.</param>
    /// <param name="supplier">A supplier function that creates the command node.</param>
    /// <exception cref="CommandBuilderException">
    /// Thrown if a node with the given name already exists or if a default node already exists when adding a node with an empty name.
    /// </exception>
    public CommandNodeCollection<TResult> Node(string name, CommandNodeSupplier<TResult> supplier) {
        if (!_allowDefaultNode && name is "") throw CommandBuilderException.DefaultNotAllowed();
        if (_nodes.TryAdd(name, new NodeInfo(name, supplier(new CommandNode<TResult>())))) return this;
        if (name is "") {
            throw CommandBuilderException.DuplicateDefaultNode();
        }
        else {
            throw CommandBuilderException.DuplicateNode(name);
        }
    }

    /// <summary>
    /// Adds a new command node to the collection with the specified name and supplier.
    /// </summary>
    /// <param name="info">The information about the command node to be added.</param>
    /// <exception cref="CommandBuilderException">
    /// Thrown if a node with the given name already exists or
    /// if a default node already exists when adding a node with an empty name or
    /// if the node is not of type <see cref="ICommandNode{TResult}"/>.
    /// </exception>
    public new CommandNodeCollection Node(NodeInfo info) {
        if (!_allowDefaultNode && info.Name is "") throw CommandBuilderException.DefaultNotAllowed();
        if (info.Node is not ICommandNode<TResult>) 
            throw new CommandBuilderException("Invalid node type.");
        
        return base.Node(info);
    }
}

public class CommandNodeCollection : IEnumerable<NodeInfo> {
    
    private protected readonly Dictionary<string, NodeInfo> _nodes = [];
    private protected readonly bool _allowDefaultNode;
    
    public bool AllowDefaultNode => _allowDefaultNode;

    public NodeInfo[] Nodes => _nodes.Values.ToArray();

    /// <summary>
    /// Represents a collection of command nodes, allowing for the addition and retrieval of nodes by name.
    /// </summary>
    /// <param name="allowDefaultNode">
    /// Specifies whether a default node is allowed to be added to the collection.
    /// </param>
    public CommandNodeCollection(bool allowDefaultNode) {
        _allowDefaultNode = allowDefaultNode;
    }
    
    public CommandNodeCollection() : this(true) { }
    
    /// <summary>
    /// Adds a new command node to the collection with the specified name and supplier.
    /// </summary>
    /// <param name="name">The name of the command node to be added.</param>
    /// <param name="supplier">A supplier function that creates the command node.</param>
    /// <exception cref="CommandBuilderException">
    /// Thrown if a node with the given name already exists or if a default node already exists when adding a node with an empty name.
    /// </exception>
    public CommandNodeCollection Node(string name, CommandNodeSupplier supplier) {
        if (!_allowDefaultNode && name is "") throw CommandBuilderException.DefaultNotAllowed();
        if (_nodes.TryAdd(name, new NodeInfo(name, supplier(new CommandNode<object>())))) return this;
        if (name is "")
            throw CommandBuilderException.DuplicateDefaultNode();
        else
            throw CommandBuilderException.DuplicateNode(name);
    }

    /// <summary>
    /// Adds a new command node to the collection with the specified name and supplier.
    /// </summary>
    /// <param name="info">The information about the command node to be added.</param>
    /// <exception cref="CommandBuilderException">
    /// Thrown if a node with the given name already exists or if a default node already exists when adding a node with an empty name.
    /// </exception>
    public CommandNodeCollection Node(NodeInfo info) {
        if (!_allowDefaultNode && info.Name is "") throw CommandBuilderException.DefaultNotAllowed();
        if (_nodes.TryAdd(info.Name, info)) return this;
        if (info.Name is "")
            throw CommandBuilderException.DuplicateDefaultNode();
        else
            throw CommandBuilderException.DuplicateNode(info.Name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<NodeInfo> GetEnumerator() => _nodes.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}