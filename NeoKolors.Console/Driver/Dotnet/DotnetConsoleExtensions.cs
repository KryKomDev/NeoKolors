// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Runtime.CompilerServices;
using Metriks;
using NeoKolors.Extensions;

namespace NeoKolors.Console.Driver.Dotnet;

public static class DotnetConsoleExtensions {
    
    extension(Stdio) {
        
        /// <summary>
        /// Retrieves the current dimensions of the console buffer as a 2D size structure.
        /// </summary>
        /// <returns>A Size2D structure representing the width and height of the console buffer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size2D GetBufferSize() => new(Stdio.BufferWidth, Stdio.BufferHeight);

        /// <summary>
        /// Retrieves the current dimensions of the console window as a 2D size structure.
        /// </summary>
        /// <returns>A Size2D structure representing the width and height of the console window.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size2D GetWindowSize() => new(Stdio.WindowWidth, Stdio.WindowHeight);

        /// <summary>
        /// Retrieves the current dimensions of the console buffer as a 2D size structure.
        /// </summary>
        /// <returns>A Size2D structure representing the width and height of the console buffer.</returns>
        public static Size2D BufferSize {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Stdio.GetBufferSize();
        }

        /// <summary>
        /// Retrieves the current dimensions of the console window as a 2D size structure.
        /// </summary>
        /// <returns>A Size2D structure representing the width and height of the console window.</returns>
        public static Size2D WindowSize {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Stdio.GetWindowSize();
        }

        /// <summary>
        /// Reads the next key press or an input character from the console input stream,
        /// with an option to intercept the key press.
        /// </summary>
        /// <param name="intercept">A boolean value indicating whether the key press should be intercepted.
        /// If true, the key press will not appear in the console. Defaults to false.</param>
        /// <returns>A ConsoleKeyInfo structure representing the key press, or null if no key is
        /// available or input is redirected.</returns>
        public static ConsoleKeyInfo? ReadKeyU(bool intercept = false) {
            if (!Stdio.IsInputRedirected)
                return Stdio.KeyAvailable 
                    ? Stdio.ReadKey(intercept) 
                    : null;

            var c = Stdio.Read();
            
            return c == -1 
                ? null
                : ConsoleKeyInfo.FromChar((char)c);
        }

        /// <summary>
        /// Determines whether there is any input available to be read from the console input stream.
        /// </summary>
        /// <returns>true if input can be read; otherwise, false.</returns>
        public static bool PeekU()
            => Stdio.IsInputRedirected
                ? Stdio.In.Peek() != -1
                : Stdio.KeyAvailable; 
    }
}