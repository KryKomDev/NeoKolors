//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace NeoKolors.Console;

/// <summary>
/// static NKLogger and ExceptionFormatter instance
/// </summary>
public static class NKDebug {

    static NKDebug() {
        Logger = new NKLogger();
        Formatter = new ExceptionFormatter();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => { Logger.Close(); };
    }

    /// <summary>
    /// Retrieves an instance of <see cref="NKLogger"/> configured with the specified source.
    /// </summary>
    /// <param name="source">The source identifier for the logger instance.</param>
    /// <returns>An instance of <see cref="NKLogger"/> configured with the specified source.</returns>
    public static NKLogger GetLogger(string source) => new(Logger.Config, source, true);

    /// <summary>
    /// global instance of the NKLogger
    /// </summary>
    public static NKLogger Logger { get; }
    
    /// <inheritdoc cref="NKLogger.Trace(string)"/>
    public static void Trace(string message) => Logger.Trace(message);
    
    /// <inheritdoc cref="NKLogger.Trace(object)"/>
    public static void Trace(object message) => Logger.Trace(message);
    
    /// <inheritdoc cref="NKLogger.Debug(string)"/>
    public static void Debug(string message) => Logger.Debug(message);
    
    /// <inheritdoc cref="NKLogger.Debug(object)"/>
    public static void Debug(object message) => Logger.Debug(message);
    
    /// <inheritdoc cref="NKLogger.Info(string)"/>
    public static void Info(string message) => Logger.Info(message);
    
    /// <inheritdoc cref="NKLogger.Info(object)"/>
    public static void Info(object message) => Logger.Info(message);
    
    /// <inheritdoc cref="NKLogger.Warn(string)"/>
    public static void Warn(string message) => Logger.Warn(message);
    
    /// <inheritdoc cref="NKLogger.Warn(object)"/>
    public static void Warn(object message) => Logger.Warn(message);
    
    /// <inheritdoc cref="NKLogger.Error(string)"/>
    public static void Error(string message) => Logger.Error(message);
    
    /// <inheritdoc cref="NKLogger.Error(object)"/>
    public static void Error(object message) => Logger.Error(message);
    
    /// <inheritdoc cref="NKLogger.Fatal(string)"/>
    public static void Fatal(string message) => Logger.Fatal(message);
    
    /// <inheritdoc cref="NKLogger.Fatal(object)"/>
    public static void Fatal(object message) => Logger.Fatal(message);
    
    #if NET8_0_OR_GREATER
    public static void Fatal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Fatal(string.Format(s, args));
    public static void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Error(string.Format(s, args));
    public static void Warn([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Warn(string.Format(s, args));
    public static void Info([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Info(string.Format(s, args));
    public static void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Debug(string.Format(s, args));
    public static void Trace([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Trace(string.Format(s, args));
    #else
    public static void Fatal(string s, params object[] args) => Fatal(string.Format(s, args));
    public static void Error(string s, params object[] args) => Error(string.Format(s, args));
    public static void Warn(string s, params object[] args) => Warn(string.Format(s, args));
    public static void Info(string s, params object[] args) => Info(string.Format(s, args));
    public static void Debug(string s, params object[] args) => Debug(string.Format(s, args));
    public static void Trace(string s, params object[] args) => Trace(string.Format(s, args));
    #endif

    /// <summary>
    /// Sets the output destination for the logger.
    /// </summary>
    /// <param name="output">The TextWriter to use as the output destination.</param>
    public static void SetOutput(TextWriter output) {
        Logger.FileConfig = LogFileConfig.Custom();
        Logger.Output = output;
    }

    /// <summary>
    /// Retrieves the current output stream for logging messages.
    /// </summary>
    /// <returns>The <see cref="TextWriter"/> currently used for logging output.</returns>
    public static TextWriter GetOutput() => Logger.Output;

    /// <inheritdoc cref="NKLogger.LogAll"/>
    public static void LogAll() => Logger.LogAll();
    
    /// <inheritdoc cref="NKLogger.NoDebug"/>
    public static void NoDebug() => Logger.NoDebug();

    /// <inheritdoc cref="NKLogger.LogWarn"/>
    public static void LogWarn() => Logger.LogWarn();
    
    /// <inheritdoc cref="NKLogger.LogErrors"/>
    public static void LogErrors() => Logger.LogErrors();
    
    /// <inheritdoc cref="NKLogger.LogFatal"/>
    public static void LogFatal() => Logger.LogFatal();
    
    /// <inheritdoc cref="NKLogger.LogNone"/>
    public static void LogNone() => Logger.LogNone();
    
    /// <summary>
    /// the global instance of the ExceptionFormatter
    /// </summary>
    public static ExceptionFormatter Formatter { get; }
    
    /// <summary>
    /// if true, makes all unhandled exceptions look fancy
    /// </summary>
    public static bool ExceptionFormatting { 
        get;
        set {
            if (value)
                AppDomain.CurrentDomain.UnhandledException += Formatter.WriteUnhandled;
            else
                AppDomain.CurrentDomain.UnhandledException -= Formatter.WriteUnhandled;

            field = value;
        } 
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether exception output is redirected to a log.
    /// </summary>
    /// <remarks>
    /// When enabled, unhandled exceptions are also logged instead of purely being output to the console.
    /// This can be useful for tracking issues in applications where console output is not always monitored.
    /// </remarks>
    public static bool RedirectFatalToLog {
        get => Formatter.RedirectToLog;
        set => Formatter.RedirectToLog = value;
    }
}