//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;
using NeoKolors.Settings.Attributes;

namespace NeoKolors.Settings.Argument;

[DisplayType("double")]
public class DoubleArgument : IArgument<double>, IXsdArgument {
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
    
    public void Set(object? value) {
        if (value is double i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            double v;
            try {
                v = double.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(double), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(double), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else if (value is DoubleArgument d) {
            Set(d.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(double), value?.GetType());
        }
    }

    public void Set(double value) {
        Validate(value);
        Value = value;
    }

    public void Set(string value) {
        double d;
        try {
            d = double.Parse(value);
        }
        catch (System.Exception e) {
            throw new ArgumentInputFormatException(typeof(double), value, e.Message);
        }
        
        Validate(d);
        Value = d;
    }

    public string ToXsd() =>
        $"""
         <xsd:simpleType>
             <xsd:restriction base="xsd:double">
                  <xsd:minInclusive value="{MinValue}"/>
                  <xsd:maxInclusive value="{MaxValue}"/>
             </xsd:restriction>
         </xsd:simpleType>
         """;

    public double Get() => Value;
    object IArgument.Get() => Get();
    public double GetDefault() => DefaultValue;
    object IArgument.GetDefault() => GetDefault();
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

    public bool Equals(IArgument? other) {
        const double tolerance = 0.00000000000001;
        return other is DoubleArgument d && 
               Math.Abs(Get() - d.Get()) < tolerance && 
               Math.Abs(DefaultValue - d.DefaultValue) < tolerance &&
               Math.Abs(MinValue - d.MinValue) < tolerance &&
               Math.Abs(MaxValue - d.MaxValue) < tolerance &&
               CustomValidate == d.CustomValidate;
    }
    
    public override string ToString() => $"{{\"type\": \"double\", \"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
    object ICloneable.Clone() => Clone();
}