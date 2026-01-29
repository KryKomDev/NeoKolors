// NeoKolors
// Copyright (c) 2025 KryKom

using System.IO.Compression;
using System.Text;
using Metriks;
using NeoKolors.Tui.Fonts.Exceptions;
using NeoKolors.Tui.Fonts.Serialization.Xml;
using AlignPointReplace = NeoKolors.Tui.Fonts.Serialization.Xml.AlignPointReplace;
using Mask = NeoKolors.Tui.Fonts.Serialization.Xml.Mask;

namespace NeoKolors.Tui.Fonts.Serialization;

public static class NKGlyphReader {

    /// <summary>
    /// Reads a glyph from the specified base path and glyph information, and constructs a
    /// <see cref="GlyphInfo"/> object.
    /// </summary>
    /// <param name="basePath">The base path to retrieve the glyph data.</param>
    /// <param name="glyphInfo">The XML representation of the glyph information.</param>
    /// <returns>An instance of <see cref="GlyphInfo"/> representing the constructed glyph.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public static SimpleGlyphInfo ReadComponent(string basePath, XmlComponentGlyphInfo glyphInfo) {
        var content = ReadGlyphFile(Path.Combine(basePath, glyphInfo.File), glyphInfo.Mask);
        return CreateComponent(content, glyphInfo);
    }
    
    /// <summary>
    /// Reads a glyph from the specified base path and glyph information, and constructs a
    /// <see cref="GlyphInfo"/> object.
    /// </summary>
    /// <param name="basePath">The base path to retrieve the glyph data.</param>
    /// <param name="glyphInfo">The XML representation of the glyph information.</param>
    /// <returns>An instance of <see cref="GlyphInfo"/> representing the constructed glyph.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public static LigatureGlyphInfo ReadLigature(string basePath, XmlLigatureGlyphInfo glyphInfo) {
        var content = ReadGlyphFile(Path.Combine(basePath, glyphInfo.File), glyphInfo.Mask);
        return CreateLigature(content, glyphInfo);
    }

    /// <summary>
    /// Reads a component glyph from the specified stream and glyph information, returning a
    /// <see cref="SimpleGlyphInfo"/> object.
    /// </summary>
    /// <param name="zip">The zip file from which the font is loaded</param>
    /// <param name="glyphInfo">The XML representation of the component glyph information.</param>
    /// <returns>An instance of <see cref="SimpleGlyphInfo"/> representing the constructed component glyph.</returns>
    /// <exception cref="IOException">Thrown when an error occurs during reading from the stream.</exception>
    public static SimpleGlyphInfo ReadComponent(ZipArchive zip, XmlComponentGlyphInfo glyphInfo) {
        var stream = FindStream(zip, glyphInfo.File);
        var content = ReadGlyphStream(stream, glyphInfo.Mask);
        return CreateComponent(content, glyphInfo);
    }

    /// <summary>
    /// Reads a ligature glyph from the specified stream and glyph information, returning a
    /// <see cref="SimpleGlyphInfo"/> object.
    /// </summary>
    /// <param name="zip">The zip file from which the font is loaded</param>
    /// <param name="glyphInfo">The XML representation of the component glyph information.</param>
    /// <returns>An instance of <see cref="SimpleGlyphInfo"/> representing the constructed component glyph.</returns>
    /// <exception cref="IOException">Thrown when an error occurs during reading from the stream.</exception>
    public static LigatureGlyphInfo ReadLigature(ZipArchive zip, XmlLigatureGlyphInfo glyphInfo) {
        var stream = FindStream(zip, glyphInfo.File);
        var content = ReadGlyphStream(stream, glyphInfo.Mask);
        return CreateLigature(content, glyphInfo);
    }

    private static Stream FindStream(ZipArchive zip, string path) {
        var entry = zip.GetEntry(path);
        return entry is null 
            ? throw new FileNotFoundException(path) 
            : entry.Open();
    }

    private static GlyphInfo CreateGlyph(char?[,] content, XmlGlyphInfo glyphInfo) {
        return glyphInfo.Match<GlyphInfo>(
            cnt => CreateComponent(content, cnt), 
            lig => CreateLigature(content, lig), 
            _   => throw new InvalidOperationException("Cannot read compound glyphs."), 
            _   => throw new InvalidOperationException("Cannot read auto-compound glyphs.") 
        );
    }

    private static SimpleGlyphInfo CreateComponent(char?[,] glyphContent, XmlComponentGlyphInfo glyphInfo) {
        var alignPoints =
            GetAlignPoints(glyphContent, glyphInfo.AlignPoints.ToCharArray(), glyphInfo.AlignPointReplace);
        
        glyphContent = Truncate(glyphContent);
        
        var glyph = new NKComponentGlyph(
            glyphContent, 
            glyphInfo.Baseline, 
            alignPoints
        );
        
        return new SimpleGlyphInfo(glyph, ParseSymbol(glyphInfo.Symbol));
    }
    
    private static LigatureGlyphInfo CreateLigature(char?[,] glyphContent, XmlLigatureGlyphInfo glyphInfo) {
        glyphContent = Truncate(glyphContent);
        
        var glyph = new NKComponentGlyph(
            glyphContent, 
            glyphInfo.Baseline 
        );
        
        return new LigatureGlyphInfo(glyph, glyphInfo.Symbol);
    }
    
    /// <summary>
    /// Reads a glyph from the specified file path and converts it into a 2D character array.
    /// </summary>
    /// <param name="path">The file path of the glyph to be read.</param>
    /// <param name="mask">The masking strategy.</param>
    /// <returns>A 2D character array representing the glyph, where null values are empty spaces.</returns>
    private static char?[,] ReadGlyphFile(string path, Mask mask) {
        string[] lines = File.ReadAllLines(path);

        return ReadGlyph(lines, mask);
    }

    /// <summary>
    /// Reads glyph data from the provided stream based on the specified mask
    /// and returns a two-dimensional array representation of the glyph content.
    /// </summary>
    /// <param name="stream">The input stream containing glyph data to be read.</param>
    /// <param name="mask">The masking pattern used to filter or process the glyph content.</param>
    /// <returns>A two-dimensional array of characters representing the processed glyph content.</returns>
    /// <exception cref="FontSerializerException">Thrown when the input stream cannot be read.</exception>
    private static char?[,] ReadGlyphStream(Stream stream, Mask mask) {
        if (!stream.CanRead)
            throw FontSerializerException.UnreadableStream();

        if (stream.CanSeek) stream.Position = 0;

        using var reader = new StreamReader(stream, Encoding.UTF8);
        string s = reader.ReadToEnd();
        
        var lines = s.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
        
        return ReadGlyph(lines, mask);
    }
    
    private static char?[,] ReadGlyph(string[] lines, Mask mask) {

        char? replaced = mask.Value.IsCustom ? mask.Value.CustomChar : ' ';
        bool replaceSpace = mask.Value is { IsCustom: true, IsForg: true };
        char? replacement = mask.Value.IsForg ? ' ' : null;
        
        int max = lines.Select(t => t.Length).Prepend(0).Max();

        char?[,] glyph = new char?[max, lines.Length];
        
        for (int y = 0; y < lines.Length; y++) {
            for (int x = 0; x < lines[y].Length; x++) {
                glyph[x, y] = x >= lines[y].Length ? null : lines[y][x];
                if (replaceSpace && glyph[x, y] == ' ') glyph[x, y] = null;
                if (glyph[x, y] == replaced) glyph[x, y] = replacement; // apply mask
            }
        }
        
        return glyph;
    }

    /// <summary>
    /// Extracts alignment points from a glyph based on the specified alignment characters.
    /// </summary>
    /// <param name="glyph">The 2D array representation of the glyph, where each elementOld corresponds to a character at
    /// a specific position.</param>
    /// <param name="alignChars">An array of characters used as alignment markers within the glyph.</param>
    /// <param name="replace">The way of replacing detected align points.</param>
    /// <returns>A collection of alignment points representing the positions of the specified alignment characters in
    /// the glyph.</returns>
    private static GlyphAlignmentPointCollection GetAlignPoints(
        char?[,] glyph,
        char[] alignChars, 
        AlignPointReplace? replace = null) 
    {
        HashSet<char> set = new(alignChars);
        var list = new GlyphAlignmentPointCollection();
        replace ??= AlignPointReplace.NewBckg();
        
        if (alignChars.Length == 0) return list;
        
        for (int x = 0; x < glyph.Len0; x++) {
            for (int y = 0; y < glyph.Len1; y++) {
                if (glyph[x, y] is null) continue;
                
                var c = glyph[x, y]!.Value;
                
                if (set.Contains(c) && !list.ContainsKey(c)) 
                    list.Add(c, new Point(x, y));
                else 
                    continue;

                glyph[x, y] = replace.Value.Match<char?>(
                    _ => c,
                    _ => null,
                    _ => ' ',
                    custom => custom.Value
                );
            }
        }
        
        return list;
    }

    private static char?[,] Truncate(char?[,] glyph) {
        List2D<char?> list = new(glyph);

        while (list.XSize >= 0) {
            if (!list.AllAtX(0, t => !t.HasValue)) break;
            list.RemoveAtX(0);
        }

        for (int x = list.XSize - 1; x >= 0; x--) {
            if (!list.AllAtX(x, t => !t.HasValue)) break;
            list.ShrinkX();
        }

        while (list.YSize >= 0) {
            if (!list.AllAtY(0, t => !t.HasValue)) break;
            list.RemoveAtY(0);
        }

        for (int y = list.YSize - 1; y >= 0; y--) {
            if (!list.AllAtY(y, t => !t.HasValue)) break;
            list.ShrinkY();
        }
        
        return list.ToArray();
    }

    /// <summary>
    /// Parses a string representation of a symbol and returns its corresponding character or Unicode value.
    /// </summary>
    /// <param name="s">The input string representing the symbol, which can be either
    /// a single character or a hexadecimal Unicode definition.</param>
    /// <returns>The parsed character corresponding to the given input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not a single character
    /// or does not conform to a valid hexadecimal Unicode definition format.</exception>
    public static char ParseSymbol(string s) {
        if (s.Length == 1) return s[0];
        if (s.Length != 3..6) throw FontSerializerException.InvalidSymbolFormat(s);
        
        string raw = s[2..];
        int hex;
            
        try {
            hex = Convert.ToInt32(raw, 16);
        }
        catch (Exception) {
            throw FontSerializerException.InvalidSymbolFormat(s);
        }
            
        return (char)hex;
    }
}