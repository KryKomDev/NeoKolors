//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;
using NeoKolors.Settings.Argument.Exception;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// single selection argument
/// </summary>
public class SingleSelectArgument<T> : IArgument<T>, IXsdArgument where T : notnull {
    
    public T[] Options { get; }
    public int Index { get; private set; }
    public int DefaultIndex { get; }
    public T Value => Options[Index];

    /// <summary>
    /// Creates a new single selection argument.
    /// </summary>
    /// <param name="options">the options to choose from</param>
    /// <param name="defaultValue">the index of the default value</param>
    public SingleSelectArgument(T[] options, int defaultValue = 0) {
        Options = options;
        DefaultIndex = Math.Min(Math.Max(0, defaultValue), Options.Length - 1);
        Index = DefaultIndex;
    }

    /// <summary>
    /// Creates a new single selection argument.
    /// </summary>
    /// <param name="options">the options to choose from</param>
    /// <param name="defaultValue">the default value</param>
    public SingleSelectArgument(T[] options, T defaultValue) {
        Options = options;
        Set(defaultValue);
        DefaultIndex = Index;
    }
    
    /// <summary>
    /// Creates a new single selection argument from an enum.
    /// </summary>
    /// <param name="defaultValue">the default value</param>
    /// <typeparam name="TEnum">the enum to get the values from</typeparam>
    public static SingleSelectArgument<TEnum> FromEnum<TEnum>(TEnum? defaultValue = default) where TEnum : Enum {
        List<TEnum> values = [];
        foreach (var v in Enum.GetValues(typeof(TEnum))) values.Add((TEnum)v);

        return defaultValue is null
            ? new SingleSelectArgument<TEnum>(values.ToArray())
            : new SingleSelectArgument<TEnum>(values.ToArray(), defaultValue);
    }

    public void Set(T value) {
        for (int i = 0; i < Options.Length; i++) {
            if (!Options[i].Equals(value)) continue;
            Index = i;
            return;
        }

        throw new InvalidArgumentInputException("Inputted string is not a valid option.");
    }

    public void Set(object? value) {
        switch (value) {
            case T s:
                Set(s);
                break;
            case int i when i < 0 || i >= Options.Length:
                throw new InvalidArgumentInputException($"Option index is out of range. Must be between 0 and {Options.Length - 1} inclusive.");
            case int i:
                Index = i;
                break;
            case SingleSelectArgument<T> a:
                Set(a.Index);
                break;
            default:
                throw new InvalidArgumentInputTypeException(typeof(string[]), value?.GetType());
        }
    }

    public T Get() => Value;
    public T GetDefault() => Options[DefaultIndex];
    object IArgument.GetDefault() => GetDefault();
    public void Reset() => Index = DefaultIndex;
    public IArgument<T> Clone() => (IArgument<T>)MemberwiseClone();
    
    public void Set(string value) {
        for (int i = 0; i < Options.Length; i++) {
            if (Options[i].ToString() != value) continue;
            Index = i;
            return;
        }
        
        throw new InvalidArgumentInputException("Inputted string is not a valid option.");
    }
    
    public string ToXsd() =>
        $"""
         <xsd:simpleType>
             <xsd:restriction base="xsd:string">
                 {Options.Select(o => $"<xsd:enumeration value=\"{o}\"/>").Join("\n").PadLinesLeft(8)}
             </xsd:restriction>
         </xsd:simpleType>
         """;
    
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