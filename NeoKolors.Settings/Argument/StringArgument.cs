//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text.RegularExpressions;
using NeoKolors.Common.Util;
using NeoKolors.Settings.Exception;
using static NeoKolors.Settings.Argument.StringFeatures;

namespace NeoKolors.Settings.Argument;

public class StringArgument : IArgument<string> {
    public readonly uint MinLength;
    public readonly uint MaxLength;
    public readonly bool AllowSpaces;
    public readonly bool AllowNewlines;
    public readonly bool AllowSpecial;
    public readonly bool AllowNumbers;
    public readonly bool AllowUpper;
    public readonly bool AllowLower;
    public readonly bool CountVisibleOnly;
    public readonly string DefaultValue;
    public readonly Func<string, string?>? CustomValidate;

    public StringArgument(
        uint minLength = 0, 
        uint maxLength = UInt32.MaxValue, 
        string defaultValue = "", 
        bool allowSpaces = true, 
        bool allowNewlines = true,
        bool allowSpecial = true,
        bool allowNumbers = true, 
        bool allowUpper = true, 
        bool allowLower = true,
        bool countVisibleOnly = false,
        Func<string, string?>? customValidate = null) 
    {
        MinLength = Math.Min(minLength, maxLength);
        MaxLength = Math.Max(minLength, maxLength);
        DefaultValue = defaultValue;
        AllowSpaces = allowSpaces;
        AllowNewlines = allowNewlines;
        AllowSpecial = allowSpecial;
        AllowNumbers = allowNumbers;
        AllowUpper = allowUpper;
        AllowLower = allowLower;
        CountVisibleOnly = countVisibleOnly;
        CustomValidate = customValidate;
        Value = "";
        if (defaultValue != "") Set(defaultValue);
    }

    public StringArgument(
        uint minLength = 0,
        uint maxLength = UInt32.MaxValue,
        string defaultValue = "",
        StringFeatures allowedStringFeatures = ALL,
        bool countVisibleOnly = false,
        Func<string, string?>? customValidate = null) 
    {
        MinLength = Math.Min(minLength, maxLength);
        MaxLength = Math.Max(minLength, maxLength);
        DefaultValue = defaultValue;
        AllowSpaces = allowedStringFeatures.HasFlag(SPACES);
        AllowNewlines = allowedStringFeatures.HasFlag(NEWLINES);
        AllowSpecial = allowedStringFeatures.HasFlag(SPECIAL);
        AllowNumbers = allowedStringFeatures.HasFlag(NUMBERS);
        AllowUpper = allowedStringFeatures.HasFlag(UPPER);
        AllowLower = allowedStringFeatures.HasFlag(LOWER);
        CountVisibleOnly = countVisibleOnly;
        CustomValidate = customValidate;
        Value = "";
        if (defaultValue != "") Set(defaultValue);
    }

    public StringArgument() {
        MinLength = 0;
        MaxLength = uint.MaxValue;
        DefaultValue = "";
        AllowSpaces = true;
        AllowNewlines = true;
        AllowSpecial = true;
        AllowNumbers = true;
        AllowUpper = true;
        AllowLower = true;
        CountVisibleOnly = false;
        CustomValidate = null;
        Value = "";
    }
    
    public string Value { get; private set; }
    
    public void Set(object value) {
        if (value is string s) {
            Validate(s);
            Value = s;
        }
        else if (value is StringArgument a) {
            Set(a.Value);
        } 
        else {
            string? str = value.ToString();
            if (str is null) throw new InvalidArgumentInputException("ToString method of input value returned null.");
            Validate(str);
            Value = str;
        }
    }

    public void Set(string value) {
        Validate(value);
        Value = value;
    }
    public string Get() => Value;
    object IArgument.Get() => Get();
    public void Reset() => Value = DefaultValue;
    public IArgument<string> Clone() => (IArgument<string>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();
    
    private void Validate(string value) {
        if (CountVisibleOnly) {
            if (value.VisibleLength() < MinLength) throw new InvalidArgumentInputException($"Input value length is too small (must be at least {MinLength}).");
            if (value.VisibleLength() > MaxLength) throw new InvalidArgumentInputException($"Input value length is too big (must be at most {MaxLength}).");
        }
        else {
            if (value.Length < MinLength) throw new InvalidArgumentInputException($"Input value length is too small (must be at least {MinLength}).");
            if (value.Length > MaxLength) throw new InvalidArgumentInputException($"Input value length is too big (must be at most {MaxLength}).");
        }
        
        if (!AllowUpper && Regex.IsMatch(value, "[A-Z]")) throw new InvalidArgumentInputException("Input value contains uppercase characters.");
        if (!AllowLower && Regex.IsMatch(value, "[a-z]")) throw new InvalidArgumentInputException("Input value contains lower case characters.");
        if (!AllowNumbers && Regex.IsMatch(value, "[0-9]")) throw new InvalidArgumentInputException("Input value contains number characters.");
        if (!AllowSpecial && Regex.IsMatch(value, "[^a-zA-Z0-9 \n]")) throw new InvalidArgumentInputException("Input value contains special characters.");
        if (!AllowNewlines && value.Contains('\n')) throw new InvalidArgumentInputException("Input value contains newline characters.");
        if (!AllowSpaces && value.Contains(' ')) throw new InvalidArgumentInputException("Input value contains spaces.");
    }

    public bool IsValid(char c) {
        if (!AllowUpper && c is >= 'A' and <= 'Z') return false;
        if (!AllowLower && c is >= 'a' and <= 'z') return false;
        if (!AllowNumbers && c is >= '0' and <= '9') return false;
        if (!AllowNewlines && c is '\n') return false;
        if (!AllowSpaces && c is ' ') return false;
        if (!AllowSpecial && c is < '0' or > '9' and < 'A' or > 'Z' and < 'a' or > 'z' and not '\n' and not ' ') return false;
        
        return true;
    }

    public bool IsValid(string value) {
        if (CountVisibleOnly) {
            if (value.VisibleLength() < MinLength) return false;
            if (value.VisibleLength() > MaxLength) return false;
        }
        else {
            if (value.Length < MinLength) return false;
            if (value.Length > MaxLength) return false;
        }
        
        if (!AllowUpper && Regex.IsMatch(value, "[A-Z]")) return false;
        if (!AllowLower && Regex.IsMatch(value, "[a-z]")) return false;
        if (!AllowNumbers && Regex.IsMatch(value, "[0-9]")) return false;
        if (!AllowSpecial && Regex.IsMatch(value, "[^a-zA-Z0-9 \n]")) return false;
        if (!AllowNewlines && value.Contains('\n')) return false;
        if (!AllowSpaces && value.Contains(' ')) return false;
        
        return true;
    }

    /// <summary>
    /// sets the value of the argument without having to use the <see cref="Set"/> method
    /// </summary>
    /// <example>
    /// <code>
    /// argument &lt;&lt;= 123;
    /// // argument.Value is now 123
    /// </code>
    /// </example>
    public static StringArgument operator <<(StringArgument argument, object value) {
        argument.Set(value);
        return argument;
    }

    public static StringArgument operator +(StringArgument argument, object value) {
        argument.Set(argument.Value + value);
        return argument;
    }

    public static StringArgument operator +(object value, StringArgument argument) {
        argument.Set(value + argument.Value);
        return argument;
    }

    public static implicit operator string(StringArgument argument) {
        return argument.Value;
    }

    public static implicit operator StringArgument(string value) {
        var argument = new StringArgument();
        argument.Set(value);
        return argument;
    }

    public bool Equals(IArgument? other) {
        return other is StringArgument s &&
               Value == s.Value &&
               DefaultValue == s.DefaultValue &&
               MinLength == s.MinLength &&
               MaxLength == s.MaxLength &&
               AllowSpaces == s.AllowSpaces &&
               AllowNewlines == s.AllowNewlines &&
               AllowSpecial == s.AllowSpecial &&
               AllowNumbers == s.AllowNumbers &&
               AllowUpper == s.AllowUpper &&
               AllowLower == s.AllowLower &&
               CountVisibleOnly == s.CountVisibleOnly;
    }

    public override string ToString() => $"{{\"value\": {Value}, " +
                                         $"\"default-value\": {DefaultValue}, " +
                                         $"\"min-length\": {MinLength}, " +
                                         $"\"max-length\": {MaxLength}, " +
                                         $"\"allow-spaces\": {AllowSpaces}, " +
                                         $"\"allow-newlines\": {AllowNewlines}, " +
                                         $"\"allow-special\": {AllowSpecial}, " +
                                         $"\"allow-numbers\": {AllowNumbers}, " +
                                         $"\"allow-upper\": {AllowUpper}, " +
                                         $"\"allow-lower\": {AllowLower}, " +
                                         $"\"count-visible-only\": {CountVisibleOnly}}}";

    object ICloneable.Clone() => Clone();
}

/// <summary>
/// specifies what features of a string are allowed
/// </summary>
[Flags]
public enum StringFeatures {
    ALL = SPACES | NEWLINES | SPECIAL | NUMBERS | UPPER | LOWER,
    SPACES = 1 << 0,
    NO_SPACES = ALL ^ SPACES,
    NEWLINES = 1 << 1,
    NO_NEWLINES = ALL ^ NEWLINES,
    SPECIAL = 1 << 2,
    NO_SPECIAL = ALL ^ SPECIAL,
    NUMBERS = 1 << 3,
    NO_NUMBERS = ALL ^ NUMBERS,
    UPPER = 1 << 4,
    NO_UPPER = ALL ^ UPPER,
    LOWER = 1 << 5,
    NO_LOWER = ALL ^ LOWER,
}