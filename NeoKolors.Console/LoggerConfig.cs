//
// NeoKolors
// Copyright (c) 2025 KryKom
//

#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System;
using System.IO;
using NeoKolors.Common;
using OneOf;
using static NeoKolors.Console.LoggerLevel;

namespace NeoKolors.Console;

/// <summary>
/// configures the NKLogger
/// </summary>
public struct LoggerConfig : ICloneable {

    /// <summary>
    /// configures what messages will be logged
    /// </summary>
    public LoggerLevel Level { get; set; } = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE;
    
    /// <summary>
    /// defines the color of the fatal error messages
    /// </summary>
    public NKColor FatalColor { get; set; } = NKConsoleColor.DARK_RED;
    
    /// <summary>
    /// defines the color of the error messages
    /// </summary>
    public NKColor ErrorColor { get; set; } = NKConsoleColor.RED;
    
    /// <summary>
    /// defines the color of the warning messages
    /// </summary>
    public NKColor WarnColor { get; set; } = NKConsoleColor.YELLOW;
    
    /// <summary>
    /// defines the color of the info messages
    /// </summary>
    public NKColor InfoColor { get; set; } = NKConsoleColor.GREEN;
    
    /// <summary>
    /// defines the color of the debug messages
    /// </summary>
    public NKColor DebugColor { get; set; } = NKConsoleColor.BLUE;
    
    /// <summary>
    /// defines the color of the trace messages
    /// </summary>
    public NKColor TraceColor { get; set; } = NKConsoleColor.GRAY;
    
    /// <summary>
    /// defines the output stream of the logger
    /// </summary>
    internal TextWriterReference OutputReference { get; set; } = new(System.Console.Out);

    /// <summary>
    /// defines the output stream of the logger
    /// </summary>
    public TextWriter Output { get => OutputReference.TextWriter; set => OutputReference.TextWriter = value; }

    /// <summary>
    /// if true, prints simple uncolored messages
    /// </summary>
    public bool SimpleMessages { get; set; } = false;
    
    /// <summary>
    /// if true, hides the timestamp
    /// </summary>
    public bool HideTime { get; set; } = false;

    /// <summary>
    /// Specifies the format used to display timestamps in log messages.
    /// </summary>
    public string TimeFormat { get; set; } = "HH:mm:ss";

    /// <summary>
    /// Configures how log files are managed, including options such as replacing, appending,
    /// or creating new files based on various strategies.
    /// </summary>
    public LogFileConfig FileConfig { 
        get;
        set {
            field = value;
            if (value.Config != LogFileConfigType.CUSTOM) {
                Output = value.CreateOutput();
            }
        } 
    } = LogFileConfig.Custom();

    /// <summary>
    /// Configures the indentation style applied to logged messages.
    /// </summary>
    public OneOf<InlineIndent, Indent> IndentMessage { get; set; } = new InlineIndent();

    /// <summary>
    /// Determines whether each log message is visually highlighted with a surrounding line for emphasis.
    /// </summary>
    public bool MessageHighlightLine { get; set; } = true;
    
    
    public LoggerConfig() { }
    
    [OverloadResolutionPriority(-100)]
    public LoggerConfig(
        LoggerLevel level = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE, 
        NKColor? fatalColor = null, 
        NKColor? errorColor = null, 
        NKColor? warnColor = null, 
        NKColor? infoColor = null, 
        NKColor? debugColor = null, 
        NKColor? traceColor = null,
        TextWriter? output = null,
        bool simpleMessages = false,
        bool hideTime = false,
        string timeFormat = "HH:mm:ss",
        OneOf<InlineIndent, Indent>? indentMessage = null,
        bool messageHighlightLine = true) 
    {
        Level = level;
        FatalColor = fatalColor ?? NKConsoleColor.DARK_RED;
        ErrorColor = errorColor ?? NKConsoleColor.RED;
        WarnColor = warnColor ?? NKConsoleColor.YELLOW;
        InfoColor = infoColor ?? NKConsoleColor.GREEN;
        DebugColor = debugColor ?? NKConsoleColor.BLUE;
        TraceColor = traceColor ?? NKConsoleColor.GRAY;
        Output = output ?? System.Console.Out;
        SimpleMessages = simpleMessages;
        HideTime = hideTime;
        TimeFormat = timeFormat;
        FileConfig = LogFileConfig.Custom();
        IndentMessage = indentMessage ?? new InlineIndent();
        MessageHighlightLine = messageHighlightLine;
    }
    
    public LoggerConfig(
        LoggerLevel level = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE, 
        NKColor? fatalColor = null, 
        NKColor? errorColor = null, 
        NKColor? warnColor = null, 
        NKColor? infoColor = null, 
        NKColor? debugColor = null, 
        NKColor? traceColor = null,
        LogFileConfig? fileConfig = null,
        bool simpleMessages = false,
        bool hideTime = false,
        string timeFormat = "HH:mm:ss",
        OneOf<InlineIndent, Indent>? indentMessage = null,
        bool messageHighlightLine = true) 
    {
        Level = level;
        FatalColor = fatalColor ?? NKConsoleColor.DARK_RED;
        ErrorColor = errorColor ?? NKConsoleColor.RED;
        WarnColor = warnColor ?? NKConsoleColor.YELLOW;
        InfoColor = infoColor ?? NKConsoleColor.GREEN;
        DebugColor = debugColor ?? NKConsoleColor.BLUE;
        TraceColor = traceColor ?? NKConsoleColor.GRAY;
        SimpleMessages = simpleMessages;
        HideTime = hideTime;
        TimeFormat = timeFormat;
        FileConfig = fileConfig ?? LogFileConfig.Custom();
        Output = System.Console.Out;
        IndentMessage = indentMessage ?? new InlineIndent();
        MessageHighlightLine = messageHighlightLine;
    }
    
    public LoggerConfig(
        LoggerLevel level = CRITICAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE, 
        NKColor? fatalColor = null, 
        NKColor? errorColor = null, 
        NKColor? warnColor = null, 
        NKColor? infoColor = null, 
        NKColor? debugColor = null, 
        NKColor? traceColor = null,
        bool simpleMessages = false,
        bool hideTime = false,
        string timeFormat = "HH:mm:ss",
        OneOf<InlineIndent, Indent>? indentMessage = null,
        bool messageHighlightLine = true) 
    {
        Level = level;
        FatalColor = fatalColor ?? NKConsoleColor.DARK_RED;
        ErrorColor = errorColor ?? NKConsoleColor.RED;
        WarnColor = warnColor ?? NKConsoleColor.YELLOW;
        InfoColor = infoColor ?? NKConsoleColor.GREEN;
        DebugColor = debugColor ?? NKConsoleColor.BLUE;
        TraceColor = traceColor ?? NKConsoleColor.GRAY;
        SimpleMessages = simpleMessages;
        HideTime = hideTime;
        TimeFormat = timeFormat;
        FileConfig = LogFileConfig.Custom();
        Output = System.Console.Out;
        IndentMessage = indentMessage ?? new InlineIndent();
        MessageHighlightLine = messageHighlightLine;
    }

    public object Clone() => MemberwiseClone();

    public sealed record InlineIndent;

    public sealed record Indent(int Spaces) {
        public int Spaces { get; } = Spaces >= 0 ? Spaces : throw new ArgumentOutOfRangeException(nameof(Spaces), "Indentation must be greater than or equal to 0.");
    }
}