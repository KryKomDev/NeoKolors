//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;
using NeoKolors.Common;
using OneOf;
using static NeoKolors.Console.BoolStrings;

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
            
            string? s = System.Console.ReadLine();
            
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
            
            string? s = System.Console.ReadLine();
            
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
            
            string? s = System.Console.ReadLine();
            
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
            
            string? s = System.Console.ReadLine();
            
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
            
            string? s = System.Console.ReadLine();
            
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
            
            string? s = System.Console.ReadLine();
            
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