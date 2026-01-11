// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions.Tests;

public class ConsoleKeyInfoTests {
    
    [Fact]
    public void AsString_ReturnsCorrectFormat() {
        var key = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
        Assert.Equal("A => 'a'", key.AsString());

        var keyShift = new ConsoleKeyInfo('A', ConsoleKey.A, true, false, false);
        Assert.Equal("Shift + A => 'A'", keyShift.AsString());

        var keyCtrlAlt = new ConsoleKeyInfo('a', ConsoleKey.A, false, true, true);
        Assert.Equal("Ctrl + Alt + A => 'a'", keyCtrlAlt.AsString());
        
        var keyAll = new ConsoleKeyInfo('B', ConsoleKey.B, true, true, true);
        Assert.Equal("Ctrl + Alt + Shift + B => 'B'", keyAll.AsString());
    }

    [Fact]
    public void HasModifiers_ReturnsCorrectly() {
        var keyShift = new ConsoleKeyInfo('a', ConsoleKey.A, true, false, false);
        Assert.True(keyShift.HasShift);
        Assert.False(keyShift.HasAlt);
        Assert.False(keyShift.HasCtrl);

        var keyAlt = new ConsoleKeyInfo('a', ConsoleKey.A, false, true, false);
        Assert.False(keyAlt.HasShift);
        Assert.True(keyAlt.HasAlt);
        Assert.False(keyAlt.HasCtrl);

        var keyCtrl = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, true);
        Assert.False(keyCtrl.HasShift);
        Assert.False(keyCtrl.HasAlt);
        Assert.True(keyCtrl.HasCtrl);
    }
    
    [Theory]
    [InlineData('a', ConsoleKey.A, false)]
    [InlineData('A', ConsoleKey.A, true)]
    [InlineData('1', ConsoleKey.D1, false)]
    [InlineData('!', ConsoleKey.D1, true)]
    [InlineData(' ', ConsoleKey.Spacebar, false)]
    [InlineData('\b', ConsoleKey.Backspace, false)]
    [InlineData('\t', ConsoleKey.Tab, false)]
    [InlineData('\n', ConsoleKey.Enter, false)]
    [InlineData('\u001b', ConsoleKey.Escape, false)]
    [InlineData('+', ConsoleKey.OemPlus, true)]
    [InlineData('=', ConsoleKey.OemPlus, false)]
    public void FromChar_ReturnsCorrectKeyInfo(char input, ConsoleKey expectedKey, bool expectedShift) {
        var result = ConsoleKeyInfoExtensions.FromChar(input);
        Assert.Equal(expectedKey, result.Key);
        Assert.Equal(expectedShift, result.Modifiers.HasFlag(ConsoleModifiers.Shift));
        Assert.Equal(input, result.KeyChar);
    }
}
