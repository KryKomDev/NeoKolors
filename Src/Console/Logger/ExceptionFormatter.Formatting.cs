//
// NeoKolors
// Copyright (c) 2025-2026 KryKom
//

using System.Collections;
using NeoKolors.Common;
using Polyfills;
using static NeoKolors.Common.NKConsoleColor;

namespace NeoKolors.Console;

public static partial class ExceptionFormatter {

    /// <summary>
    /// If added to <c>AppDomain.CurrentDomain.UnhandledException</c>, makes all unhandled exceptions formatted.
    /// </summary>
    internal static void WriteUnhandled(object? sender, UnhandledExceptionEventArgs args) {
        if (RedirectToLog) {
            if (args.ExceptionObject is Exception e1) {
                NKDebug.Crit(
                    $"{e1.GetType().Name}: {e1.Message}\n" +
                    $"   Stack Trace:\n{e1.StackTrace?.Replace("   ", "      ")}"
                );
            }
            else {
                NKDebug.Crit($"An unhandled exception occured.{(args.IsTerminating ? " Terminating..." : "")}");
            }
        }

        if (!FormatUnhandled || args.ExceptionObject is not Exception e0)
            return;

        Stdio.Write(Format(e0).ToString());

        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            WinExceptionSuppressor.Mute();
        else
            Stdio.SetError(TextWriter.Null);
    }

    /// <summary>
    /// Formats an exception using the global configuration.
    /// </summary>
    public static AnsiString Format(Exception e) {
        return Format(e, Config);
    }

    /// <summary>
    /// Formats an exception into a stylized <see cref="AnsiString"/> using the specified <see cref="ExceptionFormat"/>.
    /// </summary>
    public static AnsiString Format(Exception e, ExceptionFormat? format) {
        ArgumentNullException.ThrowIfNull(e);

        format ??= Config;

        var sb = new AnsiStringBuilder();
        FormatSingleException(sb, e, format, depth: 0);

        var rawOutput = sb.ToAnsiString();

        return ApplyBorder(rawOutput, format);
    }

    /// <summary>
    /// Formats an exception and prints it directly to standard output.
    /// </summary>
    public static void Print(Exception e, ExceptionFormat? format = null) {
        NKConsole.Write(Format(e, format).ToString());
    }

    /// <summary>
    /// Formats an exception and returns a plain string containing ANSI control sequences.
    /// </summary>
    public static string FormatToString(Exception e, ExceptionFormat? format = null) {
        return Format(e, format).ToString();
    }

    private static void FormatSingleException(AnsiStringBuilder sb, Exception e, ExceptionFormat format, int depth) {
        while (true) {
            string indent = depth > 0 ? new string(' ', depth * 2) : string.Empty;

            if (depth > 0) {
                sb.Append(indent);
                sb.AppendLine("└── Inner Exception:", format.InnerExceptionStyle);
            }

            // Header (Namespace + TypeName + Message + HResult)
            if (format.ShowExceptionNamespace) {
                string? ns = e.GetType().Namespace;

                if (!string.IsNullOrEmpty(ns)) {
                    sb.Append(indent);
                    sb.Append(ns + ".", format.ExceptionNamespaceStyle);
                }
                else {
                    sb.Append(indent);
                }
            }
            else {
                sb.Append(indent);
            }

            sb.Append(e.GetType().Name, format.ExceptionTypeStyle);

            if (format.ShowMessage && !string.IsNullOrEmpty(e.Message)) {
                sb.Append(": ");
                sb.Append(e.Message, format.MessageStyle);
            }

            if (format.ShowHResult && e.HResult != 0) {
                sb.Append($" [0x{e.HResult:X8}]", format.HResultStyle);
            }

            sb.AppendLine();

            // HelpLink
            if (format.ShowHelpLink && !string.IsNullOrEmpty(e.HelpLink)) {
                sb.Append(indent);
                sb.Append(format.HelpLinkLabel, format.TextStyle);
                sb.AppendLine(e.HelpLink, format.HelpLinkStyle);
            }

            // Exception Data Dictionary
            if (format.ShowData && e.Data is { Count: > 0 }) {
                sb.Append(indent);
                sb.AppendLine("  Data:", format.TextStyle);

                foreach (DictionaryEntry entry in e.Data) {
                    sb.Append(indent);
                    sb.Append("    ",                          format.TextStyle);
                    sb.Append(entry.Key?.ToString() ?? "null", format.DataKeyStyle);
                    sb.Append(" = ",                           format.TextStyle);
                    sb.AppendLine(entry.Value?.ToString() ?? "null", format.DataValueStyle);
                }
            }

            // Stack Trace
            if (format.ShowStackTrace && !string.IsNullOrEmpty(e.StackTrace)) {
                string[] rawLines        = e.StackTrace.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
                int      framesProcessed = 0;

                foreach (string rawLine in rawLines) {
                    var frame = ParsedStackFrame.Parse(rawLine);

                    // Frame filtering
                    if (format.FilterSystemFrames && frame.IsSystemFrame)
                        continue;

                    if (format.FrameFilter != null && !format.FrameFilter(frame))
                        continue;

                    FormatSingleFrame(sb, frame, format, indent);
                    framesProcessed++;

                    if (format.MaxStackTraceDepth > 0 && framesProcessed >= format.MaxStackTraceDepth) {
                        break;
                    }
                }
            }

            // Inner Exceptions
            if (!format.ShowInnerExceptions || depth >= format.MaxInnerExceptionDepth)
                return;

            if (e is AggregateException { InnerExceptions.Count: > 0 } aggEx) {
                foreach (var inner in aggEx.InnerExceptions) {
                    FormatSingleException(sb, inner, format, depth + 1);
                }
            }
            else if (e.InnerException != null) {
                e     =  e.InnerException;
                depth += 1;

                continue;
            }

            break;
        }
    }

    private static void FormatSingleFrame(AnsiStringBuilder sb, ParsedStackFrame frame, ExceptionFormat format, string indent) {
        if (string.IsNullOrWhiteSpace(frame.RawText))
            return;

        var dimStyle     = new NKStyle(DARK_GRAY);
        var mSourceStyle = format.DimSystemFrames && frame.IsSystemFrame ? dimStyle : format.MethodSourceStyle;
        var mStyle       = format.DimSystemFrames && frame.IsSystemFrame ? dimStyle : format.MethodStyle;
        var mArgsStyle   = format.DimSystemFrames && frame.IsSystemFrame ? dimStyle : format.MethodArgumentsStyle;

        string atPrefix = new string(' ', Math.Max(0, format.StackTraceIndent)) + "at ";
        string inPrefix = new string(' ', Math.Max(0, format.StackTraceIndent + 2)) + "in ";

        sb.Append(indent);
        sb.Append(atPrefix, dimStyle);

        if (!string.IsNullOrEmpty(frame.DeclaringType)) {
            string declaringTypeToDisplay = frame.DeclaringType;
            if (!format.ShowMethodNamespace) {
                int lastDot = declaringTypeToDisplay.LastIndexOf('.');
                if (lastDot != -1) {
                    declaringTypeToDisplay = declaringTypeToDisplay.Substring(lastDot + 1);
                }
            }
            sb.Append(declaringTypeToDisplay + ".", mSourceStyle);
        }

        sb.Append(frame.MethodName, mStyle);

        FormatParameters(sb, frame.Parameters, format, frame.IsSystemFrame);

        if (frame.HasSourceInfo) {
            bool isCompactSingleLine = !format.ShowMethodNamespace && !format.ShowFilePath;

            if (isCompactSingleLine) {
                sb.Append(" in ", dimStyle);
            }
            else {
                sb.AppendLine();
                sb.Append(indent);
                sb.Append(inPrefix, dimStyle);
            }

            string displayPath = frame.FilePath!;

            if (!format.ShowFilePath) {
                displayPath = Path.GetFileName(displayPath);
            }
            else if (format.PathTransformer != null) {
                displayPath = format.PathTransformer(displayPath);
            }
            else {
                displayPath = format.PathFormat switch {
                    PathFormatMode.FILE_NAME_ONLY => Path.GetFileName(displayPath),
                    PathFormatMode.RELATIVE_PATH => GetRelativePath(displayPath),
                    _                           => displayPath
                };
            }

            int lastSlash = Math.Max(displayPath.LastIndexOf('/'), displayPath.LastIndexOf('\\'));

            if (lastSlash != -1) {
                sb.Append(displayPath.Substring(0, lastSlash + 1), format.PathStyle);
                sb.Append(displayPath.Substring(lastSlash    + 1), format.FileNameStyle);
            }
            else {
                sb.Append(displayPath, format.FileNameStyle);
            }

            sb.Append(":line ", dimStyle);
            sb.Append(frame.LineNumber.ToString(), format.LineNumberStyle);

            if (format.ShowSourceSnippet && File.Exists(frame.FilePath)) {
                AppendSourceSnippet(sb, frame.FilePath!, frame.LineNumber, format, indent);
            }
        }

        sb.AppendLine();
    }

    private static void FormatParameters(AnsiStringBuilder sb, string rawParameters, ExceptionFormat format, bool isSystemFrame) {
        var dimStyle = new NKStyle(DARK_GRAY);
        NKStyle pPunctStyle = (format.DimSystemFrames && isSystemFrame) ? dimStyle : format.MethodArgumentsStyle;
        NKStyle pTypeStyle = (format.DimSystemFrames && isSystemFrame) ? dimStyle : format.MethodParamTypeStyle;
        NKStyle pNameStyle = (format.DimSystemFrames && isSystemFrame) ? dimStyle : format.MethodParamNameStyle;

        if (string.IsNullOrEmpty(rawParameters)) {
            return;
        }

        string trimmed = rawParameters.Trim();
        if (!trimmed.StartsWith("(") || !trimmed.EndsWith(")")) {
            sb.Append(rawParameters, pPunctStyle);
            return;
        }

        string content = trimmed.Substring(1, trimmed.Length - 2).Trim();

        sb.Append("(", pPunctStyle);

        if (!string.IsNullOrEmpty(content)) {
            string[] paramsList = SplitParameters(content);
            for (int i = 0; i < paramsList.Length; i++) {
                if (i > 0) {
                    sb.Append(", ", pPunctStyle);
                }

                string paramToken = paramsList[i].Trim();
                int lastSpace = paramToken.LastIndexOf(' ');
                if (lastSpace != -1) {
                    string typePart = paramToken.Substring(0, lastSpace);
                    string namePart = paramToken.Substring(lastSpace + 1);

                    sb.Append(typePart, pTypeStyle);
                    sb.Append(" ", pPunctStyle);
                    sb.Append(namePart, pNameStyle);
                }
                else {
                    sb.Append(paramToken, pTypeStyle);
                }
            }
        }

        sb.Append(")", pPunctStyle);
    }

    private static string[] SplitParameters(string content) {
        var list = new List<string>();
        int depth = 0;
        int lastStart = 0;

        for (int i = 0; i < content.Length; i++) {
            char c = content[i];
            if (c is '<' or '[' or '(') depth++;
            else if (c is '>' or ']' or ')') depth--;
            else if (c == ',' && depth == 0) {
                list.Add(content.Substring(lastStart, i - lastStart));
                lastStart = i + 1;
            }
        }

        if (lastStart < content.Length) {
            list.Add(content.Substring(lastStart));
        }

        return list.ToArray();
    }

    private static void AppendSourceSnippet(AnsiStringBuilder sb, string filePath, int targetLineNumber, ExceptionFormat format, string indent) {
        try {
            string[] lines       = File.ReadAllLines(filePath);
            int      targetIndex = targetLineNumber - 1;

            if (targetIndex < 0 || targetIndex >= lines.Length)
                return;

            var dimStyle        = new NKStyle(DARK_GRAY);
            int context         = Math.Max(0, format.SourceSnippetContextLines);
            int start           = Math.Max(0, targetIndex - context);
            int end             = Math.Min(lines.Length   - 1, targetIndex + context);
            int maxLineNumWidth = (end + 1).ToString().Length;

            string snippetIndent = new string(' ', Math.Max(0, format.StackTraceIndent));
            string headerSpaces  = new string(' ', Math.Max(0, format.StackTraceIndent) + maxLineNumWidth + 1);
            string pointerStr = format.SourceSnippetPointer.EndsWith(" ")
                ? format.SourceSnippetPointer
                : format.SourceSnippetPointer + " ";
            string targetSep = " " + pointerStr;
            string nonTargetSep = " │ ";

            if (targetSep.Length > nonTargetSep.Length) {
                nonTargetSep = " │" + new string(' ', targetSep.Length - 2);
            }

            string displayFilePath = format.ShowFilePath ? filePath : Path.GetFileName(filePath);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(indent);
            sb.Append(headerSpaces, dimStyle);
            sb.Append("┌─ ",             dimStyle);
            sb.Append(displayFilePath,  format.FileNameStyle);
            sb.Append(":",               dimStyle);
            sb.AppendLine(targetLineNumber.ToString(), format.LineNumberStyle);

            for (int i = start; i <= end; i++) {
                bool   isTarget   = i == targetIndex;
                string lineNumStr = (i + 1).ToString().PadLeft(maxLineNumWidth);

                sb.Append(indent);
                sb.Append(snippetIndent);
                sb.Append(lineNumStr, isTarget ? format.SourceSnippetHighlightLineNumberStyle : format.SourceSnippetLineNumberStyle);

                if (isTarget) {
                    sb.Append(targetSep, format.SourceSnippetHighlightLineStyle);
                    sb.AppendLine(lines[i], format.SourceSnippetHighlightLineStyle);
                }
                else {
                    sb.Append(nonTargetSep, dimStyle);
                    sb.AppendLine(lines[i], format.SourceSnippetLineStyle);
                }
            }
        }
        catch {
            // Silently ignore snippet failures if file read errors occur
        }
    }

    private static AnsiString ApplyBorder(AnsiString rawOutput, ExceptionFormat format) {
        if (!format.ShowHighlight || format.BorderStyle == ExceptionBorderStyle.NONE) {
            return rawOutput;
        }

        var rawLines     = rawOutput.Split('\n');
        var cleanedLines = new List<AnsiString>();

        foreach (var line in rawLines) {
            cleanedLines.Add(line.TrimEnd('\r'));
        }

        while (cleanedLines.Count > 0 && cleanedLines[^1].Length == 0) {
            cleanedLines.RemoveAt(cleanedLines.Count - 1);
        }

        if (format.BorderStyle == ExceptionBorderStyle.BOX) {
            int maxLen = 0;

            foreach (var l in cleanedLines) {
                if (l.Length > maxLen)
                    maxLen = l.Length;
            }

            maxLen = Math.Max(maxLen, 30);

            var sb          = new AnsiStringBuilder();
            var borderStyle = new NKStyle(format.HighlightColor);

            sb.Append("╭─ ",        borderStyle);
            sb.Append("Exception ", format.ExceptionTypeStyle);
            sb.AppendLine(new string('─', Math.Max(0, maxLen - 8)) + "╮", borderStyle);

            foreach (var l in cleanedLines) {
                int pad = Math.Max(0, maxLen - l.Length);

                sb.Append("│ ", borderStyle);
                sb.Append(l);
                sb.Append(new string(' ', pad));
                sb.AppendLine(" │", borderStyle);
            }

            sb.AppendLine("╰" + new string('─', maxLen + 2) + "╯", borderStyle);

            return sb.ToAnsiString();
        }

        // LeftBar border (default)
        var highlightStyle = new NKStyle(format.HighlightColor);
        var barSb          = new AnsiStringBuilder();

        foreach (var l in cleanedLines) {
            barSb.Append(format.HighlightChar, highlightStyle);
            barSb.Append(l);
            barSb.AppendLine();
        }

        return barSb.ToAnsiString();
    }

    private static string GetRelativePath(string fullPath) {
        try {
            string currentDir = Directory.GetCurrentDirectory();

            if (fullPath.StartsWith(currentDir, StringComparison.OrdinalIgnoreCase)) {
                string rel = fullPath[currentDir.Length..].TrimStart('/', '\\');

                return string.IsNullOrEmpty(rel) ? fullPath : rel;
            }
        }
        catch {
            // ignored
        }

        return fullPath;
    }
}