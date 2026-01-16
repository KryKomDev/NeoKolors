using NeoKolors.Common;

namespace NeoKolors.Console.Tests;

public class ExceptionFormatterTests {

    [Fact]
    public void Properties_DelegateToFormatCorrectly() {
        // Arrange
        var formatter = new ExceptionFormatter();
        var style = new NKStyle(NKConsoleColor.RED);
        var color = NKConsoleColor.BLUE;

        // Act
        formatter.ShowHighlight = false;
        formatter.HighlightColor = color;
        formatter.MessageStyle = style;
        formatter.ExceptionTypeStyle = style;
        formatter.ExceptionNamespaceStyle = style;
        formatter.FileNameStyle = style;
        formatter.PathStyle = style;
        formatter.MethodStyle = style;
        formatter.MethodSourceStyle = style;
        formatter.MethodArgumentsStyle = style;
        formatter.LineNumberStyle = style;
        formatter.HelpLinkStyle = style;

        // Assert
        Assert.False(formatter.Config.ShowHighlight);
        Assert.Equal((NKColor)color, formatter.Config.HighlightColor);
        
        Assert.Equal(style, formatter.Config.MessageStyle);
        Assert.Equal(style, formatter.Config.ExceptionTypeStyle);
        Assert.Equal(style, formatter.Config.ExceptionNamespaceStyle);
        Assert.Equal(style, formatter.Config.FileNameStyle);
        Assert.Equal(style, formatter.Config.PathStyle);
        Assert.Equal(style, formatter.Config.MethodStyle);
        Assert.Equal(style, formatter.Config.MethodSourceStyle);
        Assert.Equal(style, formatter.Config.MethodArgumentsStyle);
        Assert.Equal(style, formatter.Config.LineNumberStyle);
        Assert.Equal(style, formatter.Config.HelpLinkStyle);
        
        // Check reading back
        Assert.False(formatter.ShowHighlight);
        Assert.Equal((NKColor)color, formatter.HighlightColor);
        Assert.Equal(style, formatter.MessageStyle);
    }

    [Fact]
    public void Format_SimpleException_ContainsBasicInfo() {
        // Arrange
        var formatter = new ExceptionFormatter();
        var message = "Test Message";
        var ex = new Exception(message);

        // Act
        var result = formatter.Format(ex);

        // Assert
        Assert.Contains(message, result);
        Assert.Contains(nameof(Exception), result); // Type name
        Assert.Contains("System", result); // Namespace
    }

    [Fact]
    public void Format_WithHighlight_PrefixesLines() {
        // Arrange
        var formatter = new ExceptionFormatter();
        formatter.ShowHighlight = true;
        var ex = new Exception("Test");

        // Act
        var result = formatter.Format(ex);

        // Assert
        Assert.Contains("▍ ", result);
    }
    
    [Fact]
    public void Format_WithoutHighlight_NoPrefix() {
        // Arrange
        var formatter = new ExceptionFormatter();
        formatter.ShowHighlight = false;
        var ex = new Exception("Test");

        // Act
        var result = formatter.Format(ex);

        // Assert
        Assert.DoesNotContain("▍ ", result);
    }

    [Fact]
    public void RedirectToLog_Property_RoundTrips() {
        // Arrange
        var formatter = new ExceptionFormatter();

        // Act
        formatter.RedirectToLog = false;

        // Assert
        Assert.False(formatter.RedirectToLog);
        
        formatter.RedirectToLog = true;
        Assert.True(formatter.RedirectToLog);
    }

    [Fact]
    public void Format_WithHelpLink_ContainsLink() {
        // Arrange
        var formatter = new ExceptionFormatter();
        formatter.ShowHighlight = false;
        var ex = new Exception("Test") {
            HelpLink = "http://example.com"
        };

        // Act
        var result = formatter.Format(ex);

        // Assert
        Assert.Contains("http://example.com", result);
        Assert.Contains("For more information about this exception, see:", result);
    }
}
