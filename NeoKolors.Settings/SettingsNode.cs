//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.ArgumentTypes;
using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings;

public class SettingsNode<TResult> : ISettingsNode<TResult> where TResult : class {
    
    public string Name { get; }
    public Context Context { get; }
    public List<SettingsGroup> Groups { get; }
    public Func<Context, TResult>? ResultConstructor { get; private set; }

    public SettingsGroup this[string name] {
        get {
            foreach (var g in Groups) {
                if (g.Name == name) return g;
            }
            
            throw SettingsNodeException.InvalidGroupName(Name, name);
        }
    } 
    
    private SettingsNode(string name) {
        Name = name;
        Groups = new List<SettingsGroup>();
        Context = new Context();
    }

    /// <summary>
    /// creates a new instance of <see cref="SettingsNode{TResut}"/>
    /// </summary>
    public static SettingsNode<TResult> New(string name) {
        return new SettingsNode<TResult>(name);
    }

    /// <summary>
    /// sets the function for creating the result object
    /// </summary>
    public ISettingsNode<TResult> Constructs(Func<Context, TResult> onParse) {
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
    
    /// <seealso cref="Argument"/>
    ISettingsNode ISettingsNode.Argument(string name, IArgument argument) => Argument(name, argument);

    /// <seealso cref="Group"/>
    ISettingsNode ISettingsNode.Group(SettingsGroup group) => Group(group);

    /// <summary>
    /// sets the constructor of the result
    /// </summary>
    /// <exception cref="SettingsNodeException">type checking failed</exception>
    /// <seealso cref="Constructs(System.Func{NeoKolors.Settings.Context,TResult})"/>
    public ISettingsNode Constructs(Func<Context, object> resultConstructor) {
        Type? resultType = resultConstructor.GetType().GetMethod("Invoke")?.ReturnType;

        if (resultType is null) {
            throw SettingsNodeException.InvalidResultType(Name, typeof(TResult));
        }
        
        if (resultType == typeof(TResult)) {
            ResultConstructor = (Func<Context, TResult>?)resultConstructor;
        }
        else {
            throw SettingsNodeException.InvalidResultType(Name, typeof(TResult), resultType);
        }
        
        return this;
    }

    /// <seealso cref="GetResult"/>
    object ISettingsNode.GetResult() => GetResult();
}