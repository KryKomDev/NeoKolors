// NeoKolors
// Copyright (c) 2025 KryKom

using System.Xml.Serialization;
using NeoKolors.Tui.Fonts.V2.Exceptions;
using static NeoKolors.Tui.Fonts.V2.Serialization.Xml.Align.Direction;

namespace NeoKolors.Tui.Fonts.V2.Serialization.Xml;

[XmlRoot("Align", Namespace = NKFontSchema.COMMON_SCHEMA_LOCATION)]
public partial class Align {
    
    [XmlText]
    public string? Raw { get; set; }

    private OneOf<Direction, Custom>? _value; 
    
    [XmlIgnore]
    public OneOf<Direction, Custom> Value {
        get {
            if (_value is not null) return _value.Value;
            
            if (TryParse(Raw, out var align)) _value = align;
            else throw FontSerializerException.InvalidGlyphAlign(Raw ?? "<null>");
            return _value.Value;
        } 
    }

    public CompoundGlyphAlignment Convert() {
        return Value.Match(
            dir => dir switch {
                TOP_LEFT      => CompoundGlyphAlignment.TopLeft(),
                TOP_CENTER    => CompoundGlyphAlignment.TopCenter(),
                TOP_RIGHT     => CompoundGlyphAlignment.TopRight(),
                MIDDLE_LEFT   => CompoundGlyphAlignment.MiddleLeft(),
                MIDDLE_CENTER => CompoundGlyphAlignment.Center(),
                MIDDLE_RIGHT  => CompoundGlyphAlignment.MiddleRight(),
                BOTTOM_LEFT   => CompoundGlyphAlignment.BottomLeft(),
                BOTTOM_CENTER => CompoundGlyphAlignment.BottomCenter(),
                BOTTOM_RIGHT  => CompoundGlyphAlignment.BottomRight(),
                _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
            },
            custom => CompoundGlyphAlignment.Custom(custom.Points[0])
        );
    }
    
    public static implicit operator string?(Align align) => align.Raw;
    public static implicit operator Align(string value) => new() { Raw = value };
    
    public static implicit operator OneOf<Direction, Custom>(Align align) => align.Value;
    public static implicit operator Align(OneOf<Direction, Custom> value) => new() { _value = value };
    
    public override string? ToString() => Raw;

    private static bool TryParse(string? raw, out OneOf<Direction, Custom> align) {
        if (raw is null) {
            align = default;
            return false;       
        }
        
        switch (raw.Split(',')) {
            case ["top-left"]:      align = TOP_LEFT;                    return true;
            case ["top-center"]:    align = TOP_CENTER;                  return true;
            case ["top-right"]:     align = TOP_RIGHT;                   return true;
            case ["middle-left"]:   align = MIDDLE_LEFT;                 return true;
            case ["middle-center"]: align = MIDDLE_CENTER;               return true;
            case ["middle-right"]:  align = MIDDLE_RIGHT;                return true;
            case ["bottom-left"]:   align = BOTTOM_LEFT;                 return true;
            case ["bottom-center"]: align = BOTTOM_CENTER;               return true;
            case ["bottom-right"]:  align = BOTTOM_RIGHT;                return true;
            case ["custom", var c]: align = new Custom(c.ToCharArray()); return true;
            default:                align = default;                     return false;
        }
    }
    
    public enum Direction {
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        MIDDLE_LEFT,
        MIDDLE_CENTER,
        MIDDLE_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT,
    }
    
    public record struct Custom(char[] Points) {
        public char[] Points { get; } = Points;
    }
}