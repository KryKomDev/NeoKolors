// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

namespace NeoKolors.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="ReadOnlySpan{T}"/> and string spans.
/// </summary>
public static class SpanExtensions {
    extension(ReadOnlySpan<char> first) {
        /// <summary>
        /// Concatenates two <see cref="ReadOnlySpan{T}"/> instances of characters.
        /// </summary>
        /// <param name="second">The second span to concatenate.</param>
        /// <returns>A new <see cref="ReadOnlySpan{T}"/> containing the concatenated characters.</returns>
        public ReadOnlySpan<char> Concat(ReadOnlySpan<char> second)
            => new string(((IEnumerable<char>)first.ToArray()).Concat(second.ToArray()).ToArray()).AsSpan();
    }

    extension(string first) {
        /// <summary>
        /// Concatenates a string and a <see cref="ReadOnlySpan{T}"/> of characters.
        /// </summary>
        /// <param name="second">The second span to concatenate.</param>
        /// <returns>A new <see cref="ReadOnlySpan{T}"/> containing the concatenated characters.</returns>
        public ReadOnlySpan<char> Concat(ReadOnlySpan<char> second)
            => new string(((IEnumerable<char>)first.ToArray()).Concat(second.ToArray()).ToArray()).ToArray();
    }
}