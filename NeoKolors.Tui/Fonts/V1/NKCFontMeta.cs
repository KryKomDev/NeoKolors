//
// NeoKolors
// Copyright (c) 2025 KryKom
//

// ReSharper disable InconsistentNaming

using System.ComponentModel;
using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V1;

/// <summary>
/// Represents the metadata for fonts used in the NeoKolors terminal user interface (TUI).
/// Defines various properties and configurations essential to the rendering and behavior
/// of fonts in the TUI environment.
/// </summary>
[XmlRoot("NKCFont", Namespace = IFontMeta.SCHEMA_URL)]
public struct NKCFontMeta : IFontMeta {

    private FontMeta.Header _header;
    private FontMeta.Dimensions _dimensions;
    private FontMeta.GlyphConf _glyphConf;
    private FontMeta.CharFontConf _cfConf;
    
    [XmlElement]
    public FontMeta.Header Header { get => _header; set => _header = value; }

    [XmlElement]
    public FontMeta.Dimensions Dimensions { get => _dimensions; set => _dimensions = value; }
    
    [XmlElement]
    public FontMeta.GlyphConf GlyphConf { get => _glyphConf; set => _glyphConf = value; }
    
    [XmlElement("CharFontConf")]
    [DefaultValue(null)]
    public FontMeta.CharFontConf? CFConf { get => _cfConf; set => _cfConf = value ?? new FontMeta.CharFontConf(); }
    
    public string Version {
        get => _header.Version; 
        set => _header.Version = value;
    }
    
    public string Name {
        get => _header.Name; 
        set => _header.Name = value;
    }
    
    public string Path {
        get => _header.Path; 
        set => _header.Path = value;
    }

    public int LetterSpacing {
        get => _dimensions.LetterSpacing; 
        set => _dimensions.LetterSpacing = value;
    }
    
    public int WordSpacing {
        get => _dimensions.WordSpacing; 
        set => _dimensions.WordSpacing = value;
    }
    
    public int LineSpacing {
        get => _dimensions.LineSpacing; 
        set => _dimensions.LineSpacing = value;
    }
    
    public int LineSize {
        get => _dimensions.LineSize; 
        set => _dimensions.LineSize = value;
    }

    public MissingGlyphMode MissingGlyphMode {
        get => _glyphConf.MissingGlyphMode; 
        set => _glyphConf.MissingGlyphMode = value;
    }
    
    public string MissingGlyphSubstitute {
        get => _glyphConf.MissingGlyphSubstitute; 
        set => _glyphConf.MissingGlyphSubstitute = value;
    }

    public int Ligatures {
        get => _glyphConf.Ligatures;
        set => _glyphConf.Ligatures = value;
    }

    public string GlyphDefStartMarker {
        get => _cfConf.GlyphDefStartMarker;
        set => _cfConf.GlyphDefStartMarker = value;
    }

    public string GlyphDefEndMarker {
        get => _cfConf.GlyphDefEndMarker;
        set => _cfConf.GlyphDefEndMarker = value;
    }

    public OverlapMode Overlap {
        get => _cfConf.Overlap;
        set => _cfConf.Overlap = value;
    }

    public string MaskCharacter {
        get => _cfConf.MaskCharacter;
        set => _cfConf.MaskCharacter = value;
    }
}