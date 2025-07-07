//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;
using NeoKolors.Settings.Argument.Exception;
using NeoKolors.Settings.Attributes;

namespace NeoKolors.Settings.Argument;

[DisplayType("bool")]
public class BoolArgument : IArgument<bool>, IXsdArgument {
    public bool Value { get; private set; }
    public bool DefaultValue { get; }
    public BoolStringType AllowedStringType { get; set; }

    public BoolArgument(bool defaultValue = false, BoolStringType allowedStringType = BoolStringType.ALL) {
        DefaultValue = defaultValue;
        Value = DefaultValue;
        AllowedStringType = allowedStringType;
    }
    
    public void Set(object? value) {
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
                throw new InvalidArgumentInputTypeException(typeof(bool), value?.GetType());
        }
    }

    /// <summary>
    /// sets the argument using a string, all variations of true / false are allowed, e.g. TrUe, fALSe
    /// </summary>
    /// <exception cref="ArgumentInputFormatException">the string is not a valid boolean string</exception>
    public void Set(string s) {
        bool v;

        switch (AllowedStringType) {
            case BoolStringType.LOWER:
                if (!s.ContainsUpper()) 
                    throw new ArgumentInputFormatException(typeof(bool), s, "String contains uppercase characters.");
                break;
            case BoolStringType.UPPER:
                if (!s.ContainsLower()) 
                    throw new ArgumentInputFormatException(typeof(bool), s, "String contains lowercase characters.");
                break;
            case BoolStringType.CAPITAL:
                if (!s.FirstAndAll(char.IsUpper, char.IsLower)) 
                    throw new ArgumentInputFormatException(typeof(bool), s, "String is not a capital word.");
                break;
            case BoolStringType.ALL:
                break;
            default:
                throw new ArgumentInputFormatException(typeof(bool), s, "Invalid BoolStringType.");
        }
        
        s = s.ToLowerInvariant();
        s = s.CapitalizeFirst();
        
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
    public bool GetDefault() => DefaultValue;
    object IArgument.GetDefault() => GetDefault();
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

    public string ToXsd() =>
        """
        <xsd:simpleType>
            <xsd:restriction base="xsd:boolean"/>
        </xsd:simpleType>
        """;

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
    
    public enum BoolStringType {
        
        /// <summary>
        /// All characters must be lowercase.
        /// </summary>
        LOWER =   1,
        
        /// <summary>
        /// All characters must be uppercase.
        /// </summary>
        UPPER =   2,
        
        /// <summary>
        /// The first letter must be uppercase.
        /// </summary>
        CAPITAL = 3,

        /// <summary>
        /// Allows any string format regardless of casing.
        /// </summary>
        ALL = 4
    }
}