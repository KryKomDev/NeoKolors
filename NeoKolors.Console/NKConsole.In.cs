//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console.Driver;
using NeoKolors.Console.Driver.DotNet;
using NeoKolors.Console.Events;
using NeoKolors.Console.Mouse;
using OneOf;
using static NeoKolors.Console.BoolStrings;
using ArgumentException = System.ArgumentException;
using ConsoleKeyInfo = System.ConsoleKeyInfo;
using FormatException = System.FormatException;
using InvalidOperationException = System.InvalidOperationException;
using OverflowException = System.OverflowException;
using Std = System.Console;

#if NK_ENABLE_NATIVE_INPUT
using NeoKolors.Console.Driver.Linux;
using NeoKolors.Console.Driver.Windows;
using System.Runtime.InteropServices;
#endif


namespace NeoKolors.Console;

public static partial class NKConsole {
    
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
        style = style == default ? new NKStyle(NKColor.Default, NKColor.Default) : style;
        
        while (true) {
            
            string? s = Std.ReadLine();
            
            if (s == null) {
                if (reply) WriteLine("Invalid input.", style);
                continue;
            }

            long i;

            try {
                i = long.Parse(s);
            }
            catch (FormatException) {
                if (reply) WriteLine("Invalid input. Please enter a valid integer.", style);
                continue;
            }
            catch (OverflowException) {
                if (reply) WriteLine("Input overflow. Please enter a valid integer.", style);
                continue;
            }
            
            var res = validator(i);
            if (res == null) return i;
            if (reply) WriteLine(res, style);
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
        style = style == default ? new NKStyle(NKColor.Default, NKColor.Default) : style;
        
        while (true) {
            
            string? s = Std.ReadLine();
            
            if (s == null) {
                if (reply) WriteLine("Invalid input.", style);
                continue;
            }

            int i;

            try {
                i = int.Parse(s);
            }
            catch (FormatException) {
                if (reply) WriteLine("Invalid input. Please enter a valid integer.", style);
                continue;
            }
            catch (OverflowException) {
                if (reply) WriteLine("Input overflow. Please enter a valid integer.", style);
                continue;
            }
            
            var res = validator(i);
            if (res == null) return i;
            if (reply) WriteLine(res, style);
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
        style = style == default ? new NKStyle(NKColor.Default, NKColor.Default) : style;
        
        while (true) {
            
            string? s = Std.ReadLine();
            
            if (s == null) {
                if (reply) WriteLine("Invalid input.", style);
                continue;
            }

            short i;

            try {
                i = short.Parse(s);
            }
            catch (FormatException) {
                if (reply) WriteLine("Invalid input. Please enter a valid integer.", style);
                continue;
            }
            catch (OverflowException) {
                if (reply) WriteLine("Input overflow. Please enter a valid integer.", style);
                continue;
            }
            
            var res = validator(i);
            if (res == null) return i;
            if (reply) WriteLine(res, style);
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
        style = style == default ? new NKStyle(NKColor.Default, NKColor.Default) : style;
        
        while (true) {
            
            string? s = Std.ReadLine();
            
            if (s == null) {
                if (reply) WriteLine("Invalid input.", style);
                continue;
            }

            byte i;

            try {
                i = byte.Parse(s);
            }
            catch (FormatException) {
                if (reply) WriteLine("Invalid input. Please enter a valid integer.", style);
                continue;
            }
            catch (OverflowException) {
                if (reply) WriteLine("Input overflow. Please enter a valid integer.", style);
                continue;
            }
            
            var res = validator(i);
            if (res == null) return i;
            if (reply) WriteLine(res, style);
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
        style ??= new NKStyle(NKColor.Default, NKColor.Default);
        
        while (true) {
            
            string? s = Std.ReadLine();
            
            if (s == null) {
                if (reply) WriteLine("Invalid input.", style.Value);
                continue;
            }
            
            var res = validator(s);
            if (res == null) return s;
            if (reply) WriteLine(res, style.Value);
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
            
            string? s = Std.ReadLine();
            
            if (s == null) {
                if (reply) WriteLine("Invalid input.", style.Value);
                continue;
            }

            var res = ParseBool(s, boolStrings);

            if (res.IsT0) 
                return res.AsT0;

            if (reply) WriteLine(res.AsT1, style.Value);
        }
    }
    
    [Pure]
    [JetBrains.Annotations.Pure]
    private static OneOf<bool, string> ParseBool(string s, BoolStrings b) {
        return s.ToLower() switch {
            "true" => b.HasFlag(TRUE_FALSE) ? true : InvBoolMsg(b),
            "false" => b.HasFlag(TRUE_FALSE) ? false : InvBoolMsg(b),
            "yes" => b.HasFlag(YES_NO) ? true : InvBoolMsg(b),
            "no" => b.HasFlag(YES_NO) ? false : InvBoolMsg(b),
            "y" => b.HasFlag(Y_N) ? true : InvBoolMsg(b),
            "n" => b.HasFlag(Y_N) ? false : InvBoolMsg(b),
            "on" => b.HasFlag(ON_OFF) ? true : InvBoolMsg(b),
            "off" => b.HasFlag(ON_OFF) ? false : InvBoolMsg(b),
            "t" => b.HasFlag(T_F) ? true : InvBoolMsg(b),
            "f" => b.HasFlag(T_F) ? false : InvBoolMsg(b),
            "1" => b.HasFlag(ZERO_ONE) ? true : InvBoolMsg(b),
            "0" => b.HasFlag(ZERO_ONE) ? false : InvBoolMsg(b),
            _ => InvBoolMsg(b)
        };
    }
    
    [Pure]
    [JetBrains.Annotations.Pure]
    private static string InvBoolMsg(BoolStrings b) 
        => $"Invalid input. Please enter a valid boolean. (Allowed: {ToString(b)})";

    [Pure]
    [JetBrains.Annotations.Pure]
    private static string ToString(this BoolStrings b) => 
        b is ALL ? "true, false, yes, no, y, n, on, off, t, f, 1, 0" : b.ToString().ToLower().Replace('_', ',');

    
    #region INPUT INTERCEPTION

    private static readonly IInputDriver INPUT_DRIVER = 
        #if NK_ENABLE_NATIVE_INPUT
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new WindowsInputDriver() :
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? new LinuxInputDriver() : 
        new DotNetInputDriver();
        #else
        new DotNetInputDriver();
        #endif

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
        
        INPUT_DRIVER.Key += OnKey;
        INPUT_DRIVER.Mouse += OnMouse;
        INPUT_DRIVER.FocusIn += OnFocusIn;
        INPUT_DRIVER.FocusOut += OnFocusOut;
        INPUT_DRIVER.Paste += OnPaste;
        INPUT_DRIVER.WinOpsResponse += OnWinOpsResponse;
        INPUT_DRIVER.DecReqResponse += OnDecReqResponse;
        
        INPUT_DRIVER.Start();
    }

    /// <summary>
    /// Stops the interception of input by setting the internal state to false.
    /// This is used to cease capturing or processing input events, thereby
    /// restoring the default behavior where input is no longer intercepted.
    /// </summary>
    public static void StopInputInterception() {
        InterceptInput = false;
        LOGGER.Info("Stopping input interception...");
        
        INPUT_DRIVER.Stop();
        
        INPUT_DRIVER.Key -= OnKey;
        INPUT_DRIVER.Mouse -= OnMouse;
        INPUT_DRIVER.FocusIn -= OnFocusIn;
        INPUT_DRIVER.FocusOut -= OnFocusOut;
        INPUT_DRIVER.Paste -= OnPaste;
        INPUT_DRIVER.WinOpsResponse -= OnWinOpsResponse;
        INPUT_DRIVER.DecReqResponse -= OnDecReqResponse;
    }

    private static void OnKey(ConsoleKeyInfo k) => KeyEvent(k);
    private static void OnMouse(MouseEventArgs m) => MouseEvent(m);
    private static void OnFocusIn() => FocusInEvent();
    private static void OnFocusOut() => FocusOutEvent();
    private static void OnPaste(string s) => PasteEvent(s);
    private static void OnWinOpsResponse(WinOpsResponseArgs a) => WinOpsResponseEvent(a);
    private static void OnDecReqResponse(DecReqResponseArgs a) => DecReqResponseEvent(a);

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
    
        var buffer = new StringBuilder();
        var sequenceIndex = 0;
    
        while (true) {
            var key = Std.ReadKey(intercept);
            var ch = key.KeyChar;
        
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

    /// <summary>
    /// Reads characters from the console until one of the specified characters is encountered.
    /// Returns the string composed of all characters read before the specified character is found.
    /// </summary>
    /// <param name="last"></param>
    /// <param name="oneOf">
    ///     An array of characters that act as delimiters; reading stops when one of these characters is encountered.
    /// </param>
    /// <param name="intercept">
    ///     If true, the read characters are read from the console without being displayed.
    ///     If false, the characters will be displayed as they are typed.
    /// </param>
    /// <returns>
    /// A string containing all characters read before encountering a specified delimiter character.
    /// </returns>
    public static string ReadUntil(out char last, bool intercept = false, params char[] oneOf) {
        var key = Std.ReadKey(intercept);
        string s = "";

        while (!oneOf.Contains(key.KeyChar)) {
            s += key.KeyChar;
            key = Std.ReadKey(intercept);
        }

        last = key.KeyChar;
        return s;
    }

    /// <summary>
    /// Reads characters from the console until the specified character is encountered.
    /// All characters read, including the termination character, are discarded if interception is enabled.
    /// </summary>
    /// <param name="c">The character at which to stop reading input.</param>
    /// <param name="intercept">
    /// If true, the characters entered will not be displayed in the console.
    /// If false, the characters will be visible as they are typed.
    /// </param>
    /// <returns>A string containing all characters entered up to, but not including, the specified character.</returns>
    public static string ReadUntil(char c, bool intercept = false) {
        var key = Std.ReadKey(intercept);
        string s = "";

        while (key.KeyChar != c) {
            s += key.KeyChar;
            key = Std.ReadKey(intercept);
        }

        return s;
    }

    /// <summary>
    /// Returns true if the alternate console buffer is enabled.
    /// </summary>
    public static bool GetAltBufState() => GetAltBufStateAsync().Result;

    /// <summary>
    /// Returns true if the alternate console buffer is enabled.
    /// </summary>
    public static async Task<bool> GetAltBufStateAsync() {
        Std.Write(EscapeCodes.REQUEST_ALTBUF_STATE);
        
        // If multithreaded input interception is not enabled
        if (!InterceptInput) {
            var res = ReadUntil('y');
            return res[8] == '1'; 
        }
        
        var tcs = new TaskCompletionSource<bool>();
        
        DecReqResponseEventHandler h = null!;
        h = a => {
            if (a.Mode != 1049) return;
            DecReqResponseEvent -= h;
            tcs.SetResult(a.Response == DecReqResponseType.ENABLED);
        };

        DecReqResponseEvent += h;

        return await tcs.Task;
    }

    /// <summary>
    /// Retrieves the current size of the console window in pixels as a tuple containing
    /// the width and height.
    /// </summary>
    /// <returns>
    /// A tuple where the first value is the width in pixels and the second
    /// value is the height in pixels of the console window.
    /// </returns>
    public static Size2D GetScreenSizePx() => GetScreenSizePxAsync().Result;

    /// <summary>
    /// Retrieves the current size of the console window in pixels as a tuple containing
    /// the width and height.
    /// </summary>
    /// <returns>
    /// A tuple where the first value is the width in pixels and the second
    /// value is the height in pixels of the console window.
    /// </returns>
    public static async Task<Size2D> GetScreenSizePxAsync() {
        Std.Write(EscapeCodes.REPORT_WINDOW_SIZE_PX);
        
        // If multithreaded input interception is not enabled
        if (!InterceptInput) {
            var res = ReadUntil('t', true);
            var split = res.Split(';');
            return new Size2D(int.Parse(split[1]), int.Parse(split[2]));
        }
        
        var tcs = new TaskCompletionSource<Size2D>();
        
        WinOpsResponseEventHandler h = null!;
        h = a => {
            if (a.Type != WinOpsResponseType.WIN_SIZE_PX) return;
            WinOpsResponseEvent -= h;
            tcs.SetResult(a.AsWinSizePx());
        };

        WinOpsResponseEvent += h;

        return await tcs.Task;
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