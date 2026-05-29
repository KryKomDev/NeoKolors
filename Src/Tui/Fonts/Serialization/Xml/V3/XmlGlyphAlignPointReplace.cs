// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Diagnostics.CodeAnalysis;
using System.Text;
using NeoKolors.Extensions;
using static NeoKolors.Tui.Fonts.Serialization.Xml.V3.XmlGlyphAlignPointReplaceType;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

/// <summary>
/// Defines how the characters interpreted as align-points in the glyph file should be erased or replaced.
/// </summary>
public readonly struct XmlGlyphAlignPointReplace {

    private readonly XmlGlyphAlignPointReplaceType _default;
    private readonly HashSet<char>                 _customNone;
    private readonly HashSet<char>                 _customForg;
    private readonly HashSet<char>                 _customBckg;
    private readonly Dictionary<char, char>        _customPairs;
    private readonly char?                         _singleReplacement;

    public XmlGlyphAlignPointReplaceType Default           => _default;
    public HashSet<char>                 CustomNone        => _customNone;
    public HashSet<char>                 CustomForg        => _customForg;
    public HashSet<char>                 CustomBckg        => _customBckg;
    public Dictionary<char, char>        CustomPairs       => _customPairs;
    public char?                         SingleReplacement => _singleReplacement;

    // For backward compatibility
    public bool Forg => _default == FORG;
    public bool Bckg => _default == BCKG;

    public XmlGlyphAlignPointReplace(
        XmlGlyphAlignPointReplaceType @default          = UNSPECIFIED,
        HashSet<char>?                customNone        = null,
        HashSet<char>?                customForg        = null,
        HashSet<char>?                customBckg        = null,
        Dictionary<char, char>?       customPairs       = null,
        char?                         singleReplacement = null) 
    {
        _default           = @default;
        _customNone        = customNone  ?? [];
        _customForg        = customForg  ?? [];
        _customBckg        = customBckg  ?? [];
        _customPairs       = customPairs ?? [];
        _singleReplacement = singleReplacement;
    }

    public XmlGlyphAlignPointReplace() {
        _default           = BCKG;
        _customNone        = [];
        _customForg        = [];
        _customBckg        = [];
        _customPairs       = [];
        _singleReplacement = null;
    }
    
    public XmlGlyphAlignPointReplace(bool isForg) : this(isForg ? FORG : BCKG) { }

    public static XmlGlyphAlignPointReplace? Parse(string value) {
        if (TryParse(value, out var output)) return output;
        return null;
    }

    public static bool TryParse(string value, [NotNullWhen(true)] out XmlGlyphAlignPointReplace? output) {
        if (string.IsNullOrEmpty(value)) {
            NKFontSerializer.LOGGER.Error("The value for AlignPointReplace is empty or null.");
            output = null;
            return false;
        }

        var                defaultType       = UNSPECIFIED;
        List<char>         customNone        = [];
        List<char>         customForg        = [];
        List<char>         customBckg        = [];
        List<(char, char)> customPairs       = [];
        char?              singleReplacement = null;

        var parts = SplitByComma(value);

        foreach (var p in parts.Select(part => part.Trim()).Where(p => !string.IsNullOrEmpty(p))) {
            if (p is "forg" or "bckg" or "none") {
                var type = p switch {
                    "forg" => FORG,
                    "bckg" => BCKG,
                    "none" => NONE,
                    _      => throw new Exception("Unreachable")
                };

                if (defaultType != UNSPECIFIED) {
                    NKFontSerializer.LOGGER.Error(
                        "The value for AlignPointReplace cannot contain multiple of 'forg', 'bckg' and 'none' without specified characters.");
                    output = null;
                    return false;
                }

                defaultType = type;
                continue;
            }

            if (p.StartsWith("forg:")) {
                var content = ExtractQuotedContent(p["forg:".Length..].Trim());

                if (content == null) {
                    output = null; 
                    return false;
                }
                
                customForg.AddRange(content.Unescape());
            }
            else if (p.StartsWith("bckg:")) {
                var content = ExtractQuotedContent(p["bckg:".Length..].Trim());

                if (content == null) {
                    output = null;
                    return false;
                }
                
                customBckg.AddRange(content.Unescape());
            }
            else if (p.StartsWith("none:")) {
                var content = ExtractQuotedContent(p["none:".Length..].Trim());

                if (content == null) {
                    output = null;
                    return false;
                }
                
                customNone.AddRange(content.Unescape());
            }
            else if (p.StartsWith("custom:")) {
                var content = p["custom:".Length..].Trim();
                var quotes  = SplitByQuotes(content);
                
                if (quotes.Count < 2 || quotes.Count % 2 != 0) {
                    NKFontSerializer.LOGGER.Error($"Invalid format for custom replacement: '{p}'.");
                    output = null;
                    return false;
                }

                for (int i = 0; i < quotes.Count; i += 2) {
                    var replaced    = quotes[i].Unescape();
                    var replacement = quotes[i + 1].Unescape();

                    if (replaced.Length != replacement.Length) {
                        NKFontSerializer.LOGGER.Error(
                            $"Mismatch in replacement character counts: '{quotes[i]}' and '{quotes[i + 1]}'.");
                        output = null;
                        return false;
                    }

                    for (int j = 0; j < replaced.Length; j++) {
                        customPairs.Add((replaced[j], replacement[j]));
                    }
                }
            }
            else {
                // Try as single character replacement
                var unescaped = p.Unescape();
                
                if (unescaped.Length == 1) {
                    if (singleReplacement.HasValue) {
                        NKFontSerializer.LOGGER.Error("Multiple single character replacements are not allowed.");
                        output = null;
                        return false;
                    }

                    singleReplacement = unescaped[0];
                }
                else {
                    NKFontSerializer.LOGGER.Error($"Unknown value for AlignPointReplace: '{p}'.");
                    output = null;
                    return false;
                }
            }
        }

        output = new XmlGlyphAlignPointReplace(
            defaultType,
            customNone.ToHashSet(),
            customForg.ToHashSet(),
            customBckg.ToHashSet(),
            customPairs.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2),
            singleReplacement
        );
        
        return true;
    }

    private static List<string> SplitByComma(string input) {
        List<string> parts    = [];
        bool         inQuotes = false;
        bool         escaped  = false;
        int          start    = 0;

        for (int i = 0; i < input.Length; i++) {
            if (escaped) {
                escaped = false;
                continue;
            }

            switch (input[i]) {
                case '\\': {
                    escaped = true;
                    continue;
                }
                case '\'': {
                    inQuotes = !inQuotes;
                    break;
                }
                case ',' when !inQuotes: {
                    parts.Add(input.Substring(start, i - start));
                    start = i + 1;
                    break;
                }
            }
        }

        parts.Add(input[start..]);
        return parts;
    }

    private static List<string> SplitByQuotes(string input) {
        List<string> parts    = [];
        bool         inQuotes = false;
        bool         escaped  = false;
        int          start    = 0;

        for (int i = 0; i < input.Length; i++) {
            if (escaped) {
                escaped = false;
                continue;
            }

            switch (input[i]) {
                case '\\': {
                    escaped = true;
                    continue;
                }
                case '\'': {
                    if (inQuotes) {
                        parts.Add(input.Substring(start, i - start));
                    }
                    else {
                        start = i + 1;
                    }

                    inQuotes = !inQuotes;
                    break;
                }
            }
        }

        return parts;
    }

    private static string? ExtractQuotedContent(string input) {
        if (input is ['\'', _, ..] && input[^1] == '\'')
            return input.Substring(1, input.Length - 2);

        NKFontSerializer.LOGGER.Error(
            $"Invalid format for custom replace value: '{input}'. It must be enclosed in single quotes.");

        return null;
    }

    public override string ToString() {
        var sb = new StringBuilder();

        if (_singleReplacement.HasValue) {
            sb.Append(char.ToStringEsc(_singleReplacement.Value));
        }

        if (_customPairs.Count > 0) {
            if (sb.Length > 0) sb.Append(", ");

            sb.Append("custom: '");
            foreach (var pair in _customPairs) sb.Append(char.ToStringEsc(pair.Key));
            sb.Append("' '");
            foreach (var pair in _customPairs) sb.Append(char.ToStringEsc(pair.Value));
            sb.Append('\'');
        }

        if (_customForg.Count > 0) {
            if (sb.Length > 0) sb.Append(", ");
            sb.Append("forg: '");
            foreach (var c in _customForg) sb.Append(char.ToStringEsc(c));
            sb.Append('\'');
        }

        if (_customBckg.Count > 0) {
            if (sb.Length > 0) sb.Append(", ");
            sb.Append("bckg: '");
            foreach (var c in _customBckg) sb.Append(char.ToStringEsc(c));
            sb.Append('\'');
        }

        if (_customNone.Count > 0) {
            if (sb.Length > 0) sb.Append(", ");
            sb.Append("none: '");
            foreach (var c in _customNone) sb.Append(char.ToStringEsc(c));
            sb.Append('\'');
        }

        if (_default == UNSPECIFIED) 
            return sb.ToString();

        if (sb.Length > 0) 
            sb.Append(", ");
        
        sb.Append(_default switch {
            FORG => "forg",
            BCKG => "bckg",
            NONE => "none",
            _    => ""
        });

        return sb.ToString();
    }
}
