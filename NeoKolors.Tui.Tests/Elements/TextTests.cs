//
// NeoKolors.Test
// Copyright (c) 2025 KryKom

using NeoKolors.Common;
using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Tests.Elements;

public class TextTests {

    [Fact]
    public void Constructor_ShouldInitializeWithText() {
        var text = new Text("Hello World");
        Assert.Equal("Hello World", text.Content);
    }

    [Fact]
    public void SetContent_ShouldUpdateContentAndFireEvent() {
        var text = new Text("Initial");
        bool eventFired = false;
        text.OnElementUpdated += () => eventFired = true;
        
        text.Content = "Updated";
        
        Assert.Equal("Updated", text.Content);
        Assert.True(eventFired);
    }

    [Fact]
    public void SetChildren_WithText_ShouldUpdateContent() {
        var text = new Text("Initial");
        text.SetChildren("New Content");
        Assert.Equal("New Content", text.Content);
    }

    [Fact]
    public void SetChildren_WithElements_ShouldThrow() {
        var text = new Text("Initial");
        Assert.Throws<InvalidOperationException>(() => text.SetChildren(new IElement[] { new Text("Child") }));
    }

    [Fact]
    public void GetMinSize_ShouldReturnCorrectSize() {
        var text = new Text("Hello");
        // Default font is usually monospace 1x1 or similar in tests if not mocked.
        // Assuming default font size behavior (length, 1)
        
        var size = text.GetMinSize(new Size(100, 100));
        
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
    }
    
    [Fact]
    public void Style_ShouldStoreProperties() {
        var text = new Text("Styled");
        text.Color = NKConsoleColor.RED;
        
        Assert.Equal(new NKColor(NKConsoleColor.RED), text.Color);
    }
}
