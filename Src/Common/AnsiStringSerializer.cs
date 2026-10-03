// NeoKolors
// Copyright (c) krystof 2026

using ProtoBuf;

namespace NeoKolors.Common;

/// <summary>
/// Provides serialization and deserialization utilities for <see cref="AnsiString"/> instances using Protobuf.
/// </summary>
public static class AnsiStringSerializer {

    [ProtoContract]
    public class ProtoAnsiStringPayload {
        [ProtoMember(1)] public string Plain { get; set; } = string.Empty;
        [ProtoMember(2)] public List<ProtoStyleMarker> Markers { get; set; } = [];
    }

    [ProtoContract]
    public struct ProtoStyleMarker {
        [ProtoMember(1)] public int Index { get; set; }
        [ProtoMember(2)] public NKStyle Style { get; set; }
    }

    [ProtoContract]
    public class ProtoNullableAnsiString {
        [ProtoMember(1)] public bool HasValue { get; set; }
        [ProtoMember(2)] public ProtoAnsiStringPayload? Payload { get; set; }
    }

    /// <summary>
    /// Serializes an <see cref="AnsiString"/> instance into a byte array using Protobuf.
    /// </summary>
    public static byte[] Serialize(AnsiString? value) {
        using var ms = new MemoryStream();
        Serialize(ms, value);
        return ms.ToArray();
    }

    /// <summary>
    /// Serializes an <see cref="AnsiString"/> instance into the specified stream using Protobuf.
    /// </summary>
    public static void Serialize(Stream stream, AnsiString? value) {
        var proto = new ProtoNullableAnsiString {
            HasValue = value != null,
            Payload  = value == null ? null : ToPayload(value)
        };
        Serializer.Serialize(stream, proto);
    }

    /// <summary>
    /// Deserializes an <see cref="AnsiString"/> instance from a byte array using Protobuf.
    /// </summary>
    public static AnsiString? Deserialize(byte[] bytes) {
        using var ms = new MemoryStream(bytes);
        return Deserialize(ms);
    }

    /// <summary>
    /// Deserializes an <see cref="AnsiString"/> instance from a stream using Protobuf.
    /// </summary>
    public static AnsiString? Deserialize(Stream stream) {
        var proto = Serializer.Deserialize<ProtoNullableAnsiString>(stream);
        if (proto is not { HasValue: true } || proto.Payload == null)
            return null;

        return ToDomain(proto.Payload);
    }

    public static ProtoAnsiStringPayload ToPayload(AnsiString value) {
        var payload = new ProtoAnsiStringPayload {
            Plain   = value.Plain,
            Markers = new List<ProtoStyleMarker>(value.Styles.Length)
        };

        foreach (var marker in value.Styles) {
            payload.Markers.Add(new ProtoStyleMarker {
                Index = marker.Index,
                Style = marker.Style
            });
        }

        return payload;
    }

    public static AnsiString ToDomain(ProtoAnsiStringPayload payload) {
        var markers = new List<AnsiString.StyleMarker>(payload.Markers.Count);
        foreach (var marker in payload.Markers) {
            markers.Add(new AnsiString.StyleMarker(marker.Index, marker.Style));
        }

        return new AnsiString(payload.Plain, markers);
    }
}