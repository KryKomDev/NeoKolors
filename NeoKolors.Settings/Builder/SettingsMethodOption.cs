// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.CompilerServices;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Builder.Exception;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder;

public class SettingsMethodOption {
    
    private readonly HashSet<ArgumentInfo> _arguments = [];
    private ContextMerger? _merger;

    /// <summary>
    /// Adds an argument with the specified name to the settings method group.
    /// </summary>
    /// <param name="name">The name of the argument to add. Must be unique within the settings group.</param>
    /// <param name="argument">The argument object implementing <see cref="IArgument"/> to associate with the provided name.</param>
    /// <returns>An instance of <see cref="SettingsMethodOption"/> to allow method chaining.</returns>
    /// <exception cref="SettingsBuilderException">Thrown when an argument with the same name already exists in the settings group.</exception>
    public SettingsMethodOption Argument(string name, IArgument argument) {
        if (_arguments.Add(new ArgumentInfo(name, argument))) return this;
        throw SettingsBuilderException.DuplicateElement(name);
    }

    /// <summary>
    /// Assigns a custom context merger implementation to the settings method option.
    /// </summary>
    /// <param name="merger">A delegate of type <see cref="ContextMerger"/> to define how contexts should be merged.</param>
    /// <returns>An instance of <see cref="SettingsMethodOption"/> to allow method chaining.</returns>
    public SettingsMethodOption OnMerge(ContextMerger merger) {
        _merger = merger;
        return this;
    }

    /// <summary>
    /// Automatically sets up the default context merging logic for the settings method option.
    /// </summary>
    /// <returns>An instance of <see cref="SettingsMethodOption"/> to allow method chaining.</returns>
    public SettingsMethodOption AutoMerges() {
        _merger = (in Context c, in Context _) => {
            c.Set(new Context(_arguments)); 
        };
        return this;
    }

    /// <summary>
    /// Merges the current context into the specified <see cref="Context"/> using the configured context merger.
    /// </summary>
    /// <param name="c">The <see cref="Context"/> instance to merge into.</param>
    /// <exception cref="SettingsExecutionException">Thrown if no context merger is configured.</exception>
    public void MergeTo(in Context c) {
        if (_merger is null)
            throw SettingsExecutionException.NoContextMerger();
        _merger(c, GetContext());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Context GetContext() => new(_arguments);

    /// <summary>
    /// Gets an array of <see cref="ArgumentInfo"/> objects that represent the arguments
    /// associated with the current settings method option.
    /// </summary>
    /// <value>
    /// An array of <see cref="ArgumentInfo"/> containing the details of the arguments
    /// added to the settings method option.
    /// </value>
    public ArgumentInfo[] Arguments {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _arguments.ToArray();
    }
}