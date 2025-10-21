//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Xml.Serialization;

namespace NeoKolors.Tui.Fonts.V1;

/// <summary>
/// determines how whitespaces will be treated
/// </summary>
public enum OverlapMode {
    
    [XmlEnum("transparent")]
    TRANSPARENT = 0,
    
    [XmlEnum("overlap")]
    OVERLAP = 1,
    
    [XmlEnum("mask")]
    MASK = 2,
}