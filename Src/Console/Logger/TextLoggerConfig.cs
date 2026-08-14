// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Console;

public class TextLoggerConfig {
    public bool   ShowTime   { get; set; }
    public string TimeFormat { get; set; }
    public bool   ShowSource { get; set; }

    public LogIndentationConfig Indentation { get; set; }

    public TextLoggerConfig(
        bool                  showTime    = true,
        string                timeFormat  = "HH:mm:ss",
        bool                  showSource  = true,
        LogIndentationConfig? indentation = null
    ) {
        ShowTime    = showTime;
        TimeFormat  = timeFormat;
        ShowSource  = showSource;
        Indentation = indentation ?? new LogIndentationConfig(2);
    }
}