// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;
using static NeoKolors.Common.NKConsoleColor;

namespace NeoKolors.Console;

public class AnsiLoggerConfig : TextLoggerConfig {

    private static readonly NKStyle DEFAULT_CRITICAL = new(DARK_RED);
    private static readonly NKStyle DEFAULT_ERROR    = new(RED);
    private static readonly NKStyle DEFAULT_WARNING  = new(YELLOW);
    private static readonly NKStyle DEFAULT_INFO     = new(GREEN);
    private static readonly NKStyle DEFAULT_DEBUG    = new(BLUE);
    private static readonly NKStyle DEFAULT_TRACE    = new(DARK_GRAY);

    public NKStyle CriticalStyle { get; set; } = DEFAULT_CRITICAL;
    public NKStyle ErrorStyle    { get; set; } = DEFAULT_ERROR;
    public NKStyle WarningStyle  { get; set; } = DEFAULT_WARNING;
    public NKStyle InfoStyle     { get; set; } = DEFAULT_INFO;
    public NKStyle DebugStyle    { get; set; } = DEFAULT_DEBUG;
    public NKStyle TraceStyle    { get; set; } = DEFAULT_TRACE;

    public bool             FormatExceptions { get; set; } = true;
    public ExceptionFormat? ExceptionFormat  { get; set; } = new();

    public bool UseColors { get; set; }

    public AnsiLoggerConfig(
        bool                  useColors        = true,
        bool                  showTime         = true,
        string                timeFormat       = "HH:mm:ss",
        bool                  showSource       = true,
        LogIndentationConfig? indentation      = null,
        bool                  formatExceptions = true,
        ExceptionFormat?      exceptionFormat  = null
    ) : base(
        showTime,
        timeFormat,
        showSource,
        indentation
    ) {
        ShowTime         = showTime;
        TimeFormat       = timeFormat;
        ShowSource       = showSource;
        FormatExceptions = formatExceptions;
        ExceptionFormat  = exceptionFormat;
        UseColors        = useColors;
    }

    public AnsiLoggerConfig() {
        UseColors = true;
    }
}