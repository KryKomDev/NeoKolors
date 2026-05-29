// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions.Tests;

public class CharTests {

    #region IsVowel Tests

    [Theory]
    [InlineData('a', true)]
    [InlineData('e', true)]
    [InlineData('i', true)]
    [InlineData('o', true)]
    [InlineData('u', true)]
    [InlineData('y', true)]
    [InlineData('A', true)]
    [InlineData('E', true)]
    [InlineData('I', true)]
    [InlineData('O', true)]
    [InlineData('U', true)]
    [InlineData('Y', true)]
    public void IsVowel_BasicVowels_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsVowel(c));
    }

    [Theory]
    [InlineData('á', true)]
    [InlineData('à', true)]
    [InlineData('â', true)]
    [InlineData('ä', true)]
    [InlineData('ã', true)]
    [InlineData('å', true)]
    [InlineData('æ', true)]
    [InlineData('é', true)]
    [InlineData('è', true)]
    [InlineData('ê', true)]
    [InlineData('ë', true)]
    [InlineData('í', true)]
    [InlineData('ì', true)]
    [InlineData('î', true)]
    [InlineData('ï', true)]
    [InlineData('ó', true)]
    [InlineData('ò', true)]
    [InlineData('ô', true)]
    [InlineData('ö', true)]
    [InlineData('õ', true)]
    [InlineData('ø', true)]
    [InlineData('ú', true)]
    [InlineData('ù', true)]
    [InlineData('û', true)]
    [InlineData('ü', true)]
    [InlineData('ý', true)]
    [InlineData('ÿ', true)]
    public void IsVowel_AccentedVowels_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsVowel(c));
    }

    [Theory]
    [InlineData('Á', true)]
    [InlineData('À', true)]
    [InlineData('Â', true)]
    [InlineData('Ä', true)]
    [InlineData('Ã', true)]
    [InlineData('Å', true)]
    [InlineData('Æ', true)]
    [InlineData('É', true)]
    [InlineData('È', true)]
    [InlineData('Ê', true)]
    [InlineData('Ë', true)]
    [InlineData('Í', true)]
    [InlineData('Ì', true)]
    [InlineData('Î', true)]
    [InlineData('Ï', true)]
    [InlineData('Ó', true)]
    [InlineData('Ò', true)]
    [InlineData('Ô', true)]
    [InlineData('Ö', true)]
    [InlineData('Õ', true)]
    [InlineData('Ø', true)]
    [InlineData('Ú', true)]
    [InlineData('Ù', true)]
    [InlineData('Û', true)]
    [InlineData('Ü', true)]
    [InlineData('Ý', true)]
    [InlineData('Ÿ', true)]
    public void IsVowel_AccentedUppercaseVowels_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsVowel(c));
    }

    [Theory]
    [InlineData('b', false)]
    [InlineData('c', false)]
    [InlineData('d', false)]
    [InlineData('f', false)]
    [InlineData('g', false)]
    [InlineData('B', false)]
    [InlineData('C', false)]
    [InlineData('D', false)]
    [InlineData('F', false)]
    [InlineData('G', false)]
    [InlineData('1', false)]
    [InlineData('!', false)]
    [InlineData(' ', false)]
    [InlineData('\n', false)]
    public void IsVowel_NonVowels_ReturnsFalse(char c, bool expected) {
        Assert.Equal(expected, char.IsVowel(c));
    }

    #endregion

    #region IsVowelUpper Tests

    [Theory]
    [InlineData('A', true)]
    [InlineData('E', true)]
    [InlineData('I', true)]
    [InlineData('O', true)]
    [InlineData('U', true)]
    [InlineData('Y', true)]
    [InlineData('Á', true)]
    [InlineData('É', true)]
    [InlineData('Í', true)]
    [InlineData('Ó', true)]
    [InlineData('Ú', true)]
    [InlineData('Ý', true)]
    public void IsVowelUpper_UppercaseVowels_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsVowelUpper(c));
    }

    [Theory]
    [InlineData('a', false)]
    [InlineData('e', false)]
    [InlineData('i', false)]
    [InlineData('o', false)]
    [InlineData('u', false)]
    [InlineData('y', false)]
    [InlineData('B', false)]
    [InlineData('C', false)]
    [InlineData('1', false)]
    [InlineData('!', false)]
    public void IsVowelUpper_NonUppercaseVowels_ReturnsFalse(char c, bool expected) {
        Assert.Equal(expected, char.IsVowelUpper(c));
    }

    #endregion

    #region IsVowelLower Tests

    [Theory]
    [InlineData('a', true)]
    [InlineData('e', true)]
    [InlineData('i', true)]
    [InlineData('o', true)]
    [InlineData('u', true)]
    [InlineData('y', true)]
    [InlineData('á', true)]
    [InlineData('é', true)]
    [InlineData('í', true)]
    [InlineData('ó', true)]
    [InlineData('ú', true)]
    [InlineData('ý', true)]
    public void IsVowelLower_LowercaseVowels_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsVowelLower(c));
    }

    [Theory]
    [InlineData('A', false)]
    [InlineData('E', false)]
    [InlineData('I', false)]
    [InlineData('O', false)]
    [InlineData('U', false)]
    [InlineData('Y', false)]
    [InlineData('b', false)]
    [InlineData('c', false)]
    [InlineData('1', false)]
    [InlineData('!', false)]
    public void IsVowelLower_NonLowercaseVowels_ReturnsFalse(char c, bool expected) {
        Assert.Equal(expected, char.IsVowelLower(c));
    }

    #endregion

    #region IsConsonant Tests

    [Theory]
    [InlineData('b', true)]
    [InlineData('c', true)]
    [InlineData('d', true)]
    [InlineData('f', true)]
    [InlineData('g', true)]
    [InlineData('h', true)]
    [InlineData('j', true)]
    [InlineData('k', true)]
    [InlineData('l', true)]
    [InlineData('m', true)]
    [InlineData('n', true)]
    [InlineData('p', true)]
    [InlineData('q', true)]
    [InlineData('r', true)]
    [InlineData('s', true)]
    [InlineData('t', true)]
    [InlineData('v', true)]
    [InlineData('w', true)]
    [InlineData('x', true)]
    [InlineData('z', true)]
    public void IsConsonant_LowercaseConsonants_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsConsonant(c));
    }

    [Theory]
    [InlineData('B', true)]
    [InlineData('C', true)]
    [InlineData('D', true)]
    [InlineData('F', true)]
    [InlineData('G', true)]
    [InlineData('H', true)]
    [InlineData('J', true)]
    [InlineData('K', true)]
    [InlineData('L', true)]
    [InlineData('M', true)]
    [InlineData('N', true)]
    [InlineData('P', true)]
    [InlineData('Q', true)]
    [InlineData('R', true)]
    [InlineData('S', true)]
    [InlineData('T', true)]
    [InlineData('V', true)]
    [InlineData('W', true)]
    [InlineData('X', true)]
    [InlineData('Z', true)]
    public void IsConsonant_UppercaseConsonants_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsConsonant(c));
    }

    [Theory]
    [InlineData('a', false)]
    [InlineData('e', false)]
    [InlineData('i', false)]
    [InlineData('o', false)]
    [InlineData('u', false)]
    [InlineData('y', false)]
    [InlineData('A', false)]
    [InlineData('E', false)]
    [InlineData('I', false)]
    [InlineData('O', false)]
    [InlineData('U', false)]
    [InlineData('Y', false)]
    [InlineData('1', false)]
    [InlineData('2', false)]
    [InlineData('!', false)]
    [InlineData('@', false)]
    [InlineData(' ', false)]
    [InlineData('\n', false)]
    public void IsConsonant_VowelsAndNonLetters_ReturnsFalse(char c, bool expected) {
        Assert.Equal(expected, char.IsConsonant(c));
    }

    #endregion

    #region IsConsonantUpper Tests

    [Theory]
    [InlineData('B', true)]
    [InlineData('C', true)]
    [InlineData('D', true)]
    [InlineData('F', true)]
    [InlineData('G', true)]
    [InlineData('H', true)]
    [InlineData('Z', true)]
    public void IsConsonantUpper_UppercaseConsonants_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsConsonantUpper(c));
    }

    [Theory]
    [InlineData('b', false)]
    [InlineData('c', false)]
    [InlineData('A', false)]
    [InlineData('E', false)]
    [InlineData('a', false)]
    [InlineData('e', false)]
    [InlineData('1', false)]
    [InlineData('!', false)]
    public void IsConsonantUpper_NonUppercaseConsonants_ReturnsFalse(char c, bool expected) {
        Assert.Equal(expected, char.IsConsonantUpper(c));
    }

    #endregion

    #region IsConsonantLower Tests

    [Theory]
    [InlineData('b', true)]
    [InlineData('c', true)]
    [InlineData('d', true)]
    [InlineData('f', true)]
    [InlineData('g', true)]
    [InlineData('h', true)]
    [InlineData('z', true)]
    public void IsConsonantLower_LowercaseConsonants_ReturnsTrue(char c, bool expected) {
        Assert.Equal(expected, char.IsConsonantLower(c));
    }

    [Theory]
    [InlineData('B', false)]
    [InlineData('C', false)]
    [InlineData('a', false)]
    [InlineData('e', false)]
    [InlineData('A', false)]
    [InlineData('E', false)]
    [InlineData('1', false)]
    [InlineData('!', false)]
    public void IsConsonantLower_NonLowercaseConsonants_ReturnsFalse(char c, bool expected) {
        Assert.Equal(expected, char.IsConsonantLower(c));
    }

    #endregion

    #region SeparateDiacritics Tests

    [Theory]
    [InlineData('a', 'a', null)]
    [InlineData('b', 'b', null)]
    [InlineData('Z', 'Z', null)]
    [InlineData('1', '1', null)]
    [InlineData('!', '!', null)]
    public void Separate_CharactersWithoutDiacritics_ReturnsBaseCharAndNullDiacritic(char input, char expectedBase, char? expectedDiacritic) {
        var result = char.SeparateDiacritics(input);
        
        Assert.Equal(expectedBase, result.BaseChar);
        Assert.Equal(expectedDiacritic, result.Diacritics);
    }

    [Fact]
    public void Separate_AccentedCharacters_ReturnsBaseCharAndDiacritic() {
        // Test á (a with acute accent)
        var result = char.SeparateDiacritics('á');
        Assert.Equal('a', result.BaseChar);
        Assert.NotNull(result.Diacritics);
        
        // Test é (e with acute accent)
        result = char.SeparateDiacritics('é');
        Assert.Equal('e', result.BaseChar);
        Assert.NotNull(result.Diacritics);
        
        // Test ñ (n with tilde)
        result = char.SeparateDiacritics('ñ');
        Assert.Equal('n', result.BaseChar);
        Assert.NotNull(result.Diacritics);
        
        // Test ç (c with cedilla)
        result = char.SeparateDiacritics('ç');
        Assert.Equal('c', result.BaseChar);
        Assert.NotNull(result.Diacritics);
    }

    [Fact]
    public void Separate_UppercaseAccentedCharacters_ReturnsCorrectBaseAndDiacritic() {
        // Test Á (uppercase A with acute accent)
        var result = char.SeparateDiacritics('Á');
        Assert.Equal('A', result.BaseChar);
        Assert.NotNull(result.Diacritics);
        
        // Test É (uppercase E with acute accent)
        result = char.SeparateDiacritics('É');
        Assert.Equal('E', result.BaseChar);
        Assert.NotNull(result.Diacritics);
    }

    [Theory]
    [InlineData('à')] // a with grave accent
    [InlineData('â')] // a with circumflex
    [InlineData('ä')] // a with diaeresis
    [InlineData('ã')] // a with tilde
    [InlineData('å')] // a with ring above
    public void Separate_DifferentDiacritics_ReturnsBaseAWithDifferentDiacritics(char input) {
        var result = char.SeparateDiacritics(input);
        
        Assert.Equal('a', result.BaseChar);
        Assert.NotNull(result.Diacritics);
        // Each should have different diacritic marks
    }

    [Fact]
    public void Separate_Space_ReturnsSpaceAndNullDiacritic() {
        var result = char.SeparateDiacritics(' ');
        
        Assert.Equal(' ', result.BaseChar);
        Assert.Null(result.Diacritics);
    }

    [Fact]
    public void Separate_Newline_ReturnsNewlineAndNullDiacritic() {
        var result = char.SeparateDiacritics('\n');
        
        Assert.Equal('\n', result.BaseChar);
        Assert.Null(result.Diacritics);
    }

    #endregion
}