//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// multiple choice argument
/// </summary>
/// <typeparam name="T">type of values that can be selected</typeparam>
public class MultiSelectArgument<T> : IArgument<T[]> where T : notnull {
    
    /// <summary>
    /// key is the actual value, int is its index
    /// </summary>
    public Dictionary<T, int> Choices { get; } = new();
    public bool[] Selected { get; set; }
    public T[] Value => Get();
    public bool[] DefaultSelected { get; }

    public MultiSelectArgument(T[] values, params T[] defaultValues) {
        for (int i = 0; i < values.Length; i++) Choices.Add(values[i], i);
        Selected = new bool[values.Length];
        
        if (values.Length < defaultValues.Length) 
            throw new InvalidArgumentInputException("There is more default values than there are choices.");
        
        DefaultSelected = Select(defaultValues);
    }
    
    public MultiSelectArgument(params T[] values) {
        for (int i = 0; i < values.Length; i++) Choices.Add(values[i], i);
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
    
    void IArgument.Set(object? value) => Set(value);

    /// <summary>
    /// returns the selected values
    /// </summary>
    public T[] Get() {
        List<T> selected = new();
        var a = Choices.Keys.ToArray();
        
        for (int i = 0; i < Selected.Length; i++) {
            if (Selected[i]) selected.Add(a[i]);
        }

        return selected.ToArray();
    }
    
    /// <summary>
    /// resets the argument to the default state as it was after initialization 
    /// </summary>
    public void Reset() => Selected = DefaultSelected;

    /// <summary>
    /// Sets the argument.
    /// if value is T toggles if the value is selected,
    /// if value is T[] toggles all values of the array,
    /// if value is MultiSelectArgument&lt;T&gt; the selections will be copied,
    /// if value is T the choice will be toggled
    /// </summary>
    /// <exception cref="InvalidArgumentInputException">
    /// choices of the input Multi-select argument are not the same
    /// </exception>
    /// <exception cref="InvalidArgumentInputTypeException">value does not match any of the allowed types</exception>
    public void Set(object? value) {
        switch (value) {
            case T[] ta: {
                Set(ta);
                break;
            }
            case bool[] ba: {
                for (int i = 0; i < Math.Min(ba.Length, Selected.Length); i++) Selected[i] = ba[i];
                break;
            }
            // ReSharper disable once UsageOfDefaultStructEquality
            case MultiSelectArgument<T> msa when !Choices.SequenceEqual(msa.Choices):
                throw new InvalidArgumentInputException("Choices of the input Multi-select argument are not the same.");
            case MultiSelectArgument<T> msa: {
                for (int i = 0; i < Selected.Length; i++) Selected[i] = false;
                Select(msa.Value);
                break;
            }
            case T t: {
                Set(t);
                break;
            }
            default: throw new InvalidArgumentInputTypeException(typeof(T), value?.GetType());
        }
    }

    /// <summary>
    /// toggles the selection for values of the array
    /// </summary>
    public void Set(params T[] values) {
        foreach (var value in values) Set(value);
    }

    /// <summary>
    /// toggles the selection for the input value
    /// </summary>
    /// <exception cref="InvalidArgumentInputException">value is not inside the array</exception>
    public void Set(T value) {
        if (!Choices.TryGetValue(value, out var key))
            throw new InvalidArgumentInputException(
                $"Multi-select argument does not contain choice value \"{value.ToString()}\"");

        Selected[key] = !Selected[key];
    } 

    /// <summary>
    /// selects the values from the array
    /// </summary>
    /// <exception cref="InvalidArgumentInputException">one of the values is not a valid choice</exception>
    public bool[] Select(params T[] values) {
        bool[] selected = new bool[Choices.Count];
        
        foreach (var value in values) {
            if (!Choices.TryGetValue(value, out var key))
                throw new InvalidArgumentInputException(
                    $"Multi-select argument does not contain choice value \"{value.ToString()}\"");

            Selected[key] = true;
            selected[key] = true;
        }

        return selected;
    }
    
    /// <summary>
    /// deselects the values from the array
    /// </summary>
    /// <exception cref="InvalidArgumentInputException">one of the values is not a valid choice</exception>
    public void Deselect(params T[] values) {
        foreach (var value in values) {
            if (!Choices.TryGetValue(value, out var key))
                throw new InvalidArgumentInputException(
                    $"Multi-select argument does not contain choice value \"{value.ToString()}\"");

            Selected[key] = false;
        }
    }

    object IArgument.Get() => Get();

    public T[] GetDefault() => Choices.Where(c => Selected[c.Value]).Select(c => c.Key).ToArray();
    object IArgument.GetDefault() => GetDefault();
    void IArgument.Reset() => Reset();
    public IArgument<T[]> Clone() => (IArgument<T[]>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();
    
    public bool Equals(IArgument? other) {
        return other is MultiSelectArgument<T> m &&
               Choices == m.Choices &&
               DefaultSelected == m.DefaultSelected &&
               Selected == m.Selected;
    }

    object ICloneable.Clone() => Clone();
}