using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NeoKolors.Common;

using System.Numerics;

#if NET5_0_OR_GREATER && !NET5_0

public readonly struct UInt24 :
    IComparable, 
    IComparable<UInt24>,
    IEquatable<UInt24>,
    IMinMaxValue<UInt24>,
    IBinaryInteger<UInt24>
#else 

public readonly struct UInt24 :
    IComparable, 
    IComparable<UInt24>,
    IEquatable<UInt24>
#endif
{
    private readonly byte b0, b1, b2;

    public UInt24(UInt32 value) {
        b0 = (byte)((value      ) & 0xFF);
        b1 = (byte)((value >> 8 ) & 0xFF);
        b2 = (byte)((value >> 16) & 0xFF);
    }

    public UInt24(Int32 value) {
        b0 = (byte)((value      ) & 0xFF);
        b1 = (byte)((value >> 8 ) & 0xFF);
        b2 = (byte)((value >> 16) & 0xFF);
    }

    public int CompareTo(object? obj) {
        if (obj == null) {
            return 1;
        }
        if (obj is not byte) {
            throw new ArgumentException("Compared value is not of type UInt24.");
        }
        
        return CompareTo((UInt24)obj);
    }
    
    public int CompareTo(UInt24 other) {
        return (int)this - (int)other;
    }
    
    public bool Equals(UInt24 other) {
        return b2 == other.b2 && b1 == other.b1 && b0 == other.b0;
    }

    public static UInt24 MaxValue => new(0x00FFFFFF);
    public static UInt24 MinValue => new(0x00000000);
    
    public static implicit operator UInt24(UInt32 value) => new(value);
    public static implicit operator UInt32(UInt24 value) => (uint)((value.b2 << 16) | (value.b1 << 8) | value.b0);
    public static implicit operator UInt24(Int32 value) => new(value);
    public static implicit operator Int32(UInt24 value) => (value.b2 << 16) | (value.b1 << 8) | value.b0;
    public static implicit operator UInt24(Byte value) => new(value);
    public static implicit operator UInt24(Char value) => new(value);
    public static implicit operator UInt24(Int16 value) => new(value);
    public static implicit operator UInt24(UInt16 value) => new(value);
    public static implicit operator UInt64(UInt24 value) => (UInt64)((value.b2 << 16) | (value.b1 << 8) | value.b0);
    public static implicit operator Int64(UInt24 value) => (value.b2 << 16) | (value.b1 << 8) | value.b0;
    public static implicit operator UInt128(UInt24 value) => (UInt128)((value.b2 << 16) | (value.b1 << 8) | value.b0);
    public static implicit operator Int128(UInt24 value) => (value.b2 << 16) | (value.b1 << 8) | value.b0;

    public string ToString(string? format, IFormatProvider? formatProvider) {
        FormattableString formattable = $"{nameof(b0)}: {b0}, {nameof(b1)}: {b1}, {nameof(b2)}: {b2}";
        return formattable.ToString(formatProvider);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
        destination.TryWrite(provider, $"{nameof(b0)}: {b0}, {nameof(b1)}: {b1}, {nameof(b2)}: {b2}", out charsWritten);

    public static UInt24 operator %(UInt24 left, UInt24 right) => (int)left % (int)right;
    public static UInt24 operator +(UInt24 value) => value;
    public override string ToString() => $"{(uint)this}";
    
    public static UInt24 Parse(string s, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, provider);

    public static UInt24 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
        uint value = uint.Parse(s, provider);
        if (value < MinValue || value > MaxValue) {
            throw new OverflowException($"Value {value} is less than {MinValue} or greater than {MaxValue}.");
        }
        
        return new UInt24(value);
    }
    
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out UInt24 result) {
        try {
            result = Parse(s ?? throw new ArgumentNullException(nameof(s)), provider);
            return true;
        }
        catch (Exception) {
            result = default;
            return false;
        }
    }
    
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UInt24 result) => TryParse(new string(s), provider, out result);
    public static UInt24 operator +(UInt24 left, UInt24 right) => (uint)left + (uint)right;
    public static UInt24 AdditiveIdentity => 0;
    public static UInt24 operator &(UInt24 left, UInt24 right) => ((left.b2 & right.b2) << 16) | ((left.b1 & right.b1) << 8) | (left.b0 & right.b0);
    public static UInt24 operator |(UInt24 left, UInt24 right) => ((left.b2 | right.b2) << 16) | ((left.b1 | right.b1) << 8) | (left.b0 | right.b0);
    public static UInt24 operator ^(UInt24 left, UInt24 right) => ((left.b2 ^ right.b2) << 16) | ((left.b1 ^ right.b1) << 8) | (left.b0 ^ right.b0);
    public static UInt24 operator ~(UInt24 value) => (~value.b2 << 16) | (~value.b1 << 8) | ~value.b0;
    public static bool operator ==(UInt24 left, UInt24 right) => Equals(left, right);
    public static bool operator !=(UInt24 left, UInt24 right) => !Equals(left, right);
    public static bool operator >(UInt24 left, UInt24 right) => left.CompareTo(right) > 0;
    public static bool operator >=(UInt24 left, UInt24 right) => left.CompareTo(right) >= 0;
    public static bool operator <(UInt24 left, UInt24 right) => left.CompareTo(right) < 0;
    public static bool operator <=(UInt24 left, UInt24 right) => left.CompareTo(right) <= 0;
    public static UInt24 operator --(UInt24 value) => new((uint)value - 1);
    public static UInt24 operator /(UInt24 left, UInt24 right) => (uint)left / (uint)right;
    public static UInt24 operator ++(UInt24 value) => new((uint)value + 1);
    public static UInt24 MultiplicativeIdentity => 1;
    public static UInt24 operator *(UInt24 left, UInt24 right) => (uint)left * (uint)right;
    public static UInt24 operator -(UInt24 left, UInt24 right) => (uint)left - (uint)right;
    public static UInt24 operator -(UInt24 value) => 0u - value;
    public static UInt24 Abs(UInt24 value) => value;
    public static bool IsCanonical(UInt24 value) => true;
    public static bool IsComplexNumber(UInt24 value) => false;
    public static bool IsEvenInteger(UInt24 value) => value % 2 == 0;
    public static bool IsFinite(UInt24 value) => true;
    public static bool IsImaginaryNumber(UInt24 value) => false;
    public static bool IsInfinity(UInt24 value) => false;
    public static bool IsInteger(UInt24 value) => true;
    public static bool IsNaN(UInt24 value) => false;
    public static bool IsNegative(UInt24 value) => false;
    public static bool IsNegativeInfinity(UInt24 value) => false;
    public static bool IsNormal(UInt24 value) => value != 0;
    public static bool IsOddInteger(UInt24 value) => value % 2 != 0;
    public static bool IsPositive(UInt24 value) => true;
    public static bool IsPositiveInfinity(UInt24 value) => false;
    public static bool IsRealNumber(UInt24 value) => true;
    public static bool IsSubnormal(UInt24 value) => false;
    public static bool IsZero(UInt24 value) => value == 0;
    public static UInt24 MaxMagnitude(UInt24 x, UInt24 y) => Math.Max(x, y);
    public static UInt24 MaxMagnitudeNumber(UInt24 x, UInt24 y) => Math.Max(x, y);
    public static UInt24 MinMagnitude(UInt24 x, UInt24 y) => Math.Min(x, y);
    public static UInt24 MinMagnitudeNumber(UInt24 x, UInt24 y) => Math.Min(x, y);
    
    public static UInt24 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) {
        uint value = uint.Parse(s, style, provider);
        if (value < MinValue || value > MaxValue) {
            throw new OverflowException($"Value {value} is less than {MinValue} or greater than {MaxValue}.");
        }
        
        return new UInt24(value);
    }
    
    public static UInt24 Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, style, provider);

    public static bool TryConvertFromChecked<TOther>(TOther value, out UInt24 result) where TOther : INumberBase<TOther> {
        
        // stolen from System.UInt32
        if (typeof(TOther) == typeof(byte))
        {
            byte actualValue = (byte)(object)value;
            result = actualValue;
            return true;
        }

        if (typeof(TOther) == typeof(char))
        {
            char actualValue = (char)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(decimal))
        {
            decimal actualValue = (decimal)(object)value;
            result = (uint)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ushort))
        {
            ushort actualValue = (ushort)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ulong))
        {
            ulong actualValue = (ulong)(object)value;
            result = checked((uint)actualValue);
            return true;
        }
        else if (typeof(TOther) == typeof(UInt128))
        {
            UInt128 actualValue = (UInt128)(object)value;
            result = checked((uint)actualValue);
            return true;
        }
        else if (typeof(TOther) == typeof(nuint))
        {
            nuint actualValue = (nuint)(object)value;
            result = checked((uint)actualValue);
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }
    
    public static bool TryConvertFromSaturating<TOther>(TOther value, out UInt24 result) where TOther : INumberBase<TOther> {
        
        // stolen from System.UInt32
        if (typeof(TOther) == typeof(byte))
        {
            byte actualValue = (byte)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(char))
        {
            char actualValue = (char)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(decimal))
        {
            decimal actualValue = (decimal)(object)value;
            result = (actualValue >= (int)MaxValue) ? MaxValue :
                (actualValue <= (int)MinValue) ? MinValue : (uint)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ushort))
        {
            ushort actualValue = (ushort)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ulong))
        {
            ulong actualValue = (ulong)(object)value;
            result = (actualValue >= MaxValue) ? MaxValue : (uint)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(UInt128))
        {
            UInt128 actualValue = (UInt128)(object)value;
            result = (actualValue >= MaxValue) ? MaxValue : (uint)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(nuint))
        {
            nuint actualValue = (nuint)(object)value;
            result = (actualValue >= MaxValue) ? MaxValue : (uint)actualValue;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }
    
    public static bool TryConvertFromTruncating<TOther>(TOther value, out UInt24 result) where TOther : INumberBase<TOther> {
        
        // stolen from System.UInt32
        if (typeof(TOther) == typeof(byte))
        {
            byte actualValue = (byte)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(char))
        {
            char actualValue = (char)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(decimal))
        {
            decimal actualValue = (decimal)(object)value;
            result = (actualValue >= (int)MaxValue) ? MaxValue :
                (actualValue <= (int)MinValue) ? MinValue : (uint)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ushort))
        {
            ushort actualValue = (ushort)(object)value;
            result = actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(ulong))
        {
            ulong actualValue = (ulong)(object)value;
            result = (uint)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(UInt128))
        {
            UInt128 actualValue = (UInt128)(object)value;
            result = (uint)actualValue;
            return true;
        }
        else if (typeof(TOther) == typeof(nuint))
        {
            nuint actualValue = (nuint)(object)value;
            result = (uint)actualValue;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }
    
    public static bool TryConvertToChecked<TOther>(UInt24 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
        // stolen from System.UInt32
        if (typeof(TOther) == typeof(double))
        {
            double actualResult = (int)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(Half))
        {
            Half actualResult = (Half)(uint)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(short))
        {
            short actualResult = checked((short)value);
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(int))
        {
            int actualResult = checked((int)value);
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(long))
        {
            long actualResult = value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(Int128))
        {
            Int128 actualResult = value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(nint))
        {
            nint actualResult = checked((nint)value);
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(sbyte))
        {
            sbyte actualResult = checked((sbyte)value);
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(float))
        {
            float actualResult = (int)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }
    
    public static bool TryConvertToSaturating<TOther>(UInt24 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
        // stolen from System.UInt32
        if (typeof(TOther) == typeof(double))
        {
            double actualResult = (int)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(Half))
        {
            Half actualResult = (Half)(uint)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(short))
        {
            short actualResult = (value >= (uint)short.MaxValue) ? short.MaxValue : (short)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(int))
        {
            int actualResult = (value >= int.MaxValue) ? int.MaxValue : (int)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(long))
        {
            long actualResult = value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(Int128))
        {
            Int128 actualResult = value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(nint))
        {
#if TARGET_32BIT
                nint actualResult = (value >= int.MaxValue) ? int.MaxValue : (nint)value;
                result = (TOther)(object)actualResult;
                return true;
#else
            nint actualResult = (nint)value;
            result = (TOther)(object)actualResult;
            return true;
#endif
        }
        else if (typeof(TOther) == typeof(sbyte))
        {
            sbyte actualResult = (value >= (uint)sbyte.MaxValue) ? sbyte.MaxValue : (sbyte)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else if (typeof(TOther) == typeof(float))
        {
            float actualResult = (int)value;
            result = (TOther)(object)actualResult;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }
    
    public static bool TryConvertToTruncating<TOther>(UInt24 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
        // stolen from System.UInt32
        // In order to reduce overall code duplication and improve the inlinabilty of these
            // methods for the corelib types we have `ConvertFrom` handle the same sign and
            // `ConvertTo` handle the opposite sign. However, since there is an uneven split
            // between signed and unsigned types, the one that handles unsigned will also
            // handle `Decimal`.
            //
            // That is, `ConvertFrom` for `uint` will handle the other unsigned types and
            // `ConvertTo` will handle the signed types

            if (typeof(TOther) == typeof(double))
            {
                double actualResult = (int)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Half))
            {
                Half actualResult = (Half)(uint)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                short actualResult = (short)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                int actualResult = (int)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                long actualResult = value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(Int128))
            {
                Int128 actualResult = value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                nint actualResult = (nint)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                sbyte actualResult = (sbyte)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                float actualResult = (int)value;
                result = (TOther)(object)actualResult;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
    }
    
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UInt24 result) {
        try {
            result = Parse(s, provider);
            return true;
        }
        catch (Exception) {
            result = default;
            return false;
        }
    }
    
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out UInt24 result) {
        try {
            result = Parse(s ?? throw new ArgumentNullException(nameof(s)), provider);
            return true;
        }
        catch (Exception) {
            result = default;
            return false;
        }
    }
    
    public static UInt24 One => 1;
    public static int Radix => 2;
    public static UInt24 Zero => 0;
    public static bool IsPow2(UInt24 value) {
        throw new NotImplementedException();
    }
    public static UInt24 Log2(UInt24 value) {
        throw new NotImplementedException();
    }
    public static UInt24 operator <<(UInt24 value, int shiftAmount) {
        throw new NotImplementedException();
    }
    public static UInt24 operator >> (UInt24 value, int shiftAmount) {
        throw new NotImplementedException();
    }
    public static UInt24 operator >>> (UInt24 value, int shiftAmount) {
        throw new NotImplementedException();
    }
    public int GetByteCount() {
        throw new NotImplementedException();
    }
    public int GetShortestBitLength() {
        throw new NotImplementedException();
    }
    public static UInt24 PopCount(UInt24 value) {
        throw new NotImplementedException();
    }
    public static UInt24 TrailingZeroCount(UInt24 value) {
        throw new NotImplementedException();
    }
    public static bool TryReadBigEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UInt24 value) {
        throw new NotImplementedException();
    }
    public static bool TryReadLittleEndian(ReadOnlySpan<byte> source, bool isUnsigned, out UInt24 value) {
        throw new NotImplementedException();
    }
    public bool TryWriteBigEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }
    public bool TryWriteLittleEndian(Span<byte> destination, out int bytesWritten) {
        throw new NotImplementedException();
    }
}