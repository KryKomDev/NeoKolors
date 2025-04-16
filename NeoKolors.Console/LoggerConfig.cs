//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using static NeoKolors.Console.LoggerLevel;

namespace NeoKolors.Console;

/// <summary>
/// configures the NKLogger
/// </summary>
public struct LoggerConfig {

    /// <summary>
    /// configures what messages will be logged
    /// </summary>
    public LoggerLevel Level { get; set; } = FATAL | ERROR | WARN | INFO | DEBUG;
    
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
    /// defines the output stream of the logger
    /// </summary>
    public TextWriter Output { get; set; } = System.Console.Out;
    
    /// <summary>
    /// if true, prints simple uncolored messages
    /// </summary>
    public bool SimpleMessages { get; set; } = false;
    
    /// <summary>
    /// if true, hides the timestamp
    /// </summary>
    public bool HideTime { get; set; } = false;
    
    public LoggerConfig() { }

    public LoggerConfig(
        LoggerLevel level = FATAL | ERROR | WARN | INFO | DEBUG, 
        NKColor? fatalColor = null, 
        NKColor? errorColor = null, 
        NKColor? warnColor = null, 
        NKColor? infoColor = null, 
        NKColor? debugColor = null, 
        TextWriter? output = null,
        bool simpleMessages = false,
        bool hideTime = false) 
    {
        Level = level;
        FatalColor = fatalColor ?? NKConsoleColor.DARK_RED;
        ErrorColor = errorColor ?? NKConsoleColor.RED;
        WarnColor = warnColor ?? NKConsoleColor.YELLOW;
        InfoColor = infoColor ?? NKConsoleColor.GREEN;
        DebugColor = debugColor ?? NKConsoleColor.BLUE;
        Output = output ?? System.Console.Out;
        SimpleMessages = simpleMessages;
        HideTime = hideTime;
    }
}