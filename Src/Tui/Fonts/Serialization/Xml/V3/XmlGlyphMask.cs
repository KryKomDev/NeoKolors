// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Diagnostics.CodeAnalysis;
using System.Text;
using NeoKolors.Extensions;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public readonly struct XmlGlyphMask {
    
    public XmlSpaceMask SpaceConf { get; }
    public char[]    MapForg { get; }
    public char[]    MapBckg { get; }
    
    public XmlGlyphMask(XmlSpaceMask spaceConf = XmlSpaceMask.SPACE_CHAR, char[]? mapForg = null, char[]? mapBckg = null) {
        SpaceConf = spaceConf;
        MapForg   = mapForg ?? [];
        MapBckg   = mapBckg ?? [];
    }

    public static bool TryParse(string value, [NotNullWhen(true)] out XmlGlyphMask? output) {
        if (string.IsNullOrEmpty(value)) {
            NKFontSerializer.LOGGER.Error("The value for Mask is empty or null.");
            output = null;
            return false;
        }

        var        spaceConf = XmlSpaceMask.INVALID;
        List<char> mapForg   = [];
        List<char> mapBckg   = [];

        var parts = SplitByComma(value);

        foreach (var p in parts.Select(part => part.Trim()).Where(p => !string.IsNullOrEmpty(p))) {
            switch (p) {
                case "forg" when spaceConf != XmlSpaceMask.INVALID: {
                    NKFontSerializer.LOGGER.Error(
                        "The value for Mask cannot contain a combination of 'forg', 'bckg' and 'space'.");
                   
                    output = null;
                    return false;
                }
                case "forg": {
                    spaceConf = XmlSpaceMask.FOREGROUND;
                    break;
                }
                case "bckg" when spaceConf != XmlSpaceMask.INVALID: {
                    NKFontSerializer.LOGGER.Error(
                        "The value for Mask cannot contain a combination of 'forg', 'bckg' and 'space'.");
                   
                    output = null;
                    return false;
                }
                case "bckg": {
                    spaceConf = XmlSpaceMask.BACKGROUND;
                    break;
                }
                case "space" when spaceConf != XmlSpaceMask.INVALID: {
                    NKFontSerializer.LOGGER.Error(
                        "The value for Mask cannot contain a combination of 'forg', 'bckg' and 'space'.");
                   
                    output = null;
                    return false;
                }
                case "space": {
                    spaceConf = 0;
                    break;
                }
                default: {
                    if (p.StartsWith("custom-forg:")) {
                        var content = ExtractQuotedContent(p["custom-forg:".Length..].Trim());
                        if (content == null) {
                            output = null;
                            return false;
                        }

                        mapForg.AddRange(content.Unescape());
                    }
                    else if (p.StartsWith("custom-bckg:")) {
                        var content = ExtractQuotedContent(p["custom-bckg:".Length..].Trim());
                        if (content == null) {
                            output = null;
                            return false;
                        }

                        mapBckg.AddRange(content.Unescape());
                    }
                    else {
                        NKFontSerializer.LOGGER.Error($"Unknown value for Mask: '{p}'.");
                        output = null;
                        return false;
                    }

                    break;
                }
            }
        }

        output = new XmlGlyphMask(spaceConf == XmlSpaceMask.INVALID ? XmlSpaceMask.UNSPECIFIED : spaceConf, mapForg.ToArray(), mapBckg.ToArray());
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

    private static string? ExtractQuotedContent(string input) {
        if (input is ['\'', _, ..] && input[^1] == '\'') 
            return input.Substring(1, input.Length - 2);
        
        NKFontSerializer.LOGGER.Error(
            $"Invalid format for custom mask value: '{input}'. It must be enclosed in single quotes.");
            
        return null;
    }

    public override string ToString() {
        var sb = new StringBuilder();
        
        if (MapForg.Length != 0) {
            sb.Append("custom-forg: '");
            
            foreach (var f in MapForg) {
                sb.Append(char.ToStringEsc(f));
            }

            sb.Append('\'');
        }

        if (MapBckg.Length != 0) {
            if (sb.Length != 0)
                sb.Append(", ");

            sb.Append("custom-bckg");

            foreach (var b in MapBckg) {
                sb.Append(char.ToStringEsc(b));
            }

            sb.Append('\'');
        }

        if (sb.Length != 0)
            sb.Append(", ");

        sb.Append(SpaceConf switch {
            XmlSpaceMask.FOREGROUND  => "forg",
            XmlSpaceMask.BACKGROUND  => "bckg",
            XmlSpaceMask.SPACE_CHAR  => "space",
            XmlSpaceMask.INVALID     => "space",
            XmlSpaceMask.UNSPECIFIED => "space",
            _                     => throw new ArgumentOutOfRangeException()
        });

        return sb.ToString();
    }
}
