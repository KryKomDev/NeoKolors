// NeoKolors
// Copyright (c) krystof 2026

using System.Text;
using NeoKolors.Common;
using OneOf;
using static NeoKolors.Console.NKLogLevel;

namespace NeoKolors.Console;

/// <summary>
/// A concrete implementation of the <see cref="ILogWriter"/> interface that writes log records
/// to a specified <see cref="TextWriter"/> output stream, with customizable formatting options.
/// </summary>
public sealed class TextLogWriter : ILogWriter, IAsyncDisposable {

    private const string CRITICAL_LABEL = "[ CRITICAL ]";
    private const string ERROR_LABEL    = "[ ERROR ]";
    private const string WARNING_LABEL  = "[ warn ]";
    private const string INFO_LABEL     = "[ info ]";
    private const string TRACE_LABEL    = "[ trace ]";
    private const string DEBUG_LABEL    = "[ debug ]";

    private bool _disposed = false;
    
    public TextWriter       Output { get; set; }
    public TextLoggerConfig Config { get; set; }

    public TextLogWriter(TextWriter output, TextLoggerConfig? config = null) {
        Output = output;
        Config = config ?? new TextLoggerConfig();
    }

    /// <summary>
    /// Creates a <see cref="TextLogWriter"/> configured with a log file.
    /// </summary>
    public static TextLogWriter CreateFromFile(LogFileConfig fileConfig, TextLoggerConfig? config = null) {
        var output = fileConfig.CreateOutput();
        return new TextLogWriter(output, config);
    }

    /// <summary>
    /// Creates a <see cref="TextLogWriter"/> writing to the specified log file path.
    /// </summary>
    public static TextLogWriter CreateFromFile(string filePath, TextLoggerConfig? config = null, bool append = true) {
        var fileConfig = append ? LogFileConfig.Append(filePath) : LogFileConfig.Replace(filePath);
        return CreateFromFile(fileConfig, config);
    }

    public void Write(NKLogRecord record) {
        if (_disposed)
            return;
        
        var sb = new StringBuilder();

        // append time
        if (Config.ShowTime)
            sb.Append($"[ {GetTimestamp(record)} ] ");

        // append source
        if (HasSource(record))
            sb.Append($"[ {GetSource(record)} ] ");

        // append level
        sb.Append(GetLevel(record));
        sb.Append(' ');

        // append message
        sb.Append(GetMessage(record.Message));

        // write to output
        Output.WriteLine(sb.ToString());
        
        // flush output
        Output.Flush();
    }

    private string GetTimestamp(NKLogRecord record) => record.Timestamp.ToString(Config.TimeFormat);

    private static string GetLevel(NKLogRecord record) {
        return record.Level switch {
            CRITICAL    => CRITICAL_LABEL,
            ERROR       => ERROR_LABEL,
            WARNING     => WARNING_LABEL,
            INFORMATION => INFO_LABEL,
            DEBUG       => DEBUG_LABEL,
            TRACE       => TRACE_LABEL,
            _           => "[ msg ]"
        };
    }

    private static bool HasSource(NKLogRecord record) => !string.IsNullOrEmpty(record.Source) || record.EventId != null;

    private static string GetSource(NKLogRecord record) {
        if (record.Source != null && record.EventId != null) {
            var eventId = record.EventId.Value;
            string idStr = !string.IsNullOrEmpty(eventId.Name) ? $"{eventId.Id}:{eventId.Name}" : eventId.Id.ToString();
            return $"{record.Source}:{idStr}";
        }
        if (record.Source != null) {
            return record.Source;
        }
        if (record.EventId != null) {
            var eventId = record.EventId.Value;
            return !string.IsNullOrEmpty(eventId.Name) ? $"{eventId.Id}:{eventId.Name}" : eventId.Id.ToString();
        }
        return string.Empty;
    }

    private string GetMessage(OneOf<AnsiString, Exception> message) {
        var sb = new StringBuilder();

        var msg = message.Match(
            s => s.Plain,
            e => e.ToString()
        );

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
        if (_disposed) 
            return;
        
        Output.Dispose();
        
        _disposed = true;
    }

    public async ValueTask DisposeAsync() {
        if (_disposed) 
            return;
        
        await Output.DisposeAsync();
        
        _disposed = true;
    }
}