//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Single Selection Argument Type <br/>
/// lets you select a single item from multiple options
/// </summary>
public class SingleSelectArgumentType : IArgumentType {
    public Type? Type { get; }
    public string[] Values { get; }
    private int? selectedIndex;

    public SingleSelectArgumentType(string[] values) {
        Type = null;
        Values = values;
    }
    
    /// <summary>
    /// constructor that sources values from an enum
    /// </summary>
    /// <param name="type">source enum type</param>
    /// <exception cref="TypeNotEnumException"></exception>
    public SingleSelectArgumentType(Type type) {
        Type = type;
        try {
            Values = Enum.GetNames(type);
        }
        catch (ArgumentException) {
            throw new TypeNotEnumException(type);
        }
    }

    public string GetInputType() {
        return "SingleSelect";
    }

    public string GetStringValue() {
        if (Values == null || Values.Length == 0) {
            throw new SettingsBuilderException("No values provided to argument.");
        }

        if (selectedIndex == null) {
            throw new SettingsBuilderException("No value has been selected.");
        }
        
        return Values[(int)selectedIndex];
    }

    public object GetValue() {
        if (Values == null || Values.Length == 0)
            throw new SettingsBuilderException("No values provided to argument.");

        if (selectedIndex == null)
            throw new SettingsBuilderException("No value has been selected.");

        return Type == null ? Values[(int)selectedIndex] : Enum.Parse(Type, Values[(int)selectedIndex]);
    }

    private void Set(string value) {
        
        if (Values == null || Values.Length == 0)
            throw new SettingsArgumentException("No values provided to argument.");
        
        for (int i = 0; i < Values.Length; i++) {
            if (value == Values[i]) {
                selectedIndex = i;
                return;
            }
        }
        
        throw new SettingsArgumentException("Selected invalid value.");
    }

    public void SetValue(object v) {
        if (v.GetType() == Type) {
            Set(Enum.GetName(Type, v) ?? string.Empty);
            return;
        }
        
        Set((string)v);
    }

    /// <summary>
    /// sets the index pointing to the selected value
    /// </summary>
    /// <exception cref="SettingsArgumentException">index is not valid</exception>
    public void SetIndex(int index) {
        if (index < 0 || index >= Values.Length) throw new SettingsArgumentException("Index out of range.");
        selectedIndex = index;
    }

    /// <summary>
    /// returns the index pointing to the selected value, null if no value has been selected 
    /// </summary>
    public int? GetIndex() {
        return selectedIndex;
    }

    public IArgumentType Clone() {
        if (Values == null || Values.Length == 0)
            throw new SettingsArgumentException("No values provided to argument.");

        SingleSelectArgumentType clone = Type == null ? 
            new SingleSelectArgumentType(Values) : new SingleSelectArgumentType(Type);
        
        clone.selectedIndex = selectedIndex;
        
        return clone;
    }

    public override string ToString() {
        string output = $"{{\"type\": \"single_select\", \"value\": " +
                        $"{(selectedIndex == null ? "\"null\"" : selectedIndex.ToString())}, \"values\": [";

        for (int i = 0; i < Values.Length; i++) {
            output += "\"" + Values[i] + "\"" + (i != Values.Length - 1 ? ", " : "");
        }
        
        output += "]}";
        
        return output;
    }
}