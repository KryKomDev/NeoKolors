// NeoKolors
// Copyright (c) KryKom 2026

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NeoKolors.Common;

namespace NeoKolors.Tui.Fonts.Serialization;

public static partial class NKFontSerializer {

    /// <summary>
    /// Deserializes a FIGlet font (.flf) from a file path.
    /// </summary>
    public static NKFont? DeserializeFiglet(string path, int? letterSpacing = null, int? wordSpacing = null, int? leading = null) {
        if (!File.Exists(path)) {
            LOGGER.Error($"FIGlet font file '{path}' not found.");
            return null;
        }

        using var fs = File.OpenRead(path);
        return DeserializeFiglet(fs, Path.GetFileNameWithoutExtension(path), letterSpacing, wordSpacing, leading);
    }

    /// <summary>
    /// Deserializes a FIGlet font (.flf) from a stream.
    /// </summary>
    public static NKFont? DeserializeFiglet(Stream stream, string fontName = "FigletFont", int? letterSpacing = null, int? wordSpacing = null, int? leading = null) {
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        return DeserializeFiglet(reader, fontName, letterSpacing, wordSpacing, leading);
    }

    /// <summary>
    /// Deserializes a FIGlet font (.flf) from a TextReader.
    /// </summary>
    public static NKFont? DeserializeFiglet(TextReader reader, string fontName = "FigletFont", int? letterSpacing = null, int? wordSpacing = null, int? leading = null) {
        // Read header line
        string? headerLine = reader.ReadLine();
        if (headerLine == null) {
            LOGGER.Error("Failed to read FIGlet header: End of file reached.");
            return null;
        }

        if (!headerLine.StartsWith("flf2a")) {
            LOGGER.Error("Invalid FIGlet header signature. Must start with 'flf2a'.");
            return null;
        }

        if (headerLine.Length < 6) {
            LOGGER.Error("Invalid FIGlet header. Too short.");
            return null;
        }

        char hardblank = headerLine[5];

        // Parse space-separated header parameters starting from index 6
        string paramsPart = headerLine.Substring(6);
        string[] paramTokens = paramsPart.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (paramTokens.Length < 5) {
            LOGGER.Error("FIGlet header must contain at least 5 parameters (height, baseline, max_length, old_layout, comment_lines).");
            return null;
        }

        if (!int.TryParse(paramTokens[0], out int height) ||
            !int.TryParse(paramTokens[1], out int baseline) ||
            !int.TryParse(paramTokens[2], out int maxLength) ||
            !int.TryParse(paramTokens[3], out int oldLayout) ||
            !int.TryParse(paramTokens[4], out int commentLines)) {
            LOGGER.Error("Failed to parse FIGlet header parameters as integers.");
            return null;
        }

        // Skip comments
        for (int i = 0; i < commentLines; i++) {
            reader.ReadLine();
        }

        var glyphs = new Dictionary<NKGlyphSymbol, NKGlyph>();
        char? endmark = null;

        // Helper to parse characters
        bool ParseCharacter(int codePoint) {
            var lines = new string[height];
            for (int i = 0; i < height; i++) {
                string? line = reader.ReadLine();
                if (line == null) {
                    // If we reach EOF when parsing the required standard characters, it's an error.
                    // If we reach EOF when parsing code-tagged characters, we just stop parsing.
                    return false;
                }
                lines[i] = line;
            }

            // Determine the endmark character from the first line of the first character (space)
            if (endmark == null) {
                foreach (var line in lines) {
                    if (line.Length > 0) {
                        endmark = line[line.Length - 1];
                        break;
                    }
                }
                // Fallback to '@' if not found
                endmark ??= '@';
            }

            // Process lines
            for (int i = 0; i < height; i++) {
                string line = lines[i];
                // Trim trailing endmarks
                int len = line.Length;
                while (len > 0 && line[len - 1] == endmark) {
                    len--;
                }
                line = line.Substring(0, len);
                // Replace hardblanks with space
                line = line.Replace(hardblank, ' ');
                lines[i] = line;
            }

            int charWidth = lines.Max(l => l.Length);
            if (charWidth == 0) {
                // Return an empty/transparent glyph
                var emptyGrid = new GlyphCell[0, height];
                glyphs[NKGlyphSymbol.Simple((char)codePoint)] = new NKGlyph(emptyGrid, baseline - height);
                return true;
            }

            var grid = new GlyphCell[charWidth, height];
            for (int y = 0; y < height; y++) {
                string line = lines[y];
                for (int x = 0; x < charWidth; x++) {
                    char cellChar = x < line.Length ? line[x] : ' ';
                    grid[x, y] = cellChar == ' ' ? GlyphCell.Background : GlyphCell.Char(cellChar);
                }
            }

            // Reduce/trim the glyph
            var reduceResult = NKFontSerializerHelper.Reduce(grid);
            if (reduceResult.IsT0) {
                var (reducedGrid, offset) = reduceResult.AsT0.Value;
                int bottomTrim = height - offset.Y - reducedGrid.GetLength(1);
                int baselineOffset = (baseline - height) + bottomTrim;
                glyphs[NKGlyphSymbol.Simple((char)codePoint)] = new NKGlyph(reducedGrid, baselineOffset);
            }
            else {
                // If reduce fails, use the original grid
                glyphs[NKGlyphSymbol.Simple((char)codePoint)] = new NKGlyph(grid, baseline - height);
            }

            return true;
        }

        // Read standard ASCII characters (32 to 126)
        for (int code = 32; code <= 126; code++) {
            if (!ParseCharacter(code)) {
                LOGGER.Error($"FIGlet font parsing failed: Unexpected EOF while parsing standard ASCII character '{code}' ({(char)code}).");
                return null;
            }
        }

        // Read German extended characters (127 to 133)
        int[] germanCodes = { 196, 214, 220, 228, 246, 252, 223 }; // Ä, Ö, Ü, ä, ö, ü, ß
        for (int i = 0; i < 7; i++) {
            if (!ParseCharacter(germanCodes[i])) {
                LOGGER.Error($"FIGlet font parsing failed: Unexpected EOF while parsing German extended character at index '{i}'.");
                return null;
            }
        }

        // Read code-tagged characters
        string? tagLine;
        while ((tagLine = reader.ReadLine()) != null) {
            if (string.IsNullOrWhiteSpace(tagLine)) {
                continue;
            }

            if (TryParseCodeTag(tagLine, out int codePoint)) {
                if (!ParseCharacter(codePoint)) {
                    break;
                }
            }
        }

        var fontInfo = new NKFontInfo(
            fontName,
            ligatures: false,
            leading: leading ?? height,
            letterSpacing: letterSpacing ?? 0,
            wordSpacing: wordSpacing ?? (glyphs.TryGetValue(NKGlyphSymbol.Simple(' '), out var spaceGlyph) ? spaceGlyph.Width : 4),
            new NKFontVariableConfig(false)
        );

        return new NKFont(
            fontInfo,
            glyphs,
            unknownSymbolGlyph: null,
            strikethroughGlyph: null,
            renderStrikethroughAboveLetters: true,
            underlineGlyph: null,
            renderUnderlineAboveLetters: true
        );
    }

    private static bool TryParseCodeTag(string line, out int codePoint) {
        codePoint = -1;
        line = line.Trim();
        if (string.IsNullOrEmpty(line)) return false;

        int spaceIndex = -1;
        for (int i = 0; i < line.Length; i++) {
            if (char.IsWhiteSpace(line[i])) {
                spaceIndex = i;
                break;
            }
        }
        string codePart = spaceIndex == -1 ? line : line.Substring(0, spaceIndex);

        try {
            if (codePart.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                codePoint = Convert.ToInt32(codePart.Substring(2), 16);
                return true;
            }
            if (codePart.StartsWith("0") && codePart.Length > 1) {
                try {
                    codePoint = Convert.ToInt32(codePart, 8);
                    return true;
                }
                catch {
                    // Fall through to decimal
                }
            }
            if (int.TryParse(codePart, out int val)) {
                codePoint = val;
                return true;
            }
        }
        catch {
            // Ignore
        }
        return false;
    }

    /// <summary>
    /// Functional FIGlet font deserializer that avoids direct side-effects during execution and returns all collected diagnostic logs as values.
    /// </summary>
    public static NKFontDeserializationResult TryDeserializeFiglet(string path, int? letterSpacing = null, int? wordSpacing = null, int? leading = null) {
        var context = new DeserializationContext();
        using (DeserializationContext.Enter(context)) {
            NKFont? font = null;
            try {
                font = DeserializeFiglet(path, letterSpacing, wordSpacing, leading);
            }
            catch (Exception ex) {
                context.Errors.Add($"Unhandled exception during deserialization: {ex.Message}\n{ex.StackTrace}");
            }
            return new NKFontDeserializationResult(font, context.Errors, context.Warnings, context.Infos);
        }
    }

    /// <summary>
    /// Functional FIGlet font deserializer that avoids direct side-effects during execution and returns all collected diagnostic logs as values.
    /// </summary>
    public static NKFontDeserializationResult TryDeserializeFiglet(Stream stream, string fontName = "FigletFont", int? letterSpacing = null, int? wordSpacing = null, int? leading = null) {
        var context = new DeserializationContext();
        using (DeserializationContext.Enter(context)) {
            NKFont? font = null;
            try {
                font = DeserializeFiglet(stream, fontName, letterSpacing, wordSpacing, leading);
            }
            catch (Exception ex) {
                context.Errors.Add($"Unhandled exception during deserialization: {ex.Message}\n{ex.StackTrace}");
            }
            return new NKFontDeserializationResult(font, context.Errors, context.Warnings, context.Infos);
        }
    }
}
