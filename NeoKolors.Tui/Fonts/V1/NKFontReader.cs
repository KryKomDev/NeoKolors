//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Xml.Schema.XmlSchemaValidationFlags;

namespace NeoKolors.Tui.Fonts.V1;

/// <summary>
/// Reads NeoKolors console fonts.
/// </summary>
public class NKFontReader {
    
    private static readonly XmlSchemaSet HEADER_SCHEMA;
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(NKFontReader));
    
    static NKFontReader() {
        
        LOGGER.Info("Downloading NKFont header XSD schema...");

        string headerXsd;
        using (HttpClient http = new()) {
            headerXsd = http.GetStringAsync(IFontMeta.SCHEMA_URL).Result;
        }
        
        LOGGER.Info("Finished downloading NKFont header XSD schema.");
        
        HEADER_SCHEMA = new XmlSchemaSet();
        
        LOGGER.Trace("Reading NKFont header XSD schema...");

        using (var stringReader = new StringReader(headerXsd))
        using (var xsdReader = XmlReader.Create(stringReader)) {
            HEADER_SCHEMA.Add(null, xsdReader);
        }
        
        HEADER_SCHEMA.Compile();
        
        LOGGER.Trace("Finished reading NKFont header XSD schema.");
    }


    public NKFont ReadFont(string filepath) {
        if (!File.Exists(filepath)) {
            throw new FileNotFoundException($"File {filepath} does not exist.");
        }
        
        string ext = Path.GetExtension(filepath);
        OneOf<NKCFontMeta, NKIFontMeta> meta;
        
        switch (ext) {
            case ".nkfh":
                meta = ReadHeader_Xml(filepath);
                break;
            case ".nkf":
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }

        return meta.Match(
            ReadCFont, 
            i => ReadIFont(i.Path)
        );
    }
    
    private static OneOf<NKCFontMeta, NKIFontMeta> ReadHeader_Xml(string path) {
        
        var settings = new XmlReaderSettings {
            Schemas = HEADER_SCHEMA,
            ValidationType = ValidationType.Schema,
            ValidationFlags = ProcessInlineSchema | ReportValidationWarnings | ProcessSchemaLocation
        };

        var fs = new FileStream(path, FileMode.Open);
        var reader = XmlReader.Create(fs, settings);
        
        var doc = new XDocument(reader);
        var root = doc.Root;
        
        if (root is null) 
            throw new XmlException("Root elementOld not found.");
        
        var rootName = root.Name.LocalName;

        switch (rootName) {
            case "NKCFont": {
                var xml = new XmlSerializer(typeof(NKCFontMeta));
                return (NKCFontMeta)xml.Deserialize(reader)!;
            }
            case "NKIFont": {
                var xml = new XmlSerializer(typeof(NKIFontMeta));
                return (NKIFontMeta)xml.Deserialize(reader)!;
            }
            default:
                throw new XmlException("Root elementOld not recognized.");
        }
    }

    private static NKFont ReadCFont(NKCFontMeta meta) {
        string content = File.ReadAllText(meta.Path);
        string[] lines = content.Split('\n');

        var font = new NKFont(
            meta.LetterSpacing, 
            meta.WordSpacing, 
            meta.LineSpacing,
            meta.LineSize, 
            meta.MissingGlyphMode);
        
        for (int i = 0; i < lines.Length; i++) {
            if (lines[i].SafeSubstring(0, meta.GlyphDefStartMarker.Length) != meta.GlyphDefStartMarker) 
                continue;
            
            var res = ReadGlyph(lines, i, meta);
            font.AddGlyph(res.glyph);
            i = res.end + 1;
        }

        return font;
    }

    private static (int end, NKGlyph glyph) ReadGlyph(string[] lines, int start, NKCFontMeta meta) {
        char c = ' ';
        sbyte xo = 0;
        sbyte yo = 0;
        
        // TODO: read flags
        
        List<string> chars = [];
        int i = start + 1;
        for (; i < lines.Length; i++) {
            if (lines[i].SafeSubstring(0, meta.GlyphDefEndMarker.Length) == meta.GlyphDefEndMarker) 
                break;
            
            chars.Add(lines[i]);
        }
        
        return (i, new NKGlyph(c, chars.ToArray(), xo, yo, meta.Overlap));
    }
    
    private NKFont ReadIFont(string path) {
        
        throw new NotImplementedException();
    }
}