//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.CodeAnalysis;

namespace NeoKolors.Extensions.Exception;

public static class ThrowHelper {
    
    /// <summary>
    /// Throws the specified exception if the provided condition evaluates to true.
    /// </summary>
    /// <param name="condition">The condition to evaluate. If true, the exception will be thrown.</param>
    /// <param name="exception">The exception to throw if the condition is true.</param>
    /// <exception cref="Exception">Thrown when the condition is true.</exception>
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, System.Exception exception) {
        if (condition) throw exception;
    }

    /// <summary>
    /// Throws the specified exception if the provided object is null.
    /// </summary>
    /// <param name="obj">The object to check for null. If null, the exception will be thrown.</param>
    /// <param name="exception">The exception to throw if the object is null.</param>
    /// <exception cref="Exception">Thrown when the object is null.</exception>
    public static void ThrowIfNull([NotNull] object? obj, System.Exception exception) {
        if (obj is null) throw exception;
    }
    
    public static class ArgOutOfRange {
        
        /// <summary>
        /// Throws the specified exception if the provided condition evaluates to true.
        /// </summary>
        /// <param name="condition">The condition to evaluate. If true, the exception will be thrown.</param>
        /// <param name="name">The name of the faulty argument.</param>
        /// <param name="message">Cause of the exception.</param>
        /// <exception cref="Exception">Thrown when the condition is true.</exception>
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string name, string message = "") {
            if (condition) throw new ArgumentOutOfRangeException(message: message, paramName: name);
        }

        /// <summary>
        /// Throws the specified exception if the provided object is null.
        /// </summary>
        /// <param name="obj">The object to check for null. If null, the exception will be thrown.</param>
        /// <param name="name">The name of the faulty argument.</param>
        /// <param name="message">Cause of the exception.</param>
        /// <exception cref="Exception">Thrown when the object is null.</exception>
        public static void ThrowIfNull([NotNull] object? obj, string name, string message = "") {
            if (obj is null) throw new ArgumentOutOfRangeException(message: message, paramName: name);
        }
    }
}