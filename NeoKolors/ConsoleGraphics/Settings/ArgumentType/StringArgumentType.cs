//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using System.Text.RegularExpressions;
using NeoKolors.ConsoleGraphics.Settings.Exceptions;

namespace NeoKolors.ConsoleGraphics.Settings.ArgumentType;

/// <summary>
/// String Argument Type
/// </summary>
public partial class StringArgumentType : IArgumentType {

    private string value;
    public uint MinLength { get; }
    public uint MaxLength { get; }
    public bool AllowSpaces { get; }
    public bool AllowNewlines { get; }
    public bool AllowSpecial { get; }
    public bool AllowNumbers { get; }
    public bool AllowUpper { get; }
    public bool AllowLower { get; }
    private readonly string defaultValue;

    internal StringArgumentType(
        uint minLength = 0, 
        uint maxLength = UInt32.MaxValue, 
        string defaultValue = "",
        bool allowNewlines = true,
        bool allowSpaces = true,
        bool allowSpecial = true,
        bool allowNumbers = true,
        bool allowUpper = true,
        bool allowLower = true) 
    {
        MinLength = minLength;
        MaxLength = maxLength;
        this.defaultValue = defaultValue;
        value = defaultValue;
        AllowNewlines = allowNewlines;
        AllowSpaces = allowSpaces;
        AllowSpecial = allowSpecial;
        AllowNumbers = allowNumbers;
        AllowUpper = allowUpper;
        AllowLower = allowLower;
    }
    
    public string GetInputType() {
        return "String";
    }

    public string GetStringValue() {
        return value;
    }

    public object GetValue() {
        return value;
    }

    [GeneratedRegex("[0-9]")]
    private static partial Regex NUMERIC_REGEX();
    
    [GeneratedRegex("[^a-zA-Z0-9 \n]")]
    private static partial Regex SPECIAL_REGEX();

    [GeneratedRegex("[A-Z]")]
    private static partial Regex UPPER_REGEX();
    
    [GeneratedRegex("[a-z]")]
    private static partial Regex LOWER_REGEX();
    
    public void SetValue(object v) {
        if (v.GetType() != typeof(string)) throw new SettingsArgumentException(v.GetType(), typeof(string));
        if (!AllowSpaces && ((string)v).Contains(' ')) throw new SettingsArgumentException("Spaces are not allowed.");
        if (!AllowNewlines && ((string)v).Contains('\n')) throw new SettingsArgumentException("Newlines are not allowed.");
        if (!AllowNumbers && NUMERIC_REGEX().IsMatch((string)v)) throw new SettingsArgumentException("Numbers are not allowed.");
        if (!AllowSpecial && SPECIAL_REGEX().IsMatch((string)v)) throw new SettingsArgumentException("Special characters are not allowed.");
        if (!AllowUpper && UPPER_REGEX().IsMatch((string)v)) throw new SettingsArgumentException("Upper case characters are not allowed.");
        if (!AllowLower && LOWER_REGEX().IsMatch((string)v)) throw new SettingsArgumentException("Lower case characters are not allowed.");
        if (((string)v).Length < MinLength || ((string)v).Length > MaxLength) 
            throw new SettingsArgumentException($"Length of string '{v}' must be greater than " +
                                                $"{MinLength} and smaller than {MaxLength}.");

        value = (string)v;
    }

    public bool IsValid(char c) {
        if (!AllowSpaces && c is ' ') return false;
        if (!AllowNewlines && c is '\n') return false;
        if (!AllowSpecial && SPECIAL_REGEX().IsMatch($"{c}")) return false;
        if (!AllowNumbers && char.IsDigit(c)) return false;
        if (!AllowUpper && char.IsUpper(c)) return false;
        if (!AllowLower && char.IsLower(c)) return false;
        return true;
    }

    public IArgumentType Clone() {
        StringArgumentType clone = new StringArgumentType(MinLength, MaxLength) {
            value = value
        };
        
        return clone;
    }

    public void Reset() {
        value = defaultValue;
    }

    public override string ToString() {
        return $"{{\"type\": \"string\", " +
               $"\"minLength\": \"{MinLength}\", " +
               $"\"maxLength\": \"{MaxLength}\", " +
               $"\"value\": {value}}}";
    }
    
    public static implicit operator string(StringArgumentType v) => v.value;
    public static implicit operator StringArgumentType(string v) {
        StringArgumentType s = new StringArgumentType { value = v };
        return s;
    }
    
    public static StringArgumentType operator +(StringArgumentType a, StringArgumentType b) => a.value + b.value;
    public static StringArgumentType operator +(StringArgumentType a, string b) => a.value + b;
    public static StringArgumentType operator +(string a, StringArgumentType b) => a + b.value;
}