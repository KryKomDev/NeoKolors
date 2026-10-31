// NeoKolors
// Copyright (c) KryKom 2026

using System.Text.RegularExpressions;

namespace NeoKolors.Tui.Fonts;

public static partial class NKFontSerializer {
    /// <summary>
    /// Deserializes a FIGlet font (.flf) from a file path.
    /// Automatically detects and applies any companion manifest (e.g. manifest.xml)
    /// or sidecar metadata (e.g. [font].meta.xml) located in the font's directory.
    /// </summary>
    public static NKFont? DeserializeFiglet(
        string          path,
        int?            letterSpacing = null,
        int?            wordSpacing   = null,
        int?            leading       = null,
        FigletMetadata? metadata      = null
    ) {
        if (!File.Exists(path)) {
            LOGGER.Error($"FIGlet font file '{path}' not found.");

            return null;
        }

        string  fontName = Path.GetFileNameWithoutExtension(path);
        string? dir      = Path.GetDirectoryName(Path.GetFullPath(path));

        var effectiveMeta = new FigletMetadata();

        if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir)) {
            // Check for directory-level manifest (manifest.xml, figlet.manifest.xml)
            string[] manifestCandidates = [
                Path.Combine(dir, "manifest.xml"),
                Path.Combine(dir, "figlet.manifest.xml"),
                Path.Combine(dir, "Figlet.manifest.xml")
            ];

            foreach (var mf in manifestCandidates) {
                if (!File.Exists(mf))
                    continue;

                try {
                    using var mfStream     = File.OpenRead(mf);
                    var       manifestDict = FigletMetadata.FromManifestXml(mfStream);

                    if (manifestDict.TryGetValue(fontName, out var mfMeta)) {
                        effectiveMeta.MergeWith(mfMeta);

                        break;
                    }
                }
                catch {
                    // ignored
                }
            }

            // Check for font-specific sidecar file (<font>.meta.xml, <font>.xml, <font>.flf.xml)
            string[] sidecarCandidates = [
                Path.Combine(dir, $"{fontName}.meta.xml"),
                Path.Combine(dir, $"{fontName}.xml"),
                Path.Combine(dir, $"{fontName}.flf.xml")
            ];

            foreach (var sc in sidecarCandidates) {
                if (!File.Exists(sc))
                    continue;

                try {
                    using var scStream = File.OpenRead(sc);
                    var       scMeta   = FigletMetadata.FromXml(scStream);

                    if (scMeta != null) {
                        effectiveMeta.MergeWith(scMeta);

                        break;
                    }
                }
                catch {
                    // ignored
                }
            }
        }

        effectiveMeta.MergeWith(metadata);

        using var fs = File.OpenRead(path);

        return DeserializeFiglet(fs, fontName, letterSpacing, wordSpacing, leading, effectiveMeta);
    }

    /// <summary>
    /// Deserializes a FIGlet font (.flf) from a stream.
    /// </summary>
    public static NKFont? DeserializeFiglet(
        Stream          stream,
        string          fontName      = "FigletFont",
        int?            letterSpacing = null,
        int?            wordSpacing   = null,
        int?            leading       = null,
        FigletMetadata? metadata      = null
    ) {
        using var reader = new StreamReader(
            stream,
            System.Text.Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 1024,
            leaveOpen: true
        );

        return DeserializeFiglet(reader, fontName, letterSpacing, wordSpacing, leading, metadata);
    }

    /// <summary>
    /// Deserializes a FIGlet font (.flf) from a TextReader.
    /// </summary>
    public static NKFont? DeserializeFiglet(
        TextReader      reader,
        string          fontName      = "FigletFont",
        int?            letterSpacing = null,
        int?            wordSpacing   = null,
        int?            leading       = null,
        FigletMetadata? metadata      = null
    ) {
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
        string   paramsPart  = headerLine[6..];
        string[] paramTokens = paramsPart.Split([' '], StringSplitOptions.RemoveEmptyEntries);

        if (paramTokens.Length < 5) {
            LOGGER.Error(
                "FIGlet header must contain at least 5 parameters (height, baseline, max_length, old_layout, comment_lines)."
            );

            return null;
        }

        if (!int.TryParse(paramTokens[0], out int height)   ||
            !int.TryParse(paramTokens[1], out int baseline) ||
            !int.TryParse(paramTokens[2], out _)            ||
            !int.TryParse(paramTokens[3], out _)            ||
            !int.TryParse(paramTokens[4], out int commentLines)) {
            LOGGER.Error("Failed to parse FIGlet header parameters as integers.");

            return null;
        }

        // Parse comments
        var commentLinesList = new List<string>(commentLines);

        for (int i = 0; i < commentLines; i++) {
            string? line = reader.ReadLine();

            if (line != null) {
                commentLinesList.Add(line);
            }
        }

        var effectiveMeta = ParseFigletComments(commentLinesList, fontName);
        effectiveMeta.MergeWith(metadata);

        var   glyphs  = new Dictionary<NKGlyphSymbol, NKGlyph>();
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

            // Detect endmark character from the first character parsed if not set
            if (endmark == null) {
                string lastLineOfChar = lines[height - 1];

                if (!string.IsNullOrEmpty(lastLineOfChar)) {
                    endmark = lastLineOfChar[^1];
                }
            }

            int charWidth      = 0;
            var processedLines = new string[height];

            for (int i = 0; i < height; i++) {
                string l = lines[i];

                if (endmark != null) {
                    // Strip the trailing endmark characters
                    while (l.Length > 0 && l[^1] == endmark.Value) {
                        l = l[..^1];
                    }
                }

                processedLines[i] = l;

                if (l.Length > charWidth) {
                    charWidth = l.Length;
                }
            }

            // Pad lines to uniform character width and replace hardblanks with space
            for (int i = 0; i < height; i++) {
                string l = processedLines[i];

                if (l.Length < charWidth) {
                    l = l.PadRight(charWidth, ' ');
                }

                processedLines[i] = l.Replace(hardblank, ' ');
            }

            // Create NKGlyph
            // FIGlet characters have size charWidth x height
            if (charWidth == 0) {
                // If it's a zero-width character (sometimes space or null), handle gracefully
                glyphs[NKGlyphSymbol.Simple((char)codePoint)] = new NKGlyph(
                    new GlyphCell[0, height],
                    baselineOffset: 0
                );

                return true;
            }

            var cells = new GlyphCell[charWidth, height];

            for (int x = 0; x < charWidth; x++) {
                for (int y = 0; y < height; y++) {
                    char c = processedLines[y][x];

                    if (c == ' ') {
                        cells[x, y] = GlyphCell.Background;
                    }
                    else {
                        cells[x, y] = GlyphCell.Char(c);
                    }
                }
            }

            glyphs[NKGlyphSymbol.Simple((char)codePoint)] = new NKGlyph(
                cells,
                baselineOffset: 0
            );

            return true;
        }

        // FIGlet specifies 102 required characters in order:
        // ASCII 32 to 126 (95 characters)
        for (int code = 32; code <= 126; code++) {
            if (ParseCharacter(code))
                continue;

            LOGGER.Error(
                "FIGlet font parsing failed: " +
                $"Unexpected EOF while parsing standard ASCII character '{code}' ({(char)code})."
            );

            return null;
        }

        // Read German extended characters (127 to 133)
        int[] germanCodes = [196, 214, 220, 228, 246, 252, 223]; // Ä, Ö, Ü, ä, ö, ü, ß

        for (int i = 0; i < 7; i++) {
            if (!ParseCharacter(germanCodes[i])) {
                LOGGER.Error(
                    $"FIGlet font parsing failed: Unexpected EOF while parsing German extended character at index '{i}'."
                );

                return null;
            }
        }

        while (reader.ReadLine() is { } tagLine) {
            if (string.IsNullOrWhiteSpace(tagLine))
                continue;

            if (!TryParseCodeTag(tagLine, out int codePoint))
                continue;

            if (!ParseCharacter(codePoint))
                break;
        }

        int finalLeading       = leading       ?? effectiveMeta.Leading       ?? height;
        int finalLetterSpacing = letterSpacing ?? effectiveMeta.LetterSpacing ?? 0;

        int finalWordSpacing = wordSpacing ?? effectiveMeta.WordSpacing ??
            (glyphs.TryGetValue(NKGlyphSymbol.Simple(' '), out var spaceGlyph)
                ? spaceGlyph.Width
                : 4);

        var fontInfo = new NKFontInfo(
            fontName,
            ligatures: false,
            leading: finalLeading,
            letterSpacing: finalLetterSpacing,
            wordSpacing: finalWordSpacing,
            variable: new NKFontVariableConfig(false),
            author: effectiveMeta.Author,
            licenseType: effectiveMeta.LicenseType,
            licenseFile: effectiveMeta.LicenseFile,
            licenseContent: effectiveMeta.LicenseContent,
            sourceFormat: effectiveMeta.SourceFormat ?? "Figlet",
            metadata: effectiveMeta.Custom
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

    private static FigletMetadata ParseFigletComments(List<string> comments, string fontName) {
        var meta = new FigletMetadata();

        string? GetValueAfterPrefix(string l, params string[] prefixes) {
            foreach (var prefix in prefixes) {
                if (l.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) {
                    return l[prefix.Length..].Trim();
                }
            }

            return null;
        }

        foreach (var rawLine in comments) {
            string line = rawLine.Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            // 1. Author detection
            if (string.IsNullOrWhiteSpace(meta.Author)) {
                string? candidate = null;
                var authorMatch = GetValueAfterPrefix(line, "author:", "creator:", "by:");

                if (authorMatch != null) {
                    candidate = authorMatch;
                }
                else if (line.StartsWith("by ", StringComparison.OrdinalIgnoreCase)) {
                    candidate = line[3..].Trim();
                }
                else {
                    // Check for patterns like "Shadow by Glenn Chappell 6/93 -- based on Standard"
                    int byIdx = line.IndexOf(" by ", StringComparison.OrdinalIgnoreCase);

                    if (byIdx >= 0) {
                        string prefix = line[..byIdx].Trim();

                        if (byIdx == 0                                                      ||
                            prefix.Equals(fontName, StringComparison.OrdinalIgnoreCase)     ||
                            fontName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
                            prefix.StartsWith(fontName, StringComparison.OrdinalIgnoreCase)) {
                            candidate = line[(byIdx + 4)..].Trim();
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(candidate)) {
                    int dashIdx = candidate.IndexOf(" -- ", StringComparison.Ordinal);

                    string authorPart = dashIdx >= 0
                        ? candidate[..dashIdx].Trim()
                        : candidate;

                    // Strip trailing dates like "6/93", "12/96", or "1994"
                    authorPart = Regex.Replace(authorPart, @"\s+\d{1,2}/\d{2,4}$", "")
                        .Trim();

                    authorPart = Regex.Replace(authorPart, @"\s+\b(19\d{2}|20\d{2})\b$", "")
                        .Trim();

                    if (!string.IsNullOrWhiteSpace(authorPart)) {
                        meta.Author = authorPart;
                    }
                }
            }

            // 2. License detection
            if (string.IsNullOrWhiteSpace(meta.LicenseType)) {
                var licenseMatch = GetValueAfterPrefix(
                    line,
                    "license flags:",
                    "license:",
                    "licence flags:",
                    "licence:"
                );

                if (licenseMatch != null) {
                    meta.LicenseType = licenseMatch;
                }
                else if (line.IndexOf(
                        "Permission is hereby given",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0 ||
                    line.IndexOf("freely distributable", StringComparison.OrdinalIgnoreCase) >= 0) {
                    meta.LicenseType = "Attribution / Permissive";
                }
            }

            // 3. Year / Date detection
            if (!meta.Custom.ContainsKey("Year")) {
                var yearMatch = Regex.Match(line, @"\b(19\d{2}|20\d{2})\b");

                if (yearMatch.Success) {
                    meta.Custom["Year"] = yearMatch.Value;
                }
            }

            // 4. Structured Key: Value lines
            int colonIdx = line.IndexOf(':');

            if (colonIdx is > 0 and < 30 && colonIdx + 1 < line.Length) {
                var key = line[..colonIdx].Trim();
                var val = line[(colonIdx + 1)..].Trim();

                if (!key.Contains(' ') && !string.IsNullOrEmpty(val)) {
                    meta.Custom.TryAdd(key, val);
                }
            }
        }

        return meta;
    }

    private static bool TryParseCodeTag(string line, out int codePoint) {
        codePoint = -1;
        line      = line.Trim();

        if (string.IsNullOrEmpty(line))
            return false;

        int spaceIndex = -1;

        for (int i = 0; i < line.Length; i++) {
            if (char.IsWhiteSpace(line[i])) {
                spaceIndex = i;

                break;
            }
        }

        string codePart = spaceIndex == -1 ? line : line[..spaceIndex];

        try {
            if (codePart.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                codePoint = Convert.ToInt32(codePart[2..], 16);

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
    public static NKFontDeserializationResult TryDeserializeFiglet(
        string          path,
        int?            letterSpacing = null,
        int?            wordSpacing   = null,
        int?            leading       = null,
        FigletMetadata? metadata      = null
    ) {
        var context = new DeserializationContext();

        using (DeserializationContext.Enter(context)) {
            NKFont? font = null;

            try {
                font = DeserializeFiglet(path, letterSpacing, wordSpacing, leading, metadata);
            }
            catch (Exception ex) {
                context.Errors.Add(
                    $"Unhandled exception during deserialization: {ex.Message}\n{ex.StackTrace}"
                );
            }

            return new NKFontDeserializationResult(
                font,
                context.Errors,
                context.Warnings,
                context.Infos
            );
        }
    }

    /// <summary>
    /// Functional FIGlet font deserializer that avoids direct side-effects during execution and returns all collected diagnostic logs as values.
    /// </summary>
    public static NKFontDeserializationResult TryDeserializeFiglet(
        Stream          stream,
        string          fontName      = "FigletFont",
        int?            letterSpacing = null,
        int?            wordSpacing   = null,
        int?            leading       = null,
        FigletMetadata? metadata      = null
    ) {
        var context = new DeserializationContext();

        using (DeserializationContext.Enter(context)) {
            NKFont? font = null;

            try {
                font = DeserializeFiglet(
                    stream,
                    fontName,
                    letterSpacing,
                    wordSpacing,
                    leading,
                    metadata
                );
            }
            catch (Exception ex) {
                context.Errors.Add(
                    $"Unhandled exception during deserialization: {ex.Message}\n{ex.StackTrace}"
                );
            }

            return new NKFontDeserializationResult(
                font,
                context.Errors,
                context.Warnings,
                context.Infos
            );
        }
    }
}