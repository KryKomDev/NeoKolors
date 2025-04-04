//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exception;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// list of elements that are selected from an array of predefined values
/// </summary>
/// <typeparam name="T">type of the element</typeparam>
public class SelectionListArgument<T> : IArgument<T[]> where T : notnull {
    public T[] Values { get; }
    public T[] DefaultValues { get; }
    public List<T> Selected { get; private set; }
    public T[] Value => Get();
    
    public SelectionListArgument(T[] values, params T[] defaultValues) {
        Values = values;

        foreach (var d in defaultValues)
            if (!values.Contains(d)) 
                throw new InvalidArgumentInputException("Default values must be in the list of values.");
        
        DefaultValues = defaultValues;
        Selected = new();
        Set(defaultValues);
    }
    
    /// <summary>
    /// creates a new instance from all members of an enum
    /// </summary>
    /// <param name="defaultValues">values that are selected by default</param>
    /// <typeparam name="TEnum">enum type</typeparam>
    public static SelectionListArgument<TEnum> FromEnum<TEnum>(params TEnum[] defaultValues) where TEnum : Enum {
        List<TEnum> values = new();
        foreach (var v in Enum.GetValues(typeof(TEnum))) values.Add((TEnum)v);
        return new SelectionListArgument<TEnum>(values.ToArray(), defaultValues);
    }

    void IArgument.Set(object value) => Set(value);
    public T[] Get() => Selected.ToArray();
    public void Reset() => Selected = DefaultValues.ToList();
    public IArgument<T[]> Clone() => (IArgument<T[]>)MemberwiseClone();

    /// <summary>
    /// sets the selected values
    /// </summary>
    /// <param name="value">
    /// value is array of <see cref="T"/> -> sets selected values to this array, <br/>
    /// value is <see cref="T"/> -> adds this value to the selected values, <br/>
    /// value is null -> removes the last value from the selected values, <br/>
    /// value is <see cref="SelectionListArgument{T}"/> -> sets selected values to the values of this argument
    /// </param>
    /// <exception cref="InvalidArgumentInputException">set value is not contained in <see cref="Values"/></exception>
    /// <exception cref="InvalidArgumentInputTypeException">an incompatible type was inputted</exception>
    public void Set(object? value) {
        if (value is null) {
            if (Selected.Count != 0) Selected.RemoveAt(Selected.Count - 1);
        }
        else if (value is T[] ta) {
            Selected = new();
            foreach (var t in ta) {
                if (Values.Contains(t)) {
                    Selected.Add(t);
                }
                else {
                    throw new InvalidArgumentInputException(
                        $"Value must be in the list of values. Instead it was '{t.ToString()}'");
                }
            }
        }
        else if (value is T t) {
            if (!Values.Contains(t)) {
                throw new InvalidArgumentInputException(
                    $"Value must be in the list of values. Instead it was '{t.ToString()}'");
            }
            
            Selected.Add(t);
        }
        else if (value is SelectionListArgument<T> a) {
            Set(a);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(T), value.GetType());
        }
    }

    private void Set(SelectionListArgument<T> argument) {
        foreach (var v in argument.Values)
            if (!Values.Contains(v)) 
                throw new InvalidArgumentInputException($"Value must be in the list of values. Instead it was '{v.ToString()}'");

        T[] c = [];
        argument.Selected.CopyTo(c);
        Selected = c.ToList();
    }
    
    /// <summary>
    /// moves a selected value from one index to another
    /// </summary>
    /// <param name="start">the index of the moved element</param>
    /// <param name="end">the end position</param>
    /// <exception cref="InvalidArgumentInputException">one of the indexes is not in bounds</exception>
    public void Move(int start, int end) {
        if (start < 0 || start >= Selected.Count || end < 0 || end >= Selected.Count) {
            throw new InvalidArgumentInputException("Start and end must be within the bounds of the count of selected values.");
        }

        var temp = Selected[start];
        Selected.RemoveAt(start);
        Selected.Insert(end, temp);
    }

    /// <summary>
    /// removes a selected value at the specified index
    /// </summary>
    /// <param name="index">index of the selected value</param>
    /// <exception cref="InvalidArgumentInputException">index out of bounds</exception>
    public void Remove(int index) {
        if (index < 0 || index >= Selected.Count) {
            throw new InvalidArgumentInputException("Index must be within the bounds of the count of selected values.");
        }
        
        Selected.RemoveAt(index);
    }

    object IArgument.Get() => Get();
    void IArgument.Reset() => Reset();
    IArgument IArgument.Clone() => Clone();
    public bool Equals(IArgument? other) {
        return other is SelectionListArgument<T> s &&
               Values == s.Values &&
               Selected == s.Selected &&
               DefaultValues == s.DefaultValues;
    }

    object ICloneable.Clone() => Clone();
}