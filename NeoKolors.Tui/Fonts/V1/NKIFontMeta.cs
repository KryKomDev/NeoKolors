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
public struct NKIFontMeta : IFontMeta {
    
    private FontMeta.Header _header;
    private FontMeta.Dimensions _dimensions;
    private FontMeta.GlyphConf _glyphConf;
    private FontMeta.ImgFontConf _ifConf;
    
    [XmlElement]
    public FontMeta.Header Header { get => _header; set => _header = value; }

    [XmlElement]
    public FontMeta.Dimensions Dimensions { get => _dimensions; set => _dimensions = value; }
    
    [XmlElement]
    public FontMeta.GlyphConf GlyphConf { get => _glyphConf; set => _glyphConf = value; }
    
    [XmlElement]
    [DefaultValue(null)]
    public FontMeta.ImgFontConf? ImgConf { get => _ifConf; set => _ifConf = value ?? new FontMeta.ImgFontConf(); }

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

    public int GlyphWidth {
        get => _ifConf.GlyphWidth;
        set => _ifConf.GlyphWidth = value;
    }

    public int GlyphHeight {
        get => _ifConf.GlyphHeight;
        set => _ifConf.GlyphHeight = value;
    }

    public bool Mono {
        get => _ifConf.Mono;
        set => _ifConf.Mono = value;
    }

    public GlyphLayout? Layout {
        get => _ifConf.Layout;
        set => _ifConf.Layout = value;
    }
}