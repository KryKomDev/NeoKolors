//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exception;

namespace NeoKolors.Settings.Argument;

public class IntegerArgument : IArgument<Int32> {
    public readonly int DefaultValue;
    public readonly int MinValue; 
    public readonly int MaxValue;
    
    /// <summary>
    /// custom validation function <br/>
    /// validates the value and if invalid returns the cause message, else null
    /// </summary>
    public readonly Func<int, string?>? CustomValidate;
    
    public int Value { get; private set; }

    public IntegerArgument(int min = Int32.MinValue, int max = Int32.MaxValue, int defaultValue = 0, Func<int, string?>? customValidate = null) {
        MinValue = Math.Min(min, max);
        MaxValue = Math.Max(min, max);
        DefaultValue = Math.Min(Math.Max(defaultValue, MinValue), MaxValue);
        CustomValidate = customValidate;
        Value = DefaultValue;
    }
    
    public void Set(object value) {
        if (value is Int32 i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            int v;
            try {
                v = Int32.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(Int32), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(Int32), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else if (value is IntegerArgument d) {
            Set(d.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(Int32), value.GetType());
        }
    }

    public void Set(int value) {
        Validate(value);
        Value = value;
    }
    public int Get() => Value;
    object IArgument.Get() => Get();
    public void Reset() => Value = DefaultValue;
    public IArgument<int> Clone() => (IArgument<int>)MemberwiseClone();
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
    public static IntegerArgument operator <<(IntegerArgument argument, int value) {
        argument.Set(value);
        return argument;
    }
    
    public static IntegerArgument operator +(IntegerArgument argument, int value) {
        argument.Set(argument.Value + value);
        return argument;
    }
    
    public static IntegerArgument operator +(int value, IntegerArgument argument) {
        argument.Set(value + argument.Value);
        return argument;
    }

    public static IntegerArgument operator -(IntegerArgument argument, int value) {
        argument.Set(argument.Value - value);
        return argument;
    }
    
    public static IntegerArgument operator -(int value, IntegerArgument argument) {
        argument.Set(value - argument.Value);
        return argument;
    }
    
    public static IntegerArgument operator *(IntegerArgument argument, int value) {
        argument.Set(argument.Value * value);
        return argument;
    }
    
    public static IntegerArgument operator *(int value, IntegerArgument argument) {
        argument.Set(value * argument.Value);
        return argument;
    }

    public static IntegerArgument operator /(IntegerArgument argument, int value) {
        argument.Set(argument.Value / value);
        return argument;
    }
    
    public static IntegerArgument operator /(int value, IntegerArgument argument) {
        argument.Set(value / argument.Value);
        return argument;
    }
    
    public static IntegerArgument operator %(IntegerArgument argument, int value) {
        argument.Set(argument.Value % value);
        return argument;
    }
    
    public static IntegerArgument operator %(int value, IntegerArgument argument) {
        argument.Set(value % argument.Value);
        return argument;
    }

    public static IntegerArgument operator ++(IntegerArgument argument) {
        argument.Set(argument.Value + 1);
        return argument;
    }

    public static IntegerArgument operator --(IntegerArgument argument) {
        argument.Set(argument.Value - 1);
        return argument;
    }
    
    public static implicit operator int(IntegerArgument argument) => argument.Value;
    public static implicit operator IntegerArgument(int value) => new() { Value = value };

    private void Validate(int value) {
        if (value < MinValue) throw new InvalidArgumentInputException($"Value was less then the smallest allowed value ({MinValue}).");
        if (value > MaxValue) throw new InvalidArgumentInputException($"Value was greater then the greatest allowed value ({MaxValue}).");
        string? res = CustomValidate?.Invoke(value);
        if (res != null) throw new InvalidArgumentInputException(res);
    }
    
    public override string ToString() => $"{{\"type\": \"int\", \"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
}

public class UIntegerArgument : IArgument<UInt32> {
    public readonly uint DefaultValue;
    public readonly uint MinValue; 
    public readonly uint MaxValue;
    
    /// <summary>
    /// custom validation function <br/>
    /// validates the value and if invalid returns the cause message, else null
    /// </summary>
    public readonly Func<uint, string?>? CustomValidate;
    
    public uint Value { get; private set; }

    public UIntegerArgument(uint min = UInt32.MinValue, uint max = UInt32.MaxValue, uint defaultValue = 0, Func<uint, string?>? customValidate = null) {
        MinValue = Math.Min(min, max);
        MaxValue = Math.Max(min, max);
        DefaultValue = Math.Min(Math.Max(defaultValue, MinValue), MaxValue);
        CustomValidate = customValidate;
        Value = DefaultValue;
    }
    
    public void Set(object value) {
        if (value is UInt32 i) {
            Validate(i);
            Value = i;
        }
        else if (value is string s) {
            uint v;
            try {
                v = UInt32.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(UInt32), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(UInt32), s, e.Message);
            }
            
            Validate(v);
            Value = v;
        }
        else if (value is UIntegerArgument d) {
            Set(d.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(UInt32), value.GetType());
        }
    }

    public void Set(uint value) {
        Validate(value);
        Value = value;
    }
    public uint Get() => Value;
    object IArgument.Get() => Get();
    public void Reset() => Value = DefaultValue;
    public IArgument<uint> Clone() => (IArgument<uint>)MemberwiseClone();
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
    public static UIntegerArgument operator <<(UIntegerArgument argument, uint value) {
        argument.Set(value);
        return argument;
    }

    public static UIntegerArgument operator +(UIntegerArgument argument, uint value) {
        argument.Set(argument.Value + value);
        return argument;
    }
    
    public static UIntegerArgument operator +(uint value, UIntegerArgument argument) {
        argument.Set(value + argument.Value);
        return argument;
    }

    public static UIntegerArgument operator -(UIntegerArgument argument, uint value) {
        argument.Set(argument.Value - value);
        return argument;
    }
    
    public static UIntegerArgument operator -(uint value, UIntegerArgument argument) {
        argument.Set(value - argument.Value);
        return argument;
    }
    
    public static UIntegerArgument operator *(UIntegerArgument argument, uint value) {
        argument.Set(argument.Value * value);
        return argument;
    }
    
    public static UIntegerArgument operator *(uint value, UIntegerArgument argument) {
        argument.Set(value * argument.Value);
        return argument;
    }

    public static UIntegerArgument operator /(UIntegerArgument argument, uint value) {
        argument.Set(argument.Value / value);
        return argument;
    }
    
    public static UIntegerArgument operator /(uint value, UIntegerArgument argument) {
        argument.Set(value / argument.Value);
        return argument;
    }
    
    public static UIntegerArgument operator %(UIntegerArgument argument, uint value) {
        argument.Set(argument.Value % value);
        return argument;
    }
    
    public static UIntegerArgument operator %(uint value, UIntegerArgument argument) {
        argument.Set(value % argument.Value);
        return argument;
    }

    public static UIntegerArgument operator ++(UIntegerArgument argument) {
        argument.Set(argument.Value + 1);
        return argument;
    }

    public static UIntegerArgument operator --(UIntegerArgument argument) {
        argument.Set(argument.Value - 1);
        return argument;
    }
    
    public static implicit operator uint(UIntegerArgument argument) => argument.Value;
    public static implicit operator UIntegerArgument(uint value) => new() { Value = value };
    
    private void Validate(uint value) {
        if (value < MinValue) throw new InvalidArgumentInputException($"Value was less then the smallest allowed value ({MinValue}).");
        if (value > MaxValue) throw new InvalidArgumentInputException($"Value was greater then the greatest allowed value ({MaxValue}).");
        string? res = CustomValidate?.Invoke(value);
        if (res != null) throw new InvalidArgumentInputException(res);
    }
    
    public override string ToString() => $"{{\"type\": \"uint\", \"value\": {Value}, \"default-value\": {DefaultValue}, \"min\": {MinValue}, \"max\": {MaxValue}}}";
}