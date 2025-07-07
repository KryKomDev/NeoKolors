//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Argument;

public interface IArgument<T> : IArgument {
    
    /// <summary>
    /// value held by the argument
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Assigns a value to the argument, potentially validating it.
    /// </summary>
    /// <param name="value">The value to set for the argument.</param>
    public void Set(T value);

    /// <summary>
    /// sets the <see cref="Value"/>, should also support copying a value from another argument
    /// </summary>
    public new void Set(object value);

    /// <summary>
    /// returns the value held by <see cref="Value"/>
    /// </summary>
    /// <returns></returns>
    public new T Get();
    
    /// <summary>
    /// returns the default value
    /// </summary>
    /// <returns></returns>
    public new T GetDefault();

    /// <summary>
    /// resets <see cref="Value"/> to the default state
    /// </summary>
    public new void Reset();
    
    /// <summary>
    /// returns a copy of the argument
    /// </summary>
    /// <returns></returns>
    public new IArgument<T> Clone();
    
    public static IArgument operator <<(IArgument<T> argument, T value) {
        argument.Set(value);
        return argument;
    }
}

public interface IArgument : IEquatable<IArgument>, ICloneable {
    
    public void Set(object? value);
    public object? Get();
    public object? GetDefault();
    public void Reset();
    public new IArgument Clone();
    
    public static IArgument operator <<(IArgument argument, object? value) {
        argument.Set(value);
        return argument;
    }
}