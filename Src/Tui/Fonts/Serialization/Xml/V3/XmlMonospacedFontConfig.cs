// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

namespace NeoKolors.Tui.Fonts.Xml.V3;

public class XmlMonospacedFontConfig : XmlFontSpacing {
    public int  LetterWidth  { get; set; }
    public int  LetterHeight { get; set; }
    public bool AlignToGrid  { get; set; }
    public int? WordSpacing  { get; set; }
}