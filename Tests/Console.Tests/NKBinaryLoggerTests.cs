using Microsoft.Extensions.Logging;
using NeoKolors.Common;

namespace NeoKolors.Console.Tests;

public class NKBinaryLoggerTests {

    [Fact]
    public void SerializeDeserialize_StringMessage_RestoresAllFields() {
        // Arrange
        AnsiString ansiMsg = new AnsiString("Hello Binary Log").SetFColor(NKConsoleColor.RED, 0, 5);
        var original = new NKLogRecord(
            timestamp: DateTime.SpecifyKind(new DateTime(2026, 7, 19, 12, 0, 0), DateTimeKind.Utc),
            level: NKLogLevel.INFORMATION,
            message: ansiMsg,
            eventId: new EventId(42, "TestEvent"),
            source: new AnsiString("TestRunner")
        );

        using var ms = new MemoryStream();

        // Act
        NKLogRecordSerializer.Serialize(ms, original);
        ms.Position = 0;
        var deserialized = NKLogRecordSerializer.Deserialize(ms);

        // Assert
        Assert.Equal(original.Timestamp.Ticks, deserialized.Timestamp.Ticks);
        Assert.Equal(original.Level, deserialized.Level);
        Assert.Equal(original.Source, deserialized.Source);
        Assert.Equal(original.EventId?.Id, deserialized.EventId?.Id);
        Assert.Equal(original.EventId?.Name, deserialized.EventId?.Name);
        Assert.True(deserialized.Message.IsT0);
        Assert.Equal("Hello Binary Log", deserialized.Message.AsT0.Plain);
        Assert.Equal(ansiMsg, deserialized.Message.AsT0);
    }

    [Fact]
    public void SerializeDeserialize_ExceptionMessage_RestoresExceptionDetails() {
        // Arrange
        Exception? innerEx = null;
        try {
            throw new InvalidOperationException("Inner error description");
        } catch (Exception ex) {
            innerEx = ex;
        }

        var original = new NKLogRecord(
            timestamp: DateTime.UtcNow,
            level: NKLogLevel.ERROR,
            message: innerEx,
            eventId: null,
            source: new AnsiString("CrashSource")
        );

        using var ms = new MemoryStream();

        // Act
        NKLogRecordSerializer.Serialize(ms, original);
        ms.Position = 0;
        var deserialized = NKLogRecordSerializer.Deserialize(ms);

        // Assert
        Assert.Equal(original.Level, deserialized.Level);
        Assert.Equal(original.Source, deserialized.Source);
        Assert.True(deserialized.Message.IsT1);

        var deserializedEx = deserialized.Message.AsT1;
        Assert.Equal("System.InvalidOperationException", deserializedEx.GetType().FullName == "NeoKolors.Console.DeserializedException" 
            ? deserializedEx.ToString().Split(':')[0] 
            : deserializedEx.GetType().FullName);
        Assert.Equal("Inner error description", deserializedEx.Message);
        Assert.False(string.IsNullOrWhiteSpace(deserializedEx.StackTrace));
    }

    [Fact]
    public void Logger_WithBinaryOutputEnabled_WritesLogsToStream() {
        // Arrange
        using var binaryStream = new MemoryStream();
        using var stringWriter = new StringWriter();

        var config = new LoggerConfig {
            Output = stringWriter,
            BinaryOutput = binaryStream,
            Level = NKLogLevel.INFORMATION,
            SimpleMessages = true
        };

        using var binaryWriter = new BinaryLogWriter(binaryStream);
        using var logger = new NKLogger(binaryWriter, "SystemDriver", NKLogLevel.INFORMATION);

        // Act
        logger.Info("System initialized successfully", id: new EventId(101, "Startup"));
        
        binaryStream.Position = 0;
        var record = NKLogRecordSerializer.Deserialize(binaryStream);

        // Assert
        Assert.Equal(NKLogLevel.INFORMATION, record.Level);
        Assert.Equal("SystemDriver", record.Source);
        Assert.Equal(101, record.EventId?.Id);
        Assert.Equal("Startup", record.EventId?.Name);
        Assert.True(record.Message.IsT0);
        Assert.Equal("System initialized successfully", record.Message.AsT0.Plain);
    }

    [Fact]
    public void Logger_WithBinaryOutputEnabled_FiltersLogsByLevel() {
        // Arrange
        using var binaryStream = new MemoryStream();
        using var binaryWriter = new BinaryLogWriter(binaryStream);
        using var logger = new NKLogger(binaryWriter, null, NKLogLevel.ERROR | NKLogLevel.CRITICAL);

        // Act
        logger.Info("This should be filtered out");
        logger.Error("This should be recorded");

        binaryStream.Position = 0;

        // Assert
        Assert.True(binaryStream.Length > 0);
        var record = NKLogRecordSerializer.Deserialize(binaryStream);
        Assert.Equal(NKLogLevel.ERROR, record.Level);
        Assert.Equal("This should be recorded", record.Message.AsT0.Plain);
        Assert.Equal(binaryStream.Position, binaryStream.Length); // Only one log was written
    }
}
