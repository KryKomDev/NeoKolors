namespace NeoKolors.Console.Tests;

public class NKLoggerTests {

    [Fact]
    public void Info_WhenEnabled_WritesToOutput() {
        // Arrange
        using var stringWriter = new StringWriter();
        var config = new LoggerConfig {
            Output = stringWriter,
            Level = LoggerLevel.INFORMATION,
            SimpleMessages = true // simpler for assertion
        };
        // NKLogger disposes the writer/config output if configured, so use using or be careful.
        // But here we provided the writer, NKLogger will likely just reference it.
        // Actually NKLogger disposes OutputReference which might close the writer.
        // We will manually dispose.
        using var logger = new NKLogger(config);
        var message = "Test Info Message";

        // Act
        logger.Info(message);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains(message, output);
        Assert.Contains("[ INFO ]", output);
    }

    [Fact]
    public void Debug_WhenDisabled_DoesNotWrite() {
        // Arrange
        using var stringWriter = new StringWriter();
        var config = new LoggerConfig {
            Output = stringWriter,
            Level = LoggerLevel.INFORMATION, // Debug not included
            SimpleMessages = true
        };
        using var logger = new NKLogger(config);
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
        var config = new LoggerConfig {
            Output = stringWriter,
            Level = LoggerLevel.INFORMATION,
            SimpleMessages = true
        };
        using var logger = new NKLogger(config, source);

        // Act
        logger.Info("Message");

        // Assert
        Assert.Contains($"[ {source} ]", stringWriter.ToString());
    }

    [Fact]
    public void Properties_DelegateToConfig() {
        // Arrange
        var config = new LoggerConfig();
        using var logger = new NKLogger(config);

        // Act
        logger.HideTime = true;
        logger.SimpleMessages = true;

        // Assert
        Assert.True(logger.Config.HideTime);
        Assert.True(logger.Config.SimpleMessages);
    }
}
