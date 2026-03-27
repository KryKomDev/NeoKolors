//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Diagnostics.CodeAnalysis;
using NeoKolors.Extensions;

namespace NeoKolors.Console.Driver;

public interface IOutputDriver : IDisposable {
    
    /// <summary>
    /// Writes the specified span of characters to the output.
    /// </summary>
    /// <param name="value">The span of characters to write.</param>
    public void Write(ReadOnlySpan<char> value);

    /// <summary>
    /// Writes the specified string value to the output.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public void Write(string value) => Write(value.AsSpan());

    /// <summary>
    /// Writes the specified formatted string along with its arguments to the output.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">An array of objects to write using the format string.</param>
    public void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object[] args) => 
        Write(string.Format(format, args));

    /// <summary>
    /// Writes the specified span of characters followed by a new line to the output.
    /// </summary>
    /// <param name="value">The span of characters to write followed by a new line.</param>
    public void WriteLine(ReadOnlySpan<char> value) => Write(value.Concat(Environment.NewLine));
    
    /// <summary>
    /// Writes the specified string value followed by a new line to the output.
    /// </summary>
    /// <param name="value">The string to write followed by a new line.</param>
    public void WriteLine(string value) => Write(value + Environment.NewLine);

    /// <summary>
    /// Writes the specified string value followed by a new line to the output.
    /// </summary>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">An array of objects to write using the format string.</param>
    public void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args) => 
        WriteLine(string.Format(format, args));

    /// <summary>
    /// Writes the specified object value followed by a new line to the output.
    /// </summary>
    /// <param name="value">The object to write followed by a new line.
    /// If the value is null, an empty string is written.</param>
    public void WriteLine(object? value) => WriteLine(value?.ToString() ?? string.Empty);
}