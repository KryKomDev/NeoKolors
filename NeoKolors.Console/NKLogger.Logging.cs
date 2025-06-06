﻿//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Runtime.CompilerServices;
using NeoKolors.Common;
using NeoKolors.Common.Util;

namespace NeoKolors.Console;

public partial class NKLogger {
    
    /// <summary>
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    public void Fatal(string s) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (!Level.HasFlag(LoggerLevel.ERROR)) return;

        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"{SourceStr}[ FATAL ] : {s}"
                : $"[{TimeStamp()}] {SourceStr}[ FATAL ] : {s}");
        }
        else {
            Output.WriteLine(HideTime
                ? "{0}{2}<b><n> FATAL </n></b> : {1}\e[0m".ApplyStyles().Format(FatalColor.Text, s, SourceStr)
                : "{0}[{1}] {3}<b><n> FATAL </n></b> : {2}\e[0m".ApplyStyles().Format(FatalColor.Text, TimeStamp(), s, SourceStr)
            );
        }
        
        Output.Flush();
    }

    /// <summary>
    /// prints red fatal error text using the ToString method of the object o and <see cref="Fatal(string)"/>
    /// </summary>
    public void Fatal(object o) => Fatal(o.ToString()!);

    /// <summary>
    /// prints red error text
    /// </summary>
    /// <param name="s">desired string message</param>
    public void Error(string s) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (!Level.HasFlag(LoggerLevel.ERROR)) return;

        if (SimpleMessages) {
            Output.WriteLine(HideTime
                ? $"{SourceStr}[ ERROR ] : {s}"
                : $"[{TimeStamp()}] {SourceStr}[ ERROR ] : {s}");
        }
        else {
            Output.WriteLine(HideTime
                ? "{0}{2}<b><n> ERROR </n></b> : {1}\e[0m".ApplyStyles().Format(ErrorColor.Text, s, SourceStr)
                : "{0}[{1}] {3}<b><n> ERROR </n></b> : {2}\e[0m".ApplyStyles().Format(ErrorColor.Text, TimeStamp(), s, SourceStr)
            );
        }
        
        Output.Flush();
    }
    
    /// <summary>
    /// prints red error text using the ToString method of the object o and <see cref="Error(string)"/>
    /// </summary>
    public void Error(object o) => Error(o.ToString()!);

    /// <summary>
    /// prints yellow warning text
    /// </summary>
    /// <param name="s">desired string message</param>
    public void Warn(string s) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (!Level.HasFlag(LoggerLevel.WARN)) return;

        if (SimpleMessages)
            Output.WriteLine(HideTime ? $"{SourceStr}[ WARN ] : {s}" : $"[{TimeStamp()}] {SourceStr}[ WARN ] : {s}");
        else
            Output.WriteLine((HideTime ? $"{SourceStr}[ WARN ] : {s}" : $"[{TimeStamp()}] {SourceStr}[ WARN ] : {s}\e[0m")
                .AddColor(WarnColor));

        Output.Flush();
    }
    
    /// <summary>
    /// prints yellow warning text using the ToString method of the object o and <see cref="Warn(string)"/>
    /// </summary>
    public void Warn(object o) => Warn(o.ToString()!);
    
    /// <summary>
    /// prints green info text
    /// </summary>
    /// <param name="s">desired string message</param>
    public void Info(string s) {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (!Level.HasFlag(LoggerLevel.INFO)) return;

        if (SimpleMessages)
            Output.WriteLine(HideTime ? $"{SourceStr}[ INFO ] : {s}" : $"[{TimeStamp()}] {SourceStr}[ INFO ] : {s}");
        else
            Output.WriteLine((HideTime ? $"{SourceStr}[ INFO ] : {s}" : $"[{TimeStamp()}] {SourceStr}[ INFO ] : {s}\e[0m")
                .AddColor(InfoColor));

        Output.Flush();
    }

    /// <summary>
    /// prints green info text using the ToString method of the object o and <see cref="Info(string)"/>
    /// </summary>
    public void Info(object o) => Info(o.ToString()!);

    /// <summary>
    /// prints a debug message, does not work when built in release mode
    /// </summary>
    /// <param name="s">desired string message</param>
    public void Debug(string s) {
        if (!Level.HasFlag(LoggerLevel.DEBUG)) return;
        if (s == null) throw new ArgumentNullException(nameof(s));

        if (SimpleMessages)
            Output.WriteLine(HideTime ? $"{SourceStr}[ DEBUG ] : {s}" : $"[{TimeStamp()}] {SourceStr}[ DEBUG ] : {s}");
        else
            Output.WriteLine((HideTime ? $"{SourceStr}[ DEBUG ] : {s}" : $"[{TimeStamp()}] {SourceStr}[ DEBUG ] : {s}\e[0m")
                .AddColor(DebugColor));

        Output.Flush();
    }

    /// <summary>
    /// prints debug text using the ToString method of the object o and <see cref="Debug(string)"/>
    /// </summary>
    public void Debug(object o) => Debug(o.ToString()!);
    
    /// <summary>
    /// prints a debug message, does not work when built in release mode
    /// </summary>
    /// <param name="s">desired string message</param>
    public void Trace(string s) {
        if (!Level.HasFlag(LoggerLevel.DEBUG)) return;
        if (s == null) throw new ArgumentNullException(nameof(s));

        if (SimpleMessages)
            Output.WriteLine(HideTime ? $"{SourceStr}[ TRACE ] : {s}" : $"[{TimeStamp()}] {SourceStr}[ TRACE ] : {s}");
        else
            Output.WriteLine((HideTime ? $"{SourceStr}[ TRACE ] : {s}" : $"[{TimeStamp()}] {SourceStr}[ TRACE ] : {s}\e[0m")
                .AddColor(TraceColor));

        Output.Flush();
    }

    /// <summary>
    /// prints debug text using the ToString method of the object o and <see cref="Debug(string)"/>
    /// </summary>
    public void Trace(object o) => Trace(o.ToString()!);

    private string SourceStr => Source is null ? "" : $"[ {Source} ] ";
    
    /// <summary>
    /// the timestamp
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string TimeStamp() => DateTime.Now.ToString(TimeFormat);
    
    #if NET8_0_OR_GREATER
    public void Fatal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Fatal(string.Format(s, args));
    public void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Error(string.Format(s, args));
    public void Warn([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Warn(string.Format(s, args));
    public void Info([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Info(string.Format(s, args));
    public void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Debug(string.Format(s, args));
    public void Trace([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args) => Trace(string.Format(s, args));
    #else
    public void Fatal(string s, params object[] args) => Fatal(string.Format(s, args));
    public void Error(string s, params object[] args) => Error(string.Format(s, args));
    public void Warn(string s, params object[] args) => Warn(string.Format(s, args));
    public void Info(string s, params object[] args) => Info(string.Format(s, args));
    public void Debug(string s, params object[] args) => Debug(string.Format(s, args));
    public void Trace(string s, params object[] args) => Trace(string.Format(s, args));
    #endif
}