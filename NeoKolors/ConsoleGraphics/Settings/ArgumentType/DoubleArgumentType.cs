//
// NeoKolors
// by KryKom 2024
//

using System.Globalization;
using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// Double Argument Type <br/>
/// double decimal number
/// </summary>
public sealed class DoubleArgumentType : IArgumentType {

    private double value;
    public double Min { get; }
    public double Max { get; }
    private double defaultValue;

    internal DoubleArgumentType(double min = double.MinValue, double max = double.MaxValue, double defaultValue = 0) {
        Min = min;
        Max = max;
        this.defaultValue = defaultValue;
        value = defaultValue;
    }
    
    public string GetInputType() {
        return "Double";
    }

    public string GetStringValue() {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public object GetValue() {
        return value;
    }


    public void SetValue(object v) {
        if (v.GetType() != typeof(double)) throw new SettingsArgumentException(v.GetType(), typeof(double));
        if ((double)v < Min || (double)v > Max) 
            throw new SettingsArgumentException($"Value of {v} must be greater than {Min} and smaller than {Max}.");

        value = (double)v;
    }

    public IArgumentType Clone() {
        DoubleArgumentType newArg = new DoubleArgumentType(Min, Max) {
            value = value
        };
        
        return newArg;
    }

    public void Reset() {
        value = defaultValue;
    }

    public override string ToString() {
        return $"{{\"type\": \"double\", " +
               $"\"min\": {Min}, " +
               $"\"max\": {Max}, " +
               $"\"value\": {((float)value).ToString(CultureInfo.InvariantCulture)}}}";
    }

    public static implicit operator double(DoubleArgumentType arg) => arg.value;
    public static double operator +(DoubleArgumentType a, DoubleArgumentType b) => a.value + b.value;
    public static double operator -(DoubleArgumentType a, DoubleArgumentType b) => a.value - b.value;
    public static double operator *(DoubleArgumentType a, DoubleArgumentType b) => a.value * b.value;
    public static double operator /(DoubleArgumentType a, DoubleArgumentType b) => a.value / b.value;
    public static double operator %(DoubleArgumentType a, DoubleArgumentType b) => a.value % b.value;

    public static DoubleArgumentType operator ++(DoubleArgumentType arg) {
        arg.value++;
        return arg;
    }

    public static DoubleArgumentType operator --(DoubleArgumentType arg) {
        arg.value--;
        return arg;
    }
}