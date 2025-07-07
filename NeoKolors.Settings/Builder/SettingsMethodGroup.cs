// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;
using System.Runtime.CompilerServices;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Builder.Exception;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder;

/// <summary>
/// Represents a group of settings options that can be managed and accessed collectively.
/// </summary>
/// <remarks>
/// A <c>SettingsGroup</c> allows users to define and manage a collection of options. Each option is uniquely identified by its
/// name and can be added using the <c>Option</c> method. The group supports operations such as retrieving the full list of
/// options, accessing a specific option by name, or generating a choice argument from the existing group.
/// </remarks>
public class SettingsMethodGroup : IEnumerable<SettingsMethodOptionInfo> {
    
    private readonly HashSet<SettingsMethodOptionInfo> _options = [];
    private SettingsMethodChoiceArgument? _choiceArgument;
    private readonly HashSet<ArgumentInfo> _arguments = [];

    /// <summary>
    /// Adds a new option to the settings group using the specified name and supplier.
    /// </summary>
    /// <param name="name">
    /// The unique name of the option to be added to the settings group.
    /// </param>
    /// <param name="option">
    /// The supplier function that provides the logic for creating the <c>SettingsGroupOption</c>.
    /// </param>
    /// <returns>
    /// The current instance of <c>SettingsGroup</c>, allowing for method chaining.
    /// </returns>
    /// <exception cref="SettingsBuilderException">
    /// Thrown when an option with the specified name already exists in the settings group.
    /// </exception>
    public SettingsMethodGroup Option(string name, SettingsMethodOptionSupplier option) {
        if (_options.Add(new SettingsMethodOptionInfo(name, option(new SettingsMethodOption())))) return this;
        throw SettingsBuilderException.DuplicateOption(name);
    }

    /// <summary>
    /// Gets an array of all options contained in the <see cref="SettingsMethodGroup"/>.
    /// </summary>
    /// <remarks>
    /// This property provides a snapshot of the current options in the group. Each option is represented as a
    /// <see cref="SettingsMethodOptionInfo"/> object which contains details such as the name and the associated action or setting.
    /// </remarks>
    /// <value>
    /// An array of <see cref="SettingsMethodOptionInfo"/> representing all options in the current <see cref="SettingsMethodGroup"/>.
    /// </value>
    /// <exception cref="SettingsExecutionException">
    /// Thrown by indexer if an option with the specified name does not exist in this group.
    /// </exception>
    public SettingsMethodOptionInfo[] Options => _options.ToArray();
    
    public SettingsMethodOptionInfo this[string name] {
        get {
            foreach (var o in Options) if (o.Name == name) return o;
            throw SettingsExecutionException.InvalidOptionName(name);
        }
    }

    /// <summary>
    /// Creates a choice argument based on the options defined in the current <c>SettingsMethodGroup</c>.
    /// </summary>
    /// <returns>
    /// An instance of <c>SettingsOptionChoiceArgument</c> that allows selecting a single option from the group.
    /// </returns>
    public SettingsMethodChoiceArgument GetChoiceArgument() {
        _choiceArgument = SettingsMethodChoiceArgument.FromGroup(this);
        return _choiceArgument;
    }

    /// <summary>
    /// Adds a new argument to the settings group with the specified name, argument implementation, and optional description.
    /// These arguments are automatically added to the node context on parsing. 
    /// </summary>
    /// <param name="name">
    /// The unique name that identifies the argument to be added.
    /// </param>
    /// <param name="argument">
    /// An implementation of the <c>IArgument</c> interface representing the argument's behavior and state.
    /// </param>
    /// <param name="description">
    /// An optional description providing details about the argument.
    /// </param>
    /// <returns>
    /// The current instance of <c>SettingsMethodGroup</c>, allowing for method chaining.
    /// </returns>
    /// <exception cref="SettingsBuilderException">
    /// Thrown when an argument with the specified name already exists in the settings group.
    /// </exception>
    public SettingsMethodGroup Argument(string name, IArgument argument, string? description = null) {
        if (_arguments.Add(new ArgumentInfo(name, argument, description))) return this;
        throw SettingsBuilderException.DuplicateElement(name);
    }

    /// <summary>
    /// Merges the state of the selected option within the settings group into the provided context.
    /// </summary>
    /// <param name="target">
    /// The context into which the state of the selected option will be merged. This context is provided as an input parameter.
    /// </param>
    /// <exception cref="SettingsExecutionException">
    /// Thrown if no option has been selected before attempting to perform the merge operation.
    /// </exception>
    public void MergeTo(in Context target) {
        if (_choiceArgument is null)
            throw SettingsExecutionException.NoOptionSelected();
        
        // select the option
        var option = this[_choiceArgument.Value];
        
        // create context from the arguments
        var groupContext = new Context(_arguments);
        
        // merge the option into the group context
        option.Option.MergeTo(groupContext);
        
        // add to output context
        target.Add(groupContext);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<SettingsMethodOptionInfo> GetEnumerator() => _options.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}