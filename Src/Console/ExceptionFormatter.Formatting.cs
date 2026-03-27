//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text.RegularExpressions;
using NeoKolors.Common;
using NeoKolors.Console.Driver.Windows;
using NeoKolors.Extensions;
using static NeoKolors.Common.NKConsoleColor;

// disabled because of StringUtils.InRange(string, int, int) to make code cleaner

namespace NeoKolors.Console;

public partial class ExceptionFormatter {

    /// <summary>
    /// if added to <c>AppDomain.CurrentDomain.UnhandledException</c>, makes all exceptions formatted
    /// </summary>
    internal void WriteUnhandled(object? sender, UnhandledExceptionEventArgs args) {
        if (RedirectToLog) {
            if (args.ExceptionObject is Exception e1)
                NKDebug.Crit(
                    $"{e1.GetType().Name}: {e1.Message}\n" +
                    $"   Stack Trace:\n{e1.StackTrace?.Replace("   ", "      ")}");
            else
                NKDebug.Crit($"An unhandled exception occured.{(args.IsTerminating ? " Terminating..." : "")}");
        }

        if (!FormatUnhandled || args.ExceptionObject is not Exception e0) return;

        Stdio.Write(Format(e0));
        
        if (Environment.OSVersion.Platform == PlatformID.Win32NT) 
            WinExceptionSuppressor.Mute();
        else
            Stdio.SetError(TextWriter.Null);
    }

    /// <summary>
    /// formats the exception using the format stored in this instance
    /// </summary>
    public string Format(Exception e) {
        return Format(e, _format);
    }

    /// <summary>
    /// formats the exception
    /// </summary>
    public static string Format(Exception e, ExceptionFormat format) {
        string header = FormatHeader(e, format);
        string stackTrace = FormatStackTrace(e, format);
        string help = e.HelpLink == null ? ""
            : "\nFor more information about this exception, see: {0}{1}".Format(format.HelpLinkStyle, e.HelpLink);
        
        string s = header + stackTrace + help;

        if (!format.ShowHighlight) return s;

        string[] lines = s.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        
        for (int i = 0; i < lines.Length; i++) {
            lines[i] = "▍ ".AddColor(format.HighlightColor) + lines[i];
        }
        
        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static string FormatHeader(Exception e, ExceptionFormat format) {
        string? ns = e.GetType().Namespace;
        string space = ns?.AddStyle(format.ExceptionNamespaceStyle) ?? "";
        string dot = string.IsNullOrEmpty(space) ? "" : ".";
        string type = e.GetType().Name.AddStyle(format.ExceptionTypeStyle).AddStyle(TextStyles.ITALIC);
        string message = e.Message.AddStyle(format.MessageStyle).AddStyle(TextStyles.BOLD);
        return $"{space}{dot}{type}: {message}\n";
    }

    private static string FormatStackTrace(Exception e, ExceptionFormat format) {
        if (e.StackTrace is null) return "";
        
        string[] sources = e.StackTrace.Split('\n');

        for (int i = 0; i < sources.Length; i++) {
            sources[i] = FormatSingleSource(sources[i], format);
        }
        
        return string.Join("", sources);
    }

    private static string FormatSingleSource(string s, ExceptionFormat format) {
        if (string.IsNullOrWhiteSpace(s)) return s;

        var match = Regex.Match(s, @"^(\s*at\s+)([^\(]+)(\(.*\))(?:(\s+in\s+)(.+):line\s+(\d+))?\s*$");
        if (!match.Success) {
            return s.TrimEnd() + "\n";
        }

        string atPart = match.Groups[1].Value;
        string methodPart = match.Groups[2].Value;
        string argsPart = match.Groups[3].Value;

        string output = atPart.AddColor(DARK_GRAY);

        int lastDot = methodPart.LastIndexOf('.');
        if (lastDot != -1) {
            output += methodPart.Substring(0, lastDot + 1).AddStyle(format.MethodSourceStyle);
            output += methodPart.Substring(lastDot + 1).AddStyle(format.MethodStyle);
        }
        else {
            output += methodPart.AddStyle(format.MethodStyle);
        }

        output += argsPart.AddStyle(format.MethodArgumentsStyle);

        if (!match.Groups[4].Success) 
            return output + "\n";
        
        string pathPart = match.Groups[5].Value;
        string linePart = match.Groups[6].Value;

        output += "\n     " + "in ".AddColor(DARK_GRAY);

        int lastSlash = Math.Max(pathPart.LastIndexOf('/'), pathPart.LastIndexOf('\\'));
        if (lastSlash != -1) {
            output += pathPart.Substring(0, lastSlash + 1).AddStyle(format.PathStyle);
            output += pathPart.Substring(lastSlash + 1).AddStyle(format.FileNameStyle);
        }
        else {
            output += pathPart.AddStyle(format.FileNameStyle);
        }

        output += ":line ".AddColor(DARK_GRAY) + linePart.AddStyle(format.LineNumberStyle);

        return output + "\n";
    }
}