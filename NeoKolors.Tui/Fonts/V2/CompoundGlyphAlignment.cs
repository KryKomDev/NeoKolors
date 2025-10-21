// NeoKolors
// Copyright (c) 2025 KryKom

using static NeoKolors.Tui.Fonts.V2.CompoundGlyphAlignmentType;

namespace NeoKolors.Tui.Fonts.V2;

public readonly struct CompoundGlyphAlignment : IEquatable<CompoundGlyphAlignment> {
    
    public CompoundGlyphAlignmentType Type { get; }
    public char AlignmentChar { get; }
    
    private CompoundGlyphAlignment(CompoundGlyphAlignmentType type, char alignmentChar = '\0') {
        Type = type;
        AlignmentChar = alignmentChar;
    }

    internal static readonly CompoundGlyphAlignment NONE = new(CompoundGlyphAlignmentType.NONE);
    
    public static CompoundGlyphAlignment TopLeft() => new(TOP_LEFT);
    public static CompoundGlyphAlignment TopCenter() => new(TOP_CENTER);
    public static CompoundGlyphAlignment TopRight() => new(TOP_RIGHT);
    public static CompoundGlyphAlignment MiddleLeft() => new(MIDDLE_LEFT);
    public static CompoundGlyphAlignment Center() => new(CENTER);
    public static CompoundGlyphAlignment MiddleRight() => new(MIDDLE_RIGHT);
    public static CompoundGlyphAlignment BottomLeft() => new(BOTTOM_LEFT);
    public static CompoundGlyphAlignment BottomCenter() => new(BOTTOM_CENTER);
    public static CompoundGlyphAlignment BottomRight() => new(BOTTOM_RIGHT);
    public static CompoundGlyphAlignment Custom(char alignChar) => new(CUSTOM, alignChar);

    public bool Equals(CompoundGlyphAlignment other) => Type == other.Type && AlignmentChar == other.AlignmentChar;
    public override bool Equals(object? obj) => obj is CompoundGlyphAlignment other && Equals(other);
    public override int GetHashCode() => HashCode.Combine((int)Type, AlignmentChar);
}