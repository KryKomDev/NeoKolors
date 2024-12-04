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

    private bool value;
    private bool defaultValue;

    internal BoolArgumentType(bool defaultValue = false) {
        this.defaultValue = defaultValue;
        value = defaultValue;
    }
    
    public string GetInputType() {
        return "Bool";
    }

    public string GetStringValue() {
        return value.ToString();
    }

    public object GetValue() {
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

    public void Reset() {
        value = defaultValue;
    }

    public override string ToString() {
        return $"{{\"type\": \"bool\", \"value\": \"{value}\"}}";
    }
}