//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text.RegularExpressions;
using NeoKolors.Common;
using NeoKolors.Extensions;
using static NeoKolors.Common.NKConsoleColor;

// disabled because of StringUtils.InRange(string, int, int) to make code cleaner
#pragma warning disable CS0618 // Type or member is obsolete

namespace NeoKolors.Console;

public partial class ExceptionFormatter {

    /// <summary>
    /// if added to <c>AppDomain.CurrentDomain.UnhandledException</c>, makes all exceptions formatted
    /// </summary>
    internal void WriteUnhandled(object? sender, UnhandledExceptionEventArgs args) {
        if (args.ExceptionObject is Exception e0)
            System.Console.Write(Format(e0));

        if (RedirectToLog) {
            if (args.ExceptionObject is Exception e1)
                NKDebug.Crit(
                    $"{e1.GetType().Name}: {e1.Message}\n" +
                    $"   Stack Trace:\n{e1.StackTrace?.Replace("   ", "      ")}");
            else
                NKDebug.Crit($"An unhandled exception occured.{(args.IsTerminating ? " Terminating..." : "")}");
        }
        
        if (args.IsTerminating) 
            Environment.Exit(1);
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
        
        string s = "\0" + header + stackTrace + help;

        if (!format.ShowHighlight) return s;

        string[] lines = s.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        
        for (int i = 0; i < lines.Length; i++) {
            lines[i] = "▍ ".AddColor(format.HighlightColor) + lines[i];
        }
        
        return string.Join(Environment.NewLine, lines);
    }

    private static string FormatHeader(Exception e, ExceptionFormat format) {
        string space = e.GetType().Namespace!.AddStyle(format.ExceptionNamespaceStyle);
        string type = e.GetType().Name.AddStyle(format.ExceptionTypeStyle).AddStyle(TextStyles.ITALIC);
        string message = e.Message.AddStyle(format.MessageStyle).AddStyle(TextStyles.BOLD);
        return $"{space}.{type}: {message}\n";
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
        
        // find the method name
        var m = Regex.Match(s, @"(?<=\.)[^.]*?(?=\()");
        
        // color the "at some.namespace.of.a.method" 
        string output = s.Substring(0, 5).AddColor(DARK_GRAY) +
                        s.InRange(5, m.Index).AddStyle(format.MethodSourceStyle);
        
        // color the method
        output += m.Value.AddStyle(format.MethodStyle);
        
        // find the method end
        int methodEnd = s.IndexOf(')');
        
        // color method arguments
        output += s.InRange(m.Index + m.Length, methodEnd + 1).AddStyle(format.MethodArgumentsStyle) + "\n";
        
        // color the "in" part
        output += "     " + s.Substring(methodEnd + 1, 3).AddColor(DARK_GRAY); 
        
        // find the at line
        int line = s.IndexOf(".cs:line", StringComparison.InvariantCulture);

        // find the start of the filename
        int filename = 0;
        for (int i = line; i >= 0; i--) {
            if (s[i] is not ('/' or '\\')) continue;
            filename = i + 1;
            break;
        }

        // color the path to the source file
        output += s.InRange(methodEnd + 4, filename).AddStyle(format.PathStyle);
        
        // color the filename
        output += s.InRange(filename, line + 3).AddStyle(format.FileNameStyle);
        
        // color the position
        output += " at line".AddColor(DARK_GRAY) + s.Substring(line + 8).AddStyle(format.LineNumberStyle);
        
        return output;
    }
}