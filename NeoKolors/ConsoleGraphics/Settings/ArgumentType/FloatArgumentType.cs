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

    private float value;
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
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public object GetValue() {
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
               $"\"value\": {value.ToString(CultureInfo.InvariantCulture)}}}";
    }
    
    public static implicit operator float(FloatArgumentType arg) => arg.value;
    public static float operator +(FloatArgumentType a, FloatArgumentType b) => a.value + b.value;
    public static float operator -(FloatArgumentType a, FloatArgumentType b) => a.value - b.value;
    public static float operator *(FloatArgumentType a, FloatArgumentType b) => a.value * b.value;
    public static float operator /(FloatArgumentType a, FloatArgumentType b) => a.value / b.value;
    public static float operator %(FloatArgumentType a, FloatArgumentType b) => a.value % b.value;

    public static FloatArgumentType operator ++(FloatArgumentType arg) {
        arg.value++;
        return arg;
    }

    public static FloatArgumentType operator --(FloatArgumentType arg) {
        arg.value--;
        return arg;
    }
}