//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using System.Text;
using NeoKolors.Common;
using NeoKolors.Console.Mouse;
using OneOf;
using static NeoKolors.Console.BoolStrings;
using Std = System.Console;

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
    /// <exception cref="InvalidOperationException">
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
    /// <exception cref="InvalidOperationException">
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
    /// <exception cref="InvalidOperationException">
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
    /// <exception cref="InvalidOperationException">
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
    
    [System.Diagnostics.Contracts.Pure]
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
    
    [System.Diagnostics.Contracts.Pure]
    [JetBrains.Annotations.Pure]
    private static string InvBoolMsg(BoolStrings b) 
        => $"Invalid input. Please enter a valid boolean. (Allowed: {ToString(b)})";

    [System.Diagnostics.Contracts.Pure]
    [JetBrains.Annotations.Pure]
    private static string ToString(this BoolStrings b) => 
        b is ALL ? "true, false, yes, no, y, n, on, off, t, f, 1, 0" : b.ToString().ToLower().Replace('_', ',');

    
    #region INPUT INTERCEPTION

    private static readonly Thread INPUT_THREAD = new(Intercept) {
        IsBackground = true,
        Priority = ThreadPriority.BelowNormal,
        Name = "NeoKolors Input Interceptor"
    };

    private static void Intercept() {
        ConsoleKeyInfo i;
        
        try {
            i = Std.ReadKey(intercept: true);
        }
        catch (InvalidOperationException e) {
            // if the input is redirected or is not from a
            // console or the console is a weird mf like mintty
            LOGGER.Error(e.Message);
            InterceptCompat();
            return;
        }

        while (InterceptInput) {
            if (i is { KeyChar: '\e', Modifiers: 0 }) 
                HandleEscape();
            else 
                KeyEvent.Invoke(i);
            
            i = Std.ReadKey(intercept: true);
        }
    }

    private static void HandleEscape() {
        var introducer = Std.ReadKey(intercept: true);
        
        if (introducer.KeyChar != '[') {
            KeyEvent.Invoke(new ConsoleKeyInfo(
                keyChar: '\e', 
                key: ConsoleKey.Backspace, 
                shift: false, 
                alt: false, 
                control: false)
            );
            KeyEvent.Invoke(introducer);
            return;
        }
        
        var next = Std.ReadKey(intercept: true);
        
        switch (next.KeyChar) {
            case 'O':
                FocusOutEvent.Invoke();
                break;
            case 'I':
                FocusInEvent.Invoke();
                break;
            case 'M':
                HandleX10MouseEvent();
                break;
            case '2':
                HandlePaste();
                break;
            case '<':
                HandleSGRMouseEvent();
                break;
        }
    }

    private static void HandleX10MouseEvent() {
        var type = Std.ReadKey(intercept: true);
        var x = Std.ReadKey(intercept: true);
        var y = Std.ReadKey(intercept: true);
        MouseEvent.Invoke(
            MouseReportProtocol == MouseReportProtocol.SGR
                ? MouseEventDecomposer.DecomposeSGR(type, x, y)
                : MouseEventDecomposer.DecomposeUtf8(type, x, y));
    }

    private static void HandleSGRMouseEvent() {
        throw new NotImplementedException();
    }

    private static void HandlePaste() {
        SkipKeys(3);
        var s = ReadUntil("\e[201~", true);
        PasteEvent.Invoke(s);
    }

    private static void SkipKeys(int num) {
        for (int i = 0; i < num; i++) 
            Std.ReadKey(intercept: true);
    }

    private static void InterceptCompat() {
        while (InterceptInput) {
            if (!Std.KeyAvailable) continue;
            
            var k = Std.Read();

            switch (k) {
                case -1: {
                    break;
                }
                case '\e': {
                    HandleEscapeCompat(); 
                    break;
                }
                default: {
                    KeyEvent.Invoke(new ConsoleKeyInfo(
                        keyChar: (char)k, 
                        key: (ConsoleKey)char.ToLower((char)k), 
                        shift: char.IsUpper((char)k), 
                        alt: false, 
                        control: false));
                    break;
                }
            }
        }
    }

    private static void HandleEscapeCompat() {
        int intro = -1;
        while (intro is -1) {
            intro = Std.Read(); 
        }

        if (intro is not '[') {
            KeyEvent.Invoke(new ConsoleKeyInfo(
                keyChar: '\e', 
                key: ConsoleKey.Backspace, 
                shift: false, 
                alt: false, 
                control: false)
            );
            KeyEvent.Invoke(new ConsoleKeyInfo(
                keyChar: (char)intro, 
                key: (ConsoleKey)char.ToLower((char)intro), 
                shift: char.IsUpper((char)intro), 
                alt: false, 
                control: false)
            );
        }
        
        int next = -1;
        while (next is -1) {
            next = Std.Read(); 
        }
        
        switch (next) {
            case 'O':
                FocusOutEvent.Invoke();
                break;
            case 'I':
                FocusInEvent.Invoke();
                break;
            case 'M':
                HandleX10MouseEvent();
                break;
            case '2':
                HandlePaste();
                break;
            case '<':
                HandleSGRMouseEvent();
                break;
        }
    }

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
    /// <exception cref="ArgumentException">
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
}

[Flags]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum BoolStrings {
    TRUE_FALSE = 1,
    T_F = 2,
    YES_NO = 4,
    Y_N = 8,
    ON_OFF = 0x10,
    ZERO_ONE = 0x20,
    ALL = TRUE_FALSE | T_F | YES_NO | Y_N | ON_OFF | ZERO_ONE,
}