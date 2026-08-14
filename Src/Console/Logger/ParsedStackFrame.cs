// NeoKolors
// Copyright (c) krystof 2026

using System.Text.RegularExpressions;

namespace NeoKolors.Console;

/// <summary>
/// Represents a parsed stack trace frame containing structured information about the call site.
/// </summary>
public sealed class ParsedStackFrame {
    /// <summary>Gets the raw unparsed stack trace line.</summary>
    public string RawText { get; init; } = string.Empty;

    /// <summary>Gets the "at " prefix part.</summary>
    public string AtPart { get; init; } = "   at ";

    /// <summary>Gets the full declaring type string (namespace + class name).</summary>
    public string DeclaringType { get; init; } = string.Empty;

    /// <summary>Gets the namespace of the declaring type.</summary>
    public string Namespace { get; init; } = string.Empty;

    /// <summary>Gets the class name of the declaring type.</summary>
    public string TypeName { get; init; } = string.Empty;

    /// <summary>Gets the method name.</summary>
    public string MethodName { get; init; } = string.Empty;

    /// <summary>Gets the method parameter signature string.</summary>
    public string Parameters { get; init; } = string.Empty;

    /// <summary>Gets the source file path, if available.</summary>
    public string? FilePath { get; init; }

    /// <summary>Gets the source line number, if available.</summary>
    public int LineNumber { get; init; }

    /// <summary>Gets whether valid source file and line number information is present.</summary>
    public bool HasSourceInfo => !string.IsNullOrEmpty(FilePath) && LineNumber > 0;

    /// <summary>Gets whether this stack frame belongs to system or framework code.</summary>
    public bool IsSystemFrame { get; init; }

    private static readonly Regex FRAME_REGEX = new(
        @"^\s*(at\s+)?([^\(]+)(\([^\)]*\))(?:(?:\s+in\s+)(.+):(?:line\s+)?(\d+))?\s*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Parses a single stack trace line into a <see cref="ParsedStackFrame"/> instance.
    /// </summary>
    public static ParsedStackFrame Parse(string line) {
        if (string.IsNullOrWhiteSpace(line)) {
            return new ParsedStackFrame { RawText = line };
        }

        var match = FRAME_REGEX.Match(line);
        if (!match.Success) {
            return new ParsedStackFrame { RawText = line };
        }

        string atPart = string.IsNullOrEmpty(match.Groups[1].Value) ? "   at " : match.Groups[1].Value;
        string methodPart = match.Groups[2].Value.Trim();
        string argsPart = match.Groups[3].Value;
        string? pathPart = match.Groups[4].Success ? match.Groups[4].Value : null;
        int lineNumber = match.Groups[5].Success && int.TryParse(match.Groups[5].Value, out int l) ? l : 0;

        string declaringType = string.Empty;
        string methodName = methodPart;
        string ns = string.Empty;
        string typeName = string.Empty;

        int lastDot = methodPart.LastIndexOf('.');
        if (lastDot != -1) {
            declaringType = methodPart.Substring(0, lastDot);
            methodName = methodPart.Substring(lastDot + 1);

            int lastDeclaringDot = declaringType.LastIndexOf('.');
            if (lastDeclaringDot != -1) {
                ns = declaringType.Substring(0, lastDeclaringDot);
                typeName = declaringType.Substring(lastDeclaringDot + 1);
            } else {
                typeName = declaringType;
            }
        }

        bool isSystem = !string.IsNullOrEmpty(ns) && (
            ns.StartsWith("System", StringComparison.Ordinal) ||
            ns.StartsWith("Microsoft", StringComparison.Ordinal) ||
            ns.StartsWith("Internal", StringComparison.Ordinal) ||
            ns.StartsWith("Mono", StringComparison.Ordinal)
        );

        return new ParsedStackFrame {
            RawText = line,
            AtPart = atPart,
            DeclaringType = declaringType,
            Namespace = ns,
            TypeName = typeName,
            MethodName = methodName,
            Parameters = argsPart,
            FilePath = pathPart,
            LineNumber = lineNumber,
            IsSystemFrame = isSystem
        };
    }
}
