//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Multiselect Argument Type <br/>
/// lets you select multiple items from multiple options 
/// </summary>
public class MultiSelectArgumentType : IArgumentType {
    
    public Type? Type { get; }
    public string[] Values { get; }
    private bool[] selected;
    private readonly bool[] defaultSelected;

    /// <summary>
    /// constructor that sources values from an enum
    /// </summary>
    /// <param name="type">source enum type</param>
    /// <param name="defaultSelected"></param>
    /// <exception cref="TypeNotEnumException"></exception>
    internal MultiSelectArgumentType(Type type, bool[]? defaultSelected = null) {
        Type = type;

        try {
            Values = Enum.GetNames(type);
        }
        catch (ArgumentException) {
            throw new TypeNotEnumException(type);
        }
        
        if (defaultSelected != null) {
            this.defaultSelected = defaultSelected;
            selected = (bool[])defaultSelected.Clone();
        }
        else {
            this.defaultSelected = new bool[Values.Length];
            selected = new bool[Values.Length];
        }
    }

    public MultiSelectArgumentType(string[] values, bool[]? defaultSelected = null) {
        Type = null;
        Values = values;
        selected = new bool[values.Length];
        
        if (defaultSelected != null) {
            this.defaultSelected = defaultSelected;
            selected = (bool[])defaultSelected.Clone();
        }
        else {
            this.defaultSelected = new bool[Values.Length];
            selected = new bool[Values.Length];
        }
    }
    
    public string GetInputType() {
        return "MultiSelect";
    }

    public string GetStringValue() {
        string output = "[";

        string[] selectedValues = (string[])GetValue();

        for (int i = 0; i < selectedValues.Length; i++) {
            output += selectedValues[i] + (i == selectedValues.Length - 1 ? "" : ", ");
        }

        output += "]";
        
        return output;
    }

    public object GetValue() {
        List<string> selectedValues = new List<string>();
        
        for (int i = 0; i < Values.Length; i++) {
            if (selected[i]) {
                selectedValues.Add(Values[i]);
            }
        }

        if (Type == null) return selectedValues.ToArray();
        
        List<object> enumValues = new List<object>();

        foreach (string s in selectedValues) {
            enumValues.Add(Enum.Parse(Type, s));
        }
        
        return enumValues.ToArray();
    }

    /// <summary>
    /// sets a value by the actual name of the item
    /// </summary>
    public void SetValue(string value, bool state) {
        for (int i = 0; i < Values.Length; i++) {
            if (Values[i] != value) continue;
            selected[i] = state;
            return;
        }
        
        Debug.Warn("Trying to set a value that does not exist.");
    }

    /// <summary>
    /// sets a value by its index in the <see cref="Values"/>
    /// </summary>
    /// <exception cref="SettingsArgumentException">index is out of bounds</exception>
    public void SetValue(int index, bool state) {
        if (index < 0 || index >= Values.Length) 
            throw new SettingsArgumentException("Inputted index is not valid (out of bounds).");
        
        selected[index] = state;
    }

    /// <summary>
    /// selects a value by the actual name of the item
    /// </summary>
    public void SelectValue(string value) {
        for (int i = 0; i < Values.Length; i++) {
            if (Values[i] != value) continue;
            selected[i] = true;
            return;
        }
        
        Debug.Warn("Trying to select a value that does not exist.");
    }

    /// <summary>
    /// selects a value by its index in <see cref="Values"/>
    /// </summary>
    /// <exception cref="SettingsArgumentException">index is out of bounds</exception>
    public void SelectValue(int index) {
        if (index < 0 || index >= Values.Length) 
            throw new SettingsArgumentException("Inputted index is not valid (out of bounds).");
        
        selected[index] = true;
    }

    /// <summary>
    /// deselects a value by the actual name of the item
    /// </summary>
    public void DeselectValue(string value) {
        for (int i = 0; i < Values.Length; i++) {
            if (Values[i] != value) continue;
            selected[i] = false;
            return;
        }
        
        Debug.Warn("Trying to deselect a value that does not exist.");
    }

    /// <summary>
    /// deselects a value by its index in <see cref="Values"/>
    /// </summary>
    /// <exception cref="SettingsArgumentException">index is out of bounds</exception>
    public void DeselectValue(int index) {
        if (index < 0 || index >= Values.Length) 
            throw new SettingsArgumentException("Inputted index is not valid (out of bounds).");
        
        selected[index] = false;
    }

    /// <summary>
    /// toggles a value by its actual name
    /// </summary>
    public void ToggleValue(string value) {
        for (int i = 0; i < Values.Length; i++) {
            if (Values[i] != value) continue;
            selected[i] = !selected[i];
            return;
        }
        
        Debug.Warn("Trying to toggle a value that does not exist.");
    }

    /// <summary>
    /// toggles a value by its index in <see cref="Values"/> 
    /// </summary>
    /// <exception cref="SettingsArgumentException">index is out of bounds</exception>
    public void ToggleValue(int index) {
        if (index < 0 || index >= Values.Length) 
            throw new SettingsArgumentException("Inputted index is not valid (out of bounds).");
        
        selected[index] = !selected[index];
    }

    public void SetValue(object v) {
        throw new SettingsBuilderException("This method is not supported for MultiSelectArgumentType. " +
                                           "Use SelectValue(<string>, <bool>), SelectValue(<int>, <bool>)," +
                                           " ToggleValue, SelectValue or DeselectValue methods instead.");
    }

    public IArgumentType Clone() {
        MultiSelectArgumentType clone = Type == null ? 
            new MultiSelectArgumentType(Values) : 
            new MultiSelectArgumentType(Type);

        for (int i = 0; i < selected.Length; i++) {
            clone.selected[i] = selected[i];
        }
        
        return clone;
    }

    public void Reset() {
        selected = (bool[])defaultSelected.Clone();
    }
}