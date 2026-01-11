// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions.Tests;

public class UriExtensionsTests {
    
    [Theory]
    [InlineData("C:\\test.txt", true)]
    [InlineData("/etc/hosts", true)]
    [InlineData("file:///C:/test.txt", true)]
    [InlineData("https://google.com", false)]
    [InlineData("http://example.com/foo.png", false)]
    [InlineData("ftp://server.com", false)]
    [InlineData("relative/path/to/file.txt", true)] // Treated as local
    public void IsLocal_ReturnsCorrectly(string path, bool expected) {
        Assert.Equal(expected, UriExtensions.IsLocal(path));
    }
}
