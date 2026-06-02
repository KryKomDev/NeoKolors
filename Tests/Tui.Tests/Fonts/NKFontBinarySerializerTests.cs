// NeoKolors
// Copyright (c) KryKom 2026

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Fonts.Serialization;

namespace NeoKolors.Tui.Tests;

public class NKFontBinarySerializerTests {

    private const string PATH = "NeoKolors.Tui.Tests.Fonts.Assets.Dummy.nkf";

    [Fact]
    public void SerializeAndDeserialize_ShouldBeIdentical() {
        // 1. Load the compiled NKFont using the XML deserializer
        var originalFont = NKFontSerializer.ReadEmbedded<NKFontBinarySerializerTests>(PATH);
        Assert.NotNull(originalFont);

        // 2. Serialize directly to a MemoryStream in binary format
        using var memoryStream = new MemoryStream();
        NKFontSerializer.SerializeBinary(originalFont, memoryStream);
        memoryStream.Position = 0;

        // 3. Deserialize back into a new NKFont instance
        var deserializedFont = NKFontSerializer.DeserializeBinary(memoryStream);
        Assert.NotNull(deserializedFont);

        // 4. Validate NKFontInfo metadata
        Assert.Equal(originalFont.Info.Name, deserializedFont.Info.Name);
        Assert.Equal(originalFont.Info.Ligatures, deserializedFont.Info.Ligatures);
        Assert.Equal(originalFont.Info.Leading, deserializedFont.Info.Leading);
        Assert.Equal(originalFont.Info.LetterSpacing, deserializedFont.Info.LetterSpacing);
        Assert.Equal(originalFont.Info.WordSpacing, deserializedFont.Info.WordSpacing);
        Assert.Equal(originalFont.Info.ProportionType, deserializedFont.Info.ProportionType);

        if (originalFont.Info.ProportionType == FontProportionsInfo.ProportionType.MONOSPACED) {
            Assert.Equal(originalFont.Info.FontPropoInfo.AsMono.GlyphWidth, deserializedFont.Info.FontPropoInfo.AsMono.GlyphWidth);
            Assert.Equal(originalFont.Info.FontPropoInfo.AsMono.GlyphHeight, deserializedFont.Info.FontPropoInfo.AsMono.GlyphHeight);
            Assert.Equal(originalFont.Info.FontPropoInfo.AsMono.AlignToGrid, deserializedFont.Info.FontPropoInfo.AsMono.AlignToGrid);
        } 
        else {
            Assert.Equal(originalFont.Info.FontPropoInfo.AsVar.Kerning, deserializedFont.Info.FontPropoInfo.AsVar.Kerning);
        }

        // 5. Validate Glyphs Dictionary counts
        Assert.Equal(originalFont.Glyphs.Count, deserializedFont.Glyphs.Count);

        // 6. Visual and Rendering equivalence check
        // Render a complex string containing simple chars, spaces, ligatures, and diacritics
        var originalCanvas = new NKCharCanvas(80, 20);
        var deserializedCanvas = new NKCharCanvas(80, 20);

        var testString = "A B c -> y ů Ů Á Ä Č Ď É Ě Ë Í Ï Ľ Ň Ó Ö Ř Š Ť Ú Ů Ü ý ÿ Ž!";
        
        originalFont.PlaceString(testString, originalCanvas);
        deserializedFont.PlaceString(testString, deserializedCanvas);

        for (int y = 0; y < originalCanvas.Height; y++) {
            for (int x = 0; x < originalCanvas.Width; x++) {
                var origCell = originalCanvas[x, y];
                var deserCell = deserializedCanvas[x, y];
                
                Assert.Equal(origCell.Char, deserCell.Char);
                Assert.Equal(origCell.Style, deserCell.Style);
            }
        }
    }

    [Fact]
    public void DeserializeXml_WithRelativePath_DoesNotThrowInvalidOperationException() {
        // Calling DeserializeXml with a relative path should gracefully return null (or resolve it),
        // rather than throwing "This operation is not supported for a relative URI."
        var exception = Record.Exception(() => NKFontSerializer.DeserializeXml("some/relative/path/to/font"));
        Assert.Null(exception);
    }

    [Fact]
    public void TryDeserializeXml_ReturnsErrorsAndWarningsAsValues() {
        // Calling TryDeserializeXml with a non-existent path should return a failed result 
        // with error messages instead of executing a side-effect or throwing.
        var result = NKFontSerializer.TryDeserializeXml("some/relative/path/to/font");
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Null(result.Font);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, err => err.Contains("Cannot resolve path") || err.Contains("does not exist"));
    }
}
