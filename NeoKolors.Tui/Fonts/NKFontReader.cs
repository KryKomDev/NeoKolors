//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.CompilerServices;
using NeoKolors.Tui.Exceptions;

namespace NeoKolors.Tui.Fonts;

public class NKFontReader : IFontReader {
    
    /// <summary>
    /// path to the font file
    /// </summary>
    private string Path { get; }
    
    /// <summary>
    /// determines what to do when a glyph is missing
    /// </summary>
    public UnknownGlyphMode UnknownGlyphMode { get; private set; }
    
    /// <summary>
    /// determines the glyph to substitute with if another glyph is missing
    /// </summary>
    public char? SubstituteGlyph { get; private set; }
    
    public int LetterSpacing { get; private set; }
    public int WordSpacing { get; private set; }
    public int LineSpacing { get; private set; }
    
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
    public WhitespaceMode WhitespaceMode { get; private set; }
    
    /// <summary>
    /// the character that will be treated as an overlapping whitespace
    /// </summary>
    public char? WhitespaceMask { get; private set; }
    
    // a freakin' constructor, what did you expect?
    public NKFontReader(string path) {
        Path = path;
    }
    
    /// <summary>
    /// reads the font from the font file
    /// </summary>
    /// <returns>the font stored by the font file</returns>
    public IFont ReadFont() {
        if (!File.Exists(Path)) throw FontReaderException.FontFileDoesNotExist(Path);

        string[] lines = File.ReadAllLines(Path);

        ReadHeader(lines);
        return ReadGlyphs(lines);
    }

    private void ReadHeader(string[] lines) {
        if (lines.Length < 6) throw FontReaderException.InvalidHeader();
        
        // check version of the file
        string[] version = lines[0].Split(' ');
        if (version is not ["nkaf", "1"]) throw FontReaderException.InvalidFileVersion();

        SetMissingGlyphMode(lines[1]); 
        SetSpacings(lines[2]);

        GlyphStartMarker = lines[3];
        GlyphEndMarker = lines[4];
        
        SetWhitespaceMode(lines[5]);
    }

    private void SetMissingGlyphMode(string line) {
        string[] ugm = line.Split(' ');

        UnknownGlyphMode = ugm switch {
            ["default"] => UnknownGlyphMode.DEFAULT,
            ["glyph", _] => UnknownGlyphMode.GLYPH,
            ["skip"] => UnknownGlyphMode.SKIP,
            _ => throw FontReaderException.InvalidUnknownGlyphMode(line)
        };

        if (UnknownGlyphMode != UnknownGlyphMode.GLYPH) return;
        if (ugm[1].Length != 1) throw FontReaderException.InvalidUnknownGlyphModeGlyph(ugm[1]);
        SubstituteGlyph = ugm[1][0];
    }

    private void SetSpacings(string line) {
        string[] spacings = line.Split(' ');

        if (spacings is not [_, _, _]) throw FontReaderException.InvalidSpacings(line);

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
    }

    private void SetWhitespaceMode(string line) {
        string[] split = line.Split(' ');

        WhitespaceMode = split switch {
            ["transparent"] => WhitespaceMode.TRANSPARENT,
            ["overlap"] => WhitespaceMode.OVERLAP,
            ["mask", _] => WhitespaceMode.MASK,
            _ => throw FontReaderException.InvalidWhitespaceMode(line)
        };
        
        if (WhitespaceMode != WhitespaceMode.MASK) return;
        if (split[1].Length != 1) throw FontReaderException.InvalidWhitespaceMaskCharacter(split[1]);
        WhitespaceMask = split[1][0];
    }

    private NKFont ReadGlyphs(string[] lines) {
        int startMarkerLength = GlyphStartMarker.Length;
        var font = UnknownGlyphMode != UnknownGlyphMode.GLYPH 
            ? new NKFont(LetterSpacing, WordSpacing, LineSpacing, UnknownGlyphMode) 
            : new NKFont(LetterSpacing, WordSpacing, LineSpacing, (char)SubstituteGlyph!);

        for (int i = 5; i < lines.Length; i++) {
            if (lines[i].Length != startMarkerLength + 2 ||
                lines[i][..startMarkerLength] != GlyphStartMarker) continue;
            
            int end = i;
                
            for (int j = i; j < lines.Length; j++) {
                if (lines[j] != GlyphEndMarker) continue;
                end = j;
                break;
            }

            font.AddGlyph(ReadGlyph(lines[i..(end + 1)]));
            i = end + 1;
        }

        return font;
    }

    private Glyph ReadGlyph(string[] lines) {
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
            lines[i] = WhitespaceMode switch {
                WhitespaceMode.TRANSPARENT => lines[i].Replace(' ', '\0'),
                WhitespaceMode.OVERLAP => lines[i],
                WhitespaceMode.MASK => lines[i].Replace(' ', '\0').Replace((char)WhitespaceMask!, ' '),
                _ => throw new ArgumentException()
            };
        }

        return new Glyph(c, lines[2..^1], xo, yo,  WhitespaceMode);
    }
}

/// <summary>
/// determines what will substitute a missing character
/// </summary>
public enum UnknownGlyphMode {
    DEFAULT = 0,
    GLYPH = 1,
    SKIP = 2
}

/// <summary>
/// determines how whitespaces will be treated
/// </summary>
public enum WhitespaceMode {
    TRANSPARENT = 0,
    OVERLAP = 1,
    MASK = 2,
}