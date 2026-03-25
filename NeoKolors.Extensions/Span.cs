// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

namespace NeoKolors.Extensions;

public static class SpanExtensions {
    extension(ReadOnlySpan<char> first) {
        public ReadOnlySpan<char> Concat(ReadOnlySpan<char> second)
            => new string(((IEnumerable<char>)first.ToArray()).Concat(second.ToArray()).ToArray()).AsSpan();
    }

    extension(string first) {
        public ReadOnlySpan<char> Concat(ReadOnlySpan<char> second)
            => new string(((IEnumerable<char>)first.ToArray()).Concat(second.ToArray()).ToArray()).ToArray();
    }
}