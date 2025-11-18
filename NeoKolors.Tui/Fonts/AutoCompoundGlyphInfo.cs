// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts.Serialization.Xml;
using static NeoKolors.Tui.Fonts.Serialization.Xml.AutoCompoundApplicableGroup;
using ApplicableGroup = NeoKolors.Tui.Fonts.Serialization.Xml.AutoCompoundApplicableGroup;

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// Represents information about an automatically compoundable glyph, including the base glyph,
/// the base character, applicable characters, and alignment configuration.
/// </summary>
public readonly struct AutoCompoundGlyphInfo : IEquatable<AutoCompoundGlyphInfo> {
    
    public IGlyph BaseGlyph { get; }
    public char Character { get; }
    public ApplicableChars ApplicableChars { get; }
    public CompoundGlyphAlignment Alignment { get; }
    public bool MainFirst { get; }

    /// <summary>
    /// Creates a compound glyph by combining the base glyph with another specified glyph using a defined alignment.
    /// </summary>
    /// <param name="glyph">The secondary glyph to combine with the base glyph.</param>
    /// <returns>A new compound glyph composed of the base glyph and the specified secondary glyph, aligned according
    /// to the alignment settings.</returns>
    public IGlyph GetGlyph(IGlyph glyph) => new NKCompoundGlyph(BaseGlyph, glyph, Alignment);

    public AutoCompoundGlyphInfo(
        IGlyph baseGlyph,
        char character,
        CompoundGlyphAlignment alignment, 
        ApplicableChars applicableChars,
        bool mainFirst = false)
    {
        BaseGlyph = baseGlyph;
        Character = character;
        Alignment = alignment;
        ApplicableChars = applicableChars;
        MainFirst = mainFirst;
    }

    private AutoCompoundGlyphInfo(char character) {
        BaseGlyph = null!;
        Character = character;
        Alignment = CompoundGlyphAlignment.NONE;
        ApplicableChars = NONE;
    }

    public override int GetHashCode() => Character.GetHashCode();

    public bool Equals(AutoCompoundGlyphInfo other) =>
        BaseGlyph.Equals(other.BaseGlyph) && 
        Character == other.Character && 
        ApplicableChars.Equals(other.ApplicableChars) && 
        Alignment.Equals(other.Alignment);

    public override bool Equals(object? obj) => obj is AutoCompoundGlyphInfo other && Equals(other);
    public static bool operator ==(AutoCompoundGlyphInfo left, AutoCompoundGlyphInfo right) => left.Equals(right);
    public static bool operator !=(AutoCompoundGlyphInfo left, AutoCompoundGlyphInfo right) => !left.Equals(right);
}

public readonly struct ApplicableChars : IEquatable<ApplicableChars> {
    public char[] Characters { get; }
    public ApplicableGroup ApplicableGroup { get; }
    
    public ApplicableChars(char[]? characters = null, ApplicableGroup applicableGroup = NONE) {
        Characters = characters ?? [];
        ApplicableGroup = applicableGroup;
    }

    public ApplicableChars(AutoCompoundApplicable applicable) {
        Characters = !Equals(applicable.Chars, null) ? applicable.Chars.ToArray() : [];
        ApplicableGroup = applicable.Group.Aggregate(NONE, (current, a) => current | a);
    }

    public bool IsApplicable(char c) {
        if (Characters.Contains(c)) return true;
        
        if (ApplicableGroup.AllowLowerVowels     && char.IsVowelLower(c))     return true;
        if (ApplicableGroup.AllowUpperVowels     && char.IsVowelUpper(c))     return true;
        if (ApplicableGroup.AllowLowerConsonants && char.IsConsonantLower(c)) return true;
        if (ApplicableGroup.AllowUpperConsonants && char.IsConsonantUpper(c)) return true;
        if (ApplicableGroup.AllowNumbers         && char.IsDigit(c))          return true;
        if (ApplicableGroup.AllowLowerLetters    && char.IsLower(c))          return true;
        if (ApplicableGroup.AllowUpperLetters    && char.IsUpper(c))          return true;

        return false;
    }

    public bool Equals(ApplicableChars other) 
        => Characters.Equals(other.Characters) && 
           ApplicableGroup == other.ApplicableGroup;

    public override bool Equals(object? obj) => obj is ApplicableChars other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Characters, (int)ApplicableGroup);
    
    public static implicit operator ApplicableChars(ApplicableGroup group) => new([], group);
    public static implicit operator ApplicableChars(AutoCompoundApplicable applicable) => new(applicable);
    public static bool operator ==(ApplicableChars left, ApplicableChars right) => left.Equals(right);
    public static bool operator !=(ApplicableChars left, ApplicableChars right) => !left.Equals(right);
}

file static class ApplicableGroupsExtensions {
    
    extension(ApplicableGroup g) {
        public bool AllowLowerLetters => g.HasFlag(LETTERS_LOWER);
        public bool AllowUpperLetters => g.HasFlag(LETTERS_UPPER);
        public bool AllowLowerVowels => g.HasFlag(VOWELS_LOWER);
        public bool AllowUpperVowels => g.HasFlag(VOWELS_UPPER);
        public bool AllowLowerConsonants => g.HasFlag(CONSONANTS_LOWER);
        public bool AllowUpperConsonants => g.HasFlag(CONSONANTS_UPPER);
        public bool AllowNumbers => g.HasFlag(NUMBERS);
    }
    
    private static readonly HashSet<char> VOWELS_L = [
        'a', 'e', 'i', 'o', 'u', 'y',
        'á', 'à', 'â', 'ä', 'ã', 'å', 'æ',
        'é', 'è', 'ê', 'ë',
        'í', 'ì', 'î', 'ï',
        'ó', 'ò', 'ô', 'ö', 'õ', 'ø',
        'ú', 'ù', 'û', 'ü',
        'ý', 'ÿ'
    ];
    
    private static readonly HashSet<char> VOWELS_U = VOWELS_L.Select(char.ToUpper).ToHashSet();
    
    extension(char) {
        
        public static bool IsVowel(char c) => IsVowelLower(c) | IsVowelUpper(c);
        public static bool IsVowelUpper(char c) => VOWELS_U.Contains(c);
        public static bool IsVowelLower(char c) => VOWELS_L.Contains(c);
        public static bool IsConsonant(char c) => char.IsLetter(c) && !IsVowel(c);
        public static bool IsConsonantUpper(char c) => char.IsLetter(c) && char.IsUpper(c) && !IsVowel(c);
        public static bool IsConsonantLower(char c) => char.IsLetter(c) && char.IsLower(c) && !IsVowel(c);              
    }
}