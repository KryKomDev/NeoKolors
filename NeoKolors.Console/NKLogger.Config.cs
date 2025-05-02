//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using static NeoKolors.Console.LoggerLevel;

namespace NeoKolors.Console;

public partial class NKLogger {

    public NKLogger(LoggerConfig? config = null) {
        _config = config ?? new LoggerConfig();
    }

    ~NKLogger() {
        Output.Close();
    }
    
    private LoggerConfig _config;
    
    /// <summary>
    /// configures the NKLogger
    /// </summary>
    public LoggerConfig Config { 
        get => _config; 
        set => _config = value; 
    }
    
    /// <summary>
    /// configures what messages will be logged
    /// </summary>
    public LoggerLevel Level { 
        get => _config.Level; 
        set => _config.Level = value;
    }

    /// <summary>
    /// defines the color of the fatal error messages
    /// </summary>
    public NKColor FatalColor {
        get => _config.FatalColor;
        set => _config.FatalColor = value;
    }

    /// <summary>
    /// defines the color of the error messages
    /// </summary>
    public NKColor ErrorColor {
        get => _config.ErrorColor;
        set => _config.ErrorColor = value;
    }

    /// <summary>
    /// defines the color of the warning messages
    /// </summary>
    public NKColor WarnColor {
        get => _config.WarnColor;
        set => _config.WarnColor = value;
    }

    /// <summary>
    /// defines the color of the info messages
    /// </summary>
    public NKColor InfoColor {
        get => _config.InfoColor;
        set => _config.InfoColor = value;
    }

    /// <summary>
    /// defines the color of the debug messages
    /// </summary>
    public NKColor DebugColor {
        get => _config.DebugColor;
        set => _config.DebugColor = value;
    }

    /// <summary>
    /// Specifies the color used to represent trace-level log messages.
    /// </summary>
    public NKColor TraceColor {
        get => _config.TraceColor;
        set => _config.TraceColor = value;
    }

    /// <summary>
    /// defines the output stream of the logger
    /// </summary>
    public TextWriter Output {
        get => _config.Output;
        set => _config.Output = value;
    }

    /// <summary>
    /// if true, prints simple uncolored messages
    /// </summary>
    public bool SimpleMessages {
        get => _config.SimpleMessages;
        set => _config.SimpleMessages = value;
    }

    /// <summary>
    /// if true, hide the timestamp
    /// </summary>
    public bool HideTime {
        get => _config.HideTime;
        set => _config.HideTime = value;
    }

    /// <summary>
    /// makes all messages visible
    /// </summary>
    public void LogAll() => _config.Level = FATAL | ERROR | WARN | INFO | DEBUG | TRACE;
    
    /// <summary>
    /// makes all messages except debug visible
    /// </summary>
    public void NoDebug() => _config.Level = FATAL | ERROR | WARN | INFO;
    
    /// <summary>
    /// makes fatal, error and warning messages visible
    /// </summary>
    public void LogWarn() => _config.Level = FATAL | ERROR | WARN;
    
    /// <summary>
    /// makes only error and fatal messages visible
    /// </summary>
    public void LogErrors() => _config.Level = FATAL | ERROR;
    
    /// <summary>
    /// makes only fatal visible
    /// </summary>
    public void LogFatal() => _config.Level = FATAL;
    
    /// <summary>
    /// hides all messages
    /// </summary>
    public void LogNone() => _config.Level = NONE;
}