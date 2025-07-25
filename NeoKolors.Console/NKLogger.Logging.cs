//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NeoKolors.Common;
using NeoKolors.Common.Util;

namespace NeoKolors.Console;

public sealed partial class NKLogger : ILogger {
    
    /// <summary>
    /// Logs a critical error message with optional event identifier and level formatting.
    /// </summary>
    /// <param name="message">The critical error message to be logged. Cannot be null.</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    public void Crit(string message, EventId? id) {
        if (message == null) throw new ArgumentNullException(nameof(message));
        if (!Level.HasFlag(LoggerLevel.CRITICAL)) return;

        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"{SourceStr(id)}[ CRIT ] : {Indent(message)}"
                : $"[{TimeStamp()}] {SourceStr(id)}[ CRIT ] : {Indent(message)}");
        }
        else {
            Output.WriteLine(HideTime
                ? "{0}{2}<b><n> CRIT </n></b> : {1}\e[0m"
                    .ApplyStyles()
                    .Format(FatalColor.Text, Indent(message), SourceStr(id))
                : "{0}[{1}] {3}<b><n> CRIT </n></b> : {2}\e[0m"
                    .ApplyStyles()
                    .Format(FatalColor.Text, TimeStamp(), Indent(message), SourceStr(id))
            );
        }
        
        Output.Flush();
    }

    /// <summary>
    /// Logs a critical error message.
    /// </summary>
    /// <param name="message">The critical error message to be logged. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Crit(string message) => Crit(message: message, id: null);

    /// <summary>
    /// Logs a critical error message with optional event identifier and level formatting.
    /// </summary>
    /// <param name="o">The object to be logged</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Crit(object o, EventId? id = null) => Crit(o.ToString()!, id);

    /// <summary>
    /// Logs a critical error message from an exception.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the provided exception's ToString returns null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Crit(Exception e) => Crit(e.ToString());

    /// <summary>
    /// Logs an error message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="message">The error message to be logged. Cannot be null.</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    public void Error(string message, EventId? id) {
        if (message == null) throw new ArgumentNullException(nameof(message));
        if (!Level.HasFlag(LoggerLevel.ERROR)) return;

        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"{SourceStr(id)}[ FAIL ] : {Indent(message)}"
                : $"[{TimeStamp()}] {SourceStr(id)}[ FAIL ] : {Indent(message)}");
        }
        else {
            Output.WriteLine(HideTime
                ? "{0}{2}<b><n> FAIL </n></b> : {1}\e[0m"
                    .ApplyStyles()
                    .Format(ErrorColor.Text, Indent(message), SourceStr(id))
                : "{0}[{1}] {3}<b><n> FAIL </n></b> : {2}\e[0m"
                    .ApplyStyles()
                    .Format(ErrorColor.Text, TimeStamp(), Indent(message), SourceStr(id))
            );
        }
        
        Output.Flush();
    }
    
    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The error message to be logged. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(string message) => Error(message: message, id: null);
    
    /// <summary>
    /// Logs an error message with optional event identifier, formatting, and styling as per logger configuration.
    /// </summary>
    /// <param name="o">The object to be logged</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(object o, EventId? id = null) => Error(o.ToString()!, id);
    
    /// <summary>
    /// Logs an error message from an exception.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the provided exception's ToString returns null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(Exception e) => Error(e.ToString());
    
    /// <summary>
    /// Logs a warning message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="message">The warning message to be logged. Cannot be null.</param>
    /// <param name="id">An optional event identifier associated with the log entry. Defaults to null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    public void Warn(string message, EventId? id) {
        if (message == null) throw new ArgumentNullException(nameof(message));
        if (!Level.HasFlag(LoggerLevel.WARNING)) return;

        if (SimpleMessages)
            Output.WriteLine(HideTime 
                ? $"{SourceStr(id)}[ WARN ] : {Indent(message)}" 
                : $"[{TimeStamp()}] {SourceStr(id)}[ WARN ] : {Indent(message)}");
        else
            Output.WriteLine(
                (HideTime 
                    ? $"{SourceStr(id)}[ WARN ] : {Indent(message)}" 
                    : $"[{TimeStamp()}] {SourceStr(id)}[ WARN ] : {Indent(message)}\e[0m"
                )
                .AddColor(WarnColor));

        Output.Flush();
    }
    
    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message to be logged. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warn(string message) => Warn(message: message, id: null);

    /// <summary>
    /// Logs a warning message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="o">The object to be logged</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warn(object o, EventId? id = null) => Warn(o.ToString()!, id);
    
    /// <summary>
    /// Logs a warning message from an exception.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the provided exception's ToString returns null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warn(Exception e) => Warn(e.ToString());
    
    /// <summary>
    /// Logs an informational message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="message">The informational message to be logged. Cannot be null.</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided message is null.</exception>
    public void Info(string message, EventId? id) {
        if (message == null) throw new ArgumentNullException(nameof(message));
        if (!Level.HasFlag(LoggerLevel.INFORMATION)) return;

        if (SimpleMessages)
            Output.WriteLine(HideTime 
                ? $"{SourceStr(id)}[ INFO ] : {Indent(message)}" 
                : $"[{TimeStamp()}] {SourceStr(id)}[ INFO ] : {Indent(message)}");
        else
            Output.WriteLine(
                (HideTime 
                    ? $"{SourceStr(id)}[ INFO ] : {Indent(message)}" 
                    : $"[{TimeStamp()}] {SourceStr(id)}[ INFO ] : {Indent(message)}\e[0m"
                )
                .AddColor(InfoColor));

        Output.Flush();
    }
    
    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The information message to be logged. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(string message) => Info(message: message, id: null);
    
    /// <summary>
    /// Logs an informational message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="o">The object to be logged</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(object o, EventId? id = null) => Info(o.ToString()!, id);

    /// <summary>
    /// Logs a debug-level message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="message">The debug message to be logged. Cannot be null.</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    public void Debug(string message, EventId? id) {
        if (!Level.HasFlag(LoggerLevel.DEBUG)) return;
        if (message == null) throw new ArgumentNullException(nameof(message));

        if (SimpleMessages)
            Output.WriteLine(HideTime 
                ? $"{SourceStr(id)}[ DBUG ] : {Indent(message)}" 
                : $"[{TimeStamp()}] {SourceStr(id)}[ DBUG ] : {Indent(message)}");
        else
            Output.WriteLine(
                (HideTime 
                    ? $"{SourceStr(id)}[ DBUG ] : {Indent(message)}" 
                    : $"[{TimeStamp()}] {SourceStr(id)}[ DBUG ] : {Indent(message)}\e[0m"
                )
                .AddColor(DebugColor));

        Output.Flush();
    }
    
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The debug message to be logged. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(string message) => Debug(message: message, id: null);

    /// <summary>
    /// Logs a debug-level message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="o">The object to be logged</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(object o, EventId? id = null) => Debug(o.ToString()!, id);

    /// <summary>
    /// Logs a trace-level message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="message">The trace message to be logged. Cannot be null.</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    public void Trace(string message, EventId? id) {
        if (!Level.HasFlag(LoggerLevel.DEBUG)) return;
        if (message == null) throw new ArgumentNullException(nameof(message));

        if (SimpleMessages)
            Output.WriteLine(HideTime 
                ? $"{SourceStr(id)}[ TRCE ] : {Indent(message)}" 
                : $"[{TimeStamp()}] {SourceStr(id)}[ TRCE ] : {Indent(message)}");
        else
            Output.WriteLine(
                (HideTime 
                    ? $"{SourceStr(id)}[ TRCE ] : {Indent(message)}" 
                    : $"[{TimeStamp()}] {SourceStr(id)}[ TRCE ] : {Indent(message)}\e[0m"
                )
                .AddColor(TraceColor));

        Output.Flush();
    }
    
    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The information message to be logged. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(string message) => Trace(message: message, id: null);

    /// <summary>
    /// Logs a trace-level message with optional event identifier, formatting and styling as per logger configuration.
    /// </summary>
    /// <param name="o">The object to be logged</param>
    /// <param name="id">An optional event identifier associated with the log entry.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(object o, EventId? id = null) => Trace(o.ToString()!, id);

    private string SourceStr(EventId? id) => 
        Source is null ? "" : id is null ? $"[ {Source} ] " : $"[ {Source}: {id.ToString()} ] ";

    private string Indent(string s) => IndentMessage.Match(
        _ => s, 
        indent => MessageHighlightLine 
            ? "\n" + s
              .Split('\n')
              .Select(allSelector: str => '│' + new string(' ',  indent.Spaces) + str,
                    lastSelector: str => '╵' + new string(' ',  indent.Spaces) + str,
                    defaultTo: 1)
              .Join("\n")
            : $"\n{s}".PadLinesLeft(indent.Spaces)
    );

    /// <summary>
    /// the timestamp
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string TimeStamp() => DateTime.Now.ToString(TimeFormat);
    
    public void Crit([StructuredMessageTemplate] string message, params object[] args) => Crit(message.StructuredFormat(args));
    public void Error([StructuredMessageTemplate] string message, params object[] args) => Error(message.StructuredFormat(args));
    public void Warn([StructuredMessageTemplate] string message, params object[] args) => Warn(message.StructuredFormat(args));
    public void Info([StructuredMessageTemplate] string message, params object[] args) => Info(message.StructuredFormat(args));
    public void Debug([StructuredMessageTemplate] string message, params object[] args) => Debug(message.StructuredFormat(args));
    public void Trace([StructuredMessageTemplate] string message, params object[] args) => Trace(message.StructuredFormat(args));

    public void Log(LogLevel logLevel, string message, EventId? id) {
        switch (logLevel) {
            case LogLevel.Trace:
                Trace(message, id);
                break;
            case LogLevel.Debug:
                Debug(message, id);
                break;
            case LogLevel.Information:
                Info(message, id);
                break;
            case LogLevel.Warning:
                Warn(message, id);
                break;
            case LogLevel.Error:
                Error(message, id);
                break;
            case LogLevel.Critical:
                Crit(message, id);
                break;
            case LogLevel.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }

    public void Log(LogLevel logLevel, string message) => Log(logLevel, message, null);
    
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    ) => Log(logLevel, formatter(state, exception), eventId);

    public bool IsEnabled(LogLevel logLevel) {
        return logLevel switch {
            LogLevel.Trace => Level.HasFlag(LoggerLevel.TRACE),
            LogLevel.Debug => Level.HasFlag(LoggerLevel.DEBUG),
            LogLevel.Information => Level.HasFlag(LoggerLevel.INFORMATION),
            LogLevel.Warning => Level.HasFlag(LoggerLevel.WARNING),
            LogLevel.Error => Level.HasFlag(LoggerLevel.ERROR),
            LogLevel.Critical => Level.HasFlag(LoggerLevel.CRITICAL),
            LogLevel.None => true,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }
    
    public IDisposable BeginScope<TState>(TState state) where TState : notnull => this;
}