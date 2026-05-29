// NeoKolors
// Copyright (c) 2026 KryKom

// This controls what schema location is used for the font definitions.
// Valid values are: NKFONT_XML_RELEASE, NKFONT_XML_SNAPSHOT, NKFONT_XML_WIP.
#define NKFONT_XML_WIP

namespace NeoKolors.Tui.Fonts.Serialization;

public static class NKFontSchema {

    // Pozn. autora: už mi z tech padělanejch xsd jebe.
    // Asi se z toho zabiju už fakt.
    
    public const string V2 = "https://raw.githubusercontent.com/KryKomDev/NeoKolors/refs/heads/main/NeoKolors.Tui/Schemas/Fonts/V2/Font.xsd";
    public const string V3 = "https://krykomdev.github.io/NeoKolors/Schemas/Fonts/v3/";
}