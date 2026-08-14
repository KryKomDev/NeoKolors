// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Console;

public record LogIndentationConfig {

    /// <summary>
    /// Gets the number of spaces used for indentation.
    /// </summary>
    public int? Indent { get; init; }

    /// <summary>
    /// Determines whether the first line of the message should be placed on a new line.
    /// Does not apply if the message is a single line.
    /// </summary>
    public bool IndentFirstLine { get; init; }

    /// <summary>
    /// Determines whether the first line of a console output block should always be indented,
    /// regardless of the message being a single line or not.
    /// </summary>
    public bool AlwaysIndentFirstLine { get; init; }

    /// <summary>
    /// Represents the configuration for log text indentation in a console logging system.
    /// </summary>
    public LogIndentationConfig(
        int  indent,
        bool indentFirstLine       = true,
        bool alwaysIndentFirstLine = false
    ) {
        Indent                = indent;
        IndentFirstLine       = indentFirstLine;
        AlwaysIndentFirstLine = alwaysIndentFirstLine;
    }
}