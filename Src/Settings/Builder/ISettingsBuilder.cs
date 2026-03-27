// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Settings.Builder.Delegate;

namespace NeoKolors.Settings.Builder;

public interface ISettingsBuilder<TResult> : ISettingsBuilder {
    
    /// <summary>
    /// Executes the current settings builder and returns the execution result.
    /// </summary>
    /// <returns>
    /// The result of the execution.
    /// </returns>
    public new TResult Execute();

    /// <summary>
    /// Executes the current settings builder and returns the execution result.
    /// </summary>
    /// <returns>
    /// The result of the execution.
    /// </returns>
    public new TResult Execute(Context context);

    /// <summary>
    /// Adds the nodes to the current settings builder using the provided supplier function.
    /// </summary>
    /// <param name="supplier">
    /// A supplier function that provides or modifies a collection of settings nodes.
    /// </param>
    /// <returns>
    /// The current settings builder instance with the modified nodes.
    /// </returns>
    public ISettingsBuilder<TResult> Nodes(SettingsNodeCollectionSupplier<TResult> supplier);
}

public interface ISettingsBuilder : ICloneable {

    /// <summary>
    /// Parses the current settings and returns the context containing the results.
    /// </summary>
    /// <returns>
    /// A <see cref="Context"/> containing the parsed settings.
    /// </returns>
    public Context Parse();

    /// <summary>
    /// Executes the current settings builder and returns the execution result.
    /// </summary>
    /// <returns>
    /// The result of the execution.
    /// </returns>
    public object? Execute();
    
    /// <summary>
    /// Executes the current settings builder and returns the execution result.
    /// </summary>
    /// <returns>
    /// The result of the execution.
    /// </returns>
    public object? Execute(Context context);
    
    /// <summary>
    /// Adds the nodes to the current settings builder using the provided supplier function.
    /// </summary>
    /// <param name="supplier">
    /// A supplier function that provides or modifies a collection of settings nodes.
    /// </param>
    /// <returns>
    /// The current settings builder instance with the modified nodes.
    /// </returns>
    public ISettingsBuilder Nodes(SettingsNodeCollectionSupplier supplier);

    /// <summary>
    /// Converts the current settings into an XSD (XML Schema Definition) representation.
    /// </summary>
    /// <returns>
    /// A string containing the XSD representation of the settings.
    /// </returns>
    public string ToXsd();
}