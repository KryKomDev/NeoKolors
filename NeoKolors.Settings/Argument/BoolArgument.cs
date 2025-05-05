//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exception;

namespace NeoKolors.Settings.Argument;

public class BoolArgument : IArgument<bool> {
    public bool Value { get; private set; }
    public bool DefaultValue { get; }

    public BoolArgument(bool defaultValue = false) {
        DefaultValue = defaultValue;
        Value = DefaultValue;
        
    }
    
    public void Set(object value) {
        switch (value) {
            case bool i:
                Value = i;
                break;
            case string s: {
                Set(s);
                break;
            }
            case BoolArgument b:
                Value = b.Value;
                break;
            default:
                throw new InvalidArgumentInputTypeException(typeof(Boolean), value.GetType());
        }
    }

    /// <summary>
    /// sets the argument using a string, all variations of true / false are allowed, e.g. TrUe, fALSe
    /// </summary>
    /// <exception cref="ArgumentInputFormatException">the string is not a valid boolean string</exception>
    public void Set(string s) {
        bool v;

        s = s.ToLowerInvariant();
        // s = s.CapitalizeFirst(); TODO: uncomment this
        
        try {
            v = bool.Parse(s);
        }
        catch (FormatException e) {
            throw new ArgumentInputFormatException(typeof(bool), s, e.Message);
        }
        catch (OverflowException e) {
            throw new ArgumentInputFormatException(typeof(bool), s, e.Message);
        }
            
        Value = v;
    }

    public void Set(bool value) => Value = value;
    public bool Get() => Value;
    object IArgument.Get() => Get();
    public void Reset() => Value = DefaultValue;
    public IArgument<bool> Clone() => (IArgument<bool>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();

    /// <summary>
    /// sets the value of the argument without having to use the <see cref="Set(bool)"/> method
    /// </summary>
    /// <example>
    /// <code>
    /// argument &lt;&lt;= true;
    /// // argument.Value is now true
    /// </code>
    /// </example>
    public static BoolArgument operator <<(BoolArgument argument, bool value) {
        argument.Set(value);
        return argument;
    }
    
    public static implicit operator bool(BoolArgument argument) => argument.Value;
    public static implicit operator BoolArgument(bool value) => new() { Value = value };

    public bool Equals(IArgument? other) {
        return other is BoolArgument b && Get() == b.Get() && DefaultValue == b.DefaultValue;
    }

    public override string ToString() => 
        $"{{\"type\": \"bool\", \"value\": {Value}, \"default-value\": {DefaultValue}}}";

    object ICloneable.Clone() => Clone();

    /// <summary>
    /// creates a new bool argument from a bool string (e.g. true, false, tRuE...)
    /// </summary>
    /// <returns></returns>
    public static BoolArgument Parse(string value) {
        BoolArgument argument = new();
        argument.Set(value);
        return argument;
    }
}