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
    }
    
    /// <summary>
    /// global instance of the NKLogger
    /// </summary>
    public static NKLogger Logger { get; }
    
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
    #else
    public static void Fatal(string s, params object[] args) => Fatal(string.Format(s, args));
    public static void Error(string s, params object[] args) => Error(string.Format(s, args));
    public static void Warn(string s, params object[] args) => Warn(string.Format(s, args));
    public static void Info(string s, params object[] args) => Info(string.Format(s, args));
    public static void Debug(string s, params object[] args) => Debug(string.Format(s, args));
    #endif
    
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
    
    public static ExceptionFormatter Formatter { get; }
    
    /// <summary>
    /// if true makes all unhandled exceptions look fancy
    /// </summary>
    public static bool EnableAutoFancy { 
        get;
        set {
            if (value)
                AppDomain.CurrentDomain.UnhandledException += Formatter.WriteUnhandled;
            else
                AppDomain.CurrentDomain.UnhandledException -= Formatter.WriteUnhandled;

            field = value;
        } 
    } = true;
}