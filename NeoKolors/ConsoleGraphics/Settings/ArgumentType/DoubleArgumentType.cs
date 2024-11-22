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

    private double? value;
    public double Min { get; }
    public double Max { get; }

    internal DoubleArgumentType(double min = double.MinValue, double max = double.MaxValue) {
        Min = min;
        Max = max;
    }
    
    public string GetInputType() {
        return "Double";
    }

    public string GetStringValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

        return ((double)value).ToString(CultureInfo.InvariantCulture);
    }

    public object GetValue() {
        if (value == null) throw new SettingsBuilderException("Accessing value that has not been set.");

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

    public override string ToString() {
        return $"{{\"type\": \"double\", " +
               $"\"min\": {Min}, " +
               $"\"max\": {Max}, " +
               $"\"value\": {(value == null ? "\"null\"" : ((float)value).ToString(CultureInfo.InvariantCulture))}}}";
    }
}