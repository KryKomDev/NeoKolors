// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;
using NeoKolors.Tui.Fonts.Exceptions;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("Mask", Namespace = NKFontSchema.SCHEMA_NAMESPACE)]
public partial class Mask {
    
    [XmlText]
    public string? Raw { 
        get;
        set {
            if (TryParse(value, out var mask)) Value = mask!.Value;
            else throw FontSerializerException.InvalidMask(value ?? "<null>");
            field = value;
        } 
    }
    
    [XmlIgnore]
    public MaskType Value { get; private set; } = MaskType.Bckg();
    
    public static implicit operator string?(Mask mask) => mask.Raw;
    public static implicit operator Mask(string value) => new() { Raw = value };
    
    public override string? ToString() => Raw;
    
    private static bool TryParse(string? raw, out MaskType? mask) {
        if (raw is null) {
            mask = null;
            return false;
        }
        
        switch (raw.Split(',')) {
            case ["bckg"]:               mask = MaskType.Bckg();                return true;
            case ["forg"]:               mask = MaskType.Forg();                return true;
            case ["custom-bckg", var b]: mask = MaskType.CustomBckg(b.First()); return true;
            case ["custom-forg", var f]: mask = MaskType.CustomForg(f.First()); return true;
            default:                     mask = null;                           return false;
        }
    }

    public readonly struct MaskType {
        
        // if true, then it's 'bckg' or 'custom-bckg'
        private readonly bool _l;
        
        // if null, then it's 'custom-...'
        private readonly char? _c;

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public bool IsBckg => _l;
        public bool IsForg => !_l;
        public bool IsCustom => _c is not null;
        
        public char CustomChar => _c ?? throw new InvalidOperationException("Not a custom mask.");
        
        private MaskType(bool isBckg, char? c) {
            _l = isBckg;
            _c = c;
        }
        
        public static MaskType Bckg() => new(true, null);
        public static MaskType Forg() => new(false, null);
        public static MaskType CustomBckg(char c) => new(true, c);
        public static MaskType CustomForg(char c) => new(false, c);
    }
}