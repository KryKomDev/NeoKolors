//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text.RegularExpressions;
using NeoKolors.Common;

namespace NeoKolors.Console;

/// <summary>
/// custom exception throwing
/// </summary>
public static partial class Debug {
    
    /// <summary>
    /// throws an exception
    /// </summary>
    public static void PrintException(Exception e) => System.Console.Write(ToFancyString(e));

    /// <summary>
    /// throws an exception with fancy formatting
    /// </summary>
    /// <param name="e">exception to be thrown</param>
    /// <remarks>
    /// note that this method will not throw the exception itself but rather an instance of
    /// <see cref="FancyException{TInner}"/>
    /// with the exception as the inner exception
    /// </remarks>
    public static void Throw(Exception e) => throw ((Exception)FancyException<Exception>.Create(e));

    /// <summary>
    /// creates a new fancy exception
    /// </summary>
    /// <param name="e">exception to be made fancy</param>
    /// <remarks>
    /// note that this method will not return the exception itself but rather an instance of
    /// <see cref="FancyException{TInner}"/>
    /// with the exception as the inner exception
    /// </remarks>
    public static Exception ToFancy(Exception e) => (Exception)FancyException<Exception>.Create(e);

    /// <summary>
    /// if added to <c>AppDomain.CurrentDomain.UnhandledException</c>, makes all exceptions fancy
    /// </summary>
    private static void WriteUnhandled(object? sender, UnhandledExceptionEventArgs args) {
        if (args.ExceptionObject is Exception e and not FancyException<Exception>) {
            System.Console.Write(ToFancyString(e));
        }

        Environment.Exit(1);
    }
    
    /// <summary>
    /// stringifies an exception to a fancy one
    /// </summary>
    /// <param name="e">exception that will be thrown</param>
    public static string ToFancyString(Exception e) {
        System.Console.Write("\e[0m");
        string stringified = $"{e.GetType().Namespace!.AddColor(NamespaceColor)}." +
                             $"{e.GetType().Name.AddStyle(TextStyles.ITALIC).AddColor(ExceptionNameColor)}: " +
                             $"{e.Message.AddColor(MessageColor).AddStyle(TextStyles.BOLD)}\n";
        
        stringified += SplitStackTrace(e.StackTrace ?? "");

        if (!ShowHighlight) return $"{new string('\b', 21)}" + stringified + "\e[0m";

        stringified = stringified.Trim();
        
        string[] lines = stringified.Split('\n');
        string highlighted = $"{new string('\b', 21)}";
        
        foreach (var l in lines) {
            highlighted += "▍ ".AddColor(HighlightColor) + l + "\n";
        }

        return highlighted + "\e[0m";
    }

    /// <summary>
    /// probably colors the exception stack trace, no idea how it does that 
    /// </summary>
    private static string SplitStackTrace(string stackTrace) {
        if (stackTrace == "") return "";
        
        string[] stack = stackTrace.Split('\n');
        string output = "";
        
        foreach (var s in stack) {
            var m = Regex.Match(s, @"(?<=\.)[^.]*?(?=\()");
            
            string before = s.Substring(0, m.Index);
            before = before.Substring(0, 5).AddColor(ConsoleColor.DarkGray) +
                     before.Substring(5).AddColor(NamespaceColor)
                         .AddStyle((byte)(FaintNamespace ? TextStyles.FAINT : 0));
            
            string middle = m.Value.AddColor(MethodColor).AddStyle(TextStyles.BOLD);
            if (ItalicMethodName) middle = middle.AddStyle(TextStyles.ITALIC);

            string end = s.Substring(m.Index + m.Length) + "\n";
            int separate = end.IndexOf(") in", StringComparison.InvariantCulture);
            
            string end1 = end.Substring(0, separate + 4);
            string end2 = end.Substring(separate + 4);

            end1 = end1.Replace(") in", $") {"in".AddColor(ConsoleColor.DarkGray)}\n     ");
            end2 = end2.Trim();
            end2 = ColorFileName(end2);
            
            output += before + middle + end1 + " " + end2 + "\n";
        }
        
        return output.Trim();
    }
    
    private static string ColorFileName(string line) {
        string s = line.Replace(".cs:line", ".cs]:line");
        int endIndex = 0;
        int startIndex = 0;

        for (int i = 0; i < s.Length; i++) {
            if (s[i] == ']') {
                endIndex = i;
                break;
            }
        }

        for (int i = endIndex - 1; i >= 0; i--) {
            if (s[i] == '\\') {
                startIndex = i;
                break;
            }
        }

        string s1 = s.Substring(0, startIndex + 1).AddColor(PathColor)
            .AddStyle((byte)(FaintPath ? TextStyles.FAINT : 0));
        string s2 = s.Substring(startIndex + 1, endIndex - startIndex - 1).AddColor(FileNameColor)
            .AddStyle(TextStyles.BOLD);
        string s3 = " at line <b>".AddColor(ConsoleColor.DarkGray) +
                    s.Substring(endIndex + 7).AddColor(LineNumberColor) + "</b>";

        return s1 + s2 + s3.ApplyStyles();
    }
}