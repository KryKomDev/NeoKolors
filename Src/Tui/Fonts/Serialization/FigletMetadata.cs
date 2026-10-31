// NeoKolors
// Copyright (c) KryKom 2026

using System.Xml.Linq;

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// Represents metadata associated with a FIGlet (.flf) font.
/// </summary>
public class FigletMetadata {
    public string? Author { get; set; }
    public string? LicenseType { get; set; }
    public string? LicenseFile { get; set; }
    public string? LicenseContent { get; set; }
    public string? SourceFormat { get; set; } = "Figlet";
    public int? LetterSpacing { get; set; }
    public int? WordSpacing { get; set; }
    public int? Leading { get; set; }
    public Dictionary<string, string> Custom { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Merges another metadata object into this instance, overriding existing values where specified.
    /// </summary>
    public void MergeWith(FigletMetadata? other) {
        if (other == null) return;
        if (!string.IsNullOrWhiteSpace(other.Author)) Author = other.Author;
        if (!string.IsNullOrWhiteSpace(other.LicenseType)) LicenseType = other.LicenseType;
        if (!string.IsNullOrWhiteSpace(other.LicenseFile)) LicenseFile = other.LicenseFile;
        if (!string.IsNullOrWhiteSpace(other.LicenseContent)) LicenseContent = other.LicenseContent;
        if (!string.IsNullOrWhiteSpace(other.SourceFormat)) SourceFormat = other.SourceFormat;
        if (other.LetterSpacing.HasValue) LetterSpacing = other.LetterSpacing;
        if (other.WordSpacing.HasValue) WordSpacing = other.WordSpacing;
        if (other.Leading.HasValue) Leading = other.Leading;

        foreach (var (k, v) in other.Custom) {
            Custom[k] = v;
        }
    }

    /// <summary>
    /// Parses a single metadata XML document representing one font (e.g. sidecar file).
    /// </summary>
    public static FigletMetadata? FromXml(Stream stream) {
        try {
            var doc = XDocument.Load(stream);
            var root = doc.Root;
            if (root == null) return null;

            return ParseFontElement(root);
        }
        catch {
            return null;
        }
    }

    /// <summary>
    /// Parses a manifest XML document containing metadata for multiple fonts.
    /// </summary>
    public static Dictionary<string, FigletMetadata> FromManifestXml(Stream stream) {
        var result = new Dictionary<string, FigletMetadata>(StringComparer.OrdinalIgnoreCase);
        try {
            var doc = XDocument.Load(stream);
            var root = doc.Root;
            if (root == null) return result;

            foreach (var fontElem in root.Elements()) {
                string? name = fontElem.Attribute("Name")?.Value 
                            ?? fontElem.Attribute("name")?.Value
                            ?? fontElem.Attribute("File")?.Value
                            ?? fontElem.Attribute("file")?.Value;

                if (!string.IsNullOrEmpty(name)) {
                    name = Path.GetFileNameWithoutExtension(name);
                    result[name] = ParseFontElement(fontElem);
                }
            }
        }
        catch { }

        return result;
    }

    public static FigletMetadata ParseFontElement(XElement element) {
        var meta = new FigletMetadata();

        // 1. Attributes
        meta.Author = element.Attribute("Author")?.Value ?? element.Attribute("author")?.Value;
        meta.LicenseType = element.Attribute("LicenseType")?.Value ?? element.Attribute("licenseType")?.Value 
                        ?? element.Attribute("License")?.Value ?? element.Attribute("license")?.Value;
        meta.LicenseFile = element.Attribute("LicenseFile")?.Value ?? element.Attribute("licenseFile")?.Value;
        meta.LicenseContent = element.Attribute("LicenseContent")?.Value ?? element.Attribute("licenseContent")?.Value;
        meta.SourceFormat = element.Attribute("SourceFormat")?.Value ?? element.Attribute("sourceFormat")?.Value 
                         ?? element.Attribute("Format")?.Value ?? element.Attribute("format")?.Value ?? "Figlet";

        if (int.TryParse(element.Attribute("LetterSpacing")?.Value ?? element.Attribute("letterSpacing")?.Value, out int ls))
            meta.LetterSpacing = ls;
        if (int.TryParse(element.Attribute("WordSpacing")?.Value ?? element.Attribute("wordSpacing")?.Value, out int ws))
            meta.WordSpacing = ws;
        if (int.TryParse(element.Attribute("Leading")?.Value ?? element.Attribute("leading")?.Value, out int ld))
            meta.Leading = ld;

        foreach (var attr in element.Attributes()) {
            var attrName = attr.Name.LocalName;
            if (attrName.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("File", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("Author", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("License", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("LicenseType", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("LicenseFile", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("LicenseContent", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("SourceFormat", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("Format", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("LetterSpacing", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("WordSpacing", StringComparison.OrdinalIgnoreCase) ||
                attrName.Equals("Leading", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }
            meta.Custom[attrName] = attr.Value;
        }

        // 2. Child elements
        foreach (var child in element.Elements()) {
            var childName = child.Name.LocalName;
            if (childName.Equals("Author", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(meta.Author)) {
                meta.Author = child.Value.Trim();
            }
            else if ((childName.Equals("License", StringComparison.OrdinalIgnoreCase) ||
                      childName.Equals("LicenseType", StringComparison.OrdinalIgnoreCase)) && string.IsNullOrWhiteSpace(meta.LicenseType)) {
                meta.LicenseType = child.Value.Trim();
            }
            else if (childName.Equals("LicenseFile", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(meta.LicenseFile)) {
                meta.LicenseFile = child.Value.Trim();
            }
            else if (childName.Equals("LicenseContent", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(meta.LicenseContent)) {
                meta.LicenseContent = child.Value.Trim();
            }
            else if (childName.Equals("SourceFormat", StringComparison.OrdinalIgnoreCase) || childName.Equals("Format", StringComparison.OrdinalIgnoreCase)) {
                meta.SourceFormat = child.Value.Trim();
            }
            else if (childName.Equals("LetterSpacing", StringComparison.OrdinalIgnoreCase) && !meta.LetterSpacing.HasValue && int.TryParse(child.Value.Trim(), out int cls)) {
                meta.LetterSpacing = cls;
            }
            else if (childName.Equals("WordSpacing", StringComparison.OrdinalIgnoreCase) && !meta.WordSpacing.HasValue && int.TryParse(child.Value.Trim(), out int cws)) {
                meta.WordSpacing = cws;
            }
            else if (childName.Equals("Leading", StringComparison.OrdinalIgnoreCase) && !meta.Leading.HasValue && int.TryParse(child.Value.Trim(), out int cld)) {
                meta.Leading = cld;
            }
            else if (childName.Equals("Metadata", StringComparison.OrdinalIgnoreCase)) {
                if (child.HasElements) {
                    foreach (var sub in child.Elements()) {
                        var k = sub.Attribute("Key")?.Value ?? sub.Attribute("key")?.Value ?? sub.Attribute("Name")?.Value;
                        var v = sub.Attribute("Value")?.Value ?? sub.Attribute("value")?.Value;
                        if (!string.IsNullOrEmpty(k)) {
                            meta.Custom[k] = v ?? sub.Value.Trim();
                        }
                        else if (!sub.HasElements) {
                            meta.Custom[sub.Name.LocalName] = sub.Value.Trim();
                        }
                    }
                }
                else {
                    var k = child.Attribute("Key")?.Value ?? child.Attribute("key")?.Value ?? child.Attribute("Name")?.Value;
                    var v = child.Attribute("Value")?.Value ?? child.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(k)) {
                        meta.Custom[k] = v ?? child.Value.Trim();
                    }
                }
            }
            else if (childName.Equals("Item", StringComparison.OrdinalIgnoreCase)) {
                var k = child.Attribute("Key")?.Value ?? child.Attribute("key")?.Value ?? child.Attribute("Name")?.Value;
                var v = child.Attribute("Value")?.Value ?? child.Attribute("value")?.Value;
                if (!string.IsNullOrEmpty(k)) {
                    meta.Custom[k] = v ?? child.Value.Trim();
                }
                else if (!child.HasElements) {
                    meta.Custom[child.Name.LocalName] = child.Value.Trim();
                }
            }
            else if (!child.HasElements) {
                meta.Custom[childName] = child.Value.Trim();
            }
        }

        return meta;
    }
}
