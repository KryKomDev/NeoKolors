// NeoKolors
// Copyright (c) krystof 2026

using System.Runtime.InteropServices;

namespace NeoKolors.Tui.Core;

[StructLayout(LayoutKind.Explicit, Size = 9)]
file readonly unsafe struct Grapheme {

    private enum GFlags : byte {
        SINGLE,
        DOUBLE,
        COMPLEX
    }

    [FieldOffset(0)] private readonly char[]? _complexRef = null;
    [FieldOffset(0)] private readonly int     _r0;
    [FieldOffset(4)] private readonly int     _r1;
    [FieldOffset(8)] private readonly GFlags  _flags;

    public Grapheme(string complex) {
        _complexRef = complex.ToCharArray();
        _flags      = GFlags.COMPLEX;
    }

    public Grapheme(int r0, int r1) {
        _r0    = r0;
        _r1    = r1;
        _flags = GFlags.DOUBLE;
    }

    public Grapheme(int r) {
        _r0    = r;
        _flags = GFlags.SINGLE;
    }

    public bool IsComplex => _flags == GFlags.COMPLEX;
    public bool IsDouble  => _flags == GFlags.DOUBLE;
    public bool IsSingle  => _flags == GFlags.SINGLE;

    public override string ToString() {
        return _flags switch {
            GFlags.SINGLE => char.ConvertFromUtf32(_r0),
            GFlags.DOUBLE => $"{char.ConvertFromUtf32(_r0)}{char.ConvertFromUtf32(_r1)}",
            GFlags.COMPLEX =>
                _complexRef != null
                    ? new string(_complexRef)
                    : throw new InvalidOperationException(),
            _ => throw new InvalidOperationException()
        };
    }
}