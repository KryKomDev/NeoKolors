// NeoKolors
// Copyright (c) krystof 2026

using Microsoft.Extensions.Logging;

namespace NeoKolors.Console.Tests;

public class LogFileConfigReworkTests : IDisposable {
    private readonly string _tempDirectory;

    public LogFileConfigReworkTests() {
        _tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "NKLogTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose() {
        if (Directory.Exists(_tempDirectory)) {
            try {
                Directory.Delete(_tempDirectory, true);
            }
            catch {
                // Cleanup best effort
            }
        }
    }

    [Fact]
    public void Replace_CreatesOrOverwritesFile() {
        string logPath = System.IO.Path.Combine(_tempDirectory, "test_replace.log");
        var fileConfig = LogFileConfig.Replace(logPath);

        using (var writer = fileConfig.CreateOutput()) {
            writer.WriteLine("Initial Line");
        }

        Assert.True(File.Exists(logPath));
        Assert.Contains("Initial Line", File.ReadAllText(logPath));

        using (var writer = fileConfig.CreateOutput()) {
            writer.WriteLine("Replaced Line");
        }

        string text = File.ReadAllText(logPath);
        Assert.Contains("Replaced Line", text);
        Assert.DoesNotContain("Initial Line", text);
    }

    [Fact]
    public void Append_AppendsContentToFile() {
        string logPath = System.IO.Path.Combine(_tempDirectory, "test_append.log");
        var fileConfig = LogFileConfig.Append(logPath);

        using (var writer = fileConfig.CreateOutput()) {
            writer.WriteLine("Line 1");
        }

        using (var writer = fileConfig.CreateOutput()) {
            writer.WriteLine("Line 2");
        }

        string text = File.ReadAllText(logPath);
        Assert.Contains("Line 1", text);
        Assert.Contains("Line 2", text);
    }

    [Fact]
    public void NewCount_GeneratesSequentialFiles_WithoutConfigFile() {
        string templatePath = System.IO.Path.Combine(_tempDirectory, "app_{0}.log");
        var config1 = LogFileConfig.NewCount(templatePath);

        using (var writer = config1.CreateOutput()) {
            writer.WriteLine("Log 1");
        }

        var config2 = LogFileConfig.NewCount(templatePath);
        using (var writer = config2.CreateOutput()) {
            writer.WriteLine("Log 2");
        }

        string configFile = System.IO.Path.Combine(_tempDirectory, ".nklog");
        Assert.False(File.Exists(configFile), "Config file .nklog should NOT exist.");

        Assert.True(File.Exists(System.IO.Path.Combine(_tempDirectory, "app_1.log")));
        Assert.True(File.Exists(System.IO.Path.Combine(_tempDirectory, "app_2.log")));
    }

    [Fact]
    public void CompositeLogWriter_DispatchesToAllWriters() {
        using var sw1 = new StringWriter();
        using var sw2 = new StringWriter();

        var writer1 = new TextLogWriter(sw1);
        var writer2 = new TextLogWriter(sw2);
        using var composite = new CompositeLogWriter(writer1, writer2);

        var logger = new NKLogger(composite, "CompositeTest");
        logger.Info("Dual Output Test");

        Assert.Contains("Dual Output Test", sw1.ToString());
        Assert.Contains("Dual Output Test", sw2.ToString());
    }

    [Fact]
    public void NKLoggerOptions_UseFileLogging_ConfiguresFileAndConsole() {
        string logPath = System.IO.Path.Combine(_tempDirectory, "combined.log");
        var options = new NKLoggerOptions().UseFileLogging(logPath);

        using var provider = new NKLoggerProvider(options);
        var logger = provider.CreateLogger("IntegrationTest");

        logger.LogInformation("Test message for file integration");

        // Flush & Dispose provider to ensure file is closed
        provider.Dispose();

        Assert.True(File.Exists(logPath));
        Assert.Contains("Test message for file integration", File.ReadAllText(logPath));
    }
}
