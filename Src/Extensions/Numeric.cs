// NeoKolors
// Copyright (c) 2025 KryKom

#if NET5_0_OR_GREATER
using System.Numerics;
#endif

namespace NeoKolors.Extensions;

public static class Numeric {

    private static readonly byte[] BYTE_POP_COUNT_VALS;

    static Numeric() {
        BYTE_POP_COUNT_VALS = new byte[256];
        
        for (int i = 0; i < 256; i++) {
            BYTE_POP_COUNT_VALS[i] = (byte)((i & 1) + BYTE_POP_COUNT_VALS[i >> 1]);
        }
    }
    
    public static int PopCount(uint i) {
        #if NET5_0_OR_GREATER
        
        return BitOperations.PopCount(i);
        
        #else
        
        i = i - ((i >> 1) & 0x55555555);
        i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
        return (int)((((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24); 
        
        #endif
    }

    public static int PopCount(byte i) => BYTE_POP_COUNT_VALS[i];

    extension(int val) {
        public int Clamp(int min, int max) => Math.Clamp(val, min, max);
    }

    extension(Math) {
        public static int DClamp(int val, int b0, int b1) => Math.Clamp(val, Math.Min(b0, b1), Math.Max(b0, b1));
    }
}