//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Argument;
using ContextException = NeoKolors.Settings.Exceptions.ContextException;

namespace NeoKolors.Settings;

/// <summary>
/// Context Struct <br/>
/// provides context, argument types and values to settings builders
/// </summary>
public sealed class Context : ICloneable {
    
    private readonly Dictionary<string, IArgument> arguments = new();

    /// <summary>
    /// returns the argument assigned to the supplied key
    /// </summary>
    /// <exception cref="ContextException">key was not found</exception>
    public IArgument this[string key] {
        get {
            try {
                return arguments[key];
            }
            catch (KeyNotFoundException) {
                throw ContextException.KeyNotFound(key);
            }
        }
    }

    /// <summary>
    /// returns the argument at a supplied position
    /// </summary>
    /// <exception cref="ContextException">index out of range</exception>
    public IArgument this[int index] => GetAtIndex(index).value;

    /// <summary>
    /// creates a context with arguments
    /// </summary>
    /// <param name="arguments">key-value pairs of names and arguments</param>
    /// <exception cref="ContextException">an argument with the same name already exists</exception>
    public Context(params (string name, IArgument argument)[] arguments) => Add(arguments);

    /// <summary>
    /// creates a context with arguments from other contexts
    /// </summary>
    /// <exception cref="ContextException">an argument with the same name already exists</exception>
    public Context(params Context[] contexts) {
        foreach (var c in contexts) {
            Add(c);
        }
    }
    
    /// <summary>
    /// creates an empty context
    /// </summary>
    public Context() { }
    
    /// <summary>
    /// adds arguments to the context
    /// </summary>
    /// <param name="arguments">key-value pairs of names and arguments</param>
    public void Add((string name, IArgument argument)[] arguments) {
        foreach ((string name, IArgument argument) a in arguments) {
            Add(a.name, a.argument);
        }
    }

    /// <summary>
    /// adds an argument to the context
    /// </summary>
    /// <exception cref="ContextException">an argument with the same name already exists</exception>
    public void Add(string name, IArgument argument) {
        if (!arguments.TryAdd(name, argument)) throw ContextException.KeyDuplicate(name);
    }
    
    /// <summary>
    /// adds arguments from another context to the context
    /// </summary>
    /// <exception cref="ContextException">an argument with the same name already exists</exception>
    public void Add(Context context) {
        foreach ((string key, IArgument value) in context.arguments) {
            Add(key, value);
        }
    }

    /// <summary>
    /// sets an argument with the supplied name
    /// </summary>
    /// <param name="name">name of the argument</param>
    /// <param name="argument">the new argument</param>
    /// <param name="allowAddNew">
    /// if enabled allows automatically creating a new argument if it doesn't already exist,
    /// else throws <see cref="ContextException"/> if an argument with the same name doesn't exist
    /// </param>
    /// <exception cref="ContextException">one of the set arguments doesn't exist</exception>
    public void Set(string name, IArgument argument, bool allowAddNew = false) {
        if (!allowAddNew) {
            if (arguments.ContainsKey(name)) arguments[name] = argument;
            else throw ContextException.KeyNotFound(name);
        }
        else {
            arguments[name] = argument;
        }
    }

    /// <summary>
    /// sets an argument with the supplied name
    /// </summary>
    /// <param name="context">source context</param>
    /// <param name="allowAddNew">
    /// if enabled allows automatically creating a new argument if it doesn't already exist,
    /// else throws <see cref="ContextException"/> if an argument with the same name doesn't already exist
    /// </param>
    /// <exception cref="ContextException">one of the set arguments doesn't exist</exception>
    public void Set(Context context, bool allowAddNew = false) {
        foreach (var c in context.arguments) {
            Set(c.Key, c.Value, allowAddNew);
        }
    }
    
    /// <summary>
    /// returns the number of arguments stored in the context
    /// </summary>
    public int Length => arguments.Count;
    
    /// <summary>
    /// whether the context contains an argument with the supplied name
    /// </summary>
    /// <seealso cref="Dictionary{TKey,TValue}.ContainsKey"/>
    public bool Contains(string key) => arguments.ContainsKey(key);

    /// <summary>
    /// clones the context
    /// </summary>
    public object Clone() {
        Context context = new Context();
        
        foreach ((string key, IArgument value) in arguments) {
            context.Add(key, value);
        }
        
        return context;
    }

    
    public override string ToString() {
        return "{" + string.Join(", ", arguments.Select(kvp => $"{{\"name\": \"{kvp.Key}\", \"argument\": {kvp.Value}}}")) + "}";
    }
    
    public (string name, IArgument value) GetAtIndex(int index) {
        if (index >= Length || index < 0) throw ContextException.OutOfRange(index, Length);
        return (arguments.Keys.ElementAt(index), arguments.Values.ElementAt(index));
    }

    public static Context New(params (string name, IArgument argument)[] arguments) {
        return new Context(arguments);
    }

    public static implicit operator Dictionary<string, IArgument>(Context context) => context.arguments;
}
