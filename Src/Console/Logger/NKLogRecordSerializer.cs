// NeoKolors
// Copyright (c) KryKom 2026

using Microsoft.Extensions.Logging;
using NeoKolors.Common;
using OneOf;
using ProtoBuf;

namespace NeoKolors.Console;

/// <summary>
/// Provides serialization utilities for <see cref="NKLogRecord"/> using Protobuf.
/// </summary>
public static class NKLogRecordSerializer {

    [ProtoContract]
    internal class ProtoNKLogRecord {
        [ProtoMember(1)] public long TimestampBinary { get; set; }
        [ProtoMember(2)] public int Level { get; set; }
        [ProtoMember(3)] public ProtoEventId? EventId { get; set; }
        [ProtoMember(4)] public string? Source { get; set; }
        [ProtoMember(5)] public ProtoLogMessage? Message { get; set; }
    }

    [ProtoContract]
    internal class ProtoEventId {
        [ProtoMember(1)] public int Id { get; set; }
        [ProtoMember(2)] public string? Name { get; set; }
    }

    [ProtoContract]
    internal class ProtoLogMessage {
        [ProtoMember(1)] public int MessageType { get; set; } // 0 = AnsiString, 1 = Exception
        [ProtoMember(2)] public AnsiStringSerializer.ProtoAnsiStringPayload? AnsiString { get; set; }
        [ProtoMember(3)] public ProtoExceptionInfo? Exception { get; set; }
    }

    [ProtoContract]
    internal class ProtoExceptionInfo {
        [ProtoMember(1)] public string Type { get; set; } = string.Empty;
        [ProtoMember(2)] public string Message { get; set; } = string.Empty;
        [ProtoMember(3)] public string StackTrace { get; set; } = string.Empty;
    }

    /// <summary>
    /// Serializes an <see cref="NKLogRecord"/> to the specified stream using Protobuf.
    /// </summary>
    public static void Serialize(Stream stream, NKLogRecord record) {
        var proto = new ProtoNKLogRecord {
            TimestampBinary = record.Timestamp.ToBinary(),
            Level           = (int)record.Level,
            Source          = record.Source,
            EventId         = record.EventId == null ? null : new ProtoEventId {
                Id   = record.EventId.Value.Id,
                Name = record.EventId.Value.Name
            },
            Message         = record.Message.IsT0
                ? new ProtoLogMessage {
                    MessageType = 0,
                    AnsiString  = AnsiStringSerializer.ToPayload(record.Message.AsT0)
                }
                : new ProtoLogMessage {
                    MessageType = 1,
                    Exception   = new ProtoExceptionInfo {
                        Type       = record.Message.AsT1.GetType().FullName ?? record.Message.AsT1.GetType().Name,
                        Message    = record.Message.AsT1.Message,
                        StackTrace = record.Message.AsT1.StackTrace ?? string.Empty
                    }
                }
        };

        Serializer.SerializeWithLengthPrefix(stream, proto, PrefixStyle.Base128);
    }

    /// <summary>
    /// Deserializes an <see cref="NKLogRecord"/> from the specified stream using Protobuf.
    /// </summary>
    public static NKLogRecord Deserialize(Stream stream) {
        var proto = Serializer.DeserializeWithLengthPrefix<ProtoNKLogRecord>(stream, PrefixStyle.Base128);
        if (proto == null)
            throw new InvalidOperationException("Failed to deserialize log record from stream.");

        var timestamp = DateTime.FromBinary(proto.TimestampBinary);
        var level     = (NKLogLevel)proto.Level;
        EventId? eventId = proto.EventId == null
            ? null
            : new EventId(proto.EventId.Id, proto.EventId.Name);

        OneOf<AnsiString, Exception> message = default;
        if (proto.Message != null) {
            if (proto.Message.MessageType == 0 && proto.Message.AnsiString != null) {
                message = OneOf<AnsiString, Exception>.FromT0(AnsiStringSerializer.ToDomain(proto.Message.AnsiString));
            }
            else if (proto.Message.Exception != null) {
                var exInfo = proto.Message.Exception;
                message = OneOf<AnsiString, Exception>.FromT1(new DeserializedException(exInfo.Type, exInfo.Message, exInfo.StackTrace));
            }
        }

        return new NKLogRecord(timestamp, level, message, eventId, proto.Source);
    }
}

/// <summary>
/// A synthetic exception representing a deserialized exception from the log binary data.
/// </summary>
[Serializable]
public class DeserializedException : Exception {
    private readonly string _message;
    private readonly string _stackTrace;
    private readonly string _type;

    public DeserializedException(string type, string message, string stackTrace) : base(message) {
        _type       = type;
        _message    = message;
        _stackTrace = stackTrace;
    }

    public override string Message    => _message;
    public override string StackTrace => _stackTrace;
    public override string ToString() => $"{_type}: {_message}\n{_stackTrace}";
}