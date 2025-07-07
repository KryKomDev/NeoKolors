// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.CompilerServices;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Collection;
using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Builder.Exception;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder;

public class SettingsNode<TResult> : ISettingsNode<TResult> {
    
    // private fields
    private SettingsResultConstructor<TResult>? _resultConstructor;
    private readonly SettingsElementCollection _elements = new();

    /// <summary>
    /// Gets an array of <see cref="SettingsElementInfo"/> objects representing the elements
    /// within the current settings node.
    /// </summary>
    /// <remarks>
    /// Each element in the collection is defined by a name and its associated metadata,
    /// such as argument or method group information. This property provides a snapshot
    /// of the elements available in the settings node at the time of access.
    /// </remarks>
    public SettingsElementInfo[] Elements => _elements.Elements;

    /// <summary>
    /// Represents a settings node builder that organizes settings elements and
    /// allows the configuration of settings such as arguments, groups, and result constructors.
    /// </summary>
    /// <typeparam name="TResult">The type of the result the settings node will produce.</typeparam>
    public SettingsNode(SettingsNodeSupplier<TResult> supplier) => 
        supplier(this);

    public SettingsNode() { }
    
    public ISettingsNode<TResult> Argument(string name, IArgument argument) {
        _elements.Argument(name, argument);
        return this;
    }

    public ISettingsNode<TResult> Group(string name, SettingsMethodGroupSupplier group) {
        _elements.Group(name, group);
        return this;
    }

    public ISettingsNode<TResult> Constructs(SettingsResultConstructor<TResult> resultConstructor) {
        _resultConstructor = resultConstructor;
        return this;
    }

    public ISettingsNode Constructs(SettingsResultConstructor resultConstructor) {
        _resultConstructor = (in Context context) => {
            var res = resultConstructor(context);
            if (res is TResult result)
                return result;
            throw SettingsExecutionException.InvalidResultType();
        };
        
        return this;
    }
    
    public Context Parse() {
        Context context = [];

        foreach (var e in _elements) {
            if (e.IsArgument)
                context.Add(e.Name, e.AsArgument);
            else
                e.AsGroup.MergeTo(context);
        }
        
        return context;
    }

    public TResult Execute(Context context) {
        if (_resultConstructor is null) 
            throw SettingsExecutionException.NoResultConstructor();
        return _resultConstructor(context);
    }

    public TResult Execute() {
        var c = Parse();
        return Execute(c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? ISettingsNode.Execute() => Execute();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? ISettingsNode.Execute(Context context) => Execute(context);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ISettingsNode ISettingsNode.Group(string name, SettingsMethodGroupSupplier group) => Group(name, group);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ISettingsNode ISettingsNode.Argument(string name, IArgument argument) => Argument(name, argument);
}