// NeoKolors
// Copyright (c) KryKom 2026

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

[XmlRoot("FontManifest")]
public class XmlFontManifest {

    [XmlElement] public string Version { get; set; } = null!;
    [XmlElement] public string Map     { get; set; } = null!;
    [XmlElement] public string Config  { get; set; } = null!;
}