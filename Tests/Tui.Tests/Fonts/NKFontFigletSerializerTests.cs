using System.IO;
using System.Text;
using System.Xml.Serialization;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Serialization;
using NeoKolors.Tui.Fonts.Serialization.Xml.V3;

namespace NeoKolors.Tui.Tests;

public class NKFontFigletSerializerTests {

    [Fact]
    public void DeserializeFiglet_ShouldParseCorrectly() {
        // 1. Build a minimal valid FIGlet font definition in memory
        var sb = new StringBuilder();
        
        // Header line: flf2a$, height=2, baseline=2, max_len=10, old_layout=15, comment_lines=0
        sb.AppendLine("flf2a$ 2 2 10 15 0 0 0");

        // Helper to append a character block
        void AppendChar(char top, char bottom) {
            sb.AppendLine($"{top}@");
            sb.AppendLine($"{bottom}@@");
        }

        // Standard ASCII characters 32 to 126 (95 characters)
        // Let's generate a simple pattern for each
        for (int i = 32; i <= 126; i++) {
            char c = (char)i;
            if (c == '$') {
                // If it is the hardblank, represent it using the hardblank character in the file
                AppendChar('$', '$');
            }
            else {
                AppendChar(c, c);
            }
        }

        // German extended characters (127 to 133 in FIGlet order)
        // Ä, Ö, Ü, ä, ö, ü, ß
        for (int i = 0; i < 7; i++) {
            AppendChar('G', 'G');
        }

        // A code-tagged character for Unicode Omega (0x03A9)
        sb.AppendLine("0x03A9 Omega");
        AppendChar('O', 'M');

        string figletFontContent = sb.ToString();

        // 2. Deserialize from string reader
        using var reader = new StringReader(figletFontContent);
        var font = NKFontSerializer.DeserializeFiglet(reader, "TestFiglet");

        // 3. Verify font attributes
        Assert.NotNull(font);
        Assert.Equal("TestFiglet", font.Name);
        Assert.Equal(2, font.Info.Leading);
        Assert.Equal(0, font.Info.LetterSpacing);
        Assert.Equal(1, font.Info.WordSpacing); // Space width is 1 (since we generated space as ' ' / '$')
        Assert.Equal(FontProportionsInfo.ProportionType.VARIABLE, font.Info.ProportionType);

        // 4. Verify glyphs
        Assert.True(font.Glyphs.ContainsKey(NKGlyphSymbol.Simple('A')));
        var glyphA = font.Glyphs[NKGlyphSymbol.Simple('A')];
        Assert.Equal(1, glyphA.Width);
        Assert.Equal(2, glyphA.Height);
        Assert.Equal(0, glyphA.BaselineOffset); // baseline(2) - height(2) = 0

        // Verify hardblank replacement (the '$' glyph should be rendered as spaces)
        Assert.True(font.Glyphs.ContainsKey(NKGlyphSymbol.Simple('$')));
        var glyphDollar = font.Glyphs[NKGlyphSymbol.Simple('$')];
        Assert.Equal(1, glyphDollar.Width);
        Assert.Equal(2, glyphDollar.Height);
        Assert.Equal(GlyphCellType.BACKGROUND, glyphDollar.Glyph[0, 0].Type); // Dollar should be space/background

        // Verify code-tagged character (Omega)
        Assert.True(font.Glyphs.ContainsKey(NKGlyphSymbol.Simple('\u03A9')));
        var glyphOmega = font.Glyphs[NKGlyphSymbol.Simple('\u03A9')];
        Assert.Equal(1, glyphOmega.Width);
        Assert.Equal(2, glyphOmega.Height);
        Assert.Equal('O', glyphOmega.Glyph[0, 0].Character);
        Assert.Equal('M', glyphOmega.Glyph[0, 1].Character);

        // 5. Test rendering
        var canvas = new NKCharCanvas(10, 5);
        font.PlaceString("A$", canvas); // Render 'A' and '$' (space)
        
        // A is at x=0. Glyph A has 'A' on both lines.
        Assert.Equal('A', canvas[0, 0].Char);
        Assert.Equal('A', canvas[0, 1].Char);
        
        // '$' is space, so canvas at x=1 should remain empty/transparent (which defaults to ' ')
        Assert.Equal(' ', canvas[1, 0].Char);
        Assert.Equal(' ', canvas[1, 1].Char);
    }

    [Fact]
    public void TryDeserializeFiglet_WithInvalidPath_ReturnsErrorsAsValues() {
        var result = NKFontSerializer.TryDeserializeFiglet("nonexistent_path_to_figlet_font.flf");
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Font);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void XmlFontConfig_WithMetadata_ShouldDeserializeCorrectly() {
        string xml = @"<FontConfig>
            <Name>TestFont</Name>
            <Author>Test Author</Author>
            <LicenseType>MIT</LicenseType>
            <LicenseFile>LICENSE.txt</LicenseFile>
            <Ligatures>true</Ligatures>
            <LetterSpacing>2</LetterSpacing>
            <Leading>5</Leading>
            <Variable>
                <Kerning>true</Kerning>
                <WordSpacing>5</WordSpacing>
            </Variable>
            <Defaults/>
        </FontConfig>";

        var serializer = new XmlSerializer(typeof(XmlFontConfig));
        using var reader = new StringReader(xml);
        var config = (XmlFontConfig)serializer.Deserialize(reader)!;

        Assert.NotNull(config);
        Assert.Equal("TestFont", config.Name);
        Assert.Equal("Test Author", config.Author);
        Assert.Equal("MIT", config.LicenseType);
        Assert.Equal("LICENSE.txt", config.LicenseFile);
    }

    [Fact]
    public void XmlFontConfig_WithoutMetadata_ShouldDeserializeCorrectly() {
        string xml = @"<FontConfig>
            <Name>TestFont</Name>
            <Ligatures>true</Ligatures>
            <LetterSpacing>2</LetterSpacing>
            <Leading>5</Leading>
            <Variable>
                <Kerning>true</Kerning>
                <WordSpacing>5</WordSpacing>
            </Variable>
            <Defaults/>
        </FontConfig>";

        var serializer = new XmlSerializer(typeof(XmlFontConfig));
        using var reader = new StringReader(xml);
        var config = (XmlFontConfig)serializer.Deserialize(reader)!;

        Assert.NotNull(config);
        Assert.Equal("TestFont", config.Name);
        Assert.Null(config.Author);
        Assert.Null(config.LicenseType);
    }

    [Fact]
    public void DeserializeFiglet_WithMetadataInComments_ShouldParseCorrectly() {
        var sb = new StringBuilder();
        
        // Header line: flf2a$, height=2, baseline=2, max_len=10, old_layout=15, comment_lines=3
        sb.AppendLine("flf2a$ 2 2 10 15 3 0 0");
        sb.AppendLine("by John Doe");
        sb.AppendLine("License: Apache-2.0");
        sb.AppendLine("Some other comment line");

        // Helper to append a character block
        void AppendChar(char top, char bottom) {
            sb.AppendLine($"{top}@");
            sb.AppendLine($"{bottom}@@");
        }

        // Standard ASCII characters 32 to 126 (95 characters)
        for (int i = 32; i <= 126; i++) {
            AppendChar((char)i, (char)i);
        }

        // German extended characters (127 to 133 in FIGlet order)
        for (int i = 0; i < 7; i++) {
            AppendChar('G', 'G');
        }

        using var reader = new StringReader(sb.ToString());
        var font = NKFontSerializer.DeserializeFiglet(reader, "TestFigletWithMetadata");

        Assert.NotNull(font);
        Assert.Equal("John Doe", font.Info.Author);
        Assert.Equal("Apache-2.0", font.Info.LicenseType);
    }
}
