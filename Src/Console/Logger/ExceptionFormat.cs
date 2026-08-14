// NeoKolors
// Copyright (c) krystof 2026

using NeoKolors.Common;
using static NeoKolors.Common.NKConsoleColor;

namespace NeoKolors.Console;

/// <summary>
/// Specifies the border rendering style for formatted exceptions.
/// </summary>
public enum ExceptionBorderStyle {
    /// <summary>
    /// Prefixes each line with a vertical bar highlight indicator.
    /// </summary>
    LEFT_BAR,

    /// <summary>
    /// Encloses the entire exception output inside a styled box frame.
    /// </summary>
    BOX,

    /// <summary>
    /// Renders without any surrounding border or highlight bars.
    /// </summary>
    NONE
}

/// <summary>
/// Specifies how file paths in stack traces are displayed.
/// </summary>
public enum PathFormatMode {
    /// <summary>
    /// Displays the complete absolute path to the source file.
    /// </summary>
    FULL_PATH,

    /// <summary>
    /// Displays only the file name without any directory path.
    /// </summary>
    FILE_NAME_ONLY,

    /// <summary>
    /// Displays the path relative to the current working directory if possible.
    /// </summary>
    RELATIVE_PATH
}

/// <summary>
/// Contains configuration settings for styling, layout, filtering, and visual elements of formatted exceptions.
/// </summary>
public class ExceptionFormat {

    // =================================== Styles =================================== //

    /// <summary>Gets or sets the style of the exception type name (e.g. InvalidOperationException).</summary>
    public NKStyle ExceptionTypeStyle { get; set; } = new(YELLOW, s: TextStyles.ITALIC);

    /// <summary>Gets or sets the style of the exception namespace prefix (e.g. System.IO.).</summary>
    public NKStyle ExceptionNamespaceStyle { get; set; } = new();

    /// <summary>Gets or sets the style of the exception message text.</summary>
    public NKStyle MessageStyle { get; set; } = new(f: RED, s: TextStyles.BOLD);

    /// <summary>Gets or sets the style of the source file name in stack traces.</summary>
    public NKStyle FileNameStyle { get; set; } = new(BLUE, s: TextStyles.BOLD);

    /// <summary>Gets or sets the style of the directory path in stack traces.</summary>
    public NKStyle PathStyle { get; set; } = new(GRAY);

    /// <summary>Gets or sets the style of the method name in stack traces.</summary>
    public NKStyle MethodStyle { get; set; } = new(BLUE, s: TextStyles.ITALIC | TextStyles.BOLD);

    /// <summary>Gets or sets the style of the method declaring type / namespace in stack traces.</summary>
    public NKStyle MethodSourceStyle { get; set; } = new(GRAY);

    /// <summary>Gets or sets the style of method argument signatures in stack traces.</summary>
    public NKStyle MethodArgumentsStyle { get; set; } = new();

    /// <summary>Gets or sets the style of method parameter type names in stack traces.</summary>
    public NKStyle MethodParamTypeStyle { get; set; } = new(CYAN);

    /// <summary>Gets or sets the style of method parameter variable names in stack traces.</summary>
    public NKStyle MethodParamNameStyle { get; set; } = new(NKColor.Default, s: TextStyles.FAINT);

    /// <summary>Gets or sets the style of line numbers in stack traces.</summary>
    public NKStyle LineNumberStyle { get; set; } = new(GREEN);

    /// <summary>Gets or sets the style of the HelpLink URL / text.</summary>
    public NKStyle HelpLinkStyle { get; set; } = new(s: TextStyles.ITALIC);

    /// <summary>Gets or sets the style of HResult code display.</summary>
    public NKStyle HResultStyle { get; set; } = new(DARK_GRAY);

    /// <summary>Gets or sets the style of Exception.Data keys.</summary>
    public NKStyle DataKeyStyle { get; set; } = new(CYAN);

    /// <summary>Gets or sets the style of Exception.Data values.</summary>
    public NKStyle DataValueStyle { get; set; } = new(WHITE);

    /// <summary>Gets or sets the style of normal lines in source code snippets.</summary>
    public NKStyle SourceSnippetLineStyle { get; set; } = new(GRAY);

    /// <summary>Gets or sets the style of the target error line in source code snippets.</summary>
    public NKStyle SourceSnippetHighlightLineStyle { get; set; } = new(YELLOW, s: TextStyles.BOLD);

    /// <summary>Gets or sets the style of normal line numbers in source code snippets.</summary>
    public NKStyle SourceSnippetLineNumberStyle { get; set; } = new(DARK_GRAY);

    /// <summary>Gets or sets the style of the target error line number in source code snippets.</summary>
    public NKStyle SourceSnippetHighlightLineNumberStyle { get; set; } = new(YELLOW);

    /// <summary>Gets or sets the style of un-highlighted general text elements.</summary>
    public NKStyle TextStyle { get; set; } = new();

    /// <summary>Gets or sets the style of inner exception header labels.</summary>
    public NKStyle InnerExceptionStyle { get; set; } = new(DARK_YELLOW, s: TextStyles.BOLD);

    // ================================== Decoration ================================== //

    private ExceptionBorderStyle _borderStyle = ExceptionBorderStyle.LEFT_BAR;

    /// <summary>
    /// Gets or sets the border rendering style (LeftBar, Box, or None).
    /// </summary>
    public ExceptionBorderStyle BorderStyle {
        get => _borderStyle;
        set {
            _borderStyle   = value;
            _showHighlight = value != ExceptionBorderStyle.NONE;
        }
    }

    private bool _showHighlight = true;

    /// <summary>
    /// Gets or sets whether a left bar or box highlight decoration is displayed.
    /// </summary>
    public bool ShowHighlight {
        get => _showHighlight;
        set {
            _showHighlight = value;

            if (!value)
                _borderStyle = ExceptionBorderStyle.NONE;
            else if (_borderStyle == ExceptionBorderStyle.NONE)
                _borderStyle = ExceptionBorderStyle.LEFT_BAR;
        }
    }

    /// <summary>Gets or sets the color of the left border highlight bar.</summary>
    public NKColor HighlightColor { get; set; } = DARK_RED;

    /// <summary>Gets or sets the string used for the left border highlight indicator.</summary>
    public string HighlightChar { get; set; } = "▍ ";

    // ================================= Visibility & Toggles ================================= //

    /// <summary>Gets or sets whether the exception namespace is shown before the type name.</summary>
    public bool ShowExceptionNamespace { get; set; } = true;

    /// <summary>Gets or sets whether method declaring type namespaces are shown in stack traces.</summary>
    public bool ShowMethodNamespace { get; set; } = true;

    /// <summary>Gets or sets whether full directory file paths are shown in stack traces. When false, only the file name is displayed.</summary>
    public bool ShowFilePath { get; set; } = true;

    /// <summary>Gets or sets whether the exception message is displayed.</summary>
    public bool ShowMessage { get; set; } = true;

    /// <summary>Gets or sets whether stack traces are included in the output.</summary>
    public bool ShowStackTrace { get; set; } = true;

    /// <summary>Gets or sets whether local source code context snippets are extracted and displayed.</summary>
    public bool ShowSourceSnippet { get; set; } = true;

    /// <summary>Gets or sets the number of context lines to display before and after the target line in code snippets.</summary>
    public int SourceSnippetContextLines { get; set; } = 2;

    /// <summary>Gets or sets the pointer icon indicating the error line in code snippets.</summary>
    public string SourceSnippetPointer { get; set; } = "➔";

    /// <summary>Gets or sets whether the exception HResult value is displayed.</summary>
    public bool ShowHResult { get; set; } = false;

    /// <summary>Gets or sets whether the HelpLink property is formatted.</summary>
    public bool ShowHelpLink { get; set; } = true;

    /// <summary>Gets or sets the text label prefix for HelpLink URLs.</summary>
    public string HelpLinkLabel { get; set; } = "For more information about this exception, see: ";

    /// <summary>Gets or sets whether entries from Exception.Data are displayed.</summary>
    public bool ShowData { get; set; } = true;

    /// <summary>Gets or sets whether inner exceptions are formatted recursively.</summary>
    public bool ShowInnerExceptions { get; set; } = true;

    /// <summary>Gets or sets the maximum recursion depth for inner exceptions.</summary>
    public int MaxInnerExceptionDepth { get; set; } = 10;

    /// <summary>Gets or sets the maximum number of stack trace frames to display (0 for unlimited).</summary>
    public int MaxStackTraceDepth { get; set; } = 0;

    /// <summary>Gets or sets the number of indentation spaces for stack trace lines.</summary>
    public int StackTraceIndent { get; set; } = 3;

    /// <summary>Gets or sets whether stack frames from framework/system namespaces are excluded.</summary>
    public bool FilterSystemFrames { get; set; } = false;

    /// <summary>Gets or sets whether system/framework stack frames are dimmed instead of hidden.</summary>
    public bool DimSystemFrames { get; set; } = true;

    /// <summary>Gets or sets how file paths are formatted in stack traces.</summary>
    public PathFormatMode PathFormat { get; set; } = PathFormatMode.FULL_PATH;

    /// <summary>Gets or sets an optional custom predicate filter to select which stack frames to display.</summary>
    public Func<ParsedStackFrame, bool>? FrameFilter { get; set; }

    /// <summary>Gets or sets an optional custom transformer function for file paths in stack traces.</summary>
    public Func<string, string>? PathTransformer { get; set; }

    // ================================= Construct & Presets ================================= //

    public ExceptionFormat() { }

    /// <summary>
    /// Creates a deep copy of an existing <see cref="ExceptionFormat"/> instance.
    /// </summary>
    public ExceptionFormat(ExceptionFormat other) {
        ExceptionTypeStyle                    = other.ExceptionTypeStyle;
        ExceptionNamespaceStyle               = other.ExceptionNamespaceStyle;
        MessageStyle                          = other.MessageStyle;
        FileNameStyle                         = other.FileNameStyle;
        PathStyle                             = other.PathStyle;
        MethodStyle                           = other.MethodStyle;
        MethodSourceStyle                     = other.MethodSourceStyle;
        MethodArgumentsStyle                  = other.MethodArgumentsStyle;
        MethodParamTypeStyle                  = other.MethodParamTypeStyle;
        MethodParamNameStyle                  = other.MethodParamNameStyle;
        LineNumberStyle                       = other.LineNumberStyle;
        HelpLinkStyle                         = other.HelpLinkStyle;
        HResultStyle                          = other.HResultStyle;
        DataKeyStyle                          = other.DataKeyStyle;
        DataValueStyle                        = other.DataValueStyle;
        SourceSnippetLineStyle                = other.SourceSnippetLineStyle;
        SourceSnippetHighlightLineStyle       = other.SourceSnippetHighlightLineStyle;
        SourceSnippetLineNumberStyle          = other.SourceSnippetLineNumberStyle;
        SourceSnippetHighlightLineNumberStyle = other.SourceSnippetHighlightLineNumberStyle;
        TextStyle                             = other.TextStyle;
        InnerExceptionStyle                   = other.InnerExceptionStyle;

        BorderStyle               = other.BorderStyle;
        HighlightColor            = other.HighlightColor;
        HighlightChar             = other.HighlightChar;
        ShowExceptionNamespace    = other.ShowExceptionNamespace;
        ShowMethodNamespace       = other.ShowMethodNamespace;
        ShowFilePath              = other.ShowFilePath;
        ShowMessage               = other.ShowMessage;
        ShowStackTrace            = other.ShowStackTrace;
        ShowSourceSnippet         = other.ShowSourceSnippet;
        SourceSnippetContextLines = other.SourceSnippetContextLines;
        SourceSnippetPointer      = other.SourceSnippetPointer;
        ShowHResult               = other.ShowHResult;
        ShowHelpLink              = other.ShowHelpLink;
        HelpLinkLabel             = other.HelpLinkLabel;
        ShowData                  = other.ShowData;
        ShowInnerExceptions       = other.ShowInnerExceptions;
        MaxInnerExceptionDepth    = other.MaxInnerExceptionDepth;
        MaxStackTraceDepth        = other.MaxStackTraceDepth;
        FilterSystemFrames        = other.FilterSystemFrames;
        DimSystemFrames           = other.DimSystemFrames;
        PathFormat                = other.PathFormat;
        FrameFilter               = other.FrameFilter;
        PathTransformer           = other.PathTransformer;
    }

    /// <summary>Standard balanced exception layout with colored highlight bar and source code snippets.</summary>
    public static ExceptionFormat Default => new();

    /// <summary>Compact layout with single-line stack frames and framework frames filtered out.</summary>
    public static ExceptionFormat Compact =>
        new() {
            ShowSourceSnippet  = false,
            FilterSystemFrames = true,
            ShowData           = false,
            MaxStackTraceDepth = 5,
            PathFormat         = PathFormatMode.FILE_NAME_ONLY
        };

    /// <summary>Detailed layout with box border, HResult, data dictionary, and extended source code snippets.</summary>
    public static ExceptionFormat Detailed =>
        new() {
            BorderStyle               = ExceptionBorderStyle.BOX,
            ShowSourceSnippet         = true,
            SourceSnippetContextLines = 3,
            ShowHResult               = true,
            ShowHelpLink              = true,
            ShowData                  = true,
            FilterSystemFrames        = false,
            DimSystemFrames           = true
        };

    /// <summary>Minimal layout showing only exception type name and message without stack trace.</summary>
    public static ExceptionFormat Minimal =>
        new() {
            ShowHighlight          = false,
            ShowStackTrace         = false,
            ShowSourceSnippet      = false,
            ShowExceptionNamespace = false,
            ShowHelpLink           = false,
            ShowData               = false
        };

    /// <summary>Plain layout with default text styling and no highlights.</summary>
    public static ExceptionFormat Plain =>
        new() {
            ShowHighlight      = false,
            ShowSourceSnippet  = false,
            ExceptionTypeStyle = new NKStyle(),
            MessageStyle       = new NKStyle(),
            FileNameStyle      = new NKStyle(),
            PathStyle          = new NKStyle(),
            MethodStyle        = new NKStyle(),
            MethodSourceStyle  = new NKStyle(),
            LineNumberStyle    = new NKStyle(),
            HelpLinkStyle      = new NKStyle()
        };
}