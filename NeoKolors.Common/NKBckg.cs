//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

/// <summary>
/// Represents a wrapper for background color inside the NeoKolors library.
/// </summary>
/// <remarks>
/// The <see cref="NKBckg"/> struct provides a simplified, type-safe way of handling
/// background color representation derived from the <see cref="NKColor"/> struct.
/// It is implicitly convertible from <see cref="NKColor"/> and makes use of
/// the <c>Bckg</c> property of <see cref="NKColor"/> for its functionality.
/// </remarks>
public struct NKBckg {
    private NKColor _color;

    internal NKBckg(NKColor color) => _color = color;
    
    public override string ToString() => _color.Bckg;
    public static implicit operator NKBckg(NKColor color) => new(color);
}