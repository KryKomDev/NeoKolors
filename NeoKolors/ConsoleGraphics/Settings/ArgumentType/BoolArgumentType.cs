//
// NeoKolors
// by KryKom 2024
//

using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Bool Argument Type <br/>
/// simple bool select/checkbox
/// </summary>
public sealed class BoolArgumentType : IArgumentType {

    private bool? value;
    
    public string GetInputType() {
        return "Bool";
    }

    public string GetStringValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");
        
        return ((bool)value).ToString();
    }

    public object GetValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return value;
    }

    public void SetValue(object v) {
        if (v.GetType() != typeof(bool)) throw new SettingsArgumentException(v.GetType(), typeof(bool));
        
        value = (bool)v;
    }

    public IArgumentType Clone() { 
        BoolArgumentType newArg = new BoolArgumentType {
            value = value
        };
        
        return newArg;
    }

    public override string ToString() {
        return $"{{\"type\": \"bool\", \"value\": \"{(value == null ? "null" : value)}\"}}";
    }
}