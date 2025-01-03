//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings.ArgumentTypes;

public class LongArgument : IArgument<Int64> {
    public readonly long DefaultValue;
    public readonly long MinValue; 
    public readonly long MaxValue;
    
    /// <summary>
    /// custom validation function <br/>
    /// validates the value and if invalid returns the cause message, else null
    /// </summary>
    public readonly Func<long, string?>? CustomValidate;
    
    public long Value { get; private set; }

    public LongArgument(long min = Int64.MinValue, long max = Int64.MaxValue, long defaultValue = 0, Func<long, string?>? customValidate = null) {
        MinValue = Math.Min(min, max);
        MaxValue = Math.Max(min, max);
        DefaultValue = Math.Min(Math.Max(defaultValue, MinValue), MaxValue);
        CustomValidate = customValidate;
        Value = DefaultValue;
    }
    
    public void Set(object value) {
        if (value is Int64 i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            long v;
            try {
                v = Int64.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(Int64), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(Int64), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(Int64), value.GetType());
        }
    }
    
    public long Get() => Value;
    object IArgument.Get() => Get();
    public void Reset() => Value = DefaultValue;
    public IArgument<long> Clone() => (IArgument<long>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();

    /// <summary>
    /// sets the value of the argument without having to use the <see cref="Set"/> method
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
    
    public override string ToString() => $"{{\"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
}

public class ULongArgument : IArgument<UInt64> {
    public readonly ulong DefaultValue;
    public readonly ulong MinValue; 
    public readonly ulong MaxValue;
    
    /// <summary>
    /// custom validation function <br/>
    /// validates the value and if invalid returns the cause message, else null
    /// </summary>
    public readonly Func<ulong, string?>? CustomValidate;
    
    public ulong Value { get; private set; }

    public ULongArgument(ulong min = UInt64.MinValue, ulong max = UInt64.MaxValue, ulong defaultValue = 0, Func<ulong, string?>? customValidate = null) {
        MinValue = Math.Min(min, max);
        MaxValue = Math.Max(min, max);
        DefaultValue = Math.Min(Math.Max(defaultValue, MinValue), MaxValue);
        CustomValidate = customValidate;
        Value = DefaultValue;
    }
    
    public void Set(object value) {
        if (value is UInt64 i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            ulong v;
            try {
                v = UInt64.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(UInt64), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(UInt64), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(UInt64), value.GetType());
        }
    }
    
    public ulong Get() => Value;
    object IArgument.Get() => Get();
    public void Reset() => Value = DefaultValue;
    public IArgument<ulong> Clone() => (IArgument<ulong>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();

    /// <summary>
    /// sets the value of the argument without having to use the <see cref="Set"/> method
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
    
    public override string ToString() => $"{{\"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
}