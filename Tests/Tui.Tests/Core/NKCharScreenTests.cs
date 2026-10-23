// NeoKolors
// Copyright (c) 2026 KryKom

using System.Text;
using NeoKolors.Console;

namespace NeoKolors.Tui.Tests.Core;

public class NKCharScreenTests {

    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var screen = new NKCharScreen(10, 5);
        Assert.Equal(10, screen.Width);
        Assert.Equal(5, screen.Height);
    }

    [Fact]
    public void Constructor_WithSize_ShouldInitializeCorrectly() {
        var size = new Size2D(20, 10);
        var screen = new NKCharScreen(size);
        Assert.Equal(20, screen.Width);
        Assert.Equal(10, screen.Height);
    }

    [Fact]
    public void Render_ShouldNeverEmitNewlineCharacters() {
        var originalOutputDriver = NKConsole.OutputDriver;
        var originalInputDriver = NKConsole.InputDriver;

        var mockOutputDriver = new MockOutputDriver();
        var mockInputDriver = new MockInputDriver();

        NKConsole.OutputDriver = mockOutputDriver;
        NKConsole.InputDriver = mockInputDriver;

        try {
            var screen = new NKCharScreen(10, 5);
            screen.Draw(new AnsiString("Line 0"), 0, 0, 0);
            screen.Draw(new AnsiString("Line 1"), 0, 1, 0);
            screen.Draw(new AnsiString("Line 2"), 0, 2, 0);

            screen.Render();

            var output = mockOutputDriver.Output.ToString();
            Assert.NotEmpty(output);
            Assert.DoesNotContain("\n", output);
            Assert.DoesNotContain("\r", output);
        }
        finally {
            NKConsole.OutputDriver = originalOutputDriver;
            NKConsole.InputDriver = originalInputDriver;
        }
    }

    [Fact]
    public void Render_SecondPassWithNoChanges_ShouldNotWrite() {
        var originalOutputDriver = NKConsole.OutputDriver;
        var originalInputDriver = NKConsole.InputDriver;

        var mockOutputDriver = new MockOutputDriver();
        var mockInputDriver = new MockInputDriver();

        NKConsole.OutputDriver = mockOutputDriver;
        NKConsole.InputDriver = mockInputDriver;

        try {
            var screen = new NKCharScreen(10, 5);
            screen.Draw(new AnsiString("Hello"), 0, 0, 0);
            screen.Render();

            var firstPassLength = mockOutputDriver.Output.Length;
            Assert.True(firstPassLength > 0);

            // Second render with no modifications
            mockOutputDriver.Output.Clear();
            screen.Render();

            Assert.Equal(0, mockOutputDriver.Output.Length);
        }
        finally {
            NKConsole.OutputDriver = originalOutputDriver;
            NKConsole.InputDriver = originalInputDriver;
        }
    }

    [Fact]
    public void Render_DifferentialUpdate_ShouldOnlyOutputChangedCellCoordinates() {
        var originalOutputDriver = NKConsole.OutputDriver;
        var originalInputDriver = NKConsole.InputDriver;

        var mockOutputDriver = new MockOutputDriver();
        var mockInputDriver = new MockInputDriver();

        NKConsole.OutputDriver = mockOutputDriver;
        NKConsole.InputDriver = mockInputDriver;

        try {
            var screen = new NKCharScreen(20, 10);
            screen.Draw(new AnsiString("Initial"), 0, 0, 0);
            screen.Render();

            mockOutputDriver.Output.Clear();

            // Modify a cell on row 4 (1-indexed: row 5) at col 7 (1-indexed: col 8)
            screen.Draw(new AnsiChar('Z', NKStyle.Default), 7, 4, 0);
            screen.Render();

            var diffOutput = mockOutputDriver.Output.ToString();
            Assert.NotEmpty(diffOutput);
            Assert.Contains("\x1b[5;8H", diffOutput);
            Assert.Contains("Z", diffOutput);
            Assert.DoesNotContain("\n", diffOutput);
        }
        finally {
            NKConsole.OutputDriver = originalOutputDriver;
            NKConsole.InputDriver = originalInputDriver;
        }
    }

    private class MockOutputDriver : IOutputDriver {
        public StringBuilder Output { get; } = new();

        public void Write(ReadOnlySpan<char> value) {
            Output.Append(value);
        }

        public void Dispose() { }
    }

    private class MockInputDriver : DotnetInputDriver, IInputDriver<DotnetInputDriverConfig> {
        public new Size2D GetSize() => new(100, 100);
    }
}
