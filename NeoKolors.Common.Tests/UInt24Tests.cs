//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Globalization;

namespace NeoKolors.Common.Tests;

public class UInt24Tests {

    [Fact]
    public void Constructor_ShouldInitializeCorrectly() {
        UInt24 value = 0x123456;
        Assert.Equal(0x123456, (int)value);
    }

    [Fact]
    public void ToInt32_ShouldReturnCorrectValue() {
        UInt24 value = 0x654321;
        Assert.Equal(0x654321, (int)value);
    }

    [Fact]
    public void Equals_ShouldReturnTrueForEqualValues() {
        UInt24 value1 = 0xabcdef;
        UInt24 value2 = 0xabcdef;
        Assert.True(value1.Equals(value2));
    }

    [Fact]
    public void Equals_ShouldReturnFalseForDifferentValues() {
        UInt24 value1 = 0xabcdef;
        UInt24 value2 = 0x123456;
        Assert.False(value1.Equals(value2));
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameHashCodeForEqualValues() {
        UInt24 value1 = 0xabcdef;
        UInt24 value2 = 0xabcdef;
        Assert.Equal(value1.GetHashCode(), value2.GetHashCode());
    }

    [Fact]
    public void Add_ShouldReturnCorrectValue() {
        UInt24 a = 1;
        UInt24 b = 2;
        Assert.Equal(3, (int)(a + b));
    }
    
    [Fact]
    public void Sub_ShouldReturnCorrectValue() {
        UInt24 a = 5;
        UInt24 b = 2;
        Assert.Equal(3, (int)(a - b));
    }
    
    [Fact]
    public void Mul_ShouldReturnCorrectValue() {
        UInt24 a = 3;
        UInt24 b = 2;
        Assert.Equal(6, (int)(a * b));
    }
    
    [Fact]
    public void Div_ShouldReturnCorrectValue() {
        UInt24 a = 9;
        UInt24 b = 3;
        Assert.Equal(3, (int)(a / b));
    }
    
    [Fact]
    public void TryParse_ValidString_ShouldReturnTrue() {
        string input = "123456";
        
        #if !NET5_0_OR_GREATER || NET5_0
        bool result = UInt24.TryParse(input, null, out UInt24 value);
        #else
        bool result = UInt24.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out UInt24 value);
        #endif
        
        Assert.True(result);
        Assert.Equal(123456, (int)value);
    }

    [Fact]
    public void TryParse_InvalidString_ShouldReturnFalse() {
        string input = "invalid";
        
        #if !NET5_0_OR_GREATER || NET5_0
        bool result = UInt24.TryParse(input, null, out UInt24 value);
        #else
        bool result = UInt24.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out UInt24 value);
        #endif
        
        Assert.False(result);
        Assert.Equal(default, value);
    }

    #if NET5_0_OR_GREATER && !NET5_0
    
    [Fact]
    public void TryReadBigEndian_ValidBytes_ShouldReturnTrue() {
        byte[] input = { 0x00, 0x12, 0x34, 0x56 };
        bool result = UInt24.TryReadBigEndian(input, true, out UInt24 value);
        Assert.True(result);
        Assert.Equal(0x123456, (int)value);
    }

    [Fact]
    public void TryReadLittleEndian_ValidBytes_ShouldReturnTrue() {
        byte[] input = { 0x56, 0x34, 0x12, 0x00 };
        bool result = UInt24.TryReadLittleEndian(input, true, out UInt24 value);
        Assert.True(result);
        Assert.Equal(0x123456, (int)value);
    }

    [Fact]
    public void TryWriteBigEndian_ShouldReturnCorrectBytes() {
        UInt24 value = 0x123456;
        Span<byte> destination = stackalloc byte[4];
        bool result = value.TryWriteBigEndian(destination, out int bytesWritten);
        Assert.True(result);
        Assert.Equal(4, bytesWritten);
        Assert.Equal(new byte[] { 0x00, 0x12, 0x34, 0x56 }, destination.ToArray());
    }

    [Fact]
    public void TryWriteLittleEndian_ShouldReturnCorrectBytes() {
        UInt24 value = 0x123456;
        Span<byte> destination = stackalloc byte[4];
        bool result = value.TryWriteLittleEndian(destination, out int bytesWritten);
        Assert.True(result);
        Assert.Equal(4, bytesWritten);
        Assert.Equal(new byte[] { 0x56, 0x34, 0x12, 0x00 }, destination.ToArray());
    }

    [Fact]
    public void IsPow2_ShouldReturnTrueForPowerOfTwo() {
        UInt24 value = 4;
        Assert.True(UInt24.IsPow2(value));
    }

    [Fact]
    public void IsPow2_ShouldReturnFalseForNonPowerOfTwo() {
        UInt24 value = 5;
        Assert.False(UInt24.IsPow2(value));
    }

    [Fact]
    public void Log2_ShouldReturnCorrectValue() {
        UInt24 value = 8;
        Assert.Equal(3, (int)UInt24.Log2(value));
    }

    [Fact]
    public void PopCount_ShouldReturnCorrectValue() {
        UInt24 value = 0b1011;
        Assert.Equal(3, (int)UInt24.PopCount(value));
    }

    [Fact]
    public void TrailingZeroCount_ShouldReturnCorrectValue() {
        UInt24 value = 0b1000;
        Assert.Equal(3, (int)UInt24.TrailingZeroCount(value));
    }
    #endif
}