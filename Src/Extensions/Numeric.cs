// NeoKolors
// Copyright (c) 2025 KryKom

#if NET5_0_OR_GREATER
using System.Numerics;
#endif

namespace NeoKolors.Extensions;

/// <summary>
/// Provides high-performance numeric utilities and extensions.
/// </summary>
public static class Numeric {

    private static readonly byte[] BYTE_POP_COUNT_VALS;

    static Numeric() {
        BYTE_POP_COUNT_VALS = new byte[256];
        
        for (int i = 0; i < 256; i++) {
            BYTE_POP_COUNT_VALS[i] = (byte)((i & 1) + BYTE_POP_COUNT_VALS[i >> 1]);
        }
    }
    
    /// <summary>
    /// Calculates the population count (number of set bits) of a 32-bit unsigned integer.
    /// </summary>
    /// <param name="i">The unsigned integer to count.</param>
    /// <returns>The number of set bits (1s) in the binary representation of <paramref name="i"/>.</returns>
    public static int PopCount(uint i) {
        #if NET5_0_OR_GREATER
        
        return BitOperations.PopCount(i);
        
        #else
        
        i = i - ((i >> 1) & 0x55555555);
        i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
        return (int)((((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24); 
        
        #endif
    }

    /// <summary>
    /// Calculates the population count (number of set bits) of a byte.
    /// </summary>
    /// <param name="i">The byte to count.</param>
    /// <returns>The number of set bits (1s) in the binary representation of <paramref name="i"/>.</returns>
    public static int PopCount(byte i) => BYTE_POP_COUNT_VALS[i];

    extension(int val) {
        /// <summary>
        /// Clamps the integer value to a range defined by inclusive minimum and maximum bounds.
        /// </summary>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <returns>The clamped integer value.</returns>
        public int Clamp(int min, int max) => Math.Clamp(val, min, max);
    }

    extension(Math) {
        /// <summary>
        /// Clamps an integer value to a range defined by two boundary values, automatically determining which is the minimum and maximum.
        /// </summary>
        /// <param name="val">The value to clamp.</param>
        /// <param name="b0">The first boundary.</param>
        /// <param name="b1">The second boundary.</param>
        /// <returns>The clamped integer value.</returns>
        public static int DClamp(int val, int b0, int b1) => Math.Clamp(val, Math.Min(b0, b1), Math.Max(b0, b1));
    }
}