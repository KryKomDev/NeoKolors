// NeoKolors
// Copyright (c) krystof 2026

// ReSharper disable ReplaceWithFieldKeyword

using System.Runtime.InteropServices;
using NeoKolors.Common;

namespace NeoKolors.Tui.Fonts;

[StructLayout(LayoutKind.Explicit)]
public readonly record struct NKGlyphSymbol {

    [FieldOffset(16)] private readonly byte    _type;
    [FieldOffset(0)]  private readonly NKStyle _styles;
    [FieldOffset(17)] private readonly char    _symbol_simple;
    [FieldOffset(8)]  private readonly string? _symbol_ligature;

    public byte    Type   => _type;
    public NKStyle Styles => _styles;

    public bool IsSimple       => _type == 1;
    public bool IsLigature     => _type == 2;

    public char SimpleSymbol => _type == 1 ? _symbol_simple : throw new IOp();

    public string LigatureSymbol => _type == 2 ? _symbol_ligature!        : throw new IOp();
    public int    LigatureLength => _type == 2 ? _symbol_ligature!.Length : throw new IOp();

    private NKGlyphSymbol(char simple, NKStyle styles = default) {
        _type            = 1;
        _styles          = styles;
        _symbol_simple   = simple;
        _symbol_ligature = null;
    }

    private NKGlyphSymbol(string ligature, NKStyle styles = default) {
        _type            = 2;
        _styles          = styles;
        _symbol_simple   = default;
        _symbol_ligature = ligature;
    }

    public static NKGlyphSymbol Simple(char symbol, NKStyle styles = default) =>
        new(symbol, styles);

    public static NKGlyphSymbol Ligature(string ligature, NKStyle styles = default) =>
        new(ligature, styles);

    public bool Equals(NKGlyphSymbol other) {
        if (_type != other._type || _styles != other._styles) return false;
        return _type switch {
            1 => _symbol_simple == other._symbol_simple,
            2 => _symbol_ligature == other._symbol_ligature,
            _ => true
        };
    }

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(_type);
        hash.Add(_styles);
        
        switch (_type) {
            case 1: hash.Add(_symbol_simple);   break;
            case 2: hash.Add(_symbol_ligature); break;
        }
        
        return hash.ToHashCode();
    }

    public override string ToString() {
        return _type switch {
            1 => $"Simple({_symbol_simple}, {_styles.ToString()})",
            2 => $"Ligature({_symbol_ligature}, {_styles.ToString()})",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
