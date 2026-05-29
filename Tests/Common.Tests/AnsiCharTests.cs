// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

namespace NeoKolors.Common.Tests;

public class AnsiCharTests {
    
    [Fact]
    public void AnsiChar_Constructor_SetsProperties() {
        var style = new NKStyle(NKConsoleColor.RED);
        var ansiChar = new AnsiChar('A', style);

        Assert.Equal('A', ansiChar.Char);
        Assert.Equal(style, ansiChar.Style);
    }

    [Fact]
    public void AnsiChar_ImplicitConversion_ToChar() {
        var ansiChar = new AnsiChar('B', NKStyle.Default);
        char c = ansiChar;
        Assert.Equal('B', c);
    }

}