// 
// NeoKolors
// Copyright (c) 2025 KryKom
// 

using NeoKolors.Settings.Argument.Exception;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// Represents an argument that holds a word value with configurable constraints on length
/// and a default value.
/// </summary>
public class WordArgument : IArgument<string>, IXsdArgument {
    
    public string Value { get; private set; }
    public string DefaultValue { get; }
    public int MinLength { get; }
    public int MaxLength { get; }

    public WordArgument(int minLength = 0, int maxLength = int.MaxValue, string defaultValue = "") {
        MinLength = minLength;
        MaxLength = maxLength;
        DefaultValue = defaultValue;
        Value = DefaultValue;
        Set(DefaultValue);
    }

    
    public void Set(object? value) {
        switch (value) {
            case WordArgument w:
                Set(w.Value);
                return;
            case string s:
                Set(s);
                return;
            default:
                throw new InvalidArgumentInputTypeException(value?.GetType(), typeof(string));
        }
    }
    
    void IArgument.Set(object? value) => Set(value);

    public string ToXsd() =>
        $"""
         <xsd:simpleType>
             <xsd:restriction base="xsd:string">
                 <xsd:minLength value="{MinLength}"/>
                 <xsd:maxLength value="{MaxLength}"/>
                 <xsd:pattern value="[[:alpha:]|_][\w]*">
             </xsd:restriction>
         </xsd:simpleType>
         """;
    
    public string Get() => Value;
    
    public string GetDefault() => DefaultValue;
    object IArgument.GetDefault() => GetDefault();

    public void Reset() => Value = DefaultValue;

    public void Set(string value) {
        if (value.Length < MinLength) throw new InvalidArgumentInputException($"Value was less then the smallest allowed length ({MinLength}).");
        if (value.Length > MaxLength) throw new InvalidArgumentInputException($"Value was greater then the greatest allowed length ({MaxLength}).");
        if (value.Any(c => !char.IsLetter(c) || !char.IsNumber(c))) throw new InvalidArgumentInputException("Value is not a word.");
        Value = value;
    }

    object IArgument.Get() => Get();
    void IArgument.Reset() => Reset();

    public IArgument<string> Clone() =>
        MemberwiseClone() as IArgument<string> ?? 
        new WordArgument(MinLength, MaxLength, DefaultValue) { Value = Value };

    IArgument IArgument.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    private bool Equals(WordArgument other) => 
        Value == other.Value && 
        DefaultValue == other.DefaultValue &&
        MinLength == other.MinLength && 
        MaxLength == other.MaxLength;

    public bool Equals(IArgument? y) {
        if (ReferenceEquals(this, y)) return true;
        return y is WordArgument w && Equals(w);
    }
    
    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((WordArgument)obj);
    }

    public override int GetHashCode() => HashCode.Combine(DefaultValue, MinLength, MaxLength);

    public static implicit operator string(WordArgument arg) => arg.Value;
    public static bool operator ==(WordArgument left, WordArgument right) => left.Value == right.Value;
    public static bool operator !=(WordArgument left, WordArgument right) => left.Value != right.Value;
}