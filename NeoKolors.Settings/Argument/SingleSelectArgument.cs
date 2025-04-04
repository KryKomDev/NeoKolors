//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exception;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// single selection argument
/// </summary>
public class SingleSelectArgument<T> : IArgument<T> where T : notnull {
    public T[] Options { get; }
    public int Index { get; private set; }
    public int DefaultIndex { get; }
    public T Value => Options[Index];

    public SingleSelectArgument(T[] options, int defaultValue = 0) {
        Options = options;
        DefaultIndex = Math.Min(Math.Max(0, defaultValue), Options.Length - 1);
        Index = DefaultIndex;
    }

    public SingleSelectArgument(T[] options, T defaultValue) {
        Options = options;
        Set(defaultValue);
        DefaultIndex = Index;
    }
    
    public static SingleSelectArgument<TEnum> FromEnum<TEnum>(TEnum? defaultValue = default) where TEnum : Enum {
        List<TEnum> values = [];
        foreach (var v in Enum.GetValues(typeof(TEnum))) values.Add((TEnum)v);

        return defaultValue is null
            ? new SingleSelectArgument<TEnum>(values.ToArray(), 0)
            : new SingleSelectArgument<TEnum>(values.ToArray(), defaultValue);
    }

    public void Set(object value) {
        if (value is T s) {
            for (int i = 0; i < Options.Length; i++) {
                if (!Options[i].Equals(s)) continue;
                Index = i;
                return;
            }

            throw new InvalidArgumentInputException("Inputted string is not a valid option.");
        }
        else if (value is int i) {
            if (i < 0 || i >= Options.Length)
                throw new InvalidArgumentInputException($"Option index is out of range. Must be between 0 and {Options.Length - 1} inclusive.");
            
            Index = i;
        }
        else if (value is SingleSelectArgument<T> a) {
            Set(a.Index);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(string[]), value.GetType());
        }
    }

    public T Get() => Value;
    public void Reset() => Index = DefaultIndex;
    public IArgument<T> Clone() => (IArgument<T>)MemberwiseClone();
    object IArgument.Get() => Get();
    void IArgument.Reset() => Reset();

    IArgument IArgument.Clone() => Clone();

    public void Set(int index) {
        if (index >= 0 && index < Options.Length)
            Index = index;
        else
            throw new InvalidArgumentInputException($"Option index is out of range. Must be between 0 and {Options.Length - 1} inclusive.");
    }

    public bool Equals(IArgument? other) {
        return other is SingleSelectArgument<T> s &&
               Options == s.Options &&
               Index == s.Index &&
               DefaultIndex == s.DefaultIndex;
    }

    public override string ToString() =>
        $"{{\"type\": \"single-select\", " +
        $"\"value\": \"{Options[Index]}\", " +
        $"\"index\": {Index}, " +
        $"\"default-index\": {DefaultIndex}, " +
        $"\"options\": [{string.Join(", ", Options.Select(o => $"\"{o}\""))}]}}";

    object ICloneable.Clone() {
        return Clone();
    }
}