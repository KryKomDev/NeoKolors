//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Console.Tests;

[Collection("ConsoleTests")]
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
        Assert.True(NKDebug.Logger.Level.HasFlag(NKLogLevel.TRACE));
        Assert.True(NKDebug.Logger.Level.HasFlag(NKLogLevel.CRITICAL));

        // Test SetLogInfo
        NKDebug.SetLogInfo();
        Assert.True(NKDebug.Logger.Level.HasFlag(NKLogLevel.INFORMATION));
        Assert.False(NKDebug.Logger.Level.HasFlag(NKLogLevel.DEBUG));

        // Test SetLogWarn
        NKDebug.SetLogWarn();
        Assert.True(NKDebug.Logger.Level.HasFlag(NKLogLevel.WARNING));
        Assert.False(NKDebug.Logger.Level.HasFlag(NKLogLevel.INFORMATION));

        // Test SetLogErrors
        NKDebug.SetLogErrors();
        Assert.True(NKDebug.Logger.Level.HasFlag(NKLogLevel.ERROR));
        Assert.False(NKDebug.Logger.Level.HasFlag(NKLogLevel.WARNING));

        // Test SetLogCrit
        NKDebug.SetLogCrit();
        Assert.True(NKDebug.Logger.Level.HasFlag(NKLogLevel.CRITICAL));
        Assert.False(NKDebug.Logger.Level.HasFlag(NKLogLevel.ERROR));

        // Test SetLogNone
        NKDebug.SetLogNone();
        Assert.Equal(NKLogLevel.NONE, NKDebug.Logger.Level);
    }

    [Fact]
    public void GetLogger_InheritsWriterAndLevelFromNKDebug() {
        var logger = NKDebug.GetLogger("InheritTest");

        Assert.NotNull(logger);
        Assert.Equal("InheritTest", logger.Source);
        Assert.Same(NKDebug.Logger, logger.Parent);
        Assert.Same(NKDebug.Logger.Writer, logger.Writer);
        Assert.Equal(NKDebug.Logger.Level, logger.Level);
        Assert.Equal(NKDebug.Logger.Enabled, logger.Enabled);
    }

    [Fact]
    public void GetLogger_ReflectsChangesToNKDebugLogger() {
        var logger = NKDebug.GetLogger("SyncTest");

        // Change level via NKDebug
        NKDebug.SetLogWarn();
        Assert.Equal(NKDebug.Logger.Level, logger.Level);

        // Change output destination via NKDebug
        using var customSw = new StringWriter();
        NKDebug.SetOutput(customSw);
        Assert.Same(NKDebug.Logger.Writer, logger.Writer);

        // Writing through child logger should output to the new destination with Source included
        logger.Warn("Warning from sync logger");
        var output = customSw.ToString();
        Assert.Contains("Warning from sync logger", output);
        Assert.Contains("SyncTest", output);
    }

    [Fact]
    public void GetLogger_ReflectsReplacingNKDebugLogger() {
        var originalLogger = NKDebug.Logger;
        try {
            var child = NKDebug.GetLogger("ReplacementTest");

            using var sw = new StringWriter();
            var newRootLogger = new NKLogger(new TextLogWriter(sw), level: NKLogLevel.ERROR);
            NKDebug.Logger = newRootLogger;

            // Existing child logger should immediately reflect the new root logger
            Assert.Same(newRootLogger, child.Parent);
            Assert.Same(newRootLogger.Writer, child.Writer);
            Assert.Equal(NKLogLevel.ERROR, child.Level);

            child.Error("Error message from child");
            Assert.Contains("Error message from child", sw.ToString());
            Assert.Contains("ReplacementTest", sw.ToString());
        }
        finally {
            NKDebug.Logger = originalLogger;
        }
    }

    [Fact]
    public void GetLogger_SupportsLocalOverridesAndReset() {
        var child = NKDebug.GetLogger("OverrideTest");
        NKDebug.SetLogWarn();

        Assert.Equal(NKDebug.Logger.Level, child.Level);

        // Override child's level locally
        child.Level = NKLogLevel.DEBUG;
        Assert.Equal(NKLogLevel.DEBUG, child.Level);
        Assert.Equal(NKLogLevel.CRITICAL | NKLogLevel.ERROR | NKLogLevel.WARNING, NKDebug.Logger.Level);

        // Reset child's level back to parent
        child.ResetLevel();
        Assert.Equal(NKDebug.Logger.Level, child.Level);
    }

    [Fact]
    public void ChildLogger_DisposeDoesNotDisposeSharedParentWriter() {
        NKDebug.SetLogAll();
        var child = NKDebug.GetLogger("DisposeTest");

        // Disposing child should not dispose the shared parent writer
        child.Dispose();

        // Writing through NKDebug should still work without ObjectDisposedException
        NKDebug.Info("Message after child disposed");
        Assert.Contains("Message after child disposed", _stringWriter.ToString());
    }
}