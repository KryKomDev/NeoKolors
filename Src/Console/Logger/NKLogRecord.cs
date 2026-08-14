// NeoKolors
// Copyright (c) KryKom 2026

using Microsoft.Extensions.Logging;
using NeoKolors.Common;
using OneOf;

namespace NeoKolors.Console;

public readonly record struct NKLogRecord {
    public DateTime                     Timestamp { get; init; }
    public NKLogLevel                  Level     { get; init; }
    public EventId?                     EventId   { get; init; }
    public string?                      Source    { get; init; }
    public OneOf<AnsiString, Exception> Message   { get; init; }

    public NKLogRecord(
        DateTime                     timestamp,
        NKLogLevel                  level,
        OneOf<AnsiString, Exception> message = default,
        EventId?                     eventId = null,
        string?                      source  = null
    ) {
        Timestamp = timestamp;
        Level     = level;
        EventId   = eventId;
        Source    = source;
        Message   = message;
    }

    public override string ToString() {
        return
            $"[{Timestamp}] "                                  +
            $"[{Source ?? "-"}:{EventId?.ToString() ?? "-"}] " +
            $"[{Level}] : {(Message.IsT0 ? Message.AsT0.ToString() : Message.AsT1.ToString())}";
    }
}