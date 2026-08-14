//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Console;

/// <summary>
/// Formats unhandled exceptions and exception objects into stylized console strings.
/// </summary>
public static partial class ExceptionFormatter {

    public static ExceptionFormat Config { get; set; } = new();

    public static NKStyle ExceptionTypeStyle {
        get => Config.ExceptionTypeStyle;
        set => Config.ExceptionTypeStyle = value;
    }

    public static NKStyle ExceptionNamespaceStyle {
        get => Config.ExceptionNamespaceStyle;
        set => Config.ExceptionNamespaceStyle = value;
    }

    public static NKStyle MessageStyle {
        get => Config.MessageStyle;
        set => Config.MessageStyle = value;
    }

    public static NKStyle FileNameStyle {
        get => Config.FileNameStyle;
        set => Config.FileNameStyle = value;
    }

    public static NKStyle PathStyle {
        get => Config.PathStyle;
        set => Config.PathStyle = value;
    }

    public static NKStyle MethodStyle {
        get => Config.MethodStyle;
        set => Config.MethodStyle = value;
    }

    public static NKStyle MethodSourceStyle {
        get => Config.MethodSourceStyle;
        set => Config.MethodSourceStyle = value;
    }

    public static NKStyle MethodArgumentsStyle {
        get => Config.MethodArgumentsStyle;
        set => Config.MethodArgumentsStyle = value;
    }

    public static NKStyle MethodParamTypeStyle {
        get => Config.MethodParamTypeStyle;
        set => Config.MethodParamTypeStyle = value;
    }

    public static NKStyle MethodParamNameStyle {
        get => Config.MethodParamNameStyle;
        set => Config.MethodParamNameStyle = value;
    }

    public static NKStyle LineNumberStyle {
        get => Config.LineNumberStyle;
        set => Config.LineNumberStyle = value;
    }

    public static NKStyle HelpLinkStyle {
        get => Config.HelpLinkStyle;
        set => Config.HelpLinkStyle = value;
    }

    public static NKStyle HResultStyle {
        get => Config.HResultStyle;
        set => Config.HResultStyle = value;
    }

    public static NKStyle DataKeyStyle {
        get => Config.DataKeyStyle;
        set => Config.DataKeyStyle = value;
    }

    public static NKStyle DataValueStyle {
        get => Config.DataValueStyle;
        set => Config.DataValueStyle = value;
    }

    public static NKStyle SourceSnippetLineStyle {
        get => Config.SourceSnippetLineStyle;
        set => Config.SourceSnippetLineStyle = value;
    }

    public static NKStyle SourceSnippetHighlightLineStyle {
        get => Config.SourceSnippetHighlightLineStyle;
        set => Config.SourceSnippetHighlightLineStyle = value;
    }

    public static NKStyle SourceSnippetLineNumberStyle {
        get => Config.SourceSnippetLineNumberStyle;
        set => Config.SourceSnippetLineNumberStyle = value;
    }

    public static NKStyle SourceSnippetHighlightLineNumberStyle {
        get => Config.SourceSnippetHighlightLineNumberStyle;
        set => Config.SourceSnippetHighlightLineNumberStyle = value;
    }

    public static NKStyle TextStyle {
        get => Config.TextStyle;
        set => Config.TextStyle = value;
    }

    public static NKStyle InnerExceptionStyle {
        get => Config.InnerExceptionStyle;
        set => Config.InnerExceptionStyle = value;
    }

    public static ExceptionBorderStyle BorderStyle {
        get => Config.BorderStyle;
        set => Config.BorderStyle = value;
    }

    public static bool ShowHighlight {
        get => Config.ShowHighlight;
        set => Config.ShowHighlight = value;
    }

    public static NKColor HighlightColor {
        get => Config.HighlightColor;
        set => Config.HighlightColor = value;
    }

    public static string HighlightChar {
        get => Config.HighlightChar;
        set => Config.HighlightChar = value;
    }

    public static bool ShowExceptionNamespace {
        get => Config.ShowExceptionNamespace;
        set => Config.ShowExceptionNamespace = value;
    }

    public static bool ShowMethodNamespace {
        get => Config.ShowMethodNamespace;
        set => Config.ShowMethodNamespace = value;
    }

    public static bool ShowFilePath {
        get => Config.ShowFilePath;
        set => Config.ShowFilePath = value;
    }

    public static bool ShowMessage {
        get => Config.ShowMessage;
        set => Config.ShowMessage = value;
    }

    public static bool ShowStackTrace {
        get => Config.ShowStackTrace;
        set => Config.ShowStackTrace = value;
    }

    public static bool ShowSourceSnippet {
        get => Config.ShowSourceSnippet;
        set => Config.ShowSourceSnippet = value;
    }

    public static int SourceSnippetContextLines {
        get => Config.SourceSnippetContextLines;
        set => Config.SourceSnippetContextLines = value;
    }

    public static string SourceSnippetPointer {
        get => Config.SourceSnippetPointer;
        set => Config.SourceSnippetPointer = value;
    }

    public static bool ShowHResult {
        get => Config.ShowHResult;
        set => Config.ShowHResult = value;
    }

    public static bool ShowHelpLink {
        get => Config.ShowHelpLink;
        set => Config.ShowHelpLink = value;
    }

    public static string HelpLinkLabel {
        get => Config.HelpLinkLabel;
        set => Config.HelpLinkLabel = value;
    }

    public static bool ShowData {
        get => Config.ShowData;
        set => Config.ShowData = value;
    }

    public static bool ShowInnerExceptions {
        get => Config.ShowInnerExceptions;
        set => Config.ShowInnerExceptions = value;
    }

    public static int MaxInnerExceptionDepth {
        get => Config.MaxInnerExceptionDepth;
        set => Config.MaxInnerExceptionDepth = value;
    }

    public static int MaxStackTraceDepth {
        get => Config.MaxStackTraceDepth;
        set => Config.MaxStackTraceDepth = value;
    }

    /// <summary>Gets or sets the number of indentation spaces for stack trace lines.</summary>
    public static int StackTraceIndent {
        get => Config.StackTraceIndent;
        set => Config.StackTraceIndent = value;
    }

    public static bool FilterSystemFrames {
        get => Config.FilterSystemFrames;
        set => Config.FilterSystemFrames = value;
    }

    public static bool DimSystemFrames {
        get => Config.DimSystemFrames;
        set => Config.DimSystemFrames = value;
    }

    public static PathFormatMode PathFormat {
        get => Config.PathFormat;
        set => Config.PathFormat = value;
    }

    public static Func<ParsedStackFrame, bool>? FrameFilter {
        get => Config.FrameFilter;
        set => Config.FrameFilter = value;
    }

    public static Func<string, string>? PathTransformer {
        get => Config.PathTransformer;
        set => Config.PathTransformer = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether exception output is redirected to a log.
    /// </summary>
    public static bool RedirectToLog { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether unhandled exceptions are formatted for better readability.
    /// </summary>
    public static bool FormatUnhandled { get; set; } = true;
}