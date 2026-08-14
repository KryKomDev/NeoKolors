// NeoKolors
// Copyright (c) krystof 2026

using MessagePack;
using MessagePack.Formatters;

namespace NeoKolors.Common;

/// <summary>
/// A sealed class that implements the IMessagePackFormatter interface for serializing and deserializing
/// instances of the <see cref="AnsiString"/> type using the MessagePack library.
/// Provides custom logic to transform AnsiString objects to and from MessagePack's binary representation.
/// </summary>
public sealed class AnsiStringSerializer : IMessagePackFormatter<AnsiString?> {
    
    public void Serialize(
        ref MessagePackWriter        writer,
        AnsiString?                  value,
        MessagePackSerializerOptions options
    ) {
        if (value == null) {
            writer.WriteNil();

            return;
        }

        writer.WriteArrayHeader(2);
        writer.Write(value.Plain);

        var styles = value.Styles;
        writer.WriteArrayHeader(styles.Length);

        foreach (var marker in styles) {
            writer.WriteArrayHeader(2);
            writer.WriteInt32(marker.Index);
            writer.WriteUInt64(marker.Style.Raw);
        }
    }

    public AnsiString? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        if (reader.IsNil) {
            reader.ReadNil();

            return null;
        }

        int count = reader.ReadArrayHeader();

        if (count != 2)
            throw new MessagePackSerializationException("Invalid AnsiString array length.");

        string? text = reader.ReadString();

        if (text is null)
            throw new MessagePackSerializationException("Invalid null text in AnsiString.");

        int styleCount = reader.ReadArrayHeader();
        var markers    = new List<AnsiString.StyleMarker>(styleCount);

        for (int i = 0; i < styleCount; i++) {
            int markerCount = reader.ReadArrayHeader();

            if (markerCount != 2)
                throw new MessagePackSerializationException("Invalid StyleMarker array length.");

            int   index = reader.ReadInt32();
            ulong raw   = reader.ReadUInt64();
            var   style = Unsafe.As<ulong, NKStyle>(ref raw);

            markers.Add(new AnsiString.StyleMarker(index, style));
        }

        return new AnsiString(text, markers);
    }
}