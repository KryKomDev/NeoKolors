//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;
using NeoKolors.Settings.Attributes;

namespace NeoKolors.Settings.Argument;

[DisplayType("long")]
public class LongArgument : IArgument<long>, IXsdArgument {
    public readonly long DefaultValue;
    public readonly long MinValue; 
    public readonly long MaxValue;
    
    /// <summary>
    /// custom validation function <br/>
    /// validates the value, and if invalid returns the cause message, else null
    /// </summary>
    public readonly Func<long, string?>? CustomValidate;
    
    public long Value { get; private set; }

    public LongArgument(long min = long.MinValue, long max = long.MaxValue, long defaultValue = 0, Func<long, string?>? customValidate = null) {
        MinValue = Math.Min(min, max);
        MaxValue = Math.Max(min, max);
        DefaultValue = Math.Min(Math.Max(defaultValue, MinValue), MaxValue);
        CustomValidate = customValidate;
        Value = DefaultValue;
    }
    
    public void Set(object? value) {
        if (value is long i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            long v;
            try {
                v = long.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(long), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(long), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else if (value is LongArgument d) {
            Set(d.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(long), value?.GetType());
        }
    }

    public void Set(long value) {
        Validate(value);
        Value = value;
    }
    
    public void Set(string value) {
        long l;
        try {
            l = long.Parse(value);
        }
        catch (System.Exception e) {
            throw new ArgumentInputFormatException(typeof(int), value, e.Message);
        }
        
        Validate(l);
        Value = l;
    }
    
    public string ToXsd() =>
        $"""
         <xsd:simpleType>
             <xsd:restriction base="xsd:long">
                 <xsd:minInclusive value="{MinValue}"/>
                 <xsd:maxInclusive value="{MaxValue}"/>
             </xsd:restriction>
         </xsd:simpleType>
         """;
    
    public long Get() => Value;
    object IArgument.Get() => Get();
    public long GetDefault() => DefaultValue;
    object IArgument.GetDefault() => GetDefault();
    public void Reset() => Value = DefaultValue;
    public IArgument<long> Clone() => (IArgument<long>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();

    /// <summary>
    /// sets the value of the argument without having to use the <see cref="Set(long)"/> method
    /// </summary>
    /// <example>
    /// <code>
    /// argument &lt;&lt;= 123;
    /// // argument.Value is now 123
    /// </code>
    /// </example>
    public static LongArgument operator <<(LongArgument argument, long value) {
        argument.Set(value);
        return argument;
    }
    
    public static LongArgument operator +(LongArgument argument, long value) {
        argument.Set(argument.Value + value);
        return argument;
    }
    
    public static LongArgument operator +(long value, LongArgument argument) {
        argument.Set(value + argument.Value);
        return argument;
    }

    public static LongArgument operator -(LongArgument argument, long value) {
        argument.Set(argument.Value - value);
        return argument;
    }
    
    public static LongArgument operator -(long value, LongArgument argument) {
        argument.Set(value - argument.Value);
        return argument;
    }
    
    public static LongArgument operator *(LongArgument argument, long value) {
        argument.Set(argument.Value * value);
        return argument;
    }
    
    public static LongArgument operator *(long value, LongArgument argument) {
        argument.Set(value * argument.Value);
        return argument;
    }

    public static LongArgument operator /(LongArgument argument, long value) {
        argument.Set(argument.Value / value);
        return argument;
    }
    
    public static LongArgument operator /(long value, LongArgument argument) {
        argument.Set(value / argument.Value);
        return argument;
    }
    
    public static LongArgument operator %(LongArgument argument, long value) {
        argument.Set(argument.Value % value);
        return argument;
    }
    
    public static LongArgument operator %(long value, LongArgument argument) {
        argument.Set(value % argument.Value);
        return argument;
    }

    public static LongArgument operator ++(LongArgument argument) {
        argument.Set(argument.Value + 1);
        return argument;
    }

    public static LongArgument operator --(LongArgument argument) {
        argument.Set(argument.Value - 1);
        return argument;
    }
    
    public static implicit operator long(LongArgument argument) => argument.Value;
    public static implicit operator LongArgument(long value) => new() { Value = value };
    
    private void Validate(long value) {
        if (value < MinValue) throw new InvalidArgumentInputException($"Value was less then the smallest allowed value ({MinValue}).");
        if (value > MaxValue) throw new InvalidArgumentInputException($"Value was greater then the greatest allowed value ({MaxValue}).");
        string? res = CustomValidate?.Invoke(value);
        if (res != null) throw new InvalidArgumentInputException(res);
    }
    
    public bool Equals(IArgument? other) {
        return other is LongArgument i &&
               Get() == i.Get() &&
               DefaultValue == i.DefaultValue &&
               MinValue == i.MinValue &&
               MaxValue == i.MaxValue &&
               CustomValidate == i.CustomValidate;
    }
    
    public override string ToString() => $"{{\"type\": \"long\", \"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
    object ICloneable.Clone() => Clone();
}

[DisplayType("ulong")]
public class ULongArgument : IArgument<ulong>, IXsdArgument {
    public readonly ulong DefaultValue;
    public readonly ulong MinValue; 
    public readonly ulong MaxValue;
    
    /// <summary>
    /// custom validation function <br/>
    /// validates the value and if invalid returns the cause message, else null
    /// </summary>
    public readonly Func<ulong, string?>? CustomValidate;
    
    public ulong Value { get; private set; }

    public ULongArgument(ulong min = ulong.MinValue, ulong max = ulong.MaxValue, ulong defaultValue = 0, Func<ulong, string?>? customValidate = null) {
        MinValue = Math.Min(min, max);
        MaxValue = Math.Max(min, max);
        DefaultValue = Math.Min(Math.Max(defaultValue, MinValue), MaxValue);
        CustomValidate = customValidate;
        Value = DefaultValue;
    }
    
    public void Set(object? value) {
        if (value is ulong i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            ulong v;
            try {
                v = ulong.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(ulong), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(ulong), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else if (value is ULongArgument d) {
            Set(d.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(ulong), value?.GetType());
        }
    }

    public void Set(ulong value) {
        Validate(value);
        Value = value;
    }
    
    public void Set(string value) {
        ulong l;
        try {
            l = ulong.Parse(value);
        }
        catch (System.Exception e) {
            throw new ArgumentInputFormatException(typeof(int), value, e.Message);
        }
        
        Validate(l);
        Value = l;
    }
    
    public string ToXsd() =>
        $"""
         <xsd:simpleType>
             <xsd:restriction base="xsd:unsignedLong">
                 <xsd:minInclusive value="{MinValue}"/>
                 <xsd:maxInclusive value="{MaxValue}"/>
             </xsd:restriction>
         </xsd:simpleType>
         """;
    
    public ulong Get() => Value;
    object IArgument.Get() => Get();
    public ulong GetDefault() => DefaultValue;
    object IArgument.GetDefault() => GetDefault();
    public void Reset() => Value = DefaultValue;
    public IArgument<ulong> Clone() => (IArgument<ulong>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();

    /// <summary>
    /// sets the value of the argument without having to use the <see cref="Set(ulong)"/> method
    /// </summary>
    /// <example>
    /// <code>
    /// argument &lt;&lt;= 123;
    /// // argument.Value is now 123
    /// </code>
    /// </example>
    public static ULongArgument operator <<(ULongArgument argument, ulong value) {
        argument.Set(value);
        return argument;
    }
    
    public static ULongArgument operator +(ULongArgument argument, ulong value) {
        argument.Set(argument.Value + value);
        return argument;
    }
    
    public static ULongArgument operator +(ulong value, ULongArgument argument) {
        argument.Set(value + argument.Value);
        return argument;
    }

    public static ULongArgument operator -(ULongArgument argument, ulong value) {
        argument.Set(argument.Value - value);
        return argument;
    }
    
    public static ULongArgument operator -(ulong value, ULongArgument argument) {
        argument.Set(value - argument.Value);
        return argument;
    }
    
    public static ULongArgument operator *(ULongArgument argument, ulong value) {
        argument.Set(argument.Value * value);
        return argument;
    }
    
    public static ULongArgument operator *(ulong value, ULongArgument argument) {
        argument.Set(value * argument.Value);
        return argument;
    }

    public static ULongArgument operator /(ULongArgument argument, ulong value) {
        argument.Set(argument.Value / value);
        return argument;
    }
    
    public static ULongArgument operator /(ulong value, ULongArgument argument) {
        argument.Set(value / argument.Value);
        return argument;
    }
    
    public static ULongArgument operator %(ULongArgument argument, ulong value) {
        argument.Set(argument.Value % value);
        return argument;
    }
    
    public static ULongArgument operator %(ulong value, ULongArgument argument) {
        argument.Set(value % argument.Value);
        return argument;
    }

    public static ULongArgument operator ++(ULongArgument argument) {
        argument.Set(argument.Value + 1);
        return argument;
    }

    public static ULongArgument operator --(ULongArgument argument) {
        argument.Set(argument.Value - 1);
        return argument;
    }
    
    public static implicit operator ulong(ULongArgument argument) => argument.Value;
    public static implicit operator ULongArgument(ulong value) => new() { Value = value };
    
    private void Validate(ulong value) {
        if (value < MinValue) throw new InvalidArgumentInputException($"Value was less then the smallest allowed value ({MinValue}).");
        if (value > MaxValue) throw new InvalidArgumentInputException($"Value was greater then the greatest allowed value ({MaxValue}).");
        string? res = CustomValidate?.Invoke(value);
        if (res != null) throw new InvalidArgumentInputException(res);
    }
    
    public bool Equals(IArgument? other) {
        return other is ULongArgument i &&
               Get() == i.Get() &&
               DefaultValue == i.DefaultValue &&
               MinValue == i.MinValue &&
               MaxValue == i.MaxValue &&
               CustomValidate == i.CustomValidate;
    }
    
    public override string ToString() => $"{{\"type\": \"ulong\", \"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
    object ICloneable.Clone() => Clone();
}