// NeoKolors
// Copyright (c) krystof 2026

using System.Globalization;
using NeoKolors.Common;
using OneOf.Types;

namespace NeoKolors.Tui.Core;

/// <summary>
/// Represents a cell that contains a character (rune), along with its associated styling
/// and a flag indicating whether it has changed.
/// </summary>
public readonly record struct NKCell {

    public char        Char  { get; init; }
    public NKStyle     Style { get; init; }
    public NKCellFlags Flags { get; } = NKCellFlags.SINGLE;

    public NKCell(AnsiChar @char, NKCellFlags flags) {
        Char  = @char.Char;
        Style = @char.Style;
        Flags = flags;
    }

    public NKCell(
        char        @char,
        NKStyle     style,
        NKCellFlags flags = NKCellFlags.SINGLE
    ) {
        Char  = @char;
        Style = style;
        Flags = flags;
    }

    public static OneOf<Success<NKCell>, Error> New(
        char    @char,
        NKStyle style
    ) {
        return char.IsSurrogate(@char)
            ? new Error()
            : new Success<NKCell>(new NKCell(@char, style));
    }

    public static NKCell[] FromString(AnsiString str) {
        if (str == null) {
            return [];
        }

        var cells        = new NKCell[str.Length];
        var startIndices = StringInfo.ParseCombiningCharacters(str.Plain);

        for (int i = 0; i < startIndices.Length; i++) {
            int start         = startIndices[i];
            int nextStart     = i + 1 < startIndices.Length ? startIndices[i + 1] : str.Length;
            int clusterLength = nextStart - start;

            if (clusterLength == 1) {
                cells[start] = new NKCell(str[start], NKCellFlags.SINGLE);
            }
            else {
                cells[start] = new NKCell(str[start], NKCellFlags.CLUSTER_START);

                for (int j = 1; j < clusterLength; j++) {
                    cells[start + j] = new NKCell(str[start + j], NKCellFlags.CLUSTER_CONTINUE);
                }
            }
        }

        return cells;
    }

    public static (NKCell High, NKCell Low) New(
        char    high,
        char    low,
        NKStyle style
    ) {
        return (
            new NKCell(high, style, NKCellFlags.HIGH_SURROGATE),
            new NKCell(low,  style, NKCellFlags.LOW_SURROGATE)
        );
    }

    public static NKCell Default { get; } = new(' ', NKStyle.Default);

    public bool Equals(NKCell other) => Char == other.Char && Style.Equals(other.Style);

    public override int GetHashCode() => HashCode.Combine(Char, Style);
}