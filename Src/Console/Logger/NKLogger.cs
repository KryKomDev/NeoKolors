// NeoKolors
// Copyright (c) krystof 2026

using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NeoKolors.Common;
using NeoKolors.Extensions;
using static NeoKolors.Console.NKLogLevel;

namespace NeoKolors.Console;

public sealed class NKLogger : ILogger, IDisposable {

    // =================== Config =================== 

    #region Config
    
    public bool       Enabled { get; set; }
    public ILogWriter Writer  { get; set; }
    public NKLogLevel Level   { get; set; }
    public string?    Source  { get; set; }

    internal IExternalScopeProvider? ScopeProvider { get; set; }

    public NKLogger(ILogWriter? writer = null, string? source = null, NKLogLevel? level = null, bool enabled = true) {
        Writer  = writer ?? new AnsiLogWriter();
        Source  = source;
        Level   = level ?? CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE;
        Enabled = enabled;
    }

    public NKLogger(string source) : this(null, source) { }

    /// <summary>
    /// Enables all log message levels, including critical, error, warning, information, debug, and trace.
    /// </summary>
    public void SetLogAll() => Level = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE;

    /// <summary>
    /// Enables logging for information-level messages, as well as critical, error, and warning messages.
    /// </summary>
    public void SetLogInfo() => Level = CRITICAL | ERROR | WARNING | INFORMATION;

    /// <summary>
    /// Enables logging for warning-level messages, as well as critical and error messages.
    /// </summary>
    public void SetLogWarn() => Level = CRITICAL | ERROR | WARNING;

    /// <summary>
    /// Enables logging for error-level messages, as well as critical messages.
    /// </summary>
    public void SetLogErrors() => Level = CRITICAL | ERROR;

    /// <summary>
    /// Enables logging for critical-level messages only.
    /// </summary>
    public void SetLogCrit() => Level = CRITICAL;

    /// <summary>
    /// Disables all log message levels, preventing any log messages from being written.
    /// </summary>
    public void SetLogNone() => Level = NONE;
    
    #endregion

    public void Crit(AnsiString? message, string? source = null, EventId? id = null) {
        if (message == null || !Enabled || (Level & CRITICAL) == NONE)
            return;
        
        Writer.Write(new NKLogRecord(DateTime.Now, CRITICAL, message, id, source ?? Source));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Crit(object? message, string? source = null, EventId? id = null) =>
        Crit(message?.ToAnsiString(), source, id);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Crit(Exception e, string? source = null, EventId? id = null) {
        if (!Enabled || (Level & CRITICAL) == NONE)
            return;
        Writer.Write(new NKLogRecord(DateTime.Now, CRITICAL, e, id, source ?? Source));
    }

    public void Error(AnsiString? message, string? source = null, EventId? id = null) {
        if (message == null || !Enabled || (Level & ERROR) == NONE)
            return;

        Writer.Write(new NKLogRecord(DateTime.Now, ERROR, message, id, source ?? Source));  
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(object? message, string? source = null, EventId? id = null) =>
        Error(message?.ToAnsiString(), source, id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(Exception e, string? source = null, EventId? id = null) {
        if (!Enabled || (Level & ERROR) == NONE)
            return;
        Writer.Write(new NKLogRecord(DateTime.Now, ERROR, e, id, source ?? Source));
    }

    public void Warn(AnsiString? message, string? source = null, EventId? id = null) {
        if (message == null || !Enabled || (Level & WARNING) == NONE)
            return;

        Writer.Write(new NKLogRecord(DateTime.Now, WARNING, message, id, source ?? Source));  
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warn(object? message, string? source = null, EventId? id = null) =>
        Warn(message?.ToAnsiString(), source, id);

    public void Info(AnsiString? message, string? source = null, EventId? id = null) {
        if (message == null || !Enabled || (Level & INFORMATION) == NONE)
            return;

        Writer.Write(new NKLogRecord(DateTime.Now, INFORMATION, message, id, source ?? Source));  
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(object? message, string? source = null, EventId? id = null) =>
        Info(message?.ToAnsiString(), source, id);
    
    public void Debug(AnsiString? message, string? source = null, EventId? id = null) {
        if (message == null || !Enabled || (Level & DEBUG) == NONE)
            return;

        Writer.Write(new NKLogRecord(DateTime.Now, DEBUG, message, id, source ?? Source));  
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(object? message, string? source = null, EventId? id = null) =>
        Debug(message?.ToAnsiString(), source, id);
    
    public void Trace(AnsiString? message, string? source = null, EventId? id = null) {
        if (message == null || !Enabled || (Level & TRACE) == NONE)
            return;

        Writer.Write(new NKLogRecord(DateTime.Now, TRACE, message, id, source ?? Source));  
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(object? message, string? source = null, EventId? id = null) =>
        Trace(message?.ToAnsiString(), source, id);

    public void Crit([StructuredMessageTemplate] string s, params object[] args) {
        if (args.Length == 0)
            Crit(s.ToAnsiString(), source: null, id: null);
        else
            Crit(s.StructuredFormat(args), source: null, id: null);
    }

    public void Error([StructuredMessageTemplate] string s, params object[] args) {
        if (args.Length == 0)
            Error(s.ToAnsiString(), source: null, id: null);
        else
            Error(s.StructuredFormat(args), source: null, id: null);
    }

    public void Warn([StructuredMessageTemplate] string s, params object[] args) {
        if (args.Length == 0)
            Warn(s.ToAnsiString(), source: null, id: null);
        else
            Warn(s.StructuredFormat(args), source: null, id: null);
    }

    public void Info([StructuredMessageTemplate] string s, params object[] args) {
        if (args.Length == 0)
            Info(s.ToAnsiString(), source: null, id: null);
        else
            Info(s.StructuredFormat(args), source: null, id: null);
    }

    public void Debug([StructuredMessageTemplate] string s, params object[] args) {
        if (args.Length == 0)
            Debug(s.ToAnsiString(), source: null, id: null);
        else
            Debug(s.StructuredFormat(args), source: null, id: null);
    }

    public void Trace([StructuredMessageTemplate] string s, params object[] args) {
        if (args.Length == 0)
            Trace(s.ToAnsiString(), source: null, id: null);
        else
            Trace(s.StructuredFormat(args), source: null, id: null);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel))
            return;

        if (formatter == null)
            throw new ArgumentNullException(nameof(formatter));

        string messageText = formatter(state, exception);
        if (string.IsNullOrEmpty(messageText) && exception != null) {
            Writer.Write(new NKLogRecord(DateTime.Now, FromMs(logLevel), exception, eventId, Source));
        }
        else {
            Writer.Write(new NKLogRecord(DateTime.Now, FromMs(logLevel), messageText.ToAnsiString(), eventId, Source));
        }
    }

    private static NKLogLevel FromMs(LogLevel level) {
        return level switch {
            LogLevel.Trace       => TRACE,
            LogLevel.Debug       => DEBUG,
            LogLevel.Information => INFORMATION,
            LogLevel.Warning     => WARNING,
            LogLevel.Error       => ERROR,
            LogLevel.Critical    => CRITICAL,
            LogLevel.None        => NONE,
            _                    => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }

    public bool IsEnabled(LogLevel logLevel) {
        if (!Enabled || logLevel == LogLevel.None)
            return false;

        var nkLevel = FromMs(logLevel);
        return (Level & nkLevel) != NONE;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull {
        return ScopeProvider?.Push(state) ?? NullScope.Instance;
    }

    public void Dispose() {
        Writer.Dispose();
    }

    private sealed class NullScope : IDisposable {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }
}