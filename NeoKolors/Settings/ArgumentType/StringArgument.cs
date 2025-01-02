//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using System.Text.RegularExpressions;
using NeoKolors.Common;
using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings.Argument;

public partial class StringArgument : IArgument<string> {
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
        bool countVisibleOnly = true,
        Func<string, string?>? customValidate = null) 
    {
        MinLength = Math.Min(minLength, maxLength);
        MaxLength = Math.Max(minLength, maxLength);
        DefaultValue = defaultValue;
        Value = DefaultValue;
        AllowSpaces = allowSpaces;
        AllowNewlines = allowNewlines;
        AllowSpecial = allowSpecial;
        AllowNumbers = allowNumbers;
        AllowUpper = allowUpper;
        AllowLower = allowLower;
        CountVisibleOnly = countVisibleOnly;
        CustomValidate = customValidate;
    }
    
    public string Value { get; private set; }
    
    public void Set(object value) {
        if (value is string s) {
            Validate(s);
            Value = s;
        }
        else {
            string? str = value.ToString();
            if (str is null) throw new InvalidArgumentInputException("ToString method of input value returned null.");
            Validate(str);
            Value = str;
        }
    }

    public string Get() => Value;
    object IArgument.Get() => Get();
    public void Reset() => Value = DefaultValue;
    public IArgument<string> Clone() => (IArgument<string>)MemberwiseClone();
    IArgument IArgument.Clone() => Clone();

    [GeneratedRegex("[0-9]")]
    private static partial Regex NUMERIC_REGEX();
    
    [GeneratedRegex("[^a-zA-Z0-9 \n]")]
    private static partial Regex SPECIAL_REGEX();

    [GeneratedRegex("[A-Z]")]
    private static partial Regex UPPER_REGEX();
    
    [GeneratedRegex("[a-z]")]
    private static partial Regex LOWER_REGEX();
    
    private void Validate(string value) {
        if (CountVisibleOnly) {
            if (value.VisibleLength() < MinLength) throw new InvalidArgumentInputException($"Input value length is too small (must be at least {MinLength}).");
            if (value.VisibleLength() > MaxLength) throw new InvalidArgumentInputException($"Input value length is too big (must be at most {MaxLength}).");
        }
        else {
            if (value.Length < MinLength) throw new InvalidArgumentInputException($"Input value length is too small (must be at least {MinLength}).");
            if (value.Length > MaxLength) throw new InvalidArgumentInputException($"Input value length is too big (must be at most {MaxLength}).");
        }
        
        if (!AllowUpper && UPPER_REGEX().IsMatch(value)) throw new InvalidArgumentInputException("Input value contains uppercase characters.");
        if (!AllowLower && LOWER_REGEX().IsMatch(value)) throw new InvalidArgumentInputException("Input value contains lower case characters.");
        if (!AllowNumbers && NUMERIC_REGEX().IsMatch(value)) throw new InvalidArgumentInputException("Input value contains number characters.");
        if (!AllowSpecial && SPECIAL_REGEX().IsMatch(value)) throw new InvalidArgumentInputException("Input value contains special characters.");
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
        
        if (!AllowUpper && UPPER_REGEX().IsMatch(value)) return false;
        if (!AllowLower && LOWER_REGEX().IsMatch(value)) return false;
        if (!AllowNumbers && NUMERIC_REGEX().IsMatch(value)) return false;
        if (!AllowSpecial && SPECIAL_REGEX().IsMatch(value)) return false;
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
}