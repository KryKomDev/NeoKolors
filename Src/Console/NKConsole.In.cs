//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Exceptions;
using OneOf;
using static NeoKolors.Console.BoolStrings;
using ArgumentException = System.ArgumentException;
using FormatException = System.FormatException;
using InvalidOperationException = System.InvalidOperationException;
using OverflowException = System.OverflowException;

namespace NeoKolors.Console;

public static partial class NKConsole {

    #region String Read Overrides

    /// <summary>
    /// Asynchronously waits for a key press event and returns the corresponding key event information.
    /// Ensures the input driver remains in a consistent state during this operation.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the details of the key event,
    /// including the pressed key, its modifiers, and other associated data.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if the input driver encounters an unexpected state or fails during the operation.
    /// </exception>
    public static async Task<KeyEventArgs> ReadKeyAsync(bool intercept = false) {
        // use standard console reading if the input driver is not running
        if (!InputDriver.IsRunning) {
            if (Stdio.IsInputRedirected) {
                int ch = Stdio.Read();
                if (ch == -1)
                    return default;

                char c = (char)ch;
                var key = c switch {
                    '\r' => ConsoleKey.Enter,
                    '\n' => ConsoleKey.Enter,
                    '\t' => ConsoleKey.Tab,
                    '\b' => ConsoleKey.Backspace,
                    _    => (ConsoleKey)c
                };

                return new KeyEventArgs(new ConsoleKeyInfo(c, key, false, false, false));
            }

            return new KeyEventArgs(Stdio.ReadKey(intercept));
        }

        var tcs = new TaskCompletionSource<KeyEventArgs>();

        KeyEventHandler? handler = null;

        handler = k => {
            InputDriver.Key -= handler;
            tcs.SetResult(k);
        };

        // await the key press event
        var result = await tcs.Task;

        if (!intercept)
            OutputDriver.Write(result.Char);

        return result;
    }

    /// <summary>
    /// Synchronously waits for a key press event and retrieves the corresponding key event information.
    /// Ensures the input driver remains in a consistent state during this operation.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="KeyEventArgs"/> containing details about the key event,
    /// including the pressed key, its modifiers, and other associated data.
    /// </returns>
    /// <exception cref="System.AggregateException">
    /// Thrown if the asynchronous underlying operation encounters an exception.
    /// </exception>
    /// <remarks>
    /// This method blocks the calling thread until a key is pressed. For non-blocking behavior,
    /// use the asynchronous version <see cref="ReadKeyAsync"/>.
    /// </remarks>
    public static KeyEventArgs ReadKey(bool intercept = false) {
        return ReadKeyAsync(intercept).Result;
    }

    /// <summary>
    /// Asynchronously waits for a character input from the user and returns the corresponding character.
    /// The method relies on the underlying keyboard event handling to capture user input.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the character entered by the user.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if the input driver encounters an unexpected state or fails during the operation.
    /// </exception>
    public static async Task<char> ReadCharAsync(bool intercept = false) => (await ReadKeyAsync(intercept)).Char;

    /// <summary>
    /// Reads a single character input from the console synchronously.
    /// This method blocks until a character is entered and returns the corresponding character.
    /// </summary>
    /// <returns>
    /// The character input by the user.
    /// </returns>
    public static char ReadChar(bool intercept = false) => ReadKey(intercept).Char;

    /// <summary>
    /// Asynchronously reads a line of text entered by the user until the Enter/Return key is pressed.
    /// The operation processes each character input in sequence and constructs the resulting string.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the entire line of text
    /// entered by the user, excluding the trailing newline character.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if the input driver fails to initialize or encounters an error during the operation.
    /// </exception>
    public static async Task<string> ReadLineAsync(bool intercept = false) {
        var sb = new StringBuilder();

        // use standard console reading if the input driver is not running
        if (!InputDriver.IsRunning) {
            if (Stdio.IsInputRedirected) {
                while (true) {
                    var c = Stdio.Read();
                
                    if (c is -1 or '\n')
                        return sb.ToString();
                
                    sb.Append((char)c);
                }
            }
            
            while (true) {
                var k = Stdio.ReadKey(true);
                    
                switch (k.Key) {
                    case ConsoleKey.Enter: {
                        return sb.ToString();
                    }
                    case ConsoleKey.Backspace when sb.Length > 0: {
                        sb.Remove(sb.Length - 1, 1); 
                        break;
                    }
                }

                sb.Append(k.KeyChar);
            }
        }

        var tcs = new TaskCompletionSource();

        // make sure the input driver does not change while waiting for the key press
        var inputDriver = InputDriver;
        var wasRunning  = inputDriver.IsRunning;

        // start the input driver if it's not already running
        if (!wasRunning)
            inputDriver.Start();

        inputDriver.Key += AppendChar;

        // wait for the line to be completed
        await tcs.Task;

        // restore the original state of the input driver
        if (!wasRunning)
            inputDriver.Stop();

        return sb.ToString();

        // --- Local Functions --- //

        void AppendChar(KeyEventArgs key) {
            // ignore key presses that are not part of the line
            if (key.Up)
                return;

            // print the character to the console if not intercepted
            if (!intercept)
                OutputDriver.Write(key.Char);

            if (key.Key == KeyCode.RETURN) {
                tcs.SetResult();
            }
            else if (key.Key == KeyCode.BACKSPACE && sb.Length > 0) {
                sb.Remove(sb.Length - 1, 1);
            }
            else {
                sb.Append(key.Char);
            }
        }
    }

    /// <summary>
    /// Reads a line of text input synchronously from the console until a newline character is entered.
    /// Blocks the calling thread until the user completes their input.
    /// </summary>
    /// <returns>
    /// The string representing the entire line of text entered by the user, excluding the terminating newline character.
    /// </returns>
    /// <exception cref="System.AggregateException">
    /// Thrown if the underlying asynchronous operation encounters an exception.
    /// </exception>
    public static string ReadLine(bool intercept = false) => ReadLineAsync(intercept).Result;

    /// <summary>
    /// Reads a line of input from the console, invoking a callback action for each key press.
    /// This method processes input character by character until the return key is pressed.
    /// </summary>
    /// <param name="onKeyPress">
    /// A callback function invoked for each key press event. The function receives a <see cref="KeyEventArgs"/> object
    /// containing information about the key event.
    /// </param>
    /// <param name="intercept">
    /// A boolean value indicating whether the pressed key character should be visible in the console. If set to <c>true</c>,
    /// characters are not displayed in the console; otherwise, they are displayed.
    /// </param>
    /// <returns>
    /// A string containing the characters entered by the user, excluding the return key.
    /// </returns>
    public static string ReadLine(Action<KeyEventArgs> onKeyPress, bool intercept = false) {
        var sb = new StringBuilder();

        KeyEventArgs key;
        
        do {
            // read the key press
            key = ReadKey(intercept);
            
            // invoke the action
            onKeyPress(key);
            
            // append the character to the string builder if not intercepted
            sb.Append(key.Char);
        }
        while (key.Key != KeyCode.RETURN);
        
        WriteLine();
        
        return sb.ToString();
    }
    
    #endregion

    #region Primitives Input Dialogue

    /// <summary>
    /// Reads a 64-bit integer value from the console and validates it using an optional validator function.
    /// Continues prompting the user until a valid input is provided or an exception occurs.
    /// </summary>
    /// <param name="validator">
    /// A function that accepts an integer and returns a string message if the input is invalid,
    /// or null if it is valid. If no validator is provided, all integer inputs are considered valid.
    /// </param>
    /// <param name="reply">If on the program will write a response to invalid inputs.</param>
    /// <param name="style">The style of the invalid input messages.</param>
    /// <returns>The validated integer input provided by the user.</returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if no valid integer input is provided after repeated prompts or if validation fails.
    /// </exception>
    public static long ReadLong(Func<long, string?>? validator = null, bool reply = true, NKStyle style = default) {
        validator ??= _ => null;
        style     =   style == default ? new NKStyle(NKColor.Default, NKColor.Default) : style;

        while (true) {
            string? s = Stdio.ReadLine();

            if (s == null) {
                if (reply)
                    WriteLine("Invalid input.", style);

                continue;
            }

            long i;

            try {
                i = long.Parse(s);
            }
            catch (FormatException) {
                if (reply)
                    WriteLine("Invalid input. Please enter a valid integer.", style);

                continue;
            }
            catch (OverflowException) {
                if (reply)
                    WriteLine("Input overflow. Please enter a valid integer.", style);

                continue;
            }

            var res = validator(i);

            if (res == null)
                return i;

            if (reply)
                WriteLine(res, style);
        }
    }

    /// <summary>
    /// Reads a 32-bit integer value from the console and validates it using an optional validator function.
    /// Continues prompting the user until a valid input is provided or an exception occurs.
    /// </summary>
    /// <param name="validator">
    /// A function that accepts an integer and returns a string message if the input is invalid,
    /// or null if it is valid. If no validator is provided, all integer inputs are considered valid.
    /// </param>
    /// <param name="reply">If on the program will write a response to invalid inputs.</param>
    /// <param name="style">The style of the invalid input messages.</param>
    /// <returns>The validated integer input provided by the user.</returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if no valid integer input is provided after repeated prompts or if validation fails.
    /// </exception>
    public static int ReadInt(Func<int, string?>? validator = null, bool reply = true, NKStyle style = default) {
        validator ??= _ => null;
        style     =   style == default ? new NKStyle(NKColor.Default, NKColor.Default) : style;

        while (true) {
            string? s = Stdio.ReadLine();

            if (s == null) {
                if (reply)
                    WriteLine("Invalid input.", style);

                continue;
            }

            int i;

            try {
                i = int.Parse(s);
            }
            catch (FormatException) {
                if (reply)
                    WriteLine("Invalid input. Please enter a valid integer.", style);

                continue;
            }
            catch (OverflowException) {
                if (reply)
                    WriteLine("Input overflow. Please enter a valid integer.", style);

                continue;
            }

            var res = validator(i);

            if (res == null)
                return i;

            if (reply)
                WriteLine(res, style);
        }
    }

    /// <summary>
    /// Reads a 16-bit integer value from the console and validates it using an optional validator function.
    /// Continues prompting the user until a valid input is provided or an exception occurs.
    /// </summary>
    /// <param name="validator">
    /// A function that accepts an integer and returns a string message if the input is invalid,
    /// or null if it is valid. If no validator is provided, all integer inputs are considered valid.
    /// </param>
    /// <param name="reply">If on the program will write a response to invalid inputs.</param>
    /// <param name="style">The style of the invalid input messages.</param>
    /// <returns>The validated integer input provided by the user.</returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if no valid integer input is provided after repeated prompts or if validation fails.
    /// </exception>
    public static short ReadShort(Func<short, string?>? validator = null, bool reply = true, NKStyle style = default) {
        validator ??= _ => null;
        style     =   style == default ? new NKStyle(NKColor.Default, NKColor.Default) : style;

        while (true) {
            string? s = Stdio.ReadLine();

            if (s == null) {
                if (reply)
                    WriteLine("Invalid input.", style);

                continue;
            }

            short i;

            try {
                i = short.Parse(s);
            }
            catch (FormatException) {
                if (reply)
                    WriteLine("Invalid input. Please enter a valid integer.", style);

                continue;
            }
            catch (OverflowException) {
                if (reply)
                    WriteLine("Input overflow. Please enter a valid integer.", style);

                continue;
            }

            var res = validator(i);

            if (res == null)
                return i;

            if (reply)
                WriteLine(res, style);
        }
    }

    /// <summary>
    /// Reads an 8-bit integer value from the console and validates it using an optional validator function.
    /// Continues prompting the user until a valid input is provided or an exception occurs.
    /// </summary>
    /// <param name="validator">
    /// A function that accepts an integer and returns a string message if the input is invalid,
    /// or null if it is valid. If no validator is provided, all integer inputs are considered valid.
    /// </param>
    /// <param name="reply">If on the program will write a response to invalid inputs.</param>
    /// <param name="style">The style of the invalid input messages.</param>
    /// <returns>The validated integer input provided by the user.</returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if no valid integer input is provided after repeated prompts or if validation fails.
    /// </exception>
    public static byte ReadByte(Func<byte, string?>? validator = null, bool reply = true, NKStyle style = default) {
        validator ??= _ => null;
        style     =   style == default ? new NKStyle(NKColor.Default, NKColor.Default) : style;

        while (true) {
            string? s = Stdio.ReadLine();

            if (s == null) {
                if (reply)
                    WriteLine("Invalid input.", style);

                continue;
            }

            byte i;

            try {
                i = byte.Parse(s);
            }
            catch (FormatException) {
                if (reply)
                    WriteLine("Invalid input. Please enter a valid integer.", style);

                continue;
            }
            catch (OverflowException) {
                if (reply)
                    WriteLine("Input overflow. Please enter a valid integer.", style);

                continue;
            }

            var res = validator(i);

            if (res == null)
                return i;

            if (reply)
                WriteLine(res, style);
        }
    }

    /// <summary>
    /// Reads a string value from the console and validates it using an optional validator function.
    /// Prompts the user until a valid string is provided or an exception occurs.
    /// </summary>
    /// <param name="validator">
    /// A function that accepts a string and returns a message if the input is invalid,
    /// or null if the input is valid. If no validator is provided, all string inputs are considered valid.
    /// </param>
    /// <param name="reply">If on the program will write a response to invalid inputs.</param>
    /// <param name="style">The style of the invalid input messages.</param>
    /// <returns>The validated string input provided by the user.</returns>
    public static string ReadString(Func<string, string?>? validator = null, bool reply = true, NKStyle? style = null) {
        validator ??= _ => null;
        style     ??= new NKStyle(NKColor.Default, NKColor.Default);

        while (true) {
            string? s = Stdio.ReadLine();

            if (s == null) {
                if (reply)
                    WriteLine("Invalid input.", style.Value);

                continue;
            }

            var res = validator(s);

            if (res == null)
                return s;

            if (reply)
                WriteLine(res, style.Value);
        }
    }

    /// <summary>
    /// Reads a string value from the console and validates it using an optional validator function.
    /// Prompts the user until a valid string is provided or an exception occurs.
    /// </summary>
    /// <param name="boolStrings">
    /// Controls what strings are allowed as input.
    /// Every case variation of a valid string is also considered valid
    /// </param>
    /// <param name="reply">If on the program will write a response to invalid inputs.</param>
    /// <param name="style">The style of the invalid input messages.</param>
    /// <returns>The validated string input provided by the user.</returns>
    public static bool ReadBool(BoolStrings boolStrings = ALL, bool reply = true, NKStyle? style = null) {
        style ??= new NKStyle(NKColor.Default, NKColor.Default);

        while (true) {
            string? s = Stdio.ReadLine();

            if (s == null) {
                if (reply)
                    WriteLine("Invalid input.", style.Value);

                continue;
            }

            var res = ParseBool(s, boolStrings);

            if (res.IsT0)
                return res.AsT0;

            if (reply)
                WriteLine(res.AsT1, style.Value);
        }
    }

    [Pure]
    [JetBrains.Annotations.Pure]
    private static OneOf<bool, string> ParseBool(string s, BoolStrings b) {
        return s.ToLower() switch {
            "true"  => b.GetHasTrueFalse() ? true : InvBoolMsg(b),
            "false" => b.GetHasTrueFalse() ? false : InvBoolMsg(b),
            "t"     => b.GetHasTF() ? true : InvBoolMsg(b),
            "f"     => b.GetHasTF() ? false : InvBoolMsg(b),
            "yes"   => b.GetHasYesNo() ? true : InvBoolMsg(b),
            "no"    => b.GetHasYesNo() ? false : InvBoolMsg(b),
            "y"     => b.GetHasYN() ? true : InvBoolMsg(b),
            "n"     => b.GetHasYN() ? false : InvBoolMsg(b),
            "on"    => b.GetHasOnOff() ? true : InvBoolMsg(b),
            "off"   => b.GetHasOnOff() ? false : InvBoolMsg(b),
            "1"     => b.GetHasZeroOne() ? true : InvBoolMsg(b),
            "0"     => b.GetHasZeroOne() ? false : InvBoolMsg(b),
            _       => InvBoolMsg(b)
        };
    }

    [Pure]
    [JetBrains.Annotations.Pure]
    private static string InvBoolMsg(BoolStrings b) => $"Invalid input. Please enter a valid boolean. (Allowed: {ToString(b)})";

    [Pure]
    [JetBrains.Annotations.Pure]
    private static string ToString(this BoolStrings b) => b is ALL ? "true, false, yes, no, y, n, on, off, t, f, 1, 0" : b.ToString().ToLower().Replace('_', ',');

    #endregion

    #region INPUT INTERCEPTION

    /// <summary>
    /// Activates the input interception mechanism and initiates the input handling thread.
    /// Once initiated, the console will begin to listen for user inputs such as keys, mouse events,
    /// and other interactions, enabling dynamic response handling in terminal applications.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Input interception is already enabled.
    /// </exception>
    public static void StartInputInterception() {
        if (InterceptInput)
            throw new InvalidOperationException("Input interception is already enabled.");

        InterceptInput = true;
        LOGGER.Info("Starting input interception...");

        InputDriver.Key      += OnKey;
        InputDriver.Mouse    += OnMouse;
        InputDriver.FocusIn        += OnFocusIn;
        InputDriver.FocusOut       += OnFocusOut;
        InputDriver.Paste          += OnPaste;
        InputDriver.VTQuery        += OnVTQuery;
        InputDriver.VTQueryTimeout += OnVTQueryTimeout;

        InputDriver.Start();
    }

    /// <summary>
    /// Stops the interception of input by setting the internal state to false.
    /// This is used to cease capturing or processing input events, thereby
    /// restoring the default behavior where input is no longer intercepted.
    /// </summary>
    public static void StopInputInterception() {
        InterceptInput = false;
        LOGGER.Info("Stopping input interception...");

        InputDriver.Stop();

        InputDriver.Key            -= OnKey;
        InputDriver.Mouse          -= OnMouse;
        InputDriver.FocusIn        -= OnFocusIn;
        InputDriver.FocusOut       -= OnFocusOut;
        InputDriver.Paste          -= OnPaste;
        InputDriver.VTQuery        -= OnVTQuery;
        InputDriver.VTQueryTimeout -= OnVTQueryTimeout;
    }

    private static void OnKey(KeyEventArgs     k)                                 => Key(k);
    private static void OnMouse(MouseEventArgs m)                                 => Mouse(m);
    private static void OnFocusIn()                                               => FocusIn();
    private static void OnFocusOut()                                              => FocusOut();
    private static void OnPaste(string    s)                                      => Paste(s);
    private static void OnVTQuery(VTQuery r)                                      => VTQueryResponse(r);
    private static void OnVTQueryTimeout(VTQuery r, AnsiParser.ParserResult reason) => VTQueryTimeout(r, reason);

    #endregion

    /// <summary>
    /// Reads input from the console until a specified sequence of characters is detected.
    /// Returns the input content excluding the given sequence.
    /// </summary>
    /// <param name="sequence">
    /// A string representing the sequence of characters that marks the end of the input.
    /// The input reading stops as soon as this sequence is detected.
    /// </param>
    /// <param name="intercept">
    /// A boolean value indicating whether the input characters should be intercepted
    /// (i.e., not displayed in the console) while they are being typed.
    /// Defaults to false.
    /// </param>
    /// <returns>
    /// A string containing all characters read from the console, excluding the specified sequence.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Thrown if the provided sequence is null or an empty string.
    /// </exception>
    public static string ReadUntil(string sequence, bool intercept = false) {
        if (string.IsNullOrEmpty(sequence)) {
            throw new ArgumentException("Sequence cannot be null or empty.", nameof(sequence));
        }

        var buffer        = new StringBuilder();
        var sequenceIndex = 0;

        while (true) {
            var key = Stdio.ReadKey(intercept);
            var ch  = key.KeyChar;

            buffer.Append(ch);

            // Check if the current character matches the expected character in the sequence
            if (ch == sequence[sequenceIndex]) {
                sequenceIndex++;

                // If the entire sequence matched, return the content before it
                if (sequenceIndex != sequence.Length)
                    continue;

                // Remove the sequence from the end of the buffer
                buffer.Length -= sequence.Length;

                return buffer.ToString();
            }

            // Reset sequence matching if the character doesn't match
            sequenceIndex = 0;

            // Check if the current character is the start of the sequence
            if (ch == sequence[0]) {
                sequenceIndex = 1;
            }
        }
    }

    private static bool TryReadKey(out ConsoleKeyInfo key, TimeSpan timeout, bool intercept, CancellationToken cancellationToken = default) {
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed < timeout) {
            cancellationToken.ThrowIfCancellationRequested();

            if (Stdio.PeekU()) {
                var k = Stdio.ReadKeyU(intercept);
                if (k.HasValue) {
                    key = k.Value;
                    return true;
                }
            }

            Thread.Sleep(1);
        }

        key = default;
        return false;
    }

    /// <summary>
    ///     Attempts to read characters from the console until any of the specified delimiter characters is encountered,
    ///     or until the specified timeout expires or cancellation is requested.
    /// </summary>
    public static bool TryReadUntil(
        [NotNullWhen(true)] out string? result,
        out char                        last,
        TimeSpan                        timeout,
        bool                            intercept,
        CancellationToken               cancellationToken,
        params char[]                   oneOf
    ) {
        var s  = new StringBuilder();
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed < timeout) {
            cancellationToken.ThrowIfCancellationRequested();
            var remaining = timeout - sw.Elapsed;
            if (remaining <= TimeSpan.Zero)
                break;

            if (!TryReadKey(out var key, remaining, intercept, cancellationToken))
                break;

            if (oneOf.Contains(key.KeyChar)) {
                last   = key.KeyChar;
                result = s.ToString();
                return true;
            }

            s.Append(key.KeyChar);
        }

        last   = ' ';
        result = null;
        return false;
    }

    /// <summary>
    ///     Attempts to read characters from the console until any of the specified delimiter characters is encountered,
    ///     or until the specified timeout expires.
    /// </summary>
    public static bool TryReadUntil(
        [NotNullWhen(true)] out string? result,
        out char                        last,
        TimeSpan                        timeout,
        bool                            intercept = false,
        params char[]                   oneOf
    ) => TryReadUntil(out result, out last, timeout, intercept, default, oneOf);

    /// <summary>
    ///     Reads characters from the console until any of the specified delimiter characters is encountered,
    ///     or throws a <see cref="VTQueryTimeoutException"/> if the timeout expires.
    /// </summary>
    public static string ReadUntil(
        out char          last,
        TimeSpan          timeout,
        bool              intercept,
        CancellationToken cancellationToken,
        params char[]     oneOf
    ) {
        if (TryReadUntil(out var res, out last, timeout, intercept, cancellationToken, oneOf))
            return res;

        throw new VTQueryTimeoutException($"Timed out after {timeout.TotalMilliseconds:F0}ms waiting for console input matching '{string.Join(", ", oneOf)}'.");
    }

    /// <summary>
    ///     Reads characters from the console until any of the specified delimiter characters is encountered,
    ///     or throws a <see cref="VTQueryTimeoutException"/> if the timeout expires.
    /// </summary>
    public static string ReadUntil(out char last, TimeSpan timeout, bool intercept = false, params char[] oneOf) =>
        ReadUntil(out last, timeout, intercept, default, oneOf);

    /// <summary>
    ///     Reads characters from the console until any of the specified delimiter characters is encountered.
    /// </summary>
    public static string ReadUntil(out char last, bool intercept = false, params char[] oneOf) =>
        ReadUntil(out last, DefaultQueryTimeout, intercept, oneOf);

    /// <summary>
    ///     Reads characters from the console until the specified character is encountered,
    ///     or throws a <see cref="VTQueryTimeoutException"/> if the timeout expires.
    /// </summary>
    public static string ReadUntil(char c, TimeSpan timeout, bool intercept = false) =>
        ReadUntil(out _, timeout, intercept, c);

    /// <summary>
    ///     Reads characters from the console until the specified character is encountered.
    /// </summary>
    public static string ReadUntil(char c, bool intercept = false) =>
        ReadUntil(out _, DefaultQueryTimeout, intercept, c);

    /// <summary>
    ///     Returns true if the alternate console buffer is enabled.
    /// </summary>
    public static bool GetAltBufState(TimeSpan? timeout = null) => GetAltBufStateAsync(timeout).Result;

    /// <summary>
    ///     Returns true if the alternate console buffer is enabled.
    /// </summary>
    public static async Task<bool> GetAltBufStateAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) {
        cancellationToken.ThrowIfCancellationRequested();
        var effectiveTimeout = timeout ?? DefaultQueryTimeout;
        var query            = VTQuery.RequestDec(EscapeCodes.DecMode.ALTERNATE_BUFFER);

        if (!InterceptInput) {
            OutputDriver.Write(EscapeCodes.REQUEST_ALTBUF_STATE);
            if (!TryReadUntil(out var res, out _, effectiveTimeout, true, cancellationToken, 'y'))
                throw new VTQueryTimeoutException(in query, effectiveTimeout);

            return res.Length > 8 && res[8] == '1';
        }

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(effectiveTimeout);

        VTQueryResponseHandler? h  = null;
        VTQueryTimeoutHandler?  th = null;

        void Cleanup() {
            VTQueryResponse -= h;
            VTQueryTimeout  -= th;
        }

        h = a => {
            if (a.Type != VTQueryType.DEC || a.DecMode != EscapeCodes.DecMode.ALTERNATE_BUFFER)
                return;

            Cleanup();
            tcs.TrySetResult(a.DecResponse == DecReqResponseType.ENABLED);
        };

        th = (req, reason) => {
            if (req.Matches(query)) {
                Cleanup();
                tcs.TrySetException(new VTQueryTimeoutException(in query, effectiveTimeout, $"VTQuery failed: {reason}"));
            }
        };

        VTQueryResponse += h;
        VTQueryTimeout  += th;
        InputDriver.RequestVTQuery(query);

        await using var reg = cts.Token.Register(() => {
            Cleanup();
            InputDriver.CancelVTQuery(query);
            if (cancellationToken.IsCancellationRequested)
                tcs.TrySetCanceled(cancellationToken);
            else
                tcs.TrySetException(new VTQueryTimeoutException(in query, effectiveTimeout));
        });

        return await tcs.Task;
    }

    /// <summary>
    ///     Retrieves the current size of the console window in pixels as a tuple containing the width and height.
    /// </summary>
    public static Size2D GetBuffSizePx(TimeSpan? timeout = null) => GetBuffSizePxAsync(timeout).Result;

    /// <summary>
    ///     Retrieves the current size of the console window in pixels as a tuple containing the width and height.
    /// </summary>
    public static async Task<Size2D> GetBuffSizePxAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        await GetWinReqAsync(EscapeCodes.REPORT_BUFF_SIZE_PX, EscapeCodes.WinOpts.BUFF_SIZE_PX, timeout, cancellationToken);

    /// <summary>
    ///     Retrieves the current character cell size in pixels as a tuple containing width and height.
    /// </summary>
    public static Size2D GetCellSizePx(TimeSpan? timeout = null) => GetCellSizePxAsync(timeout).Result;

    /// <summary>
    ///     Retrieves the current character cell size in pixels as a tuple containing width and height.
    /// </summary>
    public static async Task<Size2D> GetCellSizePxAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        await GetWinReqAsync(EscapeCodes.REPORT_CHAR_CELL_SIZE_PX, EscapeCodes.WinOpts.CHAR_CELL_SIZE_PX, timeout, cancellationToken);

    /// <summary>
    ///     Retrieves the current buffer size in characters as a tuple containing width and height.
    /// </summary>
    public static Size2D GetBuffSizeCh(TimeSpan? timeout = null) => GetBuffSizeChAsync(timeout).Result;

    /// <summary>
    ///     Retrieves the current buffer size in characters as a tuple containing width and height.
    /// </summary>
    public static async Task<Size2D> GetBuffSizeChAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        await GetWinReqAsync(EscapeCodes.REPORT_BUFF_SIZE_CH, EscapeCodes.WinOpts.BUFF_SIZE_CH, timeout, cancellationToken);

    private static async Task<Size2D> GetWinReqAsync(
        string                  escSeq,
        EscapeCodes.WinOpts     type,
        TimeSpan?               timeout           = null,
        CancellationToken       cancellationToken = default
    ) {
        cancellationToken.ThrowIfCancellationRequested();
        var effectiveTimeout = timeout ?? DefaultQueryTimeout;
        var query            = VTQuery.RequestWin(type);

        if (!InterceptInput) {
            OutputDriver.Write(escSeq);
            if (!TryReadUntil(out var res, out var last, effectiveTimeout, true, cancellationToken, 't'))
                throw new VTQueryTimeoutException(in query, effectiveTimeout);

            var split = res.Split(';');
            if (split.Length < 3)
                throw new InvalidOperationException($"Invalid window request response: {res}");

            return new Size2D(int.Parse(split[2]), int.Parse(split[1]));
        }

        var tcs = new TaskCompletionSource<Size2D>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(effectiveTimeout);

        VTQueryResponseHandler? h  = null;
        VTQueryTimeoutHandler?  th = null;

        void Cleanup() {
            VTQueryResponse -= h;
            VTQueryTimeout  -= th;
        }

        h = a => {
            if (a.Type != VTQueryType.WIN || a.WinMode != type)
                return;

            Cleanup();
            var res = a.WinResponse;
            tcs.TrySetResult(new Size2D(res.X, res.Y));
        };

        th = (req, reason) => {
            if (req.Matches(query)) {
                Cleanup();
                tcs.TrySetException(new VTQueryTimeoutException(in query, effectiveTimeout, $"VTQuery failed: {reason}"));
            }
        };

        VTQueryResponse += h;
        VTQueryTimeout  += th;
        InputDriver.RequestVTQuery(query);

        await using var reg = cts.Token.Register(() => {
            Cleanup();
            InputDriver.CancelVTQuery(query);
            if (cancellationToken.IsCancellationRequested)
                tcs.TrySetCanceled(cancellationToken);
            else
                tcs.TrySetException(new VTQueryTimeoutException(in query, effectiveTimeout));
        });

        return await tcs.Task;
    }

    /// <summary>
    ///     Retrieves the defined terminal color for the specified palette color.
    /// </summary>
    public static NKColor GetColor(NKConsoleColor color, TimeSpan? timeout = null) =>
        GetColorAsync(color, timeout).Result;

    /// <summary>
    ///     Retrieves the defined terminal color for the specified palette index.
    /// </summary>
    public static NKColor GetColor(int colorIndex, TimeSpan? timeout = null) =>
        GetColorAsync(colorIndex, timeout).Result;

    /// <summary>
    ///     Asynchronously retrieves the defined terminal color for the specified palette color.
    /// </summary>
    public static async Task<NKColor> GetColorAsync(
        NKConsoleColor    color,
        TimeSpan?         timeout           = null,
        CancellationToken cancellationToken = default
    ) => await GetColorAsync((int)color, timeout, cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the defined terminal color for the specified palette index.
    /// </summary>
    public static async Task<NKColor> GetColorAsync(
        int               colorIndex,
        TimeSpan?         timeout           = null,
        CancellationToken cancellationToken = default
    ) {
        cancellationToken.ThrowIfCancellationRequested();
        var effectiveTimeout = timeout ?? DefaultQueryTimeout;
        var query            = VTQuery.RequestRedefineColor(colorIndex);

        if (!InterceptInput) {
            OutputDriver.Write(query.GetEscSeq());
            if (!TryReadUntil(out var res, out var last, effectiveTimeout, true, cancellationToken, '\a', '\\'))
                throw new VTQueryTimeoutException(in query, effectiveTimeout);

            var seq = res + last;
            if (seq.StartsWith('\e'))
                seq = seq[1..];

            if (AnsiParser.ExtractResult_Osc(seq, in query, out var parsed))
                return parsed.Value.RedefineColorResponse.Color;

            throw new InvalidOperationException($"Failed to parse color query response: {seq}");
        }

        var tcs = new TaskCompletionSource<NKColor>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(effectiveTimeout);

        VTQueryResponseHandler? h  = null;
        VTQueryTimeoutHandler?  th = null;

        void Cleanup() {
            VTQueryResponse -= h;
            VTQueryTimeout  -= th;
        }

        h = a => {
            if (a.Type != VTQueryType.OSC || a.OscMode != EscapeCodes.OscMode.REDEFINE_COLOR)
                return;

            if (a.Arguments.Count > 0 && a.Arguments[0] != colorIndex)
                return;

            Cleanup();
            tcs.TrySetResult(a.RedefineColorResponse.Color);
        };

        th = (req, reason) => {
            if (req.Matches(query)) {
                Cleanup();
                tcs.TrySetException(new VTQueryTimeoutException(in query, effectiveTimeout, $"VTQuery failed: {reason}"));
            }
        };

        VTQueryResponse += h;
        VTQueryTimeout  += th;
        InputDriver.RequestVTQuery(query);

        await using var reg = cts.Token.Register(() => {
            Cleanup();
            InputDriver.CancelVTQuery(query);
            if (cancellationToken.IsCancellationRequested)
                tcs.TrySetCanceled(cancellationToken);
            else
                tcs.TrySetException(new VTQueryTimeoutException(in query, effectiveTimeout));
        });

        return await tcs.Task;
    }

    /// <summary>
    ///     Retrieves the defined terminal colors for multiple palette indices.
    /// </summary>
    public static IReadOnlyDictionary<int, NKColor> GetColors(params int[] colorIndices) =>
        GetColorsAsync(colorIndices, null).Result;

    /// <summary>
    ///     Retrieves the defined terminal colors for multiple palette indices with a specified timeout.
    /// </summary>
    public static IReadOnlyDictionary<int, NKColor> GetColors(TimeSpan? timeout, params int[] colorIndices) =>
        GetColorsAsync(colorIndices, timeout).Result;

    /// <summary>
    ///     Retrieves the defined terminal colors for multiple palette colors.
    /// </summary>
    public static IReadOnlyDictionary<NKConsoleColor, NKColor> GetColors(params NKConsoleColor[] colors) =>
        GetColorsAsync(colors, null).Result;

    /// <summary>
    ///     Retrieves the defined terminal colors for multiple palette colors with a specified timeout.
    /// </summary>
    public static IReadOnlyDictionary<NKConsoleColor, NKColor> GetColors(TimeSpan? timeout, params NKConsoleColor[] colors) =>
        GetColorsAsync(colors, timeout).Result;

    /// <summary>
    ///     Asynchronously retrieves the defined terminal colors for multiple palette indices.
    /// </summary>
    public static Task<IReadOnlyDictionary<int, NKColor>> GetColorsAsync(params int[] colorIndices) =>
        GetColorsAsync(colorIndices, null, default);

    /// <summary>
    ///     Asynchronously retrieves the defined terminal colors for multiple palette indices with a timeout.
    /// </summary>
    public static async Task<IReadOnlyDictionary<int, NKColor>> GetColorsAsync(
        int[]             colorIndices,
        TimeSpan?         timeout           = null,
        CancellationToken cancellationToken = default
    ) {
        if (colorIndices.Length == 0)
            return new Dictionary<int, NKColor>();

        cancellationToken.ThrowIfCancellationRequested();
        var effectiveTimeout = timeout ?? DefaultQueryTimeout;
        var query            = VTQuery.RequestRedefineColors(colorIndices);
        var expectedSet      = new HashSet<int>(colorIndices);
        var result           = new Dictionary<int, NKColor>();

        if (!InterceptInput) {
            var sw = Stopwatch.StartNew();
            OutputDriver.Write(query.GetEscSeq());

            while (result.Count < expectedSet.Count) {
                cancellationToken.ThrowIfCancellationRequested();
                var remaining = effectiveTimeout - sw.Elapsed;
                if (remaining <= TimeSpan.Zero) {
                    if (result.Count > 0)
                        return result;
                    throw new VTQueryTimeoutException(in query, effectiveTimeout);
                }

                if (!TryReadUntil(out var res, out var last, remaining, true, cancellationToken, '\a', '\\')) {
                    if (result.Count > 0)
                        return result;
                    throw new VTQueryTimeoutException(in query, effectiveTimeout);
                }

                var seq = res + last;
                if (seq.StartsWith('\e'))
                    seq = seq[1..];

                if (AnsiParser.ExtractResult_Osc(seq, in query, out var parsed)) {
                    foreach (var (idx, col) in parsed.Value.RedefineColorsResponse)
                        result[idx] = col;
                }
                else {
                    break;
                }
            }

            return result;
        }

        var tcs = new TaskCompletionSource<IReadOnlyDictionary<int, NKColor>>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(effectiveTimeout);

        VTQueryResponseHandler? h  = null;
        VTQueryTimeoutHandler?  th = null;

        void Cleanup() {
            VTQueryResponse -= h;
            VTQueryTimeout  -= th;
        }

        h = a => {
            if (a.Type != VTQueryType.OSC || a.OscMode != EscapeCodes.OscMode.REDEFINE_COLOR)
                return;

            foreach (var (idx, col) in a.RedefineColorsResponse) {
                if (expectedSet.Contains(idx))
                    result[idx] = col;
            }

            if (result.Count >= expectedSet.Count) {
                Cleanup();
                tcs.TrySetResult(result);
            }
        };

        th = (req, reason) => {
            if (req.Matches(query)) {
                Cleanup();
                if (result.Count > 0)
                    tcs.TrySetResult(result);
                else
                    tcs.TrySetException(new VTQueryTimeoutException(in query, effectiveTimeout, $"VTQuery failed: {reason}"));
            }
        };

        VTQueryResponse += h;
        VTQueryTimeout  += th;
        InputDriver.RequestVTQuery(query);

        await using var reg = cts.Token.Register(() => {
            Cleanup();
            InputDriver.CancelVTQuery(query);
            if (result.Count > 0) {
                tcs.TrySetResult(result);
            }
            else if (cancellationToken.IsCancellationRequested) {
                tcs.TrySetCanceled(cancellationToken);
            }
            else {
                tcs.TrySetException(new VTQueryTimeoutException(in query, effectiveTimeout));
            }
        });

        return await tcs.Task;
    }

    /// <summary>
    ///     Asynchronously retrieves the defined terminal colors for multiple palette colors.
    /// </summary>
    public static Task<IReadOnlyDictionary<NKConsoleColor, NKColor>> GetColorsAsync(params NKConsoleColor[] colors) =>
        GetColorsAsync(colors, null, default);

    /// <summary>
    ///     Asynchronously retrieves the defined terminal colors for multiple palette colors.
    /// </summary>
    public static async Task<IReadOnlyDictionary<NKConsoleColor, NKColor>> GetColorsAsync(
        NKConsoleColor[]  colors,
        TimeSpan?         timeout           = null,
        CancellationToken cancellationToken = default
    ) {
        var indices = colors.Select(c => (int)c).ToArray();
        var result  = await GetColorsAsync(indices, timeout, cancellationToken);

        return result.ToDictionary(kv => (NKConsoleColor)kv.Key, kv => kv.Value);
    }

    // --- Non-throwing TryGet convenience APIs ---

    /// <summary>
    ///     Attempts to retrieve the defined terminal color for the specified palette color without throwing on timeout.
    /// </summary>
    public static bool TryGetColor(NKConsoleColor color, out NKColor result, TimeSpan? timeout = null) =>
        TryGetColor((int)color, out result, timeout);

    /// <summary>
    ///     Attempts to retrieve the defined terminal color for the specified palette index without throwing on timeout.
    /// </summary>
    public static bool TryGetColor(int colorIndex, out NKColor result, TimeSpan? timeout = null) {
        try {
            result = GetColor(colorIndex, timeout);
            return true;
        }
        catch (Exception) {
            result = default;
            return false;
        }
    }

    /// <summary>
    ///     Attempts to asynchronously retrieve the defined terminal color for the specified palette color.
    /// </summary>
    public static async Task<(bool Success, NKColor Color)> TryGetColorAsync(
        NKConsoleColor    color,
        TimeSpan?         timeout           = null,
        CancellationToken cancellationToken = default
    ) => await TryGetColorAsync((int)color, timeout, cancellationToken);

    /// <summary>
    ///     Attempts to asynchronously retrieve the defined terminal color for the specified palette index.
    /// </summary>
    public static async Task<(bool Success, NKColor Color)> TryGetColorAsync(
        int               colorIndex,
        TimeSpan?         timeout           = null,
        CancellationToken cancellationToken = default
    ) {
        try {
            var col = await GetColorAsync(colorIndex, timeout, cancellationToken);
            return (true, col);
        }
        catch (Exception) {
            return (false, default);
        }
    }

    /// <summary>
    ///     Attempts to asynchronously retrieve the defined terminal colors for multiple palette indices.
    /// </summary>
    public static async Task<(bool Success, IReadOnlyDictionary<int, NKColor> Colors)> TryGetColorsAsync(
        int[]             colorIndices,
        TimeSpan?         timeout           = null,
        CancellationToken cancellationToken = default
    ) {
        try {
            var res = await GetColorsAsync(colorIndices, timeout, cancellationToken);

            return (res.Count > 0, res);
        }
        catch (Exception) {
            return (false, new Dictionary<int, NKColor>());
        }
    }
}

[Flags]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum BoolStrings {
    TRUE_FALSE = 0x1,
    T_F        = 0x2,
    YES_NO     = 0x4,
    Y_N        = 0x8,
    ON_OFF     = 0x10,
    ZERO_ONE   = 0x20,
    ALL        = TRUE_FALSE | T_F | YES_NO | Y_N | ON_OFF | ZERO_ONE,
}