//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Xml.Schema.XmlSchemaValidationFlags;

namespace NeoKolors.Tui.Fonts.V1;

public class NKCharFontReader : IFontReader {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(NKCharFontReader));
    
    /// <summary>
    /// path to the font file
    /// </summary>
    private string Path { get; }
    
    /// <summary>
    /// determines what to do when a glyph is missing
    /// </summary>
    public MissingGlyphMode MissingGlyphMode { get; private set; }
    
    /// <summary>
    /// determines the glyph to substitute with if another glyph is missing
    /// </summary>
    public char? SubstituteGlyph { get; private set; }
    
    public int LetterSpacing { get; private set; }
    public int WordSpacing { get; private set; }
    public int LineSpacing { get; private set; }
    public int LineSize { get; private set; }
    
    /// <summary>
    /// string that marks the beginning of a glyph section
    /// </summary>
    public string GlyphStartMarker { get; private set; } = null!;

    /// <summary>
    /// string that marks the end of a glyph section
    /// </summary>
    public string GlyphEndMarker { get; private set; } = null!;

    /// <summary>
    /// determines how whitespaces will be treated
    /// </summary>
    public OverlapMode OverlapMode { get; private set; }
    
    /// <summary>
    /// the character that will be treated as an overlapping whitespace
    /// </summary>
    public char? WhitespaceMask { get; private set; }
    
    // a freakin' constructor, what did you expect?
    public NKCharFontReader(string path) {
        Path = path;
    }
    
    /// <summary>
    /// reads the font from the font file
    /// </summary>
    /// <returns>the font stored by the font file</returns>
    public IFont ReadFont() {
        if (!File.Exists(Path)) throw FontReaderException.FontFileDoesNotExist(Path);

        string[] lines = File.ReadAllLines(Path);

        ReadHeader_A1(lines);
        return ReadGlyphs_A1(lines);
    }


    static NKCharFontReader() {
        
        LOGGER.Info("Downloading NKFont header XSD schema...");

        string headerXsd;
        using (HttpClient http = new()) {
            headerXsd = http.GetStringAsync(IFontMeta.SCHEMA_URL).Result;
        }
        
        LOGGER.Info("Finished downloading NKFont header XSD schema.");
        
        HEADER_SCHEMA = new XmlSchemaSet();
        
        LOGGER.Trace("Reading NKFont header XSD schema...");

        using (StringReader stringReader = new(headerXsd))
        using (var xsdReader = XmlReader.Create(stringReader)) {
            HEADER_SCHEMA.Add(null, xsdReader);
        }
        
        HEADER_SCHEMA.Compile();
        
        LOGGER.Trace("Finished reading NKFont header XSD schema.");
    }

    public static void Init() {}

    // VERSION XML

    private static readonly XmlSchemaSet HEADER_SCHEMA;
    
    private NKCFontMeta ReadHeader_Xml(string path) {
        var settings = new XmlReaderSettings();
        settings.Schemas = HEADER_SCHEMA;
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationFlags |= ProcessInlineSchema | ReportValidationWarnings | ProcessSchemaLocation;

        var fs = new FileStream(path, FileMode.Open);
        var reader = XmlReader.Create(fs, settings);
        var xml = new XmlSerializer(typeof(NKCFontMeta));
        return (NKCFontMeta)xml.Deserialize(reader)!;
    }
    
    
    // VERSION ALPHA 1
    
    private void ReadHeader_A1(string[] lines) {
        if (lines.Length < 6) throw FontReaderException.InvalidHeader();
        
        // check the version of the file
        string[] version = lines[0].Split(' ');
        if (version is not ["nkcf", "1"]) throw FontReaderException.InvalidFileVersion();

        SetMissingGlyphMode_A1(lines[1]); 
        SetSpacings_A1(lines[2]);

        GlyphStartMarker = lines[3];
        GlyphEndMarker = lines[4];
        
        SetWhitespaceMode_A1(lines[5]);
    }

    private void SetMissingGlyphMode_A1(string line) {
        string[] ugm = line.Split(' ');

        MissingGlyphMode = ugm switch {
            ["default"] => MissingGlyphMode.CHAR,
            ["glyph", _] => MissingGlyphMode.GLYPH,
            ["skip"] => MissingGlyphMode.SKIP,
            _ => throw FontReaderException.InvalidUnknownGlyphMode(line)
        };

        if (MissingGlyphMode != MissingGlyphMode.GLYPH) return;
        if (ugm[1].Length != 1) throw FontReaderException.InvalidUnknownGlyphModeGlyph(ugm[1]);
        SubstituteGlyph = ugm[1][0];
    }

    private void SetSpacings_A1(string line) {
        string[] spacings = line.Split(' ');

        if (spacings is not [_, _, _, _]) throw FontReaderException.InvalidSpacings(line);

        try {
            LetterSpacing = int.Parse(spacings[0]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidLetterSpacing(e);
        }
        
        try {
            WordSpacing = int.Parse(spacings[1]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidWordSpacing(e);
        }

        try {
            LineSpacing = int.Parse(spacings[2]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidLineSpacing(e);
        }

        try {
            LineSize = int.Parse(spacings[3]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidLineSize(e);
        }
    }

    private void SetWhitespaceMode_A1(string line) {
        string[] split = line.Split(' ');

        OverlapMode = split switch {
            ["transparent"] => OverlapMode.TRANSPARENT,
            ["overlap"] => OverlapMode.OVERLAP,
            ["mask", _] => OverlapMode.MASK,
            _ => throw FontReaderException.InvalidWhitespaceMode(line)
        };
        
        if (OverlapMode != OverlapMode.MASK) return;
        if (split[1].Length != 1) throw FontReaderException.InvalidWhitespaceMaskCharacter(split[1]);
        WhitespaceMask = split[1][0];
    }

    private NKFont ReadGlyphs_A1(string[] lines) {
        int startMarkerLength = GlyphStartMarker.Length;
        var font = MissingGlyphMode != MissingGlyphMode.GLYPH 
            ? new NKFont(LetterSpacing, WordSpacing, LineSpacing, LineSize, MissingGlyphMode) 
            : new NKFont(LetterSpacing, WordSpacing, LineSpacing, LineSize, (char)SubstituteGlyph!);

        for (int i = 5; i < lines.Length; i++) {
            if (lines[i].Length != startMarkerLength + 2 ||
                lines[i][..startMarkerLength] != GlyphStartMarker) continue;
            
            int end = i;
                
            for (int j = i; j < lines.Length; j++) {
                if (lines[j] != GlyphEndMarker) continue;
                end = j;
                break;
            }

            font.AddGlyph(ReadGlyph_A1(lines[i..(end + 1)]));
            i = end + 1;
        }

        return font;
    }

    private NKGlyph ReadGlyph_A1(string[] lines) {
        char c = lines[0][^1];
        string[] offsets = lines[1].Split(' ');

        if (offsets is not [_, _]) throw FontReaderException.InvalidOffsets(c, lines[1]);

        sbyte xo;
        sbyte yo;

        try {
            xo = sbyte.Parse(offsets[0]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidXOffset(c, e);
        }

        try {
            yo = sbyte.Parse(offsets[1]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidYOffset(c, e);
        }

        for (int i = 2; i < lines.Length - 1; i++) {
            lines[i] = OverlapMode switch {
                OverlapMode.TRANSPARENT => lines[i].Replace(' ', '\0'),
                OverlapMode.OVERLAP => lines[i],
                OverlapMode.MASK => lines[i].Replace(' ', '\0').Replace((char)WhitespaceMask!, ' '),
                _ => throw new ArgumentException()
            };
        }

        return new NKGlyph(c, lines[2..^1], xo, yo,  OverlapMode);
    }
}