//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings.ArgumentTypes;

public class SingleSelectEnumArgument<TEnum> : IArgument<TEnum> where TEnum : Enum {
    public TEnum Value { get; private set; }
    public TEnum Default { get; }

    public SingleSelectEnumArgument(TEnum? defaultValue = default) {
        Default = defaultValue!;
        Value = Default;
    }
    
    public void Set(TEnum value) {
        Value = value;
    }

    void IArgument<TEnum>.Set(object value) {
        if (value is TEnum v) {
            Value = v;
        }
        else if (value is SingleSelectEnumArgument<TEnum> a) {
            Set(a.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(TEnum), value.GetType());
        }
    }

    void IArgument.Set(object value) => Set((TEnum)value);
    void IArgument<TEnum>.Reset() => Value = Default;
    void IArgument.Reset() => Value = Default;
    public TEnum Get() => Value;
    object IArgument.Get() => Get();
    public IArgument<TEnum> Clone() => (IArgument<TEnum>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();
    public override string ToString() =>
        $"{{\"type\": \"single-select-enum\", " +
        $"\"value\": \"{Value}\", " +
        $"\"default\": \"{Default}\", " +
        $"\"enum-type\": \"{typeof(TEnum).FullName}\"}}";
}