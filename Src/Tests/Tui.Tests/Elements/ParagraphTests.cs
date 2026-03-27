//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Tests.Elements;

public class ParagraphTests {

    [Fact]
    public void Constructor_ShouldInitializeWithText() {
        var p = new Paragraph("Lorem Ipsum");
        Assert.Equal("Lorem Ipsum", p.Content);
    }

    [Fact]
    public void DefaultWidth_ShouldBe100Percent() {
        var p = new Paragraph("Text");
        
        // We can't access DefaultWidth directly as it is protected, 
        // but we can check the Width property if it defaults to that.
        // However, property getter gets from Style.
        
        // Let's check layout behavior instead.
        // If width is 100%, render size width should equal parent width (minus margins/padding).
        
        var parentSize = new Size(50, 20);
        var renderSize = p.GetRenderSize(parentSize);
        
        Assert.Equal(50, renderSize.Width);
    }
}
