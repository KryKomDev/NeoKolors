// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Elements.Caching;

public static class CacheAnalyzer {
    private static ulong HITS;
    private static ulong MISSES;
    
    public static ulong Hits => HITS;
    public static ulong Misses => MISSES;
    
    internal static void Hit() => HITS++;
    internal static void Miss() => MISSES++;
}

