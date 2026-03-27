using NeoKolors.Common;

namespace NeoKolors.Console.Tests;

// Use Collection to prevent parallel execution conflicts if other tests use Console
[Collection("ConsoleTests")]
public class NKConsoleOutTests : IDisposable {
    private readonly TextWriter _originalOut;
    private readonly StringWriter _stringWriter;

    public NKConsoleOutTests() {
        _originalOut = System.Console.Out;
        _stringWriter = new StringWriter();
        System.Console.SetOut(_stringWriter);
    }

    public void Dispose() {
        System.Console.SetOut(_originalOut);
        _stringWriter.Dispose();
    }

    [Fact]
    public void Write_WithColor_WritesAnsiCodes() {
        // Arrange
        var text = "Hello";
        var color = NKConsoleColor.RED;

        // Act
        NKConsole.Write(text, color);

        // Assert
        var output = _stringWriter.ToString();
        Assert.Contains(text, output);
        // We expect some ANSI code.
        Assert.True(output.Length > text.Length); 
    }

    [Fact]
    public void WriteTable_SimpleData_FormatsCorrectly() {
        // Arrange
        var header = new[] { "ID", "Name" };
        var data = new[] {
            new { ID = 1, Name = "Alice" },
            new { ID = 2, Name = "Bob" }
        };

        // Act
        NKConsole.WriteTable(header, NKTableStyle.ASCII, data);

        // Assert
        var output = _stringWriter.ToString();
        Assert.Contains("ID", output);
        Assert.Contains("Name", output);
        Assert.Contains("Alice", output);
        Assert.Contains("Bob", output);
        Assert.Contains("|", output); // ASCII style separator
    }

    [Fact]
    public void WriteLine_WithColor_WritesNewline() {
         // Arrange
        var text = "HelloLine";
        var color = NKConsoleColor.BLUE;

        // Act
        NKConsole.WriteLine(text, color);

        // Assert
        var output = _stringWriter.ToString();
        Assert.Contains(text, output);
        Assert.EndsWith(Environment.NewLine, output);
    }
}
