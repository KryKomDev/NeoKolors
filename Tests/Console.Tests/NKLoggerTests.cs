namespace NeoKolors.Console.Tests;

public class NKLoggerTests {

    [Fact]
    public void Info_WhenEnabled_WritesToOutput() {
        // Arrange
        using var stringWriter = new StringWriter();
        var writer = new TextLogWriter(stringWriter);
        using var logger = new NKLogger(writer, level: NKLogLevel.INFORMATION);
        var message = "Test Info Message";

        // Act
        logger.Info(message);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains(message, output);
        Assert.Contains("[ info ]", output);
    }

    [Fact]
    public void Debug_WhenDisabled_DoesNotWrite() {
        // Arrange
        using var stringWriter = new StringWriter();
        var writer = new TextLogWriter(stringWriter);
        using var logger = new NKLogger(writer, level: NKLogLevel.INFORMATION);
        var message = "Test Debug Message";

        // Act
        logger.Debug(message);

        // Assert
        Assert.Empty(stringWriter.ToString());
    }

    [Fact]
    public void Source_IsIncludedInOutput() {
        // Arrange
        using var stringWriter = new StringWriter();
        var source = "TestComponent";
        var writer = new TextLogWriter(stringWriter);
        using var logger = new NKLogger(writer, source, NKLogLevel.INFORMATION);

        // Act
        logger.Info("Message");

        // Assert
        Assert.Contains($"[ {source} ]", stringWriter.ToString());
    }
}
