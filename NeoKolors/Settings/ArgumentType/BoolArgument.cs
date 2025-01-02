//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings.Argument;

public class BoolArgument : IArgument<bool> {
    public bool Value { get; private set; }
    public bool DefaultValue { get; }

    public BoolArgument(bool defaultValue = false) {
        DefaultValue = defaultValue;
    }
    
    public void Set(object value) {
        if (value is Boolean i) {
            Value = i;
        }
        else if (value is string s) {
            bool v;
            try {
                v = Boolean.Parse(s);
            }
            catch (FormatException e) {
                throw new ArgumentInputFormatException(typeof(Boolean), s, e.Message);
            }
            catch (OverflowException e) {
                throw new ArgumentInputFormatException(typeof(Boolean), s, e.Message);
            }
            
            Value = v;
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(Boolean), value.GetType());
        }
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
    /// argument &lt;&lt;= 123;
    /// // argument.Value is now 123
    /// </code>
    /// </example>
    public static BoolArgument operator <<(BoolArgument argument, bool value) {
        argument.Set(value);
        return argument;
    }
    
    public static implicit operator bool(BoolArgument argument) => argument.Value;
    public static implicit operator BoolArgument(bool value) => new() { Value = value };
}