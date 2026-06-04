// NeoKolors
// Copyright (c) KryKom 2026

using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using NeoKolors.Common;

namespace NeoKolors.Tui.Fonts.Serialization;

public static partial class NKFontSerializer {
    private const uint BINARY_MAGIC = 0x4E4B4642; // "NKFB" in ASCII (NeoKolors Font Binary)
    private const byte BINARY_VERSION = 2; // Version 2 utilizes MessagePack

    private static readonly MessagePackSerializerOptions OPTIONS = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
            NeoKolorsFormatterResolver.Instance,
            StandardResolver.Instance
        ));

    /// <summary>
    /// Serializes an <see cref="NKFont"/> instance into a binary stream using MessagePack.
    /// </summary>
    public static void SerializeBinary(NKFont font, Stream stream) {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(BINARY_MAGIC);
        writer.Write(BINARY_VERSION);
        writer.Flush();

        MessagePackSerializer.Serialize(stream, font, OPTIONS);
    }

    /// <summary>
    /// Deserializes an <see cref="NKFont"/> instance from a binary stream using MessagePack.
    /// </summary>
    public static NKFont? DeserializeBinary(Stream stream) {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        if (reader.ReadUInt32() != BINARY_MAGIC) {
            LOGGER.Error("Invalid binary font magic header.");

            return null;
        }

        var version = reader.ReadByte();

        if (version == BINARY_VERSION) 
            return MessagePackSerializer.Deserialize<NKFont>(stream, OPTIONS);

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
}

#region Custom MessagePack Formatters

public sealed class NeoKolorsFormatterResolver : IFormatterResolver {
    public static readonly IFormatterResolver Instance = new NeoKolorsFormatterResolver();

    private NeoKolorsFormatterResolver() { }

    public IMessagePackFormatter<T>? GetFormatter<T>() {
        return FormatterCache<T>.Formatter;
    }

    private static class FormatterCache<T> {
        public static readonly IMessagePackFormatter<T>? Formatter;

        static FormatterCache() {
            Formatter = (IMessagePackFormatter<T>?)GetFormatterHelper(typeof(T));
        }

        private static object? GetFormatterHelper(Type t) {
            if (t == typeof(NKStyle)) return new NKStyleFormatter();
            if (t == typeof(AlignPoint)) return new AlignPointFormatter();
            if (t == typeof(GlyphCell)) return new GlyphCellFormatter();
            if (t == typeof(NKGlyph)) return new NKGlyphFormatter();
            if (t == typeof(NKGlyphSymbol)) return new NKGlyphSymbolFormatter();
            if (t == typeof(NKFontInfo)) return new NKFontInfoFormatter();
            if (t == typeof(NKFont)) return new NKFontFormatter();

            return null;
        }
    }
}

public sealed class NKStyleFormatter : IMessagePackFormatter<NKStyle> {
    public void Serialize(ref MessagePackWriter writer, NKStyle value, MessagePackSerializerOptions options) {
        writer.WriteUInt64(value.Raw);
    }

    public NKStyle Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        ulong raw = reader.ReadUInt64();

        return System.Runtime.CompilerServices.Unsafe.As<ulong, NKStyle>(ref raw);
    }
}

public sealed class AlignPointFormatter : IMessagePackFormatter<AlignPoint> {
    public void Serialize(ref MessagePackWriter writer, AlignPoint value, MessagePackSerializerOptions options) {
        writer.WriteArrayHeader(3);
        writer.Write(value.Character);
        writer.Write(value.Position.X);
        writer.Write(value.Position.Y);
    }

    public AlignPoint Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        int count = reader.ReadArrayHeader();

        if (count != 3) throw new MessagePackSerializationException("Invalid AlignPoint array length.");

        char character = reader.ReadChar();
        int x = reader.ReadInt32();
        int y = reader.ReadInt32();

        return new AlignPoint(character, new Metriks.Point2D(x, y));
    }
}

public sealed class GlyphCellFormatter : IMessagePackFormatter<GlyphCell> {
    public void Serialize(ref MessagePackWriter writer, GlyphCell value, MessagePackSerializerOptions options) {
        writer.WriteArrayHeader(2);
        writer.Write((byte)value.Type);

        if (value.Type == GlyphCellType.CHARACTER) {
            writer.Write(value.Character);
        }
        else {
            writer.Write('\0');
        }
    }

    public GlyphCell Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        int count = reader.ReadArrayHeader();

        if (count != 2) throw new MessagePackSerializationException("Invalid GlyphCell array length.");

        var type = (GlyphCellType)reader.ReadByte();
        char character = reader.ReadChar();

        return type switch {
            GlyphCellType.FOREGROUND => GlyphCell.Foreground,
            GlyphCellType.BACKGROUND => GlyphCell.Background,
            _                        => GlyphCell.Char(character)
        };
    }
}

public sealed class NKGlyphFormatter : IMessagePackFormatter<NKGlyph?> {
    public void Serialize(ref MessagePackWriter writer, NKGlyph? value, MessagePackSerializerOptions options) {
        if (value == null) {
            writer.WriteNil();

            return;
        }

        writer.WriteArrayHeader(5);
        writer.Write(value.BaselineOffset);
        writer.Write(value.Width);
        writer.Write(value.Height);

        // 2D Array of cells
        writer.WriteArrayHeader(value.Width * value.Height);
        var cellFormatter = options.Resolver.GetFormatterWithVerify<GlyphCell>();

        for (int x = 0; x < value.Width; x++) {
            for (int y = 0; y < value.Height; y++) {
                cellFormatter.Serialize(ref writer, value.Glyph[x, y], options);
            }
        }

        // AlignPoints
        var apFormatter = options.Resolver.GetFormatterWithVerify<AlignPoint>();
        writer.WriteArrayHeader(value.AlignPoints.Count);

        foreach (var ap in value.AlignPoints) {
            apFormatter.Serialize(ref writer, ap, options);
        }
    }

    public NKGlyph? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        if (reader.IsNil) {
            reader.ReadNil();

            return null;
        }

        int count = reader.ReadArrayHeader();

        if (count != 5) throw new MessagePackSerializationException("Invalid NKGlyph array length.");

        int baselineOffset = reader.ReadInt32();
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();

        var grid = new GlyphCell[width, height];
        int cellCount = reader.ReadArrayHeader();

        if (cellCount != width * height) throw new MessagePackSerializationException("Invalid cell grid size.");

        var cellFormatter = options.Resolver.GetFormatterWithVerify<GlyphCell>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y] = cellFormatter.Deserialize(ref reader, options);
            }
        }

        int apCount = reader.ReadArrayHeader();
        var alignPoints = new AlignPoint[apCount];
        var apFormatter = options.Resolver.GetFormatterWithVerify<AlignPoint>();

        for (int i = 0; i < apCount; i++) {
            alignPoints[i] = apFormatter.Deserialize(ref reader, options);
        }

        return new NKGlyph(grid, baselineOffset, alignPoints);
    }
}

public sealed class NKGlyphSymbolFormatter : IMessagePackFormatter<NKGlyphSymbol> {
    public void Serialize(ref MessagePackWriter writer, NKGlyphSymbol value, MessagePackSerializerOptions options) {
        writer.WriteArrayHeader(4);
        writer.Write(value.Type);

        var styleFormatter = options.Resolver.GetFormatterWithVerify<NKStyle>();
        styleFormatter.Serialize(ref writer, value.Styles, options);

        if (value.IsSimple) {
            writer.Write(value.SimpleSymbol);
            writer.WriteNil();
        }
        else {
            writer.Write('\0');
            writer.Write(value.LigatureSymbol);
        }
    }

    public NKGlyphSymbol Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        int count = reader.ReadArrayHeader();

        if (count != 4) throw new MessagePackSerializationException("Invalid NKGlyphSymbol array length.");

        byte type = reader.ReadByte();
        var styleFormatter = options.Resolver.GetFormatterWithVerify<NKStyle>();
        var styles = styleFormatter.Deserialize(ref reader, options);

        char simpleChar = reader.ReadChar();
        string? ligatureStr = reader.ReadString();

        return type switch {
            1 => NKGlyphSymbol.Simple(simpleChar, styles),
            2 => NKGlyphSymbol.Ligature(ligatureStr ?? string.Empty, styles),
            _ => throw new MessagePackSerializationException($"Unknown glyph symbol type '{type}'.")
        };
    }
}

public sealed class NKFontInfoFormatter : IMessagePackFormatter<NKFontInfo> {
    public void Serialize(ref MessagePackWriter writer, NKFontInfo value, MessagePackSerializerOptions options) {
        writer.WriteArrayHeader(13);
        writer.Write(value.Name);
        writer.Write(value.Ligatures);
        writer.Write(value.Leading);
        writer.Write(value.LetterSpacing);
        writer.Write(value.WordSpacing);

        writer.Write((byte)value.FontPropoInfo.Type);

        if (value.FontPropoInfo.Info.IsT1) {
            var mono = value.FontPropoInfo.AsMono;
            writer.Write(mono.GlyphWidth);
            writer.Write(mono.GlyphHeight);
            writer.Write(mono.AlignToGrid);
        }
        else {
            var variable = value.FontPropoInfo.AsVar;
            writer.Write(variable.Kerning);
            writer.Write(0);
            writer.Write(false);
        }

        writer.Write(value.Author);
        writer.Write(value.LicenseType);
        writer.Write(value.LicenseFile);
        writer.Write(value.LicenseContent);
    }

    public NKFontInfo Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        int count = reader.ReadArrayHeader();

        if (count != 9 && count != 11 && count != 13) throw new MessagePackSerializationException("Invalid NKFontInfo array length.");

        string name = reader.ReadString() ?? string.Empty;
        bool ligatures = reader.ReadBoolean();
        int leading = reader.ReadInt32();
        int letterSpacing = reader.ReadInt32();
        int wordSpacing = reader.ReadInt32();

        var propoType = (FontProportionsInfo.ProportionType)reader.ReadByte();

        bool isMono = !propoType.HasFlag(FontProportionsInfo.ProportionType.VARIABLE);
        NKFontMonospacedConfig mono = default;
        NKFontVariableConfig variable = default;

        if (isMono) {
            int glyphWidth = reader.ReadInt32();
            int glyphHeight = reader.ReadInt32();
            bool alignToGrid = reader.ReadBoolean();
            mono = new NKFontMonospacedConfig(glyphWidth, glyphHeight, alignToGrid);
        }
        else {
            bool kerning = reader.ReadBoolean();
            reader.ReadInt32(); // skip
            reader.ReadBoolean(); // skip
            variable = new NKFontVariableConfig(kerning);
        }

        string? author = null;
        string? licenseType = null;
        string? licenseFile = null;
        string? licenseContent = null;
        if (count >= 11) {
            author = reader.ReadString();
            licenseType = reader.ReadString();
        }
        if (count >= 13) {
            licenseFile = reader.ReadString();
            licenseContent = reader.ReadString();
        }

        return isMono
            ? new NKFontInfo(name, ligatures, leading, letterSpacing, wordSpacing, mono, author, licenseType, licenseFile, licenseContent)
            : new NKFontInfo(name, ligatures, leading, letterSpacing, wordSpacing, variable, author, licenseType, licenseFile, licenseContent);
    }
}

public sealed class NKFontFormatter : IMessagePackFormatter<NKFont?> {
    public void Serialize(ref MessagePackWriter writer, NKFont? value, MessagePackSerializerOptions options) {
        if (value == null) {
            writer.WriteNil();

            return;
        }

        writer.WriteArrayHeader(7);

        var infoFormatter = options.Resolver.GetFormatterWithVerify<NKFontInfo>();
        infoFormatter.Serialize(ref writer, value.Info, options);

        // Glyphs Dictionary
        var symbolFormatter = options.Resolver.GetFormatterWithVerify<NKGlyphSymbol>();
        var glyphFormatter = options.Resolver.GetFormatterWithVerify<NKGlyph?>();

        writer.WriteMapHeader(value.Glyphs.Count);

        foreach (var kvp in value.Glyphs) {
            symbolFormatter.Serialize(ref writer, kvp.Key, options);
            glyphFormatter.Serialize(ref writer, kvp.Value, options);
        }

        // Fallback Glyphs
        glyphFormatter.Serialize(ref writer, value.UnknownSymbolGlyph, options);

        if (value.StrikethroughGlyph != null) {
            glyphFormatter.Serialize(ref writer, value.StrikethroughGlyph, options);
            writer.Write(value.RenderStrikethroughAboveLetters);
        }
        else {
            writer.WriteNil();
            writer.Write(true);
        }

        if (value.UnderlineGlyph != null) {
            glyphFormatter.Serialize(ref writer, value.UnderlineGlyph, options);
            writer.Write(value.RenderUnderlineAboveLetters);
        }
        else {
            writer.WriteNil();
            writer.Write(true);
        }
    }

    public NKFont? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        if (reader.IsNil) {
            reader.ReadNil();

            return null;
        }

        int count = reader.ReadArrayHeader();

        if (count < 7) throw new MessagePackSerializationException("Invalid NKFont array length.");

        var infoFormatter = options.Resolver.GetFormatterWithVerify<NKFontInfo>();
        var info = infoFormatter.Deserialize(ref reader, options);

        var symbolFormatter = options.Resolver.GetFormatterWithVerify<NKGlyphSymbol>();
        var glyphFormatter = options.Resolver.GetFormatterWithVerify<NKGlyph?>();

        int glyphsCount = reader.ReadMapHeader();
        var glyphs = new Dictionary<NKGlyphSymbol, NKGlyph>(glyphsCount);

        for (int i = 0; i < glyphsCount; i++) {
            var symbol = symbolFormatter.Deserialize(ref reader, options);
            var glyph = glyphFormatter.Deserialize(ref reader, options);

            if (glyph != null) {
                glyphs[symbol] = glyph;
            }
        }

        // Fallback Glyphs
        NKGlyph? unknownSymbolGlyph = glyphFormatter.Deserialize(ref reader, options);

        NKGlyph? strikethroughGlyph = null;
        bool renderStrikethroughAboveLetters = true;

        if (!reader.IsNil) {
            strikethroughGlyph = glyphFormatter.Deserialize(ref reader, options);
            renderStrikethroughAboveLetters = reader.ReadBoolean();
        }
        else {
            reader.ReadNil();
            reader.ReadBoolean();
        }

        NKGlyph? underlineGlyph = null;
        bool renderUnderlineAboveLetters = true;

        if (!reader.IsNil) {
            underlineGlyph = glyphFormatter.Deserialize(ref reader, options);
            renderUnderlineAboveLetters = reader.ReadBoolean();
        }
        else {
            reader.ReadNil();
            reader.ReadBoolean();
        }

        return new NKFont(
            info,
            glyphs,
            unknownSymbolGlyph,
            strikethroughGlyph,
            renderStrikethroughAboveLetters,
            underlineGlyph,
            renderUnderlineAboveLetters
        );
    }
}

#endregion