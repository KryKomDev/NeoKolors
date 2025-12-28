// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Common;

public static class NKColorExtensions {
    extension(int value) {
        public NKColor Rgb => new(value); 
    }

    extension(uint value) {
        public NKColor Rbg => NKColor.FromRgb(value);
    }

    extension(NKConsoleColor value) {
        public NKColor NK => new(value);
    }
}