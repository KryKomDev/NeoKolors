// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using HasFlagExtension;

namespace NeoKolors.Console.Driver;

[Flags]
public enum ReportedMouseEvents {
        
    [ExcludeFlag]
    NONE = 0,

    DOWN    = 1 << 0,
    RELEASE = 1 << 1,
    DRAG    = 1 << 2,
    MOVE    = 1 << 3,
    
    [ExcludeFlag]
    ALL = DOWN | RELEASE | DRAG | MOVE
}