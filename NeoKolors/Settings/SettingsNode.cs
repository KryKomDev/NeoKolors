//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings;

public class SettingsNode<TResult> : ISettingsNode<TResult> where TResult : class {
    public string Name { get; }
    public Context Context { get; }
    public List<SettingsGroup> Groups { get; }
    public Func<Context, TResult>? ResultConstructor { get; private set; }

    private SettingsNode(string name, Func<Context, TResult>? resultConstructor = null) {
        Name = name;
        Groups = new List<SettingsGroup>();
        Context = new Context();
        ResultConstructor = resultConstructor;
    }

    /// <summary>
    /// creates a new instance of <see cref="SettingsNode{TResut}"/>
    /// </summary>
    public static SettingsNode<TResult> New(string name, Func<Context, TResult>? resultConstructor = null) {
        return new SettingsNode<TResult>(name, resultConstructor);
    }

    /// <summary>
    /// sets the function for creating the result object
    /// </summary>
    public SettingsNode<TResult> OnParse(Func<Context, TResult> onParse) {
        ResultConstructor = onParse;
        return this;
    }

    /// <summary>
    /// compiles the settings and returns the result
    /// </summary>
    public TResult GetResult() {
        Context merged = new Context(Context);

        foreach (var g in Groups) {
            g.MergeContext(merged);
        }

        if (ResultConstructor == null) {
            throw SettingsNodeException.NoResultConstructor(Name);
        }

        return ResultConstructor(merged);
    }

    /// <summary>
    /// adds a new argument to the node's context
    /// </summary>
    public ISettingsNode<TResult> Argument(string name, IArgument argument) {
        Context.Add(name, argument);
        return this;
    }

    /// <summary>
    /// adds a group to the node
    /// </summary>
    public ISettingsNode<TResult> Group(SettingsGroup group) {
        Groups.Add(group);
        return this;
    }

    /// <summary>
    /// returns a clone of the node
    /// </summary>
    public object Clone() {
        throw new NotImplementedException();
    }
}