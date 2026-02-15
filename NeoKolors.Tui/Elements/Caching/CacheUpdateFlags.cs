// NeoKolors
// Copyright (c) 2026 KryKom

using HasFlagExtension;

namespace NeoKolors.Tui.Elements.Caching;

[Flags]
internal enum CacheUpdateFlags : byte {
    MAX    = 1,
    MIN    = 1 << 1,
    RENDER = 1 << 2,
    
    [ExcludeFlag]
    ALL = MAX | MIN | RENDER,
    
    [ExcludeFlag]
    NONE = 0
}