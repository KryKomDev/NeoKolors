//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using static NeoKolors.Common.NKConsoleColor;

namespace NeoKolors.Console;

public struct ExceptionFormat {
    
    /// <summary>
    /// whether to show the highlight when throwing a fancy exception
    /// </summary>
    public bool ShowHighlight { get; set; }
    
    /// <summary>
    /// the highlight style of a fancy exception
    /// </summary>
    public NKColor HighlightColor { get; set; }
    
    /// <summary>
    /// the style of the exception type shown when a fancy exception is thrown
    /// </summary>
    public NKStyle ExceptionTypeStyle { get; set; }
    
    /// <summary>
    /// the style of the namespace printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public NKStyle ExceptionNamespaceStyle { get; set; }
    
    /// <summary>
    /// the style of the exception message shown when a fancy exception is thrown
    /// </summary>
    public NKStyle MessageStyle { get; set; }
    
    /// <summary>
    /// the style of the filename printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public NKStyle FileNameStyle { get; set; }
    
    /// <summary>
    /// the style of the path to a source file printed as a part of the stack trace shown when a fancy exception is
    /// thrown
    /// </summary>
    public NKStyle PathStyle { get; set; }
    
    /// <summary>
    /// the style of the method name printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public NKStyle MethodStyle { get; set; }
    
    /// <summary>
    /// the style of the source of the method(s) as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public NKStyle MethodSourceStyle { get; set; }
    
    /// <summary>
    /// the style of the arguments of the method(s) as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public NKStyle MethodArgumentsStyle { get; set; }

    /// <summary>
    /// the style of the line number printed as a part of the stack trace shown when a fancy exception is thrown
    /// </summary>
    public NKStyle LineNumberStyle { get; set; }
    
    /// <summary>
    /// the style of the help link
    /// </summary>
    public NKStyle HelpLinkStyle { get; set; }

    public ExceptionFormat() {
        ShowHighlight           = true;
        HighlightColor          = DARK_RED;
        ExceptionNamespaceStyle = new NKStyle();
        ExceptionTypeStyle      = new NKStyle(YELLOW, TextStyles.ITALIC);
        MessageStyle            = new NKStyle(RED);
        FileNameStyle           = new NKStyle(BLUE, TextStyles.BOLD);
        PathStyle               = new NKStyle(GRAY);
        MethodStyle             = new NKStyle(BLUE, TextStyles.ITALIC | TextStyles.BOLD);
        MethodSourceStyle       = new NKStyle(GRAY);
        MethodArgumentsStyle    = new NKStyle();
        LineNumberStyle         = new NKStyle(GREEN);
        HelpLinkStyle           = new NKStyle(TextStyles.ITALIC);
    }

    public ExceptionFormat(
        bool showHighlight               = true, 
        NKColor? highlightColor          = null,
        NKStyle? exceptionNamespaceStyle = null,
        NKStyle? exceptionTypeStyle      = null,
        NKStyle? messageStyle            = null,
        NKStyle? fileNameStyle           = null,
        NKStyle? pathStyle               = null, 
        NKStyle? methodStyle             = null,
        NKStyle? methodSourceStyle       = null,
        NKStyle? methodArgumentsStyle    = null,
        NKStyle? lineNumberStyle         = null,
        NKStyle? helpLinkStyle           = null) 
    {
        ShowHighlight           = showHighlight;
        HighlightColor          = highlightColor          ?? DARK_RED;
        ExceptionNamespaceStyle = exceptionNamespaceStyle ?? new NKStyle();
        ExceptionTypeStyle      = exceptionTypeStyle      ?? new NKStyle(YELLOW, TextStyles.ITALIC);
        MessageStyle            = messageStyle            ?? new NKStyle(RED);
        FileNameStyle           = fileNameStyle           ?? new NKStyle(BLUE, TextStyles.BOLD);
        PathStyle               = pathStyle               ?? new NKStyle(GRAY);
        MethodStyle             = methodStyle             ?? new NKStyle(BLUE, TextStyles.ITALIC | TextStyles.BOLD);
        MethodSourceStyle       = methodSourceStyle       ?? new NKStyle(GRAY);
        MethodArgumentsStyle    = methodArgumentsStyle    ?? new NKStyle();
        LineNumberStyle         = lineNumberStyle         ?? new NKStyle(GREEN);
        HelpLinkStyle           = helpLinkStyle           ?? new NKStyle(TextStyles.ITALIC);
    }
}