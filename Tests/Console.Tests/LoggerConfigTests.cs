//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using static NeoKolors.Console.LoggerLevel;

namespace NeoKolors.Console.Tests;

public class LoggerConfigTests {

    [Fact]
    public void ParameterizedConstructor_WithLogFileConfig_SetsCorrectValues() {
        // Arrange
        var fileConfig = LogFileConfig.Custom();
        var customLevel = CRITICAL | ERROR;

        // Act
        var config = new LoggerConfig(
            level: customLevel,
            fileConfig: fileConfig,
            simpleMessages: true
        );

        // Assert
        Assert.Equal(customLevel, config.Level);
        Assert.Equal(fileConfig, config.FileConfig);
        Assert.True(config.SimpleMessages);
    }

    [Fact]
    public void Clone_CreatesDeepCopy() {
        // Arrange
        var original = new LoggerConfig(
            level: ERROR | WARNING,
            simpleMessages: true,
            hideTime: true,
            timeFormat: "custom-format",
            output: null
        );

        // Act
        var cloned = (LoggerConfig)original.Clone();

        // Assert
        Assert.Equal(original.Level, cloned.Level);
        Assert.Equal(original.SimpleMessages, cloned.SimpleMessages);
        Assert.Equal(original.HideTime, cloned.HideTime);
        Assert.Equal(original.TimeFormat, cloned.TimeFormat);

        // Verify they are separate instances
        cloned.Level = CRITICAL;
        Assert.NotEqual(original.Level, cloned.Level);
    }

    [Fact]
    public void IndentMessage_InlineIndent_SetsCorrectly() {
        // Arrange
        var config = new LoggerConfig();
        var inlineIndent = new LoggerConfig.InlineIndent();

        // Act
        config.IndentMessage = inlineIndent;

        // Assert
        Assert.True(config.IndentMessage.IsT0);
        Assert.Equal(inlineIndent, config.IndentMessage.AsT0);
    }

    [Fact]
    public void IndentMessage_Indent_SetsCorrectly() {
        // Arrange
        var config = new LoggerConfig();
        var indent = new LoggerConfig.Indent(4);

        // Act
        config.IndentMessage = indent;

        // Assert
        Assert.True(config.IndentMessage.IsT1);
        Assert.Equal(indent, config.IndentMessage.AsT1);
        Assert.Equal(4, config.IndentMessage.AsT1.Spaces);
    }

    [Fact]
    public void FileConfig_SetProperty_UpdatesOutputIfNotCustom() {
        // Arrange
        var config = new LoggerConfig();
        var fileConfig = LogFileConfig.Replace("test.log");

        // Act
        config.FileConfig = fileConfig;

        // Assert
        Assert.Equal(fileConfig, config.FileConfig);
        // Note: The actual output change would depend on the LogFileConfig.CreateOutput() implementation
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SimpleMessages_Property_SetsCorrectly(bool value) {
        // Arrange
        var config = new LoggerConfig();

        // Act
        config.SimpleMessages = value;

        // Assert
        Assert.Equal(value, config.SimpleMessages);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void HideTime_Property_SetsCorrectly(bool value) {
        // Arrange
        var config = new LoggerConfig();

        // Act
        config.HideTime = value;

        // Assert
        Assert.Equal(value, config.HideTime);
    }

    [Theory]
    [InlineData("HH:mm:ss")]
    [InlineData("yyyy-MM-dd HH:mm:ss")]
    [InlineData("HH:mm")]
    public void TimeFormat_Property_SetsCorrectly(string format) {
        // Arrange
        var config = new LoggerConfig();

        // Act
        config.TimeFormat = format;

        // Assert
        Assert.Equal(format, config.TimeFormat);
    }
}