// NeoKolors
// Copyright (c) KryKom 2026

using NeoKolors.Common;
using ProtoBuf;

namespace NeoKolors.Tui.Fonts;

public static partial class NKFontSerializer {
    private const uint BINARY_MAGIC   = 0x4E4B4642; // "NKFB" in ASCII (NeoKolors Font Binary)
    private const byte BINARY_VERSION = 3;          // Version 3 utilizes Protobuf

    /// <summary>
    /// Serializes an <see cref="NKFont"/> instance into a binary stream using Protobuf.
    /// </summary>
    public static void SerializeBinary(NKFont font, Stream stream) {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(BINARY_MAGIC);
        writer.Write(BINARY_VERSION);
        writer.Flush();

        var proto = ToProto(font);
        Serializer.Serialize(stream, proto);
    }

    /// <summary>
    /// Deserializes an <see cref="NKFont"/> instance from a binary stream using Protobuf.
    /// </summary>
    public static NKFont? DeserializeBinary(Stream stream) {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        if (reader.ReadUInt32() != BINARY_MAGIC) {
            LOGGER.Error("Invalid binary font magic header.");

            return null;
        }

        var version = reader.ReadByte();

        if (version == BINARY_VERSION) {
            var proto = Serializer.Deserialize<ProtoNKFont>(stream);

            return proto == null ? null : ToDomain(proto);
        }

        LOGGER.Error($"Unsupported binary font version '{version}'. Expected '{BINARY_VERSION}'.");

        return null;
    }

    /// <summary>
    /// Serializes an <see cref="NKFont"/> instance into a binary file.
    /// </summary>
    public static void SerializeBinary(NKFont font, string path) {
        using var stream = File.Create(path);
        SerializeBinary(font, stream);
    }

    /// <summary>
    /// Deserializes an <see cref="NKFont"/> instance from a binary file.
    /// </summary>
    public static NKFont? DeserializeBinary(string path) {
        if (!File.Exists(path)) {
            LOGGER.Error($"Binary font file '{path}' not found.");

            return null;
        }

        using var stream = File.OpenRead(path);

        return DeserializeBinary(stream);
    }

    #region Protobuf Mapping

    private static ProtoNKFont ToProto(NKFont font) {
        var proto = new ProtoNKFont {
            Info                            = ToProto(font.Info),
            UnknownSymbolGlyph              = ToProto(font.UnknownSymbolGlyph),
            StrikethroughGlyph              = ToProto(font.StrikethroughGlyph),
            RenderStrikethroughAboveLetters = font.RenderStrikethroughAboveLetters,
            UnderlineGlyph                  = ToProto(font.UnderlineGlyph),
            RenderUnderlineAboveLetters     = font.RenderUnderlineAboveLetters,
            Glyphs                          = new List<ProtoGlyphEntry>(font.Glyphs.Count)
        };

        foreach (var kvp in font.Glyphs) {
            proto.Glyphs.Add(
                new ProtoGlyphEntry {
                    Symbol = ToProto(kvp.Key),
                    Glyph  = ToProto(kvp.Value)
                }
            );
        }

        return proto;
    }

    private static NKFont ToDomain(ProtoNKFont proto) {
        var info   = proto.Info != null ? ToDomain(proto.Info) : default;
        var glyphs = new Dictionary<NKGlyphSymbol, NKGlyph>(proto.Glyphs.Count, NKGlyphSymbol.Comparer);

        foreach (var entry in proto.Glyphs) {
            var symbol = ToDomain(entry.Symbol);
            var glyph  = ToDomain(entry.Glyph);

            if (glyph != null) {
                glyphs[symbol] = glyph;
            }
        }

        var unknownGlyph       = ToDomain(proto.UnknownSymbolGlyph);
        var strikethroughGlyph = ToDomain(proto.StrikethroughGlyph);
        var underlineGlyph     = ToDomain(proto.UnderlineGlyph);

        return new NKFont(
            info,
            glyphs,
            unknownGlyph,
            strikethroughGlyph,
            proto.RenderStrikethroughAboveLetters,
            underlineGlyph,
            proto.RenderUnderlineAboveLetters
        );
    }

    private static ProtoNKFontInfo ToProto(NKFontInfo info) {
        var proto = new ProtoNKFontInfo {
            Name           = info.Name ?? string.Empty,
            Ligatures      = info.Ligatures,
            Leading        = info.Leading,
            LetterSpacing  = info.LetterSpacing,
            WordSpacing    = info.WordSpacing,
            ProportionType = (int)info.ProportionType,
            Author         = info.Author,
            LicenseType    = info.LicenseType,
            LicenseFile    = info.LicenseFile,
            LicenseContent = info.LicenseContent
        };

        if (info.FontPropoInfo.Info.IsT1) {
            var mono = info.FontPropoInfo.AsMono;
            proto.MonoGlyphWidth  = mono.GlyphWidth;
            proto.MonoGlyphHeight = mono.GlyphHeight;
            proto.MonoAlignToGrid = mono.AlignToGrid;
        }
        else {
            var variable = info.FontPropoInfo.AsVar;
            proto.VarKerning = variable.Kerning;
        }

        return proto;
    }

    private static NKFontInfo ToDomain(ProtoNKFontInfo proto) {
        var  propoType = (FontProportionsInfo.ProportionType)proto.ProportionType;
        bool isMono    = !propoType.HasFlag(FontProportionsInfo.ProportionType.VARIABLE);

        int monoDefaultWordSpacing = proto.MonoGlyphWidth + proto.LetterSpacing + 1;
        int wordSpacing = isMono && proto.WordSpacing <= proto.LetterSpacing
            ? monoDefaultWordSpacing
            : proto.WordSpacing;

        return isMono
            ? new NKFontInfo(
                proto.Name,
                proto.Ligatures,
                proto.Leading,
                proto.LetterSpacing,
                wordSpacing,
                new NKFontMonospacedConfig(
                    proto.MonoGlyphWidth,
                    proto.MonoGlyphHeight,
                    proto.MonoAlignToGrid
                ),
                proto.Author,
                proto.LicenseType,
                proto.LicenseFile,
                proto.LicenseContent
            )
            : new NKFontInfo(
                proto.Name,
                proto.Ligatures,
                proto.Leading,
                proto.LetterSpacing,
                proto.WordSpacing,
                new NKFontVariableConfig(proto.VarKerning),
                proto.Author,
                proto.LicenseType,
                proto.LicenseFile,
                proto.LicenseContent
            );
    }

    private static ProtoNKGlyph? ToProto(NKGlyph? glyph) {
        if (glyph == null)
            return null;

        var proto = new ProtoNKGlyph {
            BaselineOffset = glyph.BaselineOffset,
            Width          = glyph.Width,
            Height         = glyph.Height,
            Cells          = new List<ProtoGlyphCell>(glyph.Width * glyph.Height),
            AlignPoints    = new List<ProtoAlignPoint>(glyph.AlignPoints.Count)
        };

        for (int x = 0; x < glyph.Width; x++) {
            for (int y = 0; y < glyph.Height; y++) {
                var cell = glyph.Glyph[x, y];

                proto.Cells.Add(
                    new ProtoGlyphCell {
                        Type      = (int)cell.Type,
                        Character = cell.Type == GlyphCellType.CHARACTER ? cell.Character : '\0'
                    }
                );
            }
        }

        foreach (var ap in glyph.AlignPoints) {
            proto.AlignPoints.Add(
                new ProtoAlignPoint {
                    Character = ap.Character,
                    X         = ap.Position.X,
                    Y         = ap.Position.Y
                }
            );
        }

        return proto;
    }

    private static NKGlyph? ToDomain(ProtoNKGlyph? proto) {
        if (proto == null)
            return null;

        var grid = new GlyphCell[proto.Width, proto.Height];
        int idx  = 0;

        for (int x = 0; x < proto.Width; x++) {
            for (int y = 0; y < proto.Height; y++) {
                if (idx < proto.Cells.Count) {
                    var cell = proto.Cells[idx++];

                    grid[x, y] = (GlyphCellType)cell.Type switch {
                        GlyphCellType.FOREGROUND => GlyphCell.Foreground,
                        GlyphCellType.BACKGROUND => GlyphCell.Background,
                        _                        => GlyphCell.Char((char)cell.Character)
                    };
                }
                else {
                    grid[x, y] = GlyphCell.Background;
                }
            }
        }

        var alignPoints = new AlignPoint[proto.AlignPoints.Count];

        for (int i = 0; i < proto.AlignPoints.Count; i++) {
            var ap = proto.AlignPoints[i];
            alignPoints[i] = new AlignPoint((char)ap.Character, new Metriks.Point2D(ap.X, ap.Y));
        }

        return new NKGlyph(grid, proto.BaselineOffset, alignPoints);
    }

    private static ProtoNKGlyphSymbol ToProto(NKGlyphSymbol symbol) {
        return new ProtoNKGlyphSymbol {
            Type           = symbol.Type,
            Styles         = symbol.Styles,
            SimpleSymbol   = symbol.IsSimple ? symbol.SimpleSymbol : '\0',
            LigatureSymbol = symbol.IsLigature ? symbol.LigatureSymbol : null
        };
    }

    private static NKGlyphSymbol ToDomain(ProtoNKGlyphSymbol proto) {
        return proto.Type switch {
            1 => NKGlyphSymbol.Simple((char)proto.SimpleSymbol, proto.Styles),
            2 => NKGlyphSymbol.Ligature(proto.LigatureSymbol ?? string.Empty, proto.Styles),
            _ => throw new InvalidOperationException($"Unknown glyph symbol type '{proto.Type}'.")
        };
    }

    #endregion

}

#region Protobuf Contracts

[ProtoContract]
internal class ProtoNKFont {
    [ProtoMember(1)] public ProtoNKFontInfo?      Info               { get; set; }
    [ProtoMember(2)] public List<ProtoGlyphEntry> Glyphs             { get; set; } = [];
    [ProtoMember(3)] public ProtoNKGlyph?         UnknownSymbolGlyph { get; set; }
    [ProtoMember(4)] public ProtoNKGlyph?         StrikethroughGlyph { get; set; }

    [ProtoMember(5)]
    [System.ComponentModel.DefaultValue(true)]
    public bool RenderStrikethroughAboveLetters { get; set; } = true;

    [ProtoMember(6)] public ProtoNKGlyph? UnderlineGlyph { get; set; }

    [ProtoMember(7)]
    [System.ComponentModel.DefaultValue(true)]
    public bool RenderUnderlineAboveLetters { get; set; } = true;
}

[ProtoContract]
internal class ProtoGlyphEntry {
    [ProtoMember(1)] public ProtoNKGlyphSymbol Symbol { get; set; }
    [ProtoMember(2)] public ProtoNKGlyph?      Glyph  { get; set; }
}

[ProtoContract]
internal class ProtoNKFontInfo {
    [ProtoMember(1)]  public string  Name            { get; set; } = string.Empty;
    [ProtoMember(2)]  public bool    Ligatures       { get; set; }
    [ProtoMember(3)]  public int     Leading         { get; set; }
    [ProtoMember(4)]  public int     LetterSpacing   { get; set; }
    [ProtoMember(5)]  public int     WordSpacing     { get; set; }
    [ProtoMember(6)]  public int     ProportionType  { get; set; }
    [ProtoMember(7)]  public int     MonoGlyphWidth  { get; set; }
    [ProtoMember(8)]  public int     MonoGlyphHeight { get; set; }
    [ProtoMember(9)]  public bool    MonoAlignToGrid { get; set; }
    [ProtoMember(10)] public bool    VarKerning      { get; set; }
    [ProtoMember(11)] public string? Author          { get; set; }
    [ProtoMember(12)] public string? LicenseType     { get; set; }
    [ProtoMember(13)] public string? LicenseFile     { get; set; }
    [ProtoMember(14)] public string? LicenseContent  { get; set; }
}

[ProtoContract]
internal struct ProtoNKGlyphSymbol {
    [ProtoMember(1)] public byte    Type           { get; set; }
    [ProtoMember(2)] public NKStyle Styles         { get; set; }
    [ProtoMember(3)] public int     SimpleSymbol   { get; set; }
    [ProtoMember(4)] public string? LigatureSymbol { get; set; }
}

[ProtoContract]
internal class ProtoNKGlyph {
    [ProtoMember(1)] public int                   BaselineOffset { get; set; }
    [ProtoMember(2)] public int                   Width          { get; set; }
    [ProtoMember(3)] public int                   Height         { get; set; }
    [ProtoMember(4)] public List<ProtoGlyphCell>  Cells          { get; set; } = [];
    [ProtoMember(5)] public List<ProtoAlignPoint> AlignPoints    { get; set; } = [];
}

[ProtoContract]
internal struct ProtoGlyphCell {
    [ProtoMember(1)] public int Type      { get; set; }
    [ProtoMember(2)] public int Character { get; set; }
}

[ProtoContract]
internal struct ProtoAlignPoint {
    [ProtoMember(1)] public int Character { get; set; }
    [ProtoMember(2)] public int X         { get; set; }
    [ProtoMember(3)] public int Y         { get; set; }
}

#endregion