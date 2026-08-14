using System.Runtime.CompilerServices;
using NeoKolors.Common;

namespace NeoKolors.Console.Tests;

public class ExceptionFormatterTests {

    [Fact]
    public void Properties_DelegateToFormatCorrectly() {
        // Arrange
        var style = new NKStyle(NKConsoleColor.RED);
        var color = NKConsoleColor.BLUE;

        // Act
        ExceptionFormatter.ShowHighlight = false;
        ExceptionFormatter.HighlightColor = color;
        ExceptionFormatter.MessageStyle = style;
        ExceptionFormatter.ExceptionTypeStyle = style;
        ExceptionFormatter.ExceptionNamespaceStyle = style;
        ExceptionFormatter.FileNameStyle = style;
        ExceptionFormatter.PathStyle = style;
        ExceptionFormatter.MethodStyle = style;
        ExceptionFormatter.MethodSourceStyle = style;
        ExceptionFormatter.MethodArgumentsStyle = style;
        ExceptionFormatter.LineNumberStyle = style;
        ExceptionFormatter.HelpLinkStyle = style;

        // Assert
        Assert.False(ExceptionFormatter.Config.ShowHighlight);
        Assert.Equal((NKColor)color, ExceptionFormatter.Config.HighlightColor);
        
        Assert.Equal(style, ExceptionFormatter.Config.MessageStyle);
        Assert.Equal(style, ExceptionFormatter.Config.ExceptionTypeStyle);
        Assert.Equal(style, ExceptionFormatter.Config.ExceptionNamespaceStyle);
        Assert.Equal(style, ExceptionFormatter.Config.FileNameStyle);
        Assert.Equal(style, ExceptionFormatter.Config.PathStyle);
        Assert.Equal(style, ExceptionFormatter.Config.MethodStyle);
        Assert.Equal(style, ExceptionFormatter.Config.MethodSourceStyle);
        Assert.Equal(style, ExceptionFormatter.Config.MethodArgumentsStyle);
        Assert.Equal(style, ExceptionFormatter.Config.LineNumberStyle);
        Assert.Equal(style, ExceptionFormatter.Config.HelpLinkStyle);
        
        // Check reading back
        Assert.False(ExceptionFormatter.ShowHighlight);
        Assert.Equal((NKColor)color, ExceptionFormatter.HighlightColor);
        Assert.Equal(style, ExceptionFormatter.MessageStyle);

        // Reset default config for subsequent tests
        ExceptionFormatter.Config = ExceptionFormat.Default;
    }

    [Fact]
    public void Format_SimpleException_ContainsBasicInfo() {
        // Arrange
        ExceptionFormatter.Config = ExceptionFormat.Default;
        var message = "Test Message";
        var ex = new Exception(message);

        // Act
        var result = ExceptionFormatter.Format(ex).ToString();

        // Assert
        Assert.Contains(message, result);
        Assert.Contains(nameof(Exception), result); // Type name
        Assert.Contains("System", result); // Namespace
    }

    [Fact]
    public void Format_WithHighlight_PrefixesLines() {
        // Arrange
        ExceptionFormatter.ShowHighlight = true;
        var ex = new Exception("Test");

        // Act
        var result = ExceptionFormatter.Format(ex).ToString();

        // Assert
        Assert.Contains("▍ ", result);
    }
    
    [Fact]
    public void Format_WithoutHighlight_NoPrefix() {
        // Arrange
        ExceptionFormatter.ShowHighlight = false;
        var ex = new Exception("Test");

        // Act
        var result = ExceptionFormatter.Format(ex).ToString();

        // Assert
        Assert.DoesNotContain("▍ ", result);

        ExceptionFormatter.Config = ExceptionFormat.Default;
    }

    [Fact]
    public void RedirectToLog_Property_RoundTrips() {
        // Act & Assert
        ExceptionFormatter.RedirectToLog = false;
        Assert.False(ExceptionFormatter.RedirectToLog);
        
        ExceptionFormatter.RedirectToLog = true;
        Assert.True(ExceptionFormatter.RedirectToLog);
    }

    [Fact]
    public void Format_WithHelpLink_ContainsLink() {
        // Arrange
        ExceptionFormatter.ShowHighlight = false;
        var ex = new Exception("Test") {
            HelpLink = "http://example.com"
        };

        // Act
        var result = ExceptionFormatter.Format(ex).ToString();

        // Assert
        Assert.Contains("http://example.com", result);
        Assert.Contains("For more information about this exception, see:", result);

        ExceptionFormatter.Config = ExceptionFormat.Default;
    }

    [Fact]
    public void Format_WithStackTrace_Works() {
        // Arrange
        ExceptionFormatter.ShowHighlight = false;
        Exception? ex = null;
        try {
            ThrowException();
        } catch (Exception e) {
            ex = e;
        }

        // Act
        var result = ExceptionFormatter.Format(ex!).ToString();

        // Assert
        Assert.Contains("ThrowException", result);
        Assert.Contains("Format_WithStackTrace_Works", result);

        ExceptionFormatter.Config = ExceptionFormat.Default;
    }

    [Fact]
    public void Format_SystemException_DoesNotThrow() {
        // Arrange
        ExceptionFormatter.Config = ExceptionFormat.Default;
        Exception? ex = null;
        try {
            _ = int.Parse("not a number");
        } catch (Exception e) {
            ex = e;
        }

        // Act & Assert
        var result = ExceptionFormatter.Format(ex!).ToString();
        Assert.NotNull(result);
    }

    [Fact]
    public void Format_WithBoxBorder_RendersBoxFrame() {
        // Arrange
        var config = new ExceptionFormat {
            BorderStyle = ExceptionBorderStyle.BOX
        };
        var ex = new Exception("Box test");

        // Act
        var result = ExceptionFormatter.Format(ex, config).ToString();

        // Assert
        Assert.Contains("╭─", result);
        Assert.Contains("╰─", result);
        Assert.Contains("│", result);
    }

    [Fact]
    public void Format_InnerException_RendersRecursively() {
        // Arrange
        var inner = new InvalidOperationException("Inner error");
        var outer = new Exception("Outer error", inner);

        // Act
        var result = ExceptionFormatter.Format(outer).ToString();

        // Assert
        Assert.Contains("Outer error", result);
        Assert.Contains("Inner error", result);
        Assert.Contains("Inner Exception:", result);
    }

    [Fact]
    public void Format_ExceptionData_RendersKeyValues() {
        // Arrange
        var ex = new Exception("Data test");
        ex.Data["UserKey"] = "CustomValue";

        // Act
        var result = ExceptionFormatter.Format(ex).ToString();

        // Assert
        Assert.Contains("Data:", result);
        Assert.Contains("UserKey", result);
        Assert.Contains("CustomValue", result);
    }

    [Fact]
    public void Format_Presets_ApplyExpectedConfigurations() {
        // Minimal preset
        var minimalEx = new Exception("Minimal test");
        var minimalResult = ExceptionFormatter.Format(minimalEx, ExceptionFormat.Minimal).ToString();
        Assert.DoesNotContain("▍ ", minimalResult);
        Assert.Contains("Minimal test", minimalResult);

        // Detailed preset
        var detailedResult = ExceptionFormatter.Format(minimalEx, ExceptionFormat.Detailed).ToString();
        Assert.Contains("╭─", detailedResult);
    }

    [Fact]
    public void Format_PathTransformer_TransformsFilePath() {
        // Arrange
        Exception? ex = null;
        try {
            ThrowException();
        } catch (Exception e) {
            ex = e;
        }

        var config = new ExceptionFormat {
            ShowHighlight = false,
            PathTransformer = p => "[TRANSFORMED]/" + System.IO.Path.GetFileName(p)
        };

        // Act
        var result = ExceptionFormatter.Format(ex!, config).ToString();

        // Assert
        Assert.Contains("[TRANSFORMED]", result);
    }

    [Fact]
    public void Format_SourceSnippet_LeftGutterAlignedCorrectly() {
        // Arrange
        Exception? ex = null;
        try {
            ThrowException();
        } catch (Exception e) {
            ex = e;
        }

        var config = new ExceptionFormat {
            ShowHighlight = false,
            ShowSourceSnippet = true,
            SourceSnippetContextLines = 1
        };

        // Act
        var result = ExceptionFormatter.Format(ex!, config).ToString();

        // Assert
        Assert.Contains("┌─", result);
        Assert.Contains("│", result);
        Assert.Contains("➔", result);
    }

    [Fact]
    public void Format_MethodParameterTypesAndNames_HighlightedSeparately() {
        // Arrange
        var config = new ExceptionFormat {
            MethodParamTypeStyle = new NKStyle(NKConsoleColor.YELLOW),
            MethodParamNameStyle = new NKStyle(NKConsoleColor.CYAN)
        };

        Assert.Equal((NKColor)NKConsoleColor.YELLOW, config.MethodParamTypeStyle.FColor);
        Assert.Equal((NKColor)NKConsoleColor.CYAN, config.MethodParamNameStyle.FColor);

        // Check top-level delegation
        ExceptionFormatter.MethodParamTypeStyle = new NKStyle(NKConsoleColor.GREEN);
        Assert.Equal((NKColor)NKConsoleColor.GREEN, ExceptionFormatter.Config.MethodParamTypeStyle.FColor);

        ExceptionFormatter.Config = ExceptionFormat.Default;
    }

    [Fact]
    public void Format_ShowFilePathAndShowMethodNamespace_TogglesVisibility() {
        // Arrange
        Exception? ex = null;
        try {
            ThrowException();
        } catch (Exception e) {
            ex = e;
        }

        var hideConfig = new ExceptionFormat {
            ShowHighlight = false,
            ShowSourceSnippet = false,
            ShowFilePath = false,
            ShowMethodNamespace = false
        };

        // Act
        AnsiString formatted = ExceptionFormatter.Format(ex!, hideConfig);
        string result = formatted.ToString();

        // Assert
        Assert.DoesNotContain("NeoKolors.Console.Tests.", result);
        Assert.Contains("ExceptionFormatterTests", result);
        Assert.Contains("ThrowException", result);
        Assert.Contains("ExceptionFormatterTests.cs", result);
        Assert.DoesNotContain(@"Tests\Console.Tests", result);
        Assert.DoesNotContain(@"Tests/Console.Tests", result);
        
        // Check single line format: method signature directly followed by "in filename:line"
        string plainResult = formatted.Plain;
        Assert.Contains("ThrowException() in ExceptionFormatterTests.cs:line", plainResult);
    }

    [Fact]
    public void Format_StackTraceIndent_AdjustsIndentationSpaces() {
        // Arrange
        Exception? ex = null;
        try {
            ThrowException();
        } catch (Exception e) {
            ex = e;
        }

        var config = new ExceptionFormat {
            ShowHighlight = false,
            ShowSourceSnippet = true,
            SourceSnippetContextLines = 1,
            StackTraceIndent = 6
        };

        // Act
        var result = ExceptionFormatter.Format(ex!, config).Plain;

        // Assert
        Assert.Contains("      at ", result);
        Assert.Contains("        in ", result);
        Assert.Contains("┌─", result);
        Assert.Contains("      ", result); // 6 leading spaces in snippet line

        // Check top-level delegation
        ExceptionFormatter.StackTraceIndent = 1;
        Assert.Equal(1, ExceptionFormatter.Config.StackTraceIndent);
        ExceptionFormatter.Config = ExceptionFormat.Default;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowException() => throw new Exception("Error in method");
}
