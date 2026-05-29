// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Drct = NeoKolors.Tui.Fonts.Serialization.Xml.V3.XmlGlyphAlignDirection;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

[StructLayout(LayoutKind.Explicit)]
public readonly struct XmlGlyphAlign {
    
    [field: FieldOffset(0)] public bool IsDirection { get; }
    [field: FieldOffset(1)] public char Char => IsDirection ? throw new IOp() : field;
    [field: FieldOffset(1)] public Drct Direction => !IsDirection ? throw new IOp() : field;

    public bool IsChar => !IsDirection;

    public XmlGlyphAlign(char c) {
        IsDirection = false;
        Char        = c;
    }

    public XmlGlyphAlign(Drct d) {
        IsDirection = true;
        Direction   = d;
    }

    public static bool TryParse(string s, [NotNullWhen(true)] out XmlGlyphAlign? output) {
        if (string.IsNullOrEmpty(s)) {
            NKFontSerializer.LOGGER.Error("The value for Align is empty or null.");
            output = null;
            return false;
        }

        if (s.StartsWith("custom: ")) {
            if (s.Length != 9) {
                NKFontSerializer.LOGGER.Error($"The value '{s}' is not valid for Align.");
                output = null;
                return false;
            }

            output = new XmlGlyphAlign(s[8]);
            return true;
        }

        try {
            var d = Enum.Parse<Drct>(s.ToUpperInvariant().Replace('-', '_'));
            output = new XmlGlyphAlign(d);
            return true;
        }
        catch (Exception) {
            NKFontSerializer.LOGGER.Error($"The value '{s}' is not valid for Align.");
            output = null;
            return false;
        }
    }
}