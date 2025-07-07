//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Collections;
using System.Runtime.CompilerServices;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Info;
using ContextException = NeoKolors.Settings.Exception.ContextException;

namespace NeoKolors.Settings;

/// <summary>
/// Context Struct <br/>
/// provides context, argument types, and values to settings builders
/// </summary>
public sealed class Context : 
    ICloneable, 
    IEnumerable<Tuple<string, IArgument>>, 
    IEnumerable<(string Name, IArgument Value)>,
    IEnumerable<IArgument>,
    IEnumerable<KeyValuePair<string, IArgument>> 
{
    
    private readonly Dictionary<string, IArgument> _arguments = new();

    /// <summary>
    /// Locks the context, preventing further additions of arguments.
    /// </summary>
    public void Lock() => IsLocked = true;
    public bool IsLocked { get; private set; }

    /// <summary>
    /// returns the argument assigned to the supplied key
    /// </summary>
    /// <exception cref="ContextException">the key was not found</exception>
    public IArgument this[string key] {
        get {
            try {
                return _arguments[key];
            }
            catch (KeyNotFoundException) {
                throw ContextException.KeyNotFound(key);
            }
        }
        set {
            try {
                _arguments[key] = value;
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
    /// Represents a context that provides argument types and corresponding values.
    /// Used for handling and managing arguments in settings builders.
    /// </summary>
    /// <remarks>
    /// This class allows dynamic addition, modification, and retrieval of arguments
    /// through index access, dedicated methods, and enumeration. It supports cloning
    /// and interoperability with multiple collection types.
    /// </remarks>
    public Context(params IEnumerable<ArgumentInfo> arguments) {
        foreach (var a in arguments) {
            Add(a.Name, a.Argument);
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
        foreach (var a in arguments) {
            Add(a.name, a.argument);
        }
    }

    /// <summary>
    /// adds an argument to the context
    /// </summary>
    /// <exception cref="ContextException">an argument with the same name already exists</exception>
    public void Add(string name, IArgument argument, bool clone = true) {
        if (IsLocked) 
            throw ContextException.ContextLocked();
        
        if (!_arguments.TryAdd(name, clone ? argument.Clone() : argument)) 
            throw ContextException.KeyDuplicate(name);
    }
    
    /// <summary>
    /// adds arguments from another context to the context
    /// </summary>
    /// <exception cref="ContextException">an argument with the same name already exists</exception>
    public void Add(Context context, bool clone = true) {
        if (IsLocked) 
            throw ContextException.ContextLocked();
        
        foreach (var v in context._arguments) {
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
        if (!(allowAddNew && !IsLocked)) {
            if (_arguments.ContainsKey(name)) _arguments[name] = clone ? argument.Clone() : argument;
            else throw ContextException.KeyNotFound(name);
        }
        else {
            _arguments[name] = clone ? argument.Clone() : argument;
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
        foreach (var c in context._arguments) {
            Set(c.Key, c.Value, allowAddNew);
        }
    }

    /// <summary>
    /// sets the value of an argument
    /// </summary>
    /// <exception cref="ContextException">no argument with the specified name exists</exception>
    public void Set(string name, object value) {
        if (!_arguments.TryGetValue(name, out var argument)) 
            throw ContextException.KeyNotFound(name);
        
        argument.Set(value);
    }
    
    /// <summary>
    /// returns the number of arguments stored in the context
    /// </summary>
    public int Length => _arguments.Count;
    public int Count => _arguments.Count;
    
    /// <summary>
    /// whether the context contains an argument with the supplied name
    /// </summary>
    /// <seealso cref="Dictionary{TKey,TValue}.ContainsKey"/>
    public bool Contains(string key) => _arguments.ContainsKey(key);

    /// <summary>
    /// clones the context
    /// </summary>
    public object Clone() {
        var context = new Context();
        
        foreach (var v in _arguments) {
            context.Add(v.Key, v.Value);
        }
        
        return context;
    }
    
    public override string ToString() => 
        "{" + string.Join(", ",
            _arguments.Select(kvp => $"{{\"name\": \"{kvp.Key}\", \"argument\": {kvp.Value}}}")) + "}";
    

    public (string name, IArgument value) GetAtIndex(int index) {
        if (index >= Length || index < 0) throw ContextException.OutOfRange(index, Length);
        return (_arguments.Keys.ElementAt(index), _arguments.Values.ElementAt(index));
    }

    public static Context New(params (string name, IArgument argument)[] arguments) => new(arguments);
    
    public Dictionary<string, IArgument> ToDictionary() => _arguments;

    
    // --=================== IEnumerable methods ====================--
    //          just implementations of IEnumerable interfaces       
    // --============================================================--
    
    IEnumerator<Tuple<string, IArgument>> IEnumerable<Tuple<string, IArgument>>.GetEnumerator() => 
        _arguments.Select(kvp => new Tuple<string, IArgument>(kvp.Key, kvp.Value)).GetEnumerator();
    IEnumerator<IArgument> IEnumerable<IArgument>.GetEnumerator() => _arguments.Values.GetEnumerator();
    public IEnumerator<KeyValuePair<string, IArgument>> GetEnumerator() => _arguments.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _arguments.Values.GetEnumerator();
    IEnumerator<(string Name, IArgument Value)> IEnumerable<(string Name, IArgument Value)>.GetEnumerator() =>
        _arguments.Select(kvp => (kvp.Key, kvp.Value)).GetEnumerator();
    
    
    // --=================== Get Argument Methods ====================--
    //       this part is methods that return values of arguments
    // --=============================================================--
    
    /// <summary>
    /// Retrieves the integer value associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The integer value associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified name does not exist in the context.</exception>
    /// <exception cref="ContextException">
    /// Thrown if the argument associated with the name is not of type <see cref="IntegerArgument"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInt(string name) => Get<IntegerArgument>(name).Get();
    
    /// <summary>
    /// Retrieves the unsigned integer value associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The integer value associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified name does not exist in the context.</exception>
    /// <exception cref="ContextException">
    /// Thrown if the argument associated with the name is not of type <see cref="UIntegerArgument"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetUInt(string name) => Get<UIntegerArgument>(name).Get();
    
    /// <summary>
    /// Retrieves the long value associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The integer value associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified name does not exist in the context.</exception>
    /// <exception cref="ContextException">
    /// Thrown if the argument associated with the name is not of type <see cref="LongArgument"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetLong(string name) => Get<LongArgument>(name).Get();
    
    /// <summary>
    /// Retrieves the unsigned long value associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The integer value associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified name does not exist in the context.</exception>
    /// <exception cref="ContextException">
    /// Thrown if the argument associated with the name is not of type <see cref="ULongArgument"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetULong(string name) => Get<ULongArgument>(name).Get();
    
    /// <summary>
    /// Retrieves the float value associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The integer value associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified name does not exist in the context.</exception>
    /// <exception cref="ContextException">
    /// Thrown if the argument associated with the name is not of type <see cref="FloatArgument"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetFloat(string name) => Get<FloatArgument>(name).Get();
    
    /// <summary>
    /// Retrieves the double value associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The integer value associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified name does not exist in the context.</exception>
    /// <exception cref="ContextException">
    /// Thrown if the argument associated with the name is not of type <see cref="DoubleArgument"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetDouble(string name) => Get<DoubleArgument>(name).Get();
    
    /// <summary>
    /// Retrieves the string value associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The integer value associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified name does not exist in the context.</exception>
    /// <exception cref="ContextException">
    /// Thrown if the argument associated with the name is not of type <see cref="StringArgument"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetString(string name) => Get<StringArgument>(name).Get();
    
    /// <summary>
    /// Retrieves the boolean value associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The integer value associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the specified name does not exist in the context.</exception>
    /// <exception cref="ContextException">
    /// Thrown if the argument associated with the name is not of type <see cref="BoolArgument"/>.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBool(string name) => Get<BoolArgument>(name).Get();

    /// <summary>
    /// Retrieves an argument from the context by its name and ensures it is of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the argument to retrieve, which must implement the IArgument interface.</typeparam>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The argument of the specified type associated with the given name.</returns>
    /// <exception cref="ContextException">Thrown when the argument is not of the expected type or does not exist.</exception>
    public T Get<T>(string name) where T : IArgument {
        var a = _arguments[name];
        if (a is T t) return t;
        throw ContextException.InvalidType(typeof(T), a.GetType());
    }
}