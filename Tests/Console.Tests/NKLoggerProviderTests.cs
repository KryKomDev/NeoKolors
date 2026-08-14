// NeoKolors
// Copyright (c) krystof 2026

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NeoKolors.Console.Tests;

public class NKLoggerProviderTests {

    private class TestLogWriter : ILogWriter {
        public List<NKLogRecord> Records { get; } = new();
        public bool Disposed { get; private set; }

        public void Write(NKLogRecord record) {
            Records.Add(record);
        }

        public void Dispose() {
            Disposed = true;
        }
    }

    [Fact]
    public void CreateLogger_SetsCategoryAsSource() {
        var testWriter = new TestLogWriter();
        var options = new NKLoggerOptions { Writer = testWriter };
        using var provider = new NKLoggerProvider(options);

        var logger = provider.CreateLogger("MyCategory");
        logger.LogInformation("Hello World");

        Assert.Single(testWriter.Records);
        Assert.Equal("MyCategory", testWriter.Records[0].Source);
        Assert.Equal(NKLogLevel.INFORMATION, testWriter.Records[0].Level);
    }

    [Fact]
    public void IsEnabled_FiltersBasedOnLogLevel() {
        var testWriter = new TestLogWriter();
        var options = new NKLoggerOptions {
            Writer = testWriter,
            Level = NKLogLevel.ERROR | NKLogLevel.CRITICAL
        };
        using var provider = new NKLoggerProvider(options);

        var logger = provider.CreateLogger("FilteringCategory");

        Assert.False(logger.IsEnabled(LogLevel.Information));
        Assert.True(logger.IsEnabled(LogLevel.Error));

        logger.LogInformation("Should be ignored");
        logger.LogError("Should be recorded");

        Assert.Single(testWriter.Records);
        Assert.Equal(NKLogLevel.ERROR, testWriter.Records[0].Level);
    }

    [Fact]
    public void BeginScope_DoesNotThrow() {
        var testWriter = new TestLogWriter();
        var options = new NKLoggerOptions { Writer = testWriter };
        using var provider = new NKLoggerProvider(options);

        var logger = provider.CreateLogger("ScopedCategory");
        using (var scope = logger.BeginScope("TestScope")) {
            Assert.NotNull(scope);
            logger.LogInformation("Inside scope");
        }

        Assert.Single(testWriter.Records);
    }

    [Fact]
    public void AddNeoKolors_RegistersProviderInServiceCollection() {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddNeoKolors());

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ILoggerFactory>();
        var logger = factory.CreateLogger("DIServiceTest");

        Assert.NotNull(logger);
    }

    [Fact]
    public void LoggerFactory_AddNeoKolors_WorksAsExpected() {
        var testWriter = new TestLogWriter();
        var options = new NKLoggerOptions { Writer = testWriter };

        using var factory = LoggerFactory.Create(builder => {
            builder.AddNeoKolors(options);
        });

        var logger = factory.CreateLogger("FactoryTest");
        logger.LogWarning("Warning from factory logger");

        Assert.Single(testWriter.Records);
        Assert.Equal("FactoryTest", testWriter.Records[0].Source);
        Assert.Equal(NKLogLevel.WARNING, testWriter.Records[0].Level);
    }
}
