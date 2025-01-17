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
    public static void Throw(Exception e) {
        string output = $"\n{e.GetType().Namespace}." +
                        $"{e.GetType().Name.AddColor(IsTerminalPaletteSafe ? ConsoleColor.Yellow : WarnColor)}: " +
                        $"{("<b>" + e.Message.AddColor(IsTerminalPaletteSafe ? ConsoleColor.Red : ErrorColor) + "</b>").AddTextStyles()}\n";
        
        output += SplitStackTrace(e.StackTrace ?? "");
        
        PrintException(output);
    }
    
    private static void PrintException(string output) {
        string[] lines = output.Split('\n');
        
        foreach (var l in lines) {
            ConsoleColors.PrintColored("▍ ", IsTerminalPaletteSafe ? ConsoleColor.DarkRed : FatalColor);
            System.Console.WriteLine(l);
        }
    }

    private static string SplitStackTrace(string stackTrace) {
        string[] stack = stackTrace.Split('\n');
        string output = "";
        
        foreach (var s in stack) {
            var m = Regex.Match(s, @"(?<=\.)[^.]*?(?=\()");
            
            string before = s.Substring(0, m.Index);
            before = before.Substring(0, 5).AddColor(ConsoleColor.DarkGray) + before.Substring(5);
            
            string middle = m.Value.AddColor(IsTerminalPaletteSafe ? ConsoleColor.Blue : DebugColor);

            string end = s.Substring(m.Index + m.Length) + "\n";
            int separate = end.IndexOf(") in", StringComparison.InvariantCulture);
            
            string end1 = end.Substring(0, separate + 4);
            string end2 = end.Substring(separate + 4);

            end1 = end1.Replace(") in", $") {"in".AddColor(ConsoleColor.DarkGray)}\n     ");
            end2 = ColorFileName(end2);
            
            output += before + middle + end1 + end2;
        }
        
        return output;
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

        string s1 = s.Substring(0, startIndex + 1);
        string s2 = s.Substring(startIndex + 1, endIndex - startIndex - 1).AddColor(IsTerminalPaletteSafe ? ConsoleColor.Blue : DebugColor);
        string s3 = " at line ".AddColor(ConsoleColor.DarkGray) + s.Substring(endIndex + 7).AddColor(IsTerminalPaletteSafe ? ConsoleColor.Green : InfoColor);

        return s1 + s2 + s3;
    }
}