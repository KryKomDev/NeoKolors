//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exception;
using static NeoKolors.Console.Debug;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// multiple choice argument
/// </summary>
/// <typeparam name="T">type of values that can be selected</typeparam>
public class MultiSelectArgument<T> : IArgument<T[]> where T : notnull {
    public Dictionary<int, T> Choices { get; } = new();
    public bool[] Selected { get; private set; }
    public T[] Value => Get();
    public bool[] DefaultSelected { get; }

    public MultiSelectArgument(T[] values, params T[] defaultValues) {
        for (int i = 0; i < values.Length; i++) Choices.Add(i, values[i]);
        Selected = new bool[values.Length];
        
        if (values.Length < defaultValues.Length) 
            Throw(new InvalidArgumentInputException("There is more default values than there are choices."));
        
        DefaultSelected = Select(defaultValues);
        Set(values);
    }
    
    public MultiSelectArgument(params T[] values) {
        for (int i = 0; i < values.Length; i++) Choices.Add(i, values[i]);
        Selected = new bool[values.Length];
        DefaultSelected = [];
        Set(values);
    }
    
    /// <summary>
    /// creates a new instance from all members of an enum
    /// </summary>
    /// <param name="defaultValues">values that are selected by default</param>
    /// <typeparam name="TEnum">the type of enum</typeparam>
    /// <returns>new multi-select argument</returns>
    public static MultiSelectArgument<TEnum> FromEnum<TEnum>(params TEnum[] defaultValues) where TEnum : Enum {
        List<TEnum> values = new();
        foreach (var v in Enum.GetValues(typeof(TEnum))) values.Add((TEnum)v);
        return new MultiSelectArgument<TEnum>(values.ToArray(), defaultValues);
    }
    
    void IArgument.Set(object value) => Set(value);

    public T[] Get() {
        List<T> selected = new();
        
        for (int i = 0; i < Selected.Length; i++) {
            if (Selected[i]) selected.Add(Choices[i]);
        }

        return selected.ToArray();
    }
    
    public void Reset() => Selected = DefaultSelected;

    public IArgument<T[]> Clone() => (IArgument<T[]>)MemberwiseClone();

    public void Set(object value) {
        if (value is T[] ta) {
            Set(ta);
        }
        else if (value is bool[] ba) {
            if (ba.Length <= Selected.Length) {
                for (int i = 0; i < ba.Length; i++) Selected[i] = ba[i];
            }
            else {
                for (int i = 0; i < Selected.Length; i++) Selected[i] = ba[i];
            }
        }
        else if (value is MultiSelectArgument<T> msa) {
            Set(msa.Value);
        }
        else if (value is T t) {
            if (!Choices.ContainsValue(t)) {
                Error($"Multi-select argument does not contain choice value \"{t.ToString()}\"");
                return;
            }

            for (int i = 0; i < Choices.Count; i++) if (Choices[i].Equals(t)) Selected[i] = !Selected[i];
        }
        else {
            Throw(new InvalidArgumentInputTypeException(typeof(T), value.GetType()));
        }
    }

    public void Set(T[] values) {
        foreach (var t in values) {
            if (!Choices.ContainsValue(t)) {
                Error($"Multi-select argument does not contain choice value \"{t.ToString()}\"");
                continue;
            }

            for (int i = 0; i < Choices.Count; i++) if (Choices[i].Equals(values[i])) Selected[i] = true;
        }
    }

    object IArgument.Get() => Get();
    void IArgument.Reset() => Reset();
    IArgument IArgument.Clone() => Clone();

    private bool[] Select(T[] values) {
        bool[] selected = new bool[Choices.Count];
        
        foreach (var t in values) {
            if (!Choices.ContainsValue(t)) {
                Error($"Multi-select argument does not contain choice value \"{t.ToString()}\"");
                continue;
            }

            for (int i = 0; i < Choices.Count; i++) if (Choices[i].Equals(t)) selected[i] = true;
        }

        return selected;
    }

    public bool Equals(IArgument? other) {
        return other is MultiSelectArgument<T> m &&
               Choices == m.Choices &&
               DefaultSelected == m.DefaultSelected &&
               Selected == m.Selected;
    }

    object ICloneable.Clone() => Clone();
}