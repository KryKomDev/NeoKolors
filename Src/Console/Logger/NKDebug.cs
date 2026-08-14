// NeoKolors
// Copyright (c) 2025 KryKom

using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NeoKolors.Extensions;

namespace NeoKolors.Console;

/// <summary>
/// Static NKLogger and ExceptionFormatter interface.
/// </summary>
public static class NKDebug {

    static NKDebug() {
        Logger = new NKLogger();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => { Logger.Writer.Dispose(); };
        EnableExceptionInterruption();
    }
    
    // ==================================== Logger ==================================== //

    /// <summary>
    /// Retrieves an instance of <see cref="NKLogger"/> configured with the specified source.
    /// </summary>
    /// <param name="source">The source identifier for the logger instance.</param>
    /// <returns>An instance of <see cref="NKLogger"/> configured with the specified source.</returns>
    public static NKLogger GetLogger(string source) => new NKLogger(source);
    
    /// <summary>
    /// Retrieves an instance of <see cref="NKLogger"/> configured with the specified source.
    /// </summary>
    /// <returns>An instance of <see cref="NKLogger"/> configured with the specified source.</returns>
    public static NKLogger GetLogger<TSource>() => new NKLogger(typeof(TSource).Name);

    /// <summary>
    /// Global instance of the <see cref="NKLogger"/>.
    /// </summary>
    public static NKLogger Logger { get; }
    
    /// <inheritdoc cref="NKLogger.Trace(AnsiString?, string?, EventId?)"/>
    public static void Trace(string message) => Logger.Trace(message);
    
    /// <inheritdoc cref="NKLogger.Trace(object?, string?, EventId?)"/>
    public static void Trace(object message) => Logger.Trace(message);
    
    /// <inheritdoc cref="NKLogger.Debug(AnsiString?, string?, EventId?)"/>
    public static void Debug(string message) => Logger.Debug(message);
    
    /// <inheritdoc cref="NKLogger.Debug(object?, string?, EventId?)"/>
    public static void Debug(object message) => Logger.Debug(message);
    
    /// <inheritdoc cref="NKLogger.Info(AnsiString?, string?, EventId?)"/>
    public static void Info(string message) => Logger.Info(message);
    
    /// <inheritdoc cref="NKLogger.Info(object?, string?, EventId?)"/>
    public static void Info(object message) => Logger.Info(message);
    
    /// <inheritdoc cref="NKLogger.Warn(AnsiString?, string?, EventId?)"/>
    public static void Warn(string message) => Logger.Warn(message);
    
    /// <inheritdoc cref="NKLogger.Warn(object?, string?, EventId?)"/>
    public static void Warn(object message) => Logger.Warn(message);
    
    /// <inheritdoc cref="NKLogger.Error(AnsiString?, string?, EventId?)"/>
    public static void Error(string message) => Logger.Error(message);
    
    /// <inheritdoc cref="NKLogger.Error(object?, string?, EventId?)"/>
    public static void Error(object message) => Logger.Error(message);
    
    /// <inheritdoc cref="NKLogger.Crit(AnsiString?, string?, EventId?)"/>
    public static void Crit(string message) => Logger.Crit(message);
    
    /// <inheritdoc cref="NKLogger.Crit(object?, string?, EventId?)"/>
    public static void Crit(object message) => Logger.Crit(message);
    
    public static void Crit([StructuredMessageTemplate] string s, params object[] args) => Crit(s.StructuredFormat(args));
    public static void Error([StructuredMessageTemplate] string s, params object[] args) => Error(s.StructuredFormat(args));
    public static void Warn([StructuredMessageTemplate] string s, params object[] args) => Warn(s.StructuredFormat(args));
    public static void Info([StructuredMessageTemplate] string s, params object[] args) => Info(s.StructuredFormat(args));
    public static void Debug([StructuredMessageTemplate] string s, params object[] args) => Debug(s.StructuredFormat(args));
    public static void Trace([StructuredMessageTemplate] string s, params object[] args) => Trace(s.StructuredFormat(args));

    /// <summary>
    /// Configures the logger to write to a log file.
    /// </summary>
    public static void SetFileLogging(LogFileConfig fileConfig, TextLoggerConfig? config = null) {
        Logger.Writer = TextLogWriter.CreateFromFile(fileConfig, config);
    }

    /// <summary>
    /// Configures the logger to output plain text messages without ANSI colors.
    /// </summary>
    public static void UseSimpleMessages() {
        Logger.Writer = new TextLogWriter(System.Console.Out);
    }

    /// <summary>
    /// Sets the output destination for the logger.
    /// </summary>
    /// <param name="output">The TextWriter to use as the output destination.</param>
    public static void SetOutput(TextWriter output) {
        Logger.Writer = new TextLogWriter(output);
    }

    /// <summary>
    /// Retrieves the current output stream for logging messages.
    /// </summary>
    /// <returns>The <see cref="TextWriter"/> currently used for logging output, if available.</returns>
    public static TextWriter GetOutput() => (Logger.Writer as TextLogWriter)?.Output ?? System.Console.Out;

    /// <inheritdoc cref="NKLogger.SetLogAll"/>
    public static void SetLogAll() => Logger.SetLogAll();
    
    /// <inheritdoc cref="NKLogger.SetLogInfo"/>
    public static void SetLogInfo() => Logger.SetLogInfo();

    /// <inheritdoc cref="NKLogger.SetLogWarn"/>
    public static void SetLogWarn() => Logger.SetLogWarn();
    
    /// <inheritdoc cref="NKLogger.SetLogErrors"/>
    public static void SetLogErrors() => Logger.SetLogErrors();
    
    /// <inheritdoc cref="NKLogger.SetLogCrit"/>
    public static void SetLogCrit() => Logger.SetLogCrit();
    
    /// <inheritdoc cref="NKLogger.SetLogNone"/>
    public static void SetLogNone() => Logger.SetLogNone();
    
    
    // ============================== Exception Formatter ============================== //
    
    /// <summary>
    /// if true, makes all unhandled exceptions look fancy
    /// </summary>
    public static bool ExceptionFormatting {
        get => ExceptionFormatter.FormatUnhandled;
        set => ExceptionFormatter.FormatUnhandled = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether exception output is redirected to a log.
    /// </summary>
    /// <remarks>
    /// When enabled, unhandled exceptions are also logged instead of purely being output to the console.
    /// This can be useful for tracking issues in applications where console output is not always monitored.
    /// </remarks>
    public static bool RedirectFatalToLog {
        get => ExceptionFormatter.RedirectToLog;
        set => ExceptionFormatter.RedirectToLog = value;
    }

    private static bool EXCEPTION_INTERRUPT;
    
    public static void EnableExceptionInterruption() {
        if (EXCEPTION_INTERRUPT)
            return;
            
        AppDomain.CurrentDomain.UnhandledException += ExceptionFormatter.WriteUnhandled;
        EXCEPTION_INTERRUPT = true;
    }
    
    public static void DisableExceptionInterruption() {
        if (!EXCEPTION_INTERRUPT)
            return;
        
        AppDomain.CurrentDomain.UnhandledException -= ExceptionFormatter.WriteUnhandled;
        EXCEPTION_INTERRUPT = false;
    }
}