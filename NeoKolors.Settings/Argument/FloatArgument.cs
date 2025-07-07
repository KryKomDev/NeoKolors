//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;
using NeoKolors.Settings.Attributes;

namespace NeoKolors.Settings.Argument;

[DisplayType("float")]
public class FloatArgument : IArgument<float>, IXsdArgument {
    public float MinValue { get; }
    public float MaxValue { get; }
    public float DefaultValue { get; }
    public Func<float, string?>? CustomValidate { get; }
    
    public float Value { get; private set; }

    public FloatArgument(float minValue = float.MinValue, float maxValue = float.MaxValue, float defaultValue = 0, Func<float, string?>? customValidate = null) {
        MinValue = minValue;
        MaxValue = maxValue;
        DefaultValue = defaultValue;
        CustomValidate = customValidate;
        Value = DefaultValue;
    }
    
    public void Set(object? value) {
        if (value is float i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            float v;
            try {
                v = float.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(float), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(float), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else if (value is FloatArgument d) {
            Set(d.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(float), value?.GetType());
        }
    }

    public void Set(float value) {
        Validate(value);
        Value = value;
    }

    public void Set(string value) {
        float f;
        try {
            f = float.Parse(value);
        }
        catch (System.Exception e) {
            throw new ArgumentInputFormatException(typeof(float), value, e.Message);
        }
        
        Validate(f);
        Value = f;
    }

    public string ToXsd() =>
        $"""
         <xsd:simpleType>
             <xsd:restriction base="xsd:float">
                  <xsd:minInclusive value="{MinValue}"/>
                  <xsd:maxInclusive value="{MaxValue}"/>
             </xsd:restriction>
         </xsd:simpleType>
         """;

    public float Get() => Value;
    object IArgument.Get() => Get();
    public float GetDefault() => DefaultValue;
    object IArgument.GetDefault() => GetDefault();
    public void Reset() => Value = DefaultValue;
    public IArgument<float> Clone() => (IArgument<float>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();

    /// <summary>
    /// sets the value of the argument without having to use the <see cref="Set(float)"/> method
    /// </summary>
    /// <example>
    /// <code>
    /// argument &lt;&lt;= 123;
    /// // argument.Value is now 123
    /// </code>
    /// </example>
    public static FloatArgument operator <<(FloatArgument argument, float value) {
        argument.Set(value);
        return argument;
    }
    
    public static FloatArgument operator +(FloatArgument argument, float value) {
        argument.Set(argument.Value + value);
        return argument;
    }
    
    public static FloatArgument operator +(float value, FloatArgument argument) {
        argument.Set(value + argument.Value);
        return argument;
    }

    public static FloatArgument operator -(FloatArgument argument, float value) {
        argument.Set(argument.Value - value);
        return argument;
    }
    
    public static FloatArgument operator -(float value, FloatArgument argument) {
        argument.Set(value - argument.Value);
        return argument;
    }
    
    public static FloatArgument operator *(FloatArgument argument, float value) {
        argument.Set(argument.Value * value);
        return argument;
    }
    
    public static FloatArgument operator *(float value, FloatArgument argument) {
        argument.Set(value * argument.Value);
        return argument;
    }

    public static FloatArgument operator /(FloatArgument argument, float value) {
        argument.Set(argument.Value / value);
        return argument;
    }
    
    public static FloatArgument operator /(float value, FloatArgument argument) {
        argument.Set(value / argument.Value);
        return argument;
    }
    
    public static FloatArgument operator %(FloatArgument argument, float value) {
        argument.Set(argument.Value % value);
        return argument;
    }
    
    public static FloatArgument operator %(float value, FloatArgument argument) {
        argument.Set(value % argument.Value);
        return argument;
    }

    public static FloatArgument operator ++(FloatArgument argument) {
        argument.Set(argument.Value + 1);
        return argument;
    }

    public static FloatArgument operator --(FloatArgument argument) {
        argument.Set(argument.Value - 1);
        return argument;
    }
    
    public static implicit operator float(FloatArgument argument) => argument.Value;
    public static implicit operator FloatArgument(float value) => new() { Value = value };

    private void Validate(float value) {
        if (value < MinValue) throw new InvalidArgumentInputException($"Value was less then the smallest allowed value ({MinValue}).");
        if (value > MaxValue) throw new InvalidArgumentInputException($"Value was greater then the greatest allowed value ({MaxValue}).");
        string? res = CustomValidate?.Invoke(value);
        if (res != null) throw new InvalidArgumentInputException(res);
    }

    public bool Equals(IArgument? other) {
        const float tolerance = 0.00000001f;
        return other is FloatArgument f && 
               Math.Abs(Get() - f.Get()) < tolerance && 
               Math.Abs(DefaultValue - f.DefaultValue) < tolerance &&
               Math.Abs(MinValue - f.MinValue) < tolerance &&
               Math.Abs(MaxValue - f.MaxValue) < tolerance &&
               CustomValidate == f.CustomValidate;
    }
    
    public override string ToString() => $"{{\"type\": \"float\", \"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
    object ICloneable.Clone() => Clone();
}