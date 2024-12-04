//
// NeoKolors
// by KryKom 2024
//

using System.Globalization;
using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Float Argument Type <br/>
/// float decimal number
/// </summary>
public sealed class FloatArgumentType : IArgumentType {

    private float? value;
    public float Min { get; }
    public float Max { get; }
    private float defaultValue;

    internal FloatArgumentType(float min = float.MinValue, float max = float.MaxValue, float defaultValue = 0) {
        Min = min;
        Max = max;
        this.defaultValue = defaultValue;
        value = defaultValue;
    }
    
    public string GetInputType() {
        return "Float";
    }
    
    public string GetStringValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return ((float)value).ToString(CultureInfo.InvariantCulture);
    }

    public object GetValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return value;
    }
    
    public void SetValue(object v) {
        if (v.GetType() != typeof(float)) throw new SettingsArgumentException(v.GetType(), typeof(float));
        if ((float)v < Min || (float)v > Max) 
            throw new SettingsArgumentException($"Value of {v} must be greater than {Min} and smaller than {Max}.");

        value = (float)v;
    }

    public IArgumentType Clone() {
        FloatArgumentType clone = new FloatArgumentType(Min, Max) {
            value = value
        };
        
        return clone;
    }

    public void Reset() {
        value = defaultValue;
    }

    public override string ToString() {
        return $"{{\"type\": \"float\", " +
               $"\"min\": {Min}, " +
               $"\"max\": {Max}, " +
               $"\"value\": {(value == null ? "\"null\"" : ((float)value).ToString(CultureInfo.InvariantCulture))}}}";
    }
}