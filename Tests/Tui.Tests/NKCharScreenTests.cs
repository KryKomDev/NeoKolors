//
// NeoKolors.Test
// Copyright (c) 2025 KryKom
//

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Console.Driver;
using NeoKolors.Console.Driver.Dotnet;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Tests;

public class NKCharScreenTests {
    
    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        var screen = new NKCharScreen(10, 5);
        Assert.Equal(10, screen.Width);
        Assert.Equal(5, screen.Height);
    }

    [Fact]
    public void Constructor_WithSize_ShouldInitializeCorrectly() {
        // Size is from NeoKolors.Tui (struct Size : IEquatable<Size>)
        var size = new Size(20, 10);
        var screen = new NKCharScreen(size);
        Assert.Equal(20, screen.Width);
        Assert.Equal(10, screen.Height);
    }

    [Fact]
    public void Render_NullCharWithCustomStyle_ShouldRenderSpaceWithStyle() {
        // Arrange
        var originalOutputDriver = NKConsole.OutputDriver;
        var originalInputDriver = NKConsole.InputDriver;
        
        var mockOutputDriver = new MockOutputDriver();
        var mockInputDriver = new MockInputDriver();
        
        NKConsole.OutputDriver = mockOutputDriver;
        NKConsole.InputDriver = mockInputDriver;

        try {
            var screen = new NKCharScreen(3, 1);
            
            // Set cell [0, 0] to have null character but a specific background color style
            var style = NKStyle.Default.SafeSetBColor(NKColor.FromRgb(255, 0, 0));
            screen[0, 0] = new CellInfo(null, style, true, 0);

            // Act
            screen.Render();

            // Assert
            var output = mockOutputDriver.Output.ToString();
            // The output should contain the escape sequence for the style and a space ' ' character
            Assert.Contains("\x1b[48;2;255;0;0m", output);
            Assert.Contains(" ", output);
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
        public new Size2D GetSize() => new Size2D(100, 100);
    }
}
