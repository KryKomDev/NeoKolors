// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Ansi;

public abstract partial class AnsiParser {
    
    /// <summary>
    /// Determines whether there is input data available to be read without removing it
    /// from the input source.
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if input can be read; otherwise, <c>false</c>.
    /// </returns>
    public abstract bool Peek();

    /// <summary>
    /// Reads the next input event, capturing information such as the pressed key, associated character,
    /// and any active modifiers.
    /// </summary>
    /// <returns>
    /// A <see cref="KeyEventArgs"/> instance containing details about the input event.
    /// </returns>
    public abstract KeyEventArgs Read();

    /// <summary>
    /// Reads the next character available from the input source.
    /// </summary>
    /// <returns>
    /// The next character from the input source.
    /// </returns>
    public virtual char ReadChar() => Read().Char;

    /// <summary>
    /// Releases the queue of input that has been collected via the <see cref="ReadChar"/> method.
    /// </summary>
    /// <param name="success">If true, the queue should be cleared without calling any event handlers.
    /// Otherwise, a relevant event handler should be called for every enqueued input record.</param>
    public abstract void ReleaseRead(bool success);

    /// <summary>
    /// Attempts to read the next input character from the input source.
    /// </summary>
    /// <param name="result">When this method returns, contains the character read from the input
    /// source if successful; otherwise, contains the default character value ('\0').</param>
    /// <returns>
    /// Returns <c>true</c> if the next character was successfully read; otherwise, <c>false</c>.
    /// </returns>
    protected bool TryReadNext(out char result) {
        var s = Stopwatch.StartNew();
        
        while (GetContinue()) {
            if (Peek()) {
                result = ReadChar();
                return true;
            }

            Thread.Sleep(_retryInterval);
        }
        
        result = '\0';
        return false;

        bool GetContinue() {
            #if NK_DISABLE_ANSI_TIMEOUT
            return true;
            #else
            return s.Elapsed < _timeout;
            #endif
        }
    }

    /// <summary>
    /// Attempts to read input characters from the input source until any of the specified terminator
    /// characters is encountered. The method stops reading upon reaching one of the terminators
    /// or if no further input is available.
    /// </summary>
    /// <param name="result">When this method returns, contains the string of characters read from the input
    /// source, INCLUDING the terminator character, if successful; otherwise, contains <c>null</c>.</param>
    /// <param name="terminator">An array of characters that indicate where to stop reading from the input source.</param>
    /// <returns>
    /// Returns <c>true</c> if characters were successfully read up to a terminator; otherwise, <c>false</c>.
    /// </returns>
    protected bool TryReadUntil([NotNullWhen(returnValue: true)] out string? result, params char[] terminator) {
        var sb = new StringBuilder();
        var ts = terminator.ToHashSet();

        var success = TryReadNext(out var read);

        while (success && !ts.Contains(read)) {
            sb.Append(read);
            success = TryReadNext(out read);
        }

        if (!success) {
            result = null;
            return false;
        }

        sb.Append(read);
        result = sb.ToString();
        return true;
    }
}