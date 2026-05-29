//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Console.Tests;

public class NKDebugTests : IDisposable {
    private readonly StringWriter _stringWriter;
    private readonly TextWriter _originalOutput;

    public NKDebugTests() {
        _stringWriter = new StringWriter();
        _originalOutput = NKDebug.GetOutput();
        NKDebug.SetOutput(_stringWriter);
    }

    public void Dispose() {
        NKDebug.SetOutput(_originalOutput);
        _stringWriter.Dispose();
    }

    [Fact]
    public void GetLogger_WithString_ReturnsConfiguredLogger() {
        // Arrange
        const string source = "TestSource";

        // Act
        var logger = NKDebug.GetLogger(source);

        // Assert
        Assert.NotNull(logger);
        Assert.Equal(source, logger.Source);
    }

    [Fact]
    public void GetLogger_WithGeneric_ReturnsConfiguredLogger() {
        // Act
        var logger = NKDebug.GetLogger<NKDebugTests>();

        // Assert
        Assert.NotNull(logger);
        Assert.Equal(nameof(NKDebugTests), logger.Source);
    }

    [Theory]
    [InlineData("Trace message")]
    [InlineData("Debug message")]
    [InlineData("Info message")]
    [InlineData("Warning message")]
    [InlineData("Error message")]
    [InlineData("Critical message")]
    public void LoggingMethods_WithStringMessage_WritesToOutput(string message) {
        // Arrange
        NKDebug.SetLogAll();

        // Act
        NKDebug.Trace(message);
        NKDebug.Debug(message);
        NKDebug.Info(message);
        NKDebug.Warn(message);
        NKDebug.Error(message);
        NKDebug.Crit(message);

        // Assert
        var output = _stringWriter.ToString();
        Assert.Contains(message, output);
    }

    [Fact]
    public void StructuredLogging_WithParameters_FormatsCorrectly() {
        // Arrange
        NKDebug.SetLogAll();
        const string template = "User {UserId} performed action {Action}";
        const int userId = 123;
        const string action = "Login";

        // Act
        NKDebug.Info(template, userId, action);

        // Assert
        var output = _stringWriter.ToString();
        Assert.Contains("123", output);
        Assert.Contains("Login", output);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExceptionFormatting_PropertySetter_ConfiguresUnhandledException(bool enabled) {
        // Act
        NKDebug.ExceptionFormatting = enabled;

        // Assert
        Assert.Equal(enabled, NKDebug.ExceptionFormatting);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void RedirectFatalToLog_PropertySetter_ConfiguresRedirection(bool enabled) {
        // Act
        NKDebug.RedirectFatalToLog = enabled;

        // Assert
        Assert.Equal(enabled, NKDebug.RedirectFatalToLog);
    }

    [Fact]
    public void SetLogLevel_Methods_ConfigureAppropriately() {
        // Test SetLogAll
        NKDebug.SetLogAll();
        Assert.True(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.TRACE));
        Assert.True(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.CRITICAL));

        // Test SetLogInfo
        NKDebug.SetLogInfo();
        Assert.True(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.INFORMATION));
        Assert.False(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.DEBUG));

        // Test SetLogWarn
        NKDebug.SetLogWarn();
        Assert.True(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.WARNING));
        Assert.False(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.INFORMATION));

        // Test SetLogErrors
        NKDebug.SetLogErrors();
        Assert.True(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.ERROR));
        Assert.False(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.WARNING));

        // Test SetLogCrit
        NKDebug.SetLogCrit();
        Assert.True(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.CRITICAL));
        Assert.False(NKDebug.Logger.Config.Level.HasFlag(LoggerLevel.ERROR));

        // Test SetLogNone
        NKDebug.SetLogNone();
        Assert.Equal(LoggerLevel.NONE, NKDebug.Logger.Config.Level);
    }
}