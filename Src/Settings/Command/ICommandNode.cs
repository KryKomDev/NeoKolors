//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Delegate;

namespace NeoKolors.Settings.Command;

/// <summary>
/// Represents a node in a command parsing structure. This interface is responsible
/// for configuring components such as parameters, flags, and switches, and performing
/// parsing of input to extract meaningful data or execute specific actions.
/// </summary>
public interface ICommandNode<TResult> : ICommandNode {
    
    /// <summary>
    /// Executes the defined action based on the provided context, which contains the necessary arguments
    /// and state for the command node.
    /// </summary>
    /// <param name="context">
    /// The context object containing the arguments and state required to execute the action.
    /// </param>
    /// <returns>
    /// An object representing the result of the executed action.
    /// </returns>
    public new TResult Execute(Context context);

    /// <summary>
    /// Parses the provided input to determine its corresponding result or extract meaningful data
    /// and executes the defined action based on the provided context, which contains the necessary arguments.
    /// </summary>
    /// <param name="input">The input string to be parsed.</param>
    /// <returns>An object representing the result of the parsing operation.</returns>
    public new TResult Execute(string input) {
        var context = Parse(input);
        return Execute(context);
    }
    
    /// <summary>
    /// Configures the command node to execute a specified action upon parsing the associated input.
    /// </summary>
    /// <param name="action">
    /// A function that defines the action to be executed, which takes the context as input and returns a result.
    /// </param>
    /// <returns>
    /// The current instance of the command node, allowing further configurations or chaining.
    /// </returns>
    public ICommandNode<TResult> Executes(ResultBuilder<TResult> action);

    /// <summary>
    /// Configures a nested structure of command nodes using the supplied delegate, allowing
    /// for the definition of hierarchical command parsing and execution.
    /// </summary>
    /// <param name="supplier">
    /// A delegate responsible for providing a collection of command nodes to be included within
    /// the current node's structure. The supplier typically applies custom logic to configure
    /// the node relationships.
    /// </param>
    public ICommandNode<TResult> Nodes(CommandNodeCollectionSupplier<TResult> supplier);

    /// <inheritdoc cref="ICommandNode.Arg(string, IParsableArgument)"/>
    public new ICommandNode<TResult> Arg(string name, IParsableArgument argument);

    /// <inheritdoc cref="ICommandNode.Arg(ArgumentInfo)"/>
    public new ICommandNode<TResult> Arg(ArgumentInfo argumentInfo);
    
    /// <inheritdoc cref="ICommandNode.Flag(string, IParsableArgument)"/>
    public new ICommandNode<TResult> Flag(string name, IParsableArgument argument);
    
    /// <inheritdoc cref="ICommandNode.Flag(FlagInfo)"/>
    public new ICommandNode<TResult> Flag(FlagInfo flagInfo);
    
    /// <inheritdoc cref="ICommandNode.Switch(string)"/>
    public new ICommandNode<TResult> Switch(string name);
    
    /// <inheritdoc cref="ICommandNode.Switch(SwitchInfo)"/>
    public new ICommandNode<TResult> Switch(SwitchInfo switchInfo);
}

/// <summary>
/// Represents a node in a command parsing structure. This interface is responsible
/// for configuring components such as parameters, flags, and switches, and performing
/// parsing of input to extract meaningful data or execute specific actions.
/// </summary>
public interface ICommandNode {
    
    /// <summary>
    /// Parses the provided input to determine its corresponding result or extract meaningful data
    /// based on the implementation of the ICommandNode interface.
    /// </summary>
    /// <param name="input">The input string to be parsed.</param>
    /// <returns>An object representing the result of the parsing operation.</returns>
    public Context Parse(string input);

    /// <summary>
    /// Executes the defined action based on the provided context, which contains the necessary arguments
    /// and state for the command node.
    /// </summary>
    /// <param name="context">
    /// The context object containing the arguments and state required to execute the action.
    /// </param>
    /// <returns>
    /// An object representing the result of the executed action.
    /// </returns>
    public object? Execute(Context context);

    /// <summary>
    /// Parses the provided input to determine its corresponding result or extract meaningful data
    /// and executes the defined action based on the provided context, which contains the necessary arguments.
    /// </summary>
    /// <param name="input">The input string to be parsed.</param>
    /// <returns>An object representing the result of the parsing operation.</returns>
    public object? Execute(string input) {
        var context = Parse(input);
        return Execute(context);
    }

    /// <summary>
    /// Configures a nested structure of command nodes using the supplied delegate, allowing
    /// for the definition of hierarchical command parsing and execution.
    /// </summary>
    /// <param name="supplier">
    /// A delegate responsible for providing a collection of command nodes to be included within
    /// the current node's structure. The supplier typically applies custom logic to configure
    /// the node relationships.
    /// </param>
    public ICommandNode Nodes(CommandNodeCollectionSupplier supplier);

    /// <summary>
    /// Adds a parameter to the command node with a specified argument and name.
    /// </summary>
    /// <param name="argument">The argument object defining the parameter to be added.</param>
    /// <param name="name">The name of the parameter to be added.</param>
    public ICommandNode Arg(string name, IParsableArgument argument);

    /// <summary>
    /// Adds an argument to the command node with the specified argument information.
    /// </summary>
    /// <param name="argumentInfo">
    /// The information describing the argument, including its name, description, and type.
    /// </param>
    public ICommandNode Arg(ArgumentInfo argumentInfo);

    /// <summary>
    /// Adds a flag to the command node with a specified argument and name.
    /// </summary>
    /// <param name="argument">The argument object that defines the flag to be added.</param>
    /// <param name="name">The name of the flag to be added.</param>
    public ICommandNode Flag(string name, IParsableArgument argument);

    /// <summary>
    /// Configures a flag for the command node using the provided argument information.
    /// </summary>
    /// <param name="flagInfo">
    /// The argument information that defines the name, description, and argument behavior for the flag.
    /// </param>
    public ICommandNode Flag(FlagInfo flagInfo);

    /// <summary>
    /// Adds a switch to the command node with the specified name. A switch is a binary
    /// option that generally controls a feature or behavior.
    /// </summary>
    /// <param name="name">The name of the switch to be added.</param>
    public ICommandNode Switch(string name);

    /// <summary>
    /// Adds a switch node with the specified name to the command parsing structure.
    /// A switch represents a true/false toggle that can be included in command input.
    /// </summary>
    /// <param name="switchInfo">
    /// The name of the switch to be added. It serves as the unique identifier
    /// for the switch in the command parsing structure.
    /// </param>
    public ICommandNode Switch(SwitchInfo switchInfo);

    /// <summary>
    /// Configures the command node to execute a specified action upon parsing the associated input.
    /// </summary>
    /// <param name="action">
    /// A function that defines the action to be executed, which takes the context as input and returns a result.
    /// </param>
    /// <returns>
    /// The current instance of the command node, allowing further configurations or chaining.
    /// </returns>
    public ICommandNode Executes(ResultBuilder action);

    /// <summary>
    /// Retrieves the syntax configuration of the command node, including its parameters,
    /// flags, and switches, as defined by the command structure.
    /// </summary>
    /// <returns>
    /// A <see cref="CommandSyntax"/> object representing the detailed structure and components
    /// of the command node.
    /// </returns>
    public CommandSyntax GetSyntax();

    /// <summary>
    /// Retrieves the usage details of the current command node, including information
    /// about its parameters, flags, switches, and associated actions.
    /// </summary>
    /// <returns>
    /// A <see cref="Usage"/> object containing the usage details of the command node.
    /// </returns> 
    public Usage GetUsage();
}