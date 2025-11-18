// NeoKolors
// Copyright (c) 2025 KryKom

using XGI = OneOf.OneOf<
    NeoKolors.Tui.Fonts.Serialization.Xml.XmlComponentGlyphInfo, 
    NeoKolors.Tui.Fonts.Serialization.Xml.XmlLigatureGlyphInfo,
    NeoKolors.Tui.Fonts.Serialization.Xml.XmlCompoundGlyphInfo,
    NeoKolors.Tui.Fonts.Serialization.Xml.XmlAutoCompoundGlyphInfo
>;

namespace NeoKolors.Tui.Fonts.Serialization.Xml;

public class XmlGlyphInfo : OneOfBase<XmlComponentGlyphInfo, XmlLigatureGlyphInfo, XmlCompoundGlyphInfo, XmlAutoCompoundGlyphInfo> {
    public XmlGlyphInfo(XGI input) 
        : base(input) { }

    public string Id {
        get => Match(c => c.Id, l => l.Id, c => c.Id, c => c.Id);
        set => Match(c => c.Id = value, l => l.Id = value, c => c.Id = value, c => c.Id = value);
    }

    public string Symbol => Match(c => c.Symbol, l => l.Symbol, c => c.Symbol, c => c.Symbol);
    
    public string File => Match(c => c.File, l => l.File, _ => throw new InvalidOperationException(), c => c.BaseId);
    public Mask Mask {
        get => Match(c => c.Mask, l => l.Mask, _ => throw new InvalidOperationException(), c => c.Mask);
        set => Switch(c => c.Mask = value, l => l.Mask = value, _ => throw new InvalidOperationException(), c => c.Mask = value);
    }

    public bool IsComponent => IsT0;
    public bool IsLigature => IsT1;
    public bool IsCompound => IsT2;
    public bool IsAutoCompound => IsT3;

    public XmlComponentGlyphInfo AsComponent => AsT0;
    public XmlLigatureGlyphInfo AsLigature => AsT1;
    public XmlCompoundGlyphInfo AsCompound => AsT2;
    public XmlAutoCompoundGlyphInfo AsAutoCompound => AsT3;
    
    public static implicit operator XmlGlyphInfo(XmlComponentGlyphInfo info) => new(info);
    public static implicit operator XmlGlyphInfo(XmlLigatureGlyphInfo info) => new(info);
    public static implicit operator XmlGlyphInfo(XmlCompoundGlyphInfo info) => new(info);
    public static implicit operator XmlGlyphInfo(XmlAutoCompoundGlyphInfo info) => new(info);

    public bool TryAsComponent(out XmlComponentGlyphInfo info) {
        if (IsComponent) {
            info = AsComponent;
            return true;
        }
        
        info = null!;
        return false;
    }

    public bool TryAsLigature(out XmlLigatureGlyphInfo info) {
        if (IsLigature) {
            info = AsLigature;
            return true;
        }

        info = null!;
        return false;
    }

    public bool TryAsCompound(out XmlCompoundGlyphInfo info) {
        if (IsCompound) {
            info = AsCompound;
            return true;
        }

        info = null!;
        return false;
    }

    public bool TryAsAutoCompound(out XmlAutoCompoundGlyphInfo info) {
        if (IsAutoCompound) {
            info = AsAutoCompound;
            return true;
        }

        info = null!;
        return false;
    }
}