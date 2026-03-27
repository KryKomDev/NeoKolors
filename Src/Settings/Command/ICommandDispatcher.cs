//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Command;

/// <summary>
/// Represents a builder for creating and configuring command structures for parsing input.
/// Provides methods to define command nodes and parse input into a specific result type.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the parsing operation.</typeparam>
public interface ICommandDispatcher<TResult> : ICommandDispatcher {
    
    /// <summary>
    /// Executes a command based on the provided input string and returns the result of the operation.
    /// </summary>
    /// <param name="input">The input string containing the command to be executed.</param>
    /// <returns>The result of the execution, represented as an object of type TResult.</returns>
    public new TResult Execute(string input) {
        var c = Parse(input);
        return Execute(c);   
    }

    /// <summary>
    /// Executes a command using the specified input and returns the result of the operation.
    /// </summary>
    /// <param name="context">The input string to be processed for execution.</param>
    /// <returns>The result of the execution, represented as an object of type TResult.</returns>
    public new TResult Execute(Context context);

    /// <summary>
    /// Adds a command node to the command builder. The node represents a specific
    /// component or functionality in the command parsing structure and defines how
    /// input related to it should be processed.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="supplier">Supplies the command node to be added. This node encapsulates
    /// specific logic for parsing input, managing arguments, flags, or switches.</param>
    public ICommandDispatcher<TResult> Register(string name, CommandNodeSupplier<TResult> supplier);
}

/// <summary>
/// Represents a dispatcher responsible for parsing and executing commands.
/// Provides methods for defining command nodes, parsing commands, and executing them within a specific context.
/// </summary>
public interface ICommandDispatcher {

    /// <summary>
    /// Parses the provided input string and constructs a context object representing the command.
    /// </summary>
    /// <param name="input">The input string to be parsed into a command context.</param>
    /// <returns>A context object that encapsulates the parsed command and its related arguments.</returns>
    public Context Parse(string input);

    /// <summary>
    /// Executes a command based on the input string and returns the resulting outcome.
    /// This method first parses the input into a context and then executes the command using the parsed context.
    /// </summary>
    /// <param name="input">The input string representing the command to be executed.</param>
    /// <returns>
    /// The result of executing the command. The return type is implementation-specific and may vary depending on the command.
    /// </returns>
    public object? Execute(string input) {
        var c = Parse(input);
        return Execute(c);
    }

    /// <summary>
    /// Executes a command based on the provided input string by parsing it
    /// into a context and then executing the resulting context.
    /// </summary>
    /// <param name="context">The context to be processed and executed as a command.</param>
    /// <returns>
    /// The result of the command execution. The type of the returned object is dependent on the specific command implementation.
    /// </returns>
    public object? Execute(Context context);

    /// <summary>
    /// Adds a command node to the command builder. The node represents a specific
    /// component or functionality in the command parsing structure and defines how
    /// input related to it should be processed.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="supplier">Supplies the command node to be added. This node encapsulates
    /// specific logic for parsing input, managing arguments, flags, or switches.</param>
    public ICommandDispatcher Register(string name, CommandNodeSupplier supplier);
    
    /// <summary>
    /// Retrieves the usage details of the commands, including information
    /// about its parameters, flags, switches, and associated actions.
    /// </summary>
    /// <returns>
    /// A <see cref="Usage"/> object containing the usage details of the command node.
    /// </returns>
    public Usage[] GetUsages();
}