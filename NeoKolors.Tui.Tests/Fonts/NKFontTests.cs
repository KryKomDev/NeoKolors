//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Exceptions;
using static NeoKolors.Tui.Fonts.Serialization.Xml.AutoCompoundApplicableGroup;

namespace NeoKolors.Tui.Tests;

public class NKFontTests {

    private NKComponentGlyph CreateGlyph(char c) {
        return new NKComponentGlyph(new char?[,] { { c } });
    }

    private NKFontInfo CreateFontInfo() {
        // Monospace font info
        var mono = new MonospaceInfo(1, 1, true);
        return new NKFontInfo("TestFont", mono, true, 0, 0);
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var info = CreateFontInfo();
        var glyphA = CreateGlyph('A');
        var glyphs = new GlyphInfo[] {
            new SimpleGlyphInfo(glyphA, 'A')
        };
        
        var font = new NKFont(info, glyphs);
        
        Assert.True(font.HasSimple('A'));
        Assert.False(font.HasSimple('B'));
    }

    [Fact]
    public void Constructor_ShouldThrowOnDuplicateSimpleGlyph() {
        var info = CreateFontInfo();
        var glyphA = CreateGlyph('A');
        var glyphs = new GlyphInfo[] {
            new SimpleGlyphInfo(glyphA, 'A'),
            new SimpleGlyphInfo(glyphA, 'A')
        };
        
        Assert.Throws<NKFontException>(() => new NKFont(info, glyphs));
    }

    [Fact]
    public void Tokenize_ShouldIdentifySimpleGlyphs() {
        var info = CreateFontInfo();
        var glyphA = CreateGlyph('A');
        var glyphB = CreateGlyph('B');
        var glyphs = new GlyphInfo[] {
            new SimpleGlyphInfo(glyphA, 'A'),
            new SimpleGlyphInfo(glyphB, 'B')
        };
        
        var font = new NKFont(info, glyphs);
        var tokens = NKFontStringTokenizer.Tokenize("ABA", font);
        
        Assert.Equal(3, tokens.Length);
        Assert.Equal(NKFontStringTokenizer.TokenType.SIMPLE, tokens[0].Type);
        Assert.Equal('A', tokens[0].Data!.Value.AsT0.Character);
        Assert.Equal(NKFontStringTokenizer.TokenType.SIMPLE, tokens[1].Type);
        Assert.Equal('B', tokens[1].Data!.Value.AsT0.Character);
        Assert.Equal(NKFontStringTokenizer.TokenType.SIMPLE, tokens[2].Type);
        Assert.Equal('A', tokens[2].Data!.Value.AsT0.Character);
    }

    [Fact]
    public void Tokenize_ShouldIdentifyLigatures() {
        var info = CreateFontInfo();
        var glyphEq = CreateGlyph('=');
        var glyphEqEq = CreateGlyph('='); // Represents "=="
        var glyphs = new GlyphInfo[] {
            new SimpleGlyphInfo(glyphEq, '='),
            new LigatureGlyphInfo(glyphEqEq, "==")
        };
        
        var font = new NKFont(info, glyphs);
        
        // "===" should be tokenized as "==" (Ligature) + "=" (Simple) because of greedy matching
        var tokens = NKFontStringTokenizer.Tokenize("===", font);
        
        Assert.Equal(2, tokens.Length);
        Assert.Equal(NKFontStringTokenizer.TokenType.LIGATURE, tokens[0].Type);
        Assert.Equal("==", tokens[0].Data!.Value.AsT1.Ligature);
        
        Assert.Equal(NKFontStringTokenizer.TokenType.SIMPLE, tokens[1].Type);
        Assert.Equal('=', tokens[1].Data!.Value.AsT0.Character);
    }

    [Fact]
    public void Tokenize_ShouldIdentifyAutoCompounds() {
        var info = CreateFontInfo();
        var glyphA = CreateGlyph('a');
        var glyphGrave = CreateGlyph('`');
        
        var glyphs = new GlyphInfo[] {
            new SimpleGlyphInfo(glyphA, 'a'),
            // '`' combines with letters
            new AutoCompoundGlyphInfo(glyphGrave, '`', CompoundGlyphAlignment.Center(), LETTERS_LOWER) 
        };
        
        var font = new NKFont(info, glyphs);
        
        // "a`" should be tokenized as AutoCompound
        // Assuming tokenizer checks 'next char' logic
        
        var tokens = NKFontStringTokenizer.Tokenize("a`", font);
        
        Assert.Single(tokens);
        Assert.Equal(NKFontStringTokenizer.TokenType.AUTO_COMPOUND, tokens[0].Type);
        Assert.Equal('a', tokens[0].Data!.Value.AsT2.Main); // Main/First char in token
        Assert.Equal('`', tokens[0].Data!.Value.AsT2.Second); // Second char in token
    }

    [Fact]
    public void Tokenize_ShouldIdentifySpacesAndNewlines() {
        var info = CreateFontInfo();
        var glyphA = CreateGlyph('A');
        var glyphs = new GlyphInfo[] {
            new SimpleGlyphInfo(glyphA, 'A')
        };
        var font = new NKFont(info, glyphs);
        
        var tokens = NKFontStringTokenizer.Tokenize("A \n A", font);
        
        // Tokens: "A", " ", "\n", " ", "A"
        // Wait, Tokenizer logic:
        // skip spaces -> create SpaceToken
        // skip newlines -> create NewlineToken
        
        Assert.Equal(4, tokens.Length);
        Assert.Equal(NKFontStringTokenizer.TokenType.SIMPLE, tokens[0].Type);
        Assert.Equal(NKFontStringTokenizer.TokenType.NEWLINE, tokens[1].Type);
        Assert.Equal(NKFontStringTokenizer.TokenType.SPACE, tokens[2].Type);
        Assert.Equal(NKFontStringTokenizer.TokenType.SIMPLE, tokens[3].Type);
    }
    
    [Fact]
    public void Tokenize_ShouldHandleInvalidCharacters() {
        var info = CreateFontInfo();
        var glyphs = Array.Empty<GlyphInfo>();
        var font = new NKFont(info, glyphs);
        
        var tokens = NKFontStringTokenizer.Tokenize("X", font);
        
        Assert.Single(tokens);
        Assert.Equal(NKFontStringTokenizer.TokenType.INVALID, tokens[0].Type);
    }

    [Fact]
    public void GetSize_ShouldReturnCorrectDimensions() {
        // Monospace font 1x1
        var mono = new MonospaceInfo(1, 1, true);
        var info = new NKFontInfo("MonoFont", mono, false, 0, 0);
        var glyphA = CreateGlyph('A');
        var glyphs = new GlyphInfo[] { new SimpleGlyphInfo(glyphA, 'A') };
        var font = new NKFont(info, glyphs);
        
        var size = font.GetSize("AAA", 100);
        
        // 3 chars, each 1x1. Spacing 0.
        // Width should be 3. Height 1.
        Assert.Equal(3, size.Width);
        Assert.Equal(1, size.Height);
    }
}
