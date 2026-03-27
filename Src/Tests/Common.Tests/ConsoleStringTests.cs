using NeoKolors.Common.Util;

namespace NeoKolors.Common.Tests;

public class ConsoleStringTests {

    [Fact]
    public void Empty_ReturnsEmptyConsoleString() {
        var empty = ConsoleString.Empty;
        Assert.Empty(empty);
        Assert.Equal(string.Empty, (string)empty);
    }

    [Fact]
    public void Indexer_ReturnsCorrectKeyInfo() {
        var cs = new ConsoleString();
        var key = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
        
        // ConsoleString doesn't have a public Add, but it has operator+
        cs += key;
        
        Assert.Equal(key, cs[0]);
    }

    [Fact]
    public void OperatorPlus_ConsoleString_CombinesStrings() {
        var key1 = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
        var key2 = new ConsoleKeyInfo('b', ConsoleKey.B, false, false, false);
        
        var cs1 = ConsoleString.Empty + key1;
        var cs2 = ConsoleString.Empty + key2;
        var combined = cs1 + cs2;

        Assert.Equal(2, combined.Count());
        Assert.Equal('a', combined.ElementAt(0).KeyChar);
        Assert.Equal('b', combined.ElementAt(1).KeyChar);
        Assert.Equal("ab", (string)combined);
    }

    [Fact]
    public void OperatorPlus_KeyInfo_AddsToStartAndEnd() {
        var key1 = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
        var key2 = new ConsoleKeyInfo('b', ConsoleKey.B, false, false, false);
        var cs = ConsoleString.Empty + key1;
        
        var addedEnd = cs + key2;
        var addedStart = key2 + cs;

        Assert.Equal("ab", (string)addedEnd);
        Assert.Equal("ba", (string)addedStart);
    }

    [Fact]
    public void GetEnumerator_IteratesOverKeys() {
        var key = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
        var cs = ConsoleString.Empty + key + key;

        int count = 0;
        foreach (var k in cs) {
            Assert.Equal(key, k);
            count++;
        }
        Assert.Equal(2, count);
    }
}
