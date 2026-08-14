// NeoKolors
// Copyright (c) KryKom 2026

using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Microsoft.Extensions.Logging;
using NeoKolors.Common;
using OneOf;

namespace NeoKolors.Console;

/// <summary>
/// Provides serialization utilities for <see cref="NKLogRecord"/> using MessagePack.
/// </summary>
public static class NKLogRecordSerializer {
    public static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(
            CompositeResolver.Create(
                NKLogRecordFormatterResolver.Instance,
                StandardResolver.Instance
            )
        );

    /// <summary>
    /// Serializes an <see cref="NKLogRecord"/> to the specified stream using MessagePack.
    /// </summary>
    public static void Serialize(Stream stream, NKLogRecord record) {
        MessagePackSerializer.Serialize(stream, record, Options);
    }

    /// <summary>
    /// Deserializes an <see cref="NKLogRecord"/> from the specified stream using MessagePack.
    /// </summary>
    public static NKLogRecord Deserialize(Stream stream) {
        return MessagePackSerializer.Deserialize<NKLogRecord>(stream, Options);
    }
}

public sealed class NKLogRecordFormatterResolver : IFormatterResolver {
    public static readonly IFormatterResolver Instance = new NKLogRecordFormatterResolver();

    private NKLogRecordFormatterResolver() { }

    public IMessagePackFormatter<T>? GetFormatter<T>() {
        return FormatterCache<T>.Formatter;
    }

    private static class FormatterCache<T> {
        public static readonly IMessagePackFormatter<T>? Formatter;

        static FormatterCache() {
            Formatter = (IMessagePackFormatter<T>?)GetFormatterHelper(typeof(T));
        }

        private static object? GetFormatterHelper(Type t) {
            if (t == typeof(NKLogRecord))
                return new NKLogRecordFormatter();

            if (t == typeof(EventId?))
                return new NullableEventIdFormatter();

            if (t == typeof(AnsiString))
                return new AnsiStringSerializer();

            return t == typeof(OneOf<AnsiString, Exception>)
                ? new OneOfMessageFormatter()
                : null;
        }
    }
}

public sealed class NKLogRecordFormatter : IMessagePackFormatter<NKLogRecord> {
    
    public void Serialize(
        ref MessagePackWriter        writer,
        NKLogRecord                  value,
        MessagePackSerializerOptions options
    ) {
        writer.WriteArrayHeader(5);
        writer.Write(value.Timestamp.ToBinary());
        writer.WriteInt32((int)value.Level);
        options.Resolver.GetFormatterWithVerify<EventId?>().Serialize(ref writer, value.EventId, options);
        options.Resolver.GetFormatterWithVerify<AnsiString?>().Serialize(ref writer, value.Source, options);
        options.Resolver.GetFormatterWithVerify<OneOf<AnsiString, Exception>>().Serialize(ref writer, value.Message, options);
    }

    public NKLogRecord Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        int count = reader.ReadArrayHeader();

        if (count != 5)
            throw new MessagePackSerializationException("Invalid NKLogRecord array length.");

        var         timestamp = DateTime.FromBinary(reader.ReadInt64());
        var         level     = (NKLogLevel)reader.ReadInt32();
        var         eventId   = options.Resolver.GetFormatterWithVerify<EventId?>().Deserialize(ref reader, options);
        AnsiString? source    = options.Resolver.GetFormatterWithVerify<AnsiString?>().Deserialize(ref reader, options);
        var         message   = options.Resolver.GetFormatterWithVerify<OneOf<AnsiString, Exception>>().Deserialize(ref reader, options);

        return new NKLogRecord(timestamp, level, message, eventId, source);
    }
}

public sealed class NullableEventIdFormatter : IMessagePackFormatter<EventId?> {
    
    public void Serialize(
        ref MessagePackWriter        writer,
        EventId?                     value,
        MessagePackSerializerOptions options
    ) {
        if (value == null) {
            writer.WriteNil();
        }
        else {
            writer.WriteArrayHeader(2);
            writer.WriteInt32(value.Value.Id);
            writer.Write(value.Value.Name ?? "");
        }
    }

    public EventId? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        if (reader.IsNil) {
            reader.ReadNil();

            return null;
        }

        int count = reader.ReadArrayHeader();

        if (count != 2)
            throw new MessagePackSerializationException("Invalid EventId array length.");

        int     id   = reader.ReadInt32();
        string? name = reader.ReadString();

        return new EventId(id, name);
    }
}

public sealed class OneOfMessageFormatter : IMessagePackFormatter<OneOf<AnsiString, Exception>> {
    
    public void Serialize(
        ref MessagePackWriter        writer,
        OneOf<AnsiString, Exception> value,
        MessagePackSerializerOptions options
    ) {
        writer.WriteArrayHeader(2);
        writer.WriteInt32(value.Index);

        if (value.Index == 0) {
            options.Resolver.GetFormatterWithVerify<AnsiString?>().Serialize(ref writer, value.AsT0, options);
        }
        else {
            SerializeException(ref writer, value.AsT1);
        }
    }

    private static void SerializeException(ref MessagePackWriter writer, Exception ex) {
        while (true) {
            writer.WriteMapHeader(4);
            writer.Write("Type");
            writer.Write(ex.GetType().FullName ?? ex.GetType().Name);
            writer.Write("Message");
            writer.Write(ex.Message);
            writer.Write("StackTrace");
            writer.Write(ex.StackTrace ?? "");
            writer.Write("InnerException");

            if (ex.InnerException != null) {
                ex = ex.InnerException;

                continue;
            }

            writer.WriteNil();

            break;
        }
    }

    public OneOf<AnsiString, Exception> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
        int count = reader.ReadArrayHeader();

        if (count != 2)
            throw new MessagePackSerializationException("Invalid OneOf message array length.");

        int index = reader.ReadInt32();

        if (index == 0) {
            AnsiString? ansi = options.Resolver.GetFormatterWithVerify<AnsiString?>().Deserialize(ref reader, options);

            return ansi != null 
                ? OneOf<AnsiString, Exception>.FromT0(ansi)
                : throw new MessagePackSerializationException("Invalid null AnsiString in OneOf message.");
        }

        var    dict  = DeserializeException(ref reader);
        string msg   = dict.GetValueOrDefault("Message",    "");
        string type  = dict.GetValueOrDefault("Type",       "System.Exception");
        string stack = dict.GetValueOrDefault("StackTrace", "");
        var    ex    = new DeserializedException(type, msg, stack);

        return OneOf<AnsiString, Exception>.FromT1(ex);
    }

    private static Dictionary<string, string> DeserializeException(ref MessagePackReader reader) {
        var dict  = new Dictionary<string, string>();
        int count = reader.ReadMapHeader();

        for (int i = 0; i < count; i++) {
            string? key = reader.ReadString();

            if (key is null)
                throw new MessagePackSerializationException("Invalid null key in exception dictionary.");
            
            if (reader.IsNil) {
                reader.ReadNil();
                dict[key] = "";
            }
            else {
                if (key == "InnerException") {
                    reader.Skip();
                }
                else {
                    dict[key] = reader.ReadString() 
                        ?? throw new MessagePackSerializationException("Invalid null string in exception dictionary.");
                }
            }
        }

        return dict;
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