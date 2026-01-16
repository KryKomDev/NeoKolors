// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Extensions.Exception;
// ReSharper disable NotResolvedInText

namespace NeoKolors.Extensions.Tests;

public class ExceptionTests {
    
    [Fact]
    public void ThrowHelper_ThrowIf_ThrowsOnTrue() {
        Assert.Throws<InvalidOperationException>(() => 
            ThrowHelper.ThrowIf(true, new InvalidOperationException("test")));
        
        ThrowHelper.ThrowIf(false, new InvalidOperationException("test"));
    }

    [Fact]
    public void ThrowHelper_ThrowIfNull_ThrowsOnNull() {
        object? obj = null;
        Assert.Throws<ArgumentNullException>(() => 
            ThrowHelper.ThrowIfNull(obj, new ArgumentNullException("test")));
        
        obj = new object();
        ThrowHelper.ThrowIfNull(obj, new ArgumentNullException("test"));
    }

    [Fact]
    public void ArgOutOfRange_ThrowIf_ThrowsOnTrue() {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            ThrowHelper.ArgOutOfRange.ThrowIf(true, "paramName", "message"));
            
        ThrowHelper.ArgOutOfRange.ThrowIf(false, "paramName", "message");
    }

    [Fact]
    public void ArgOutOfRange_ThrowIfNull_ThrowsOnNull() {
        object? obj = null;
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            ThrowHelper.ArgOutOfRange.ThrowIfNull(obj, "paramName", "message"));

        obj = new object();
        ThrowHelper.ArgOutOfRange.ThrowIfNull(obj, "paramName", "message");
    }
}
