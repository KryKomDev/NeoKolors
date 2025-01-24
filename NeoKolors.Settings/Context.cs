//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument;
using ContextException = NeoKolors.Settings.Exception.ContextException;

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
        set {
            try {
                arguments[key] = value;
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
    public void Add(string name, IArgument argument, bool clone = true) {
        if (!arguments.TryAdd(name, clone ? argument.Clone() : argument)) throw ContextException.KeyDuplicate(name);
    }
    
    /// <summary>
    /// adds arguments from another context to the context
    /// </summary>
    /// <exception cref="ContextException">an argument with the same name already exists</exception>
    public void Add(Context context, bool clone = true) {
        foreach (var v in context.arguments) {
            Add(v.Key, v.Value, clone);
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
    /// <param name="clone">
    /// if true clones the argument and then saves the copy to the context instead of saving the reference directly
    /// </param>
    /// <exception cref="ContextException">one of the set arguments doesn't exist</exception>
    public void Set(string name, IArgument argument, bool allowAddNew = false, bool clone = true) {
        if (!allowAddNew) {
            if (arguments.ContainsKey(name)) arguments[name] = clone ? argument.Clone() : argument;
            else throw ContextException.KeyNotFound(name);
        }
        else {
            arguments[name] = clone ? argument.Clone() : argument;
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
    /// sets the value of an argument
    /// </summary>
    /// <exception cref="ContextException">no argument with the specified name exists</exception>
    public void Set(string name, object value) {
        if (!arguments.TryGetValue(name, out var argument)) throw ContextException.KeyNotFound(name);
        argument.Set(value);
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
        
        foreach (var v in arguments) {
            context.Add(v.Key, v.Value);
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


#if NETSTANDARD2_0
internal static class DictionaryExtensions {
    public static bool TryAdd(this Dictionary<string, IArgument> arguments, string key, IArgument argument) {
        if (arguments.ContainsKey(key)) {
            return false;
        }

        arguments.Add(key, argument);
        return true;
    }
}
#endif