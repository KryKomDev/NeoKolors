//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Console;

/// <summary>
/// formats unhandled exceptions to look nice 
/// </summary>
public partial class ExceptionFormatter {
    
    private ExceptionFormat _format = new();
    
    public ExceptionFormat Config { 
        get => _format;
        set => _format = value;
    }

    public bool ShowHighlight {
        get => _format.ShowHighlight;
        set => _format.ShowHighlight = value;
    }

    public NKColor HighlightColor {
        get => _format.HighlightColor;
        set => _format.HighlightColor = value;
    }

    public NKStyle ExceptionTypeStyle {
        get => _format.ExceptionTypeStyle;
        set => _format.ExceptionTypeStyle = value;
    }

    public NKStyle ExceptionNamespaceStyle {
        get => _format.ExceptionNamespaceStyle;
        set => _format.ExceptionNamespaceStyle = value;
    }

    public NKStyle MessageStyle {
        get => _format.MessageStyle;
        set => _format.MessageStyle = value;
    }

    public NKStyle FileNameStyle {
        get => _format.FileNameStyle;
        set => _format.FileNameStyle = value;
    }

    public NKStyle PathStyle {
        get => _format.PathStyle;
        set => _format.PathStyle = value;
    }

    public NKStyle MethodStyle {
        get => _format.MethodStyle;
        set => _format.MethodStyle = value;
    }

    public NKStyle MethodSourceStyle {
        get => _format.MethodSourceStyle;
        set => _format.MethodSourceStyle = value;
    }

    public NKStyle MethodArgumentsStyle {
        get => _format.MethodArgumentsStyle;
        set => _format.MethodArgumentsStyle = value;
    }

    public NKStyle LineNumberStyle {
        get => _format.LineNumberStyle;
        set => _format.LineNumberStyle = value;
    }

    public NKStyle HelpLinkStyle {
        get => _format.HelpLinkStyle;
        set => _format.HelpLinkStyle = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether exception output is redirected to a log.
    /// </summary>
    /// <remarks>
    /// When enabled, unhandled exceptions are also logged instead of purely being output to the console.
    /// This can be useful for tracking issues in applications where console output is not always monitored.
    /// </remarks>
    public bool RedirectToLog { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether unhandled exceptions are formatted for better readability.
    /// </summary>
    /// <remarks>
    /// When enabled, unhandled exceptions are processed and displayed with the specified formatting styles.
    /// This functionality is useful for improving the clarity and presentation of exception information,
    /// particularly in console-based applications.
    /// </remarks>
    public bool FormatUnhandled { get; set; } = true;
}