//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.ComponentModel;
using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V1;

/// <summary>
/// Represents the metadata required for font headers in the NeoKolors terminal user interface (TUI) fonts.
/// </summary>
[XmlRoot("NKFont", Namespace = SCHEMA_URL)]
public interface IFontMeta {

    /// <summary>
    /// The url to the schema for headers of NKFont.
    /// </summary>
    public const string SCHEMA_URL =
        "https://raw.githubusercontent.com/KryKomDev/NeoKolors/refs/heads/main/NeoKolors.Tui/Schemas/NKFontHeader.xsd";
    
    [XmlElement]
    public FontMeta.Header Header { get; set; }
    
    [XmlElement]
    public FontMeta.Dimensions Dimensions { get; set; }
    
    [XmlElement]
    public FontMeta.GlyphConf GlyphConf { get; set; }
}

public static class FontMeta {
    
    public struct Header {
        
        /// <summary>
        /// Specifies the version of the font format.
        /// </summary>
        [XmlElement]
        public string Version { get; set; }
    
        /// <summary>
        /// The name of the font
        /// </summary>
        [XmlElement("FontName")]
        public string Name { get; set; }
    
        /// <summary>
        /// The path to the actual font file.
        /// </summary>
        [XmlElement("FontPath")]
        public string Path { get; set; }
    }
    
    public struct Dimensions {
        
        /// <summary>
        /// Spacing of letters.
        /// </summary>
        [XmlElement]
        public int LetterSpacing { get; set; }
    
        /// <summary>
        /// Spacing of words (equivalent to the width of a single space).
        /// </summary>
        [XmlElement]
        public int WordSpacing { get; set; }
    
        /// <summary>
        /// The space between lines.
        /// </summary>
        [XmlElement]
        public int LineSpacing { get; set; }
    
        /// <summary>
        /// The size of a single line.
        /// </summary>
        [XmlElement]
        public int LineSize { get; set; }
    }

    public struct GlyphConf {
        
        /// <summary>
        /// Determines what will happen when a missing glyph is encountered.
        /// </summary>
        [XmlElement]
        [DefaultValue(MissingGlyphMode.GLYPH)]
        public MissingGlyphMode MissingGlyphMode { get; set; }

        /// <summary>
        /// Determines what glyph will substitute a missing glyph.
        /// </summary>
        [XmlElement]
        [DefaultValue("?")]
        public string MissingGlyphSubstitute { get; set; }
        
        /// <summary>
        /// Determines the maximum number of characters associated in a ligature
        /// (0 and 1 represent the absence of ligatures).
        /// </summary>
        [XmlElement]
        [DefaultValue(0)]
        public int Ligatures { get; set; }
    }
    
    public struct CharFontConf {

        /// <summary>
        /// Sequence defining the start of a glyph definition.
        /// </summary>
        [XmlElement] 
        [DefaultValue("[[[")] 
        public string GlyphDefStartMarker { get; set; }

        /// <summary>
        /// Sequence defining the end of a glyph definition.
        /// </summary>
        [XmlElement] 
        [DefaultValue("]]]")] 
        public string GlyphDefEndMarker { get; set; }

        /// <summary>
        /// Defines what to do with space characters in glyph definitions.
        /// </summary>
        [field: XmlEnum]
        [DefaultValue(OverlapMode.OVERLAP)]
        public OverlapMode Overlap { get; set; }

        /// <summary>
        /// Defines the mask character for glyph definitions.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        [XmlElement]
        [DefaultValue(".")]
        public string MaskCharacter {
            get;
            set {
                if (value.Length != 1)
                    throw new ArgumentException("MaskCharacter must be a single character.");

                field = value;
            }
        }

        public CharFontConf() {
            GlyphDefStartMarker = "[[[";
            GlyphDefEndMarker = "]]]";
            Overlap = OverlapMode.OVERLAP;
            MaskCharacter = ".";
        }
    }
    
    public struct ImgFontConf {
        
        /// <summary>
        /// Defines the width of a single glyph cell.
        /// </summary>
        [XmlElement]
        public int GlyphWidth { get; set; }
        
        /// <summary>
        /// Defines the height of a single glyph cell.
        /// </summary>
        [XmlElement]
        public int GlyphHeight { get; set; }
        
        /// <summary>
        /// Whether to keep the original size, margin and padding of the glyphs (value is true),
        /// or whether to shrink the glyphs.
        /// </summary>
        [XmlElement]
        public bool Mono { get; set; }
        
        /// <summary>
        /// The layout of the characters in the image font.
        /// </summary>
        [XmlElement]
        [DefaultValue(null)]
        public GlyphLayout? Layout { 
            get;
            set => field = value ?? new GlyphLayout();
        }

        public ImgFontConf() {
            GlyphWidth = 4;
            GlyphHeight = 3;
            Mono = true;
            Layout = new GlyphLayout();
        }
    }
}
