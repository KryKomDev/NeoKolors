//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Argument;

public interface IArgument<out T> : IArgument {
    
    /// <summary>
    /// value held by the argument
    /// </summary>
    public T Value { get; }
    
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
    /// resets <see cref="Value"/> to the default state
    /// </summary>
    public new void Reset();
    
    /// <summary>
    /// returns a copy of the argument
    /// </summary>
    /// <returns></returns>
    public new IArgument<T> Clone();
}

public interface IArgument {
    public void Set(object value);
    public object Get();
    public void Reset();
    public IArgument Clone();

#if !NETSTANDARD2_0
    public static IArgument operator <<(IArgument argument, object value) {
        argument.Set(value);
        return argument;
    }
#endif 
}