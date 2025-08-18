//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;
using NeoKolors.Settings.Argument.Exception;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// list of elements that are selected from an array of predefined values
/// </summary>
/// <typeparam name="T">type of the element</typeparam>
public class SelectionListArgument<T> : IArgument<T[]>, IXsdArgument where T : notnull {
    public T[] Options { get; }
    public T[] DefaultSelected { get; }
    public List<T> Selected { get; private set; }
    public T[] Value => Get();

    public SelectionListArgument(params T[] options) {
        Options = options;
        DefaultSelected = [];
        Selected = [];
    }
    
    public SelectionListArgument(T[] options, params T[] defaultSelected) {
        Options = options;

        foreach (var d in defaultSelected)
            if (!options.Contains(d)) 
                throw new InvalidArgumentInputException("Default values must be in the list of values.");
        
        DefaultSelected = defaultSelected;
        Selected = new();
        Set(defaultSelected);
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

    void IArgument.Set(object? value) => Set(value);
    public T[] Get() => Selected.ToArray();
    public void Reset() => Selected = DefaultSelected.ToList();
    public IArgument<T[]> Clone() => (IArgument<T[]>)MemberwiseClone();
    
    public void Set(string value) {
        for (int i = 0; i < Options.Length; i++) {
            if (Options[i].ToString() != value) continue;
            Selected.Add(Options[i]);
            return;
        }
    }
    
    public string ToXsd() =>
        $"""
         <xsd:complexType>
             <xsd:choice maxOccurs="unbounded" minOccurs="0">
                 <xsd:element name="Elem">
                     <xsd:simpleType>
                         <xsd:restriction base="xsd:string">
                             {Options.Select(s => $"<xsd:enumeration value=\"{s}\"/>").Join("\n").PadLinesLeft(20)}
                         </xsd:restriction>
                     </xsd:simpleType>
                 </xsd:element>
             </xsd:choice>
         </xsd:complexType>
         """;

    /// <summary>
    /// sets the selected values
    /// </summary>
    /// <param name="value">
    /// value is array of <see cref="T"/> -> sets selected values to this array, <br/>
    /// value is <see cref="T"/> -> adds this value to the selected values, <br/>
    /// value is null -> removes the last value from the selected values, <br/>
    /// value is <see cref="SelectionListArgument{T}"/> -> sets selected values to the values of this argument
    /// </param>
    /// <exception cref="InvalidArgumentInputException">set value is not contained in <see cref="Options"/></exception>
    /// <exception cref="InvalidArgumentInputTypeException">an incompatible type was inputted</exception>
    public void Set(object? value) {
        switch (value) {
            case null: 
                if (Selected.Count != 0) Selected.RemoveAt(Selected.Count - 1);
                break;
            case T[] ta:
                Set(ta);
                break;
            case T t:
                Set(t);
                break;
            case SelectionListArgument<T> a:
                Set(a);
                break;
            default:
                throw new InvalidArgumentInputTypeException(typeof(T), value.GetType());
        }
    }

    /// <summary>
    /// copies the selected values from the input argument to this argument
    /// </summary>
    /// <param name="argument">the argument from which the values will be copied</param>
    /// <exception cref="InvalidArgumentInputException">
    /// one of the selected values of the input is not present in the selectable values
    /// </exception>
    public void Set(SelectionListArgument<T> argument) {
        foreach (var v in argument.Options)
            if (!Options.Contains(v)) 
                throw new InvalidArgumentInputException($"Value must be in the list of values. Instead it was '{v.ToString()}'");

        T[] c = [];
        argument.Selected.CopyTo(c);
        Selected = c.ToList();
    }

    /// <summary>
    /// adds the value to the list of selected values
    /// </summary>
    /// <param name="value">the value to be selected</param>
    /// <exception cref="InvalidArgumentInputException">value is not in the list of selectable values</exception>
    public void Set(T value) {
        if (!Options.Contains(value))
            throw new InvalidArgumentInputException(
                $"Value must be in the list of values. Instead it was '{value.ToString()}'");
        
        Selected.Add(value);
    }
    
    /// <summary>
    /// adds the values to the list of selected values
    /// </summary>
    /// <param name="values">the values to be selected</param>
    /// <exception cref="InvalidArgumentInputException">
    /// one of the values is not in the list of selectable values
    /// </exception>
    public void Set(params T[] values) {
        foreach (var v in values) 
            if (!Options.Contains(v))
                throw new InvalidArgumentInputException(
                    $"Value must be in the list of values. Instead it was '{v.ToString()}'");
        
        Selected.AddRange(values);
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
    public T[] GetDefault() => DefaultSelected;
    object IArgument.GetDefault() => GetDefault();
    void IArgument.Reset() => Reset();
    IArgument IArgument.Clone() => Clone();
    public bool Equals(IArgument? other) {
        return other is SelectionListArgument<T> s &&
               Options == s.Options &&
               Selected == s.Selected &&
               DefaultSelected == s.DefaultSelected;
    }

    object ICloneable.Clone() => Clone();
}