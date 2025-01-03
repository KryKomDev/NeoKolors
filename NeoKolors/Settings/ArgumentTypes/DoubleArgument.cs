//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings.ArgumentTypes;

public class DoubleArgument : IArgument<Double> {
    public double MinValue { get; }
    public double MaxValue { get; }
    public double DefaultValue { get; }
    public Func<double, string?>? CustomValidate { get; }
    
    public double Value { get; private set; }

    public DoubleArgument(double minValue = double.MinValue, double maxValue = double.MaxValue, double defaultValue = 0, Func<double, string?>? customValidate = null) {
        MinValue = minValue;
        MaxValue = maxValue;
        DefaultValue = defaultValue;
        CustomValidate = customValidate;
        Value = DefaultValue;
    }
    
    public void Set(object value) {
        if (value is Double i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            double v;
            try {
                v = Double.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(Double), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(Double), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(Double), value.GetType());
        }
    }

    public void Set(double value) => Value = value;
    public double Get() => Value;
    object IArgument.Get() => Get();
    public void Reset() => Value = DefaultValue;
    public IArgument<double> Clone() => (IArgument<double>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();

    /// <summary>
    /// sets the value of the argument without having to use the <see cref="Set(double)"/> method
    /// </summary>
    /// <example>
    /// <code>
    /// argument &lt;&lt;= 123;
    /// // argument.Value is now 123
    /// </code>
    /// </example>
    public static DoubleArgument operator <<(DoubleArgument argument, double value) {
        argument.Set(value);
        return argument;
    }
    
    public static DoubleArgument operator +(DoubleArgument argument, double value) {
        argument.Set(argument.Value + value);
        return argument;
    }
    
    public static DoubleArgument operator +(double value, DoubleArgument argument) {
        argument.Set(value + argument.Value);
        return argument;
    }

    public static DoubleArgument operator -(DoubleArgument argument, double value) {
        argument.Set(argument.Value - value);
        return argument;
    }
    
    public static DoubleArgument operator -(double value, DoubleArgument argument) {
        argument.Set(value - argument.Value);
        return argument;
    }
    
    public static DoubleArgument operator *(DoubleArgument argument, double value) {
        argument.Set(argument.Value * value);
        return argument;
    }
    
    public static DoubleArgument operator *(double value, DoubleArgument argument) {
        argument.Set(value * argument.Value);
        return argument;
    }

    public static DoubleArgument operator /(DoubleArgument argument, double value) {
        argument.Set(argument.Value / value);
        return argument;
    }
    
    public static DoubleArgument operator /(double value, DoubleArgument argument) {
        argument.Set(value / argument.Value);
        return argument;
    }
    
    public static DoubleArgument operator %(DoubleArgument argument, double value) {
        argument.Set(argument.Value % value);
        return argument;
    }
    
    public static DoubleArgument operator %(double value, DoubleArgument argument) {
        argument.Set(value % argument.Value);
        return argument;
    }

    public static DoubleArgument operator ++(DoubleArgument argument) {
        argument.Set(argument.Value + 1);
        return argument;
    }

    public static DoubleArgument operator --(DoubleArgument argument) {
        argument.Set(argument.Value - 1);
        return argument;
    }
    
    public static implicit operator double(DoubleArgument argument) => argument.Value;
    public static implicit operator DoubleArgument(double value) => new() { Value = value };

    private void Validate(double value) {
        if (value < MinValue) throw new InvalidArgumentInputException($"Value was less then the smallest allowed value ({MinValue}).");
        if (value > MaxValue) throw new InvalidArgumentInputException($"Value was greater then the greatest allowed value ({MaxValue}).");
        string? res = CustomValidate?.Invoke(value);
        if (res != null) throw new InvalidArgumentInputException(res);
    }

    public override string ToString() => $"{{\"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
}