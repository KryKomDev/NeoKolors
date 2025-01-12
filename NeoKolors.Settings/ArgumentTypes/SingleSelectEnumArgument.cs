//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings.ArgumentTypes;

public class SingleSelectEnumArgument<T> : IArgument<T> where T : Enum {
    public T Value { get; private set; }
    public T Default { get; }

    public SingleSelectEnumArgument(T? defaultValue = default) {
        Default = defaultValue!;
        Value = Default;
    }
    
    public void Set(T value) {
        Value = value;
    }

    void IArgument<T>.Set(object value) {
        if (value is T v) {
            Value = v;
        }
        else if (value is SingleSelectEnumArgument<T> a) {
            Set(a.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(T), value.GetType());
        }
    }

    void IArgument.Set(object value) => Set((T)value);
    void IArgument<T>.Reset() => Value = Default;
    void IArgument.Reset() => Value = Default;
    public T Get() => Value;
    object IArgument.Get() => Get();
    public IArgument<T> Clone() => (IArgument<T>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();
}