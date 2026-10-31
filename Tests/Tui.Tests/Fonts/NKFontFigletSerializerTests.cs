using System.Text;
using System.Xml.Serialization;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Xml.V3;

namespace NeoKolors.Tui.Tests.Fonts;

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
        using var canvas = new NKCharCanvas(10, 5);
        font.DrawString("A$", canvas); // Render 'A' and '$' (space)
        
        // A is at x=0. Glyph A has 'A' on both lines.
        Assert.Equal('A', canvas.Data[0, 0].Char);
        Assert.Equal('A', canvas.Data[0, 1].Char);
        
        // '$' is space, so canvas at x=1 should remain empty/transparent (which defaults to ' ')
        Assert.Equal(' ', canvas.Data[1, 0].Char);
        Assert.Equal(' ', canvas.Data[1, 1].Char);
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
            <LicenseType>MPL-2.0</LicenseType>
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
        Assert.Equal("MPL-2.0", config.LicenseType);
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
        Assert.Equal("Figlet", font.Info.SourceFormat);
    }

    [Fact]
    public void FigletMetadata_FromXml_ShouldParseFields() {
        const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Font Author=""Glenn Chappell"" LicenseType=""Attribution"" LetterSpacing=""1"">
    <Metadata>
        <Year>1993</Year>
        <Description>Classic 3D shadow font</Description>
    </Metadata>
</Font>";

        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var meta = FigletMetadata.FromXml(ms);

        Assert.NotNull(meta);
        Assert.Equal("Glenn Chappell", meta.Author);
        Assert.Equal("Attribution", meta.LicenseType);
        Assert.Equal(1, meta.LetterSpacing);
        Assert.Equal("1993", meta.Custom["Year"]);
        Assert.Equal("Classic 3D shadow font", meta.Custom["Description"]);
    }

    [Fact]
    public void FigletMetadata_FromManifestXml_ShouldParseMultipleFonts() {
        const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<FigletFonts>
    <Font Name=""Shadow"" Author=""Glenn Chappell"" LicenseType=""Attribution"">
        <Metadata>
            <Year>1993</Year>
        </Metadata>
    </Font>
    <Font Name=""Standard"" Author=""Glenn Chappell &amp; Ian Chai"" LicenseType=""Attribution"">
        <Metadata>
            <Year>1991</Year>
        </Metadata>
    </Font>
</FigletFonts>";

        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var dict = FigletMetadata.FromManifestXml(ms);

        Assert.Equal(2, dict.Count);
        Assert.True(dict.ContainsKey("shadow"));
        Assert.True(dict.ContainsKey("standard"));
        Assert.Equal("Glenn Chappell", dict["shadow"].Author);
        Assert.Equal("1993", dict["shadow"].Custom["Year"]);
        Assert.Equal("Glenn Chappell & Ian Chai", dict["standard"].Author);
        Assert.Equal("1991", dict["standard"].Custom["Year"]);
    }

    [Fact]
    public void FigletMetadata_MergeWith_ShouldOverrideCorrectly() {
        var baseMeta = new FigletMetadata {
            Author = "Original Author",
            LicenseType = "Original License",
            LetterSpacing = 1
        };
        baseMeta.Custom["Year"] = "1990";
        baseMeta.Custom["KeepMe"] = "Present";

        var overrideMeta = new FigletMetadata {
            Author = "New Author",
            LetterSpacing = 2
        };
        overrideMeta.Custom["Year"] = "2026";
        overrideMeta.Custom["Added"] = "NewVal";

        baseMeta.MergeWith(overrideMeta);

        Assert.Equal("New Author", baseMeta.Author);
        Assert.Equal("Original License", baseMeta.LicenseType); // Preserved from base
        Assert.Equal(2, baseMeta.LetterSpacing);
        Assert.Equal("2026", baseMeta.Custom["Year"]); // Overwritten
        Assert.Equal("Present", baseMeta.Custom["KeepMe"]); // Preserved
        Assert.Equal("NewVal", baseMeta.Custom["Added"]); // Added
    }

    [Fact]
    public void DeserializeFiglet_WithExplicitMetadata_ShouldPopulateFontInfo() {
        var sb = new StringBuilder();
        sb.AppendLine("flf2a$ 2 2 10 15 1 0 0");
        sb.AppendLine("Default comment");

        void AppendChar(char top, char bottom) {
            sb.AppendLine($"{top}@");
            sb.AppendLine($"{bottom}@@");
        }

        for (int i = 32; i <= 126; i++) {
            AppendChar((char)i, (char)i);
        }
        for (int i = 0; i < 7; i++) {
            AppendChar('G', 'G');
        }

        var customMeta = new FigletMetadata {
            Author = "Custom Author",
            LicenseType = "MIT",
            LetterSpacing = 3
        };
        customMeta.Custom["Category"] = "Banner";

        using var reader = new StringReader(sb.ToString());
        var font = NKFontSerializer.DeserializeFiglet(reader, "CustomFiglet", metadata: customMeta);

        Assert.NotNull(font);
        Assert.Equal("CustomFiglet", font.Info.Name);
        Assert.Equal("Figlet", font.Info.SourceFormat);
        Assert.Equal("Custom Author", font.Info.Author);
        Assert.Equal("MIT", font.Info.LicenseType);
        Assert.Equal(3, font.Info.LetterSpacing);
        Assert.Equal("Banner", font.Info.GetMetadata("category"));
    }
}
