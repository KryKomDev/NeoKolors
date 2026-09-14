// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;
using static NeoKolors.Console.NKLogLevel;

namespace NeoKolors.Console;

/// <summary>
/// A class responsible for writing log records to the console with ANSI stylization.
/// </summary>
/// <remarks>
/// The <see cref="AnsiLogWriter"/> utilizes ANSI escape codes to format log messages
/// based on the configuration provided through <see cref="AnsiLoggerConfig"/>. It supports
/// features such as timestamping, log levels, source information, and custom styles for
/// various log levels. The messages are written to the console using <see cref="NKConsole.WriteLine(string)"/>.
/// </remarks>
public class AnsiLogWriter : ILogWriter {

    private static readonly AnsiString CRITICAL_LABEL = AnsiString.Parse("{:n} CRITICAL {:!n}");
    private static readonly AnsiString ERROR_LABEL    = AnsiString.Parse("{:n} ERROR {:!n}"); 
    
    // i dunno why ReSharper doesn't like this, it's not a collection!
    // ReSharper disable UseCollectionExpression
    
    private static readonly AnsiString WARNING_LABEL  = new("[ warn ]");
    private static readonly AnsiString INFO_LABEL     = new("[ info ]");
    private static readonly AnsiString TRACE_LABEL    = new("[ trace ]");
    private static readonly AnsiString DEBUG_LABEL    = new("[ debug ]");
    private static readonly AnsiString DEFAULT_LABEL  = new("[ msg ]");
    
    // ReSharper restore UseCollectionExpression

    public AnsiLoggerConfig Config { get; set; }

    public AnsiLogWriter(AnsiLoggerConfig? config = null) {
        Config = config ?? new AnsiLoggerConfig();
    }

    public void Write(NKLogRecord record) {
        var sb = new AnsiStringBuilder();

        // append header
        sb.Append(GetHeader(record));

        // append message
        sb.Append(GetMessage(record));

        // write to output
        Stdio.WriteLine(sb.ToString());
    }

    private AnsiString GetHeader(NKLogRecord record) {
        var sb = new AnsiStringBuilder();

        // append time
        if (Config.ShowTime)
            sb.Append($"[ {GetTimestamp(record)} ] ");

        // append source
        if (HasSource(record))
            sb.Append($"[ {GetSource(record)} ] ");

        // append level
        sb.Append(GetLevel(record));
        sb.Append(' ');
        
        return sb.ToAnsiString().AddStyle(GetStyle(record.Level));
    }
    
    private string GetTimestamp(NKLogRecord record) => record.Timestamp.ToString(Config.TimeFormat);

    private static AnsiString GetLevel(NKLogRecord record) {
        return record.Level switch {
            CRITICAL    => CRITICAL_LABEL,
            ERROR       => ERROR_LABEL,
            WARNING     => WARNING_LABEL,
            INFORMATION => INFO_LABEL,
            DEBUG       => DEBUG_LABEL,
            TRACE       => TRACE_LABEL,
            _           => DEFAULT_LABEL
        };
    }

    private static bool HasSource(NKLogRecord record) => !string.IsNullOrEmpty(record.Source) || record.EventId != null;

    private static AnsiString GetSource(NKLogRecord record) {
        if (record is { Source: not null, EventId: not null }) {
            var    eventId = record.EventId.Value;
            string idStr   = !string.IsNullOrEmpty(eventId.Name) ? $"{eventId.Id}:{eventId.Name}" : eventId.Id.ToString();

            return $"{record.Source}:{idStr}".ToAnsiString();
        }

        if (record.Source != null) {
            return record.Source.ToAnsiString();
        }

        if (record.EventId != null) {
            var    eventId = record.EventId.Value;
            string idStr   = !string.IsNullOrEmpty(eventId.Name) ? $"{eventId.Id}:{eventId.Name}" : eventId.Id.ToString();

            return idStr.ToAnsiString();
        }

        return AnsiString.Empty;
    }

    private NKStyle GetStyle(NKLogLevel level) {
        return level switch {
            NONE        => NKStyle.Default,
            CRITICAL    => Config.CriticalStyle,
            ERROR       => Config.ErrorStyle,
            WARNING     => Config.WarningStyle,
            INFORMATION => Config.InfoStyle,
            DEBUG       => Config.DebugStyle,
            TRACE       => Config.TraceStyle,
            _           => NKStyle.Default
        };
    }

    private AnsiString? GetMessage(NKLogRecord message) {
        var sb = new AnsiStringBuilder();

        if (message.Message.IsT1) {
            return Config.FormatExceptions
                ? ExceptionFormatter.Format(message.Message.AsT1, Config.ExceptionFormat)
                : message.Message.AsT1.ToString();
        }

        var msg = message.Message.AsT0.AddStyle(GetStyle(message.Level));

        if (msg.Contains('\n')) {
            // do not indent
            if (Config.Indentation.Indent == null) {
                sb.AppendLine();
                sb.AppendLine(msg);

                return sb.ToString();
            }

            var lines = msg.Replace("\r\n", "\n").Split('\n');

            if (lines.Length < 1) {
                return string.Empty;
            }

            var indent = new string(' ', Config.Indentation.Indent.Value);

            // indent first line
            if (Config.Indentation.IndentFirstLine || Config.Indentation.AlwaysIndentFirstLine) {
                sb.AppendLine();
                sb.Append(indent);
            }

            sb.AppendLine(lines[0]);

            // indent all other lines
            for (int i = 1; i < lines.Length; i++) {
                sb.Append(indent);
                sb.AppendLine(lines[i]);
            }
        }
        else {
            if (Config.Indentation.AlwaysIndentFirstLine) {
                sb.AppendLine();
            }

            return msg;
        }

        return sb.ToString();
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
    }
}