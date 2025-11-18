// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;
using NeoKolors.Tui.Fonts.Exceptions;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("AlignPointReplace", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class AlignPointReplace {
    
    [XmlText]
    public string? Raw { 
        get;
        set {
            if (TryParse(value, out var alignPointReplace)) Value = alignPointReplace;
            else throw FontSerializerException.InvalidAlignPointReplace(value ?? "<null>");
            field = value;
        } 
    }

    [XmlIgnore]
    public OneOf<None, Bckg, Forg, Custom> Value { get; private set; } = new Bckg();
    
    public static implicit operator string?(AlignPointReplace alignPointReplace) => alignPointReplace.Raw;
    public static implicit operator AlignPointReplace(string value) => new() { Raw = value };
    
    public override string? ToString() => Raw;

    private static bool TryParse(string? raw, out OneOf<None, Bckg, Forg, Custom> alignPointReplace) {
        switch (raw) {
            case null:                          alignPointReplace = default;            return false;
            case "none":                        alignPointReplace = new None();         return true;
            case "bckg":                        alignPointReplace = new Bckg();         return true;
            case "forg":                        alignPointReplace = new Forg();         return true;
            case var _ when IsValidCustom(raw): alignPointReplace = new Custom(raw[7]); return true;
            default:                            alignPointReplace = default;            return false;
        }
        
        bool IsValidCustom(string r) => r.StartsWith("custom,") && r.Length == 8;
    }
    
    public static AlignPointReplace NewBckg() => new() { Value = new Bckg() };
    public static AlignPointReplace NewForg() => new() { Value = new Forg() };
    public static AlignPointReplace NewCustom(char c) => new() { Value = new Custom(c) };
    public static AlignPointReplace NewNone() => new() { Value = new None() };
    
    public record None;
    public record Bckg;
    public record Forg;
    public record Custom(char Value) {
        public char Value { get; } = Value;
    }
}