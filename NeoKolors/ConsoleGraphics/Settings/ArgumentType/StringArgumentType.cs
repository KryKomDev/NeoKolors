//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// String Argument Type
/// </summary>
public class StringArgumentType : IArgumentType {

    private string? value;
    public uint MinLength { get; }
    public uint MaxLength { get; }

    internal StringArgumentType(uint minLength = 0, uint maxLength = UInt32.MaxValue) {
        MinLength = minLength;
        MaxLength = maxLength;
    }
    
    public string GetInputType() {
        return "String";
    }

    public string GetStringValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return value;
    }

    public object GetValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return value;
    }

    public void SetValue(object v) {
        if (v.GetType() != typeof(string)) throw new SettingsArgumentException(v.GetType(), typeof(string));
        if (((string)v).Length < MinLength || ((string)v).Length > MaxLength) 
            throw new SettingsArgumentException($"Length of string '{v}' must be greater than " +
                                                $"{MinLength} and smaller than {MaxLength}.");

        value = (string)v;
    }

    public IArgumentType Clone() {
        StringArgumentType clone = new StringArgumentType(MinLength, MaxLength) {
            value = value
        };
        
        return clone;
    }

    public override string ToString() {
        return $"{{\"type\": \"string\", " +
               $"\"minLength\": \"{MinLength}\", " +
               $"\"maxLength\": \"{MaxLength}\", " +
               $"\"value\": {value ?? "\"null\""}}}";
    }
}