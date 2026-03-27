// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder;

public interface ISettingsNode<TResult> : ISettingsNode {
    
    /// <summary>
    /// Parses the current settings node and uses the context to execute it.
    /// </summary>
    /// <returns>
    /// An object representing the result of the execution, derived from the current settings and arguments.
    /// </returns>
    public new TResult Execute() {
        var context = Parse();
        return Execute(context);
    }
    
    /// <summary>
    /// Executes the current settings node, using the parsed context,
    /// and produces a result based on the configured settings and arguments.
    /// </summary>
    /// <returns>
    /// An object representing the result of the execution, derived from the current settings and arguments.
    /// </returns>
    public new TResult Execute(Context context);
    
    /// <summary>
    /// Adds an argument to the current settings node with the specified name and argument configuration.
    /// </summary>
    /// <param name="name">
    /// The name of the argument to be added to the settings node.
    /// </param>
    /// <param name="argument">
    /// The argument configuration to associate with the specified name.
    /// </param>
    /// <returns>
    /// An <see cref="ISettingsNode"/> instance representing the updated settings node with the new argument.
    /// </returns>
    public new ISettingsNode<TResult> Argument(string name, IArgument argument);
    
    /// <summary>
    /// Adds a group of settings to the current node using the specified name and group supplier function.
    /// </summary>
    /// <param name="name">The name of the settings group to add.</param>
    /// <param name="group">A delegate that provides the settings group to be added.</param>
    /// <returns>
    /// Returns the current <see cref="ISettingsNode"/> instance for method chaining.
    /// </returns>
    public new ISettingsNode<TResult> Group(string name, SettingsMethodGroupSupplier group);
    
    /// <summary>
    /// Configures the current settings node with a constructor function that generates a result
    /// based on a provided context.
    /// </summary>
    /// <param name="resultConstructor">
    /// A delegate that defines the logic for constructing a result object using the given <see cref="Context"/>.
    /// </param>
    /// <returns>
    /// An <see cref="ISettingsNode"/> instance representing the updated settings node
    /// configured with the specified result constructor.
    /// </returns>
    public ISettingsNode<TResult> Constructs(SettingsResultConstructor<TResult> resultConstructor);
}

public interface ISettingsNode {
    
    /// <summary>
    /// Gets the collection of settings elements defined within the current settings node.
    /// </summary>
    /// <remarks>
    /// This property provides access to a collection of <see cref="ISettingsElementInfo"/> instances,
    /// representing the individual arguments, groups, or configurations associated
    /// with the current settings node.
    /// </remarks>
    public ISettingsElementInfo[] Elements { get; }
    
    /// <summary>
    /// Parses the current settings node and generates a context representing
    /// the arguments and configurations defined in the node.
    /// </summary>
    /// <returns>
    /// A <see cref="Context"/> instance containing the parsed arguments and configurations.
    /// </returns>
    public Context Parse();

    /// <summary>
    /// Parses the current settings node and uses the context to execute it.
    /// </summary>
    /// <returns>
    /// An object representing the result of the execution, derived from the current settings and arguments.
    /// </returns>
    public object? Execute() {
        var context = Parse();
        return Execute(context);
    }

    /// <summary>
    /// Executes the current settings node, using the parsed context,
    /// and produces a result based on the configured settings and arguments.
    /// </summary>
    /// <returns>
    /// An object representing the result of the execution, derived from the current settings and arguments.
    /// </returns>
    public object? Execute(Context context);

    /// <summary>
    /// Adds an argument to the current settings node with the specified name and argument configuration.
    /// </summary>
    /// <param name="name">
    /// The name of the argument to be added to the settings node.
    /// </param>
    /// <param name="argument">
    /// The argument configuration to associate with the specified name.
    /// </param>
    /// <returns>
    /// An <see cref="ISettingsNode"/> instance representing the updated settings node with the new argument.
    /// </returns>
    public ISettingsNode Argument(string name, IArgument argument);

    /// <summary>
    /// Adds a group of settings to the current node using the specified name and group supplier function.
    /// </summary>
    /// <param name="name">The name of the settings group to add.</param>
    /// <param name="group">A delegate that provides the settings group to be added.</param>
    /// <returns>
    /// Returns the current <see cref="ISettingsNode"/> instance for method chaining.
    /// </returns>
    public ISettingsNode Group(string name, SettingsMethodGroupSupplier group);

    /// <summary>
    /// Configures the current settings node with a constructor function that generates a result
    /// based on a provided context.
    /// </summary>
    /// <param name="resultConstructor">
    /// A delegate that defines the logic for constructing a result object using the given <see cref="Context"/>.
    /// </param>
    /// <returns>
    /// An <see cref="ISettingsNode"/> instance representing the updated settings node
    /// configured with the specified result constructor.
    /// </returns>
    public ISettingsNode Constructs(SettingsResultConstructor resultConstructor);
}