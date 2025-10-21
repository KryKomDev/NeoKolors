// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NeoKolors.Tui.Fonts.V2.Exceptions;
using OneOf.Types;

namespace NeoKolors.Tui.Fonts.V2.Serialization.Xml;

public partial class FontMap {

    public int GlyphCount => Items.Length;

    public XmlGlyphInfo[] GetItems() {
        List<XmlGlyphInfo> items = [];
        
        foreach (var o in _itemsField) {
            items.Add(new XmlGlyphInfo(o switch {
                XmlComponentGlyphInfo component => component,
                XmlCompoundGlyphInfo compound => compound,
                XmlAutoCompoundGlyphInfo autoCompound => autoCompound,
                XmlLigatureGlyphInfo ligature => ligature,
                _ => throw FontSerializerException.InvalidMapItem(o.GetType())
            }));
        }
        
        return items.ToArray();
    }
    
    public OneOf<Error<Exception>, None> IsValid() {
        HashSet<string> ids = new();

        for (int i = 0; i < Items.Length; i++) {
            var o = (XmlGlyphInfo)Items[i];
            
            var idr = ValidateId(o.Id, ids);
            if (idr.IsT0) return idr;
            
            var result = o.Match(
                Validate,
                Validate,
                Validate,
                Validate
            );
            
            if (result.IsT1) continue;
            return result.AsT0;
        }
        
        return new None();
    }

    private OneOf<Error<Exception>, None> Validate(XmlComponentGlyphInfo component) {
        var syr = ValidateComponentSymbol(component.Symbol);
        if (syr.IsT0) return syr;
        
        return new None();
    }

    private static OneOf<Error<Exception>, None> ValidateComponentSymbol(string symbol) {
        if (symbol.Length is 1 || Regex.IsMatch(symbol, @"^(\S|(\\x(\d|[a-f]|[A-F]){2}))$")) return new None();
        return new Error<Exception>(FontSerializerException.InvalidSymbol(symbol));
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToNullCoalescingAssignment")]
    public void SetDefaults(FontConfig conf) {
        foreach (var o in _itemsField) {
            
            XmlGlyphInfo i = o switch {
                XmlComponentGlyphInfo component => component,
                XmlCompoundGlyphInfo compound => compound,
                XmlAutoCompoundGlyphInfo autoCompound => autoCompound,
                XmlLigatureGlyphInfo ligature => ligature,
                _ => throw new InvalidOperationException()
            };

            if (i is { IsCompound: false, Mask: null }) {
                i.Mask = conf.Global.Mask;
            }
            
            if (i.IsComponent) {
                var cnt = i.AsComponent;

                if (Equals(cnt.AlignPointReplace, null)) {
                    cnt.AlignPointReplace = conf.Global.AlignPointReplace ?? AlignPointReplace.NewBckg();
                }
                
                if (Equals(cnt.AlignPoints, null)) {
                    cnt.AlignPoints = conf.Global.AlignPoints ?? "";
                }
            }
            else if (i.IsCompound) {
                var cnd = i.AsCompound;
                
                if (Equals(cnd.Align, null)) {
                    cnd.Align = conf.Global.Align;
                }
            }
            else if (i.IsAutoCompound) {
                var acd = i.AsAutoCompound;
                
                if (Equals(acd.Align, null)) {
                    acd.Align = conf.Global.Align;
                }
            }
        }
    }
    
    // TODO: Finish glyph validation methods.
    private OneOf<Error<Exception>, None> Validate(XmlCompoundGlyphInfo compound) {
        return new None();
    }

    private OneOf<Error<Exception>, None> Validate(XmlLigatureGlyphInfo ligature) {
        return new None();       
    }
    
    private OneOf<Error<Exception>, None> Validate(XmlAutoCompoundGlyphInfo autoCompound) {
        return new None();
    }
    
    private static OneOf<Error<Exception>, None> ValidateId(string id, HashSet<string> ids) => 
        !ids.Add(id) 
            ? new Error<Exception>(FontSerializerException.DuplicateGlyphId(id))
            : new None();
}