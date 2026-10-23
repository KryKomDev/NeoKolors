// NeoKolors
// Copyright (c) krystof 2026

using HasFlagExtension;
using static NeoKolors.Tui.Core.NKCellFlags;

namespace NeoKolors.Tui.Core;

[Flags]
[HasFlagPrefix("Is")]
public enum NKCellFlags : ushort {
    [ExcludeFlag] SINGLE = 0b0000,
    [ExcludeFlag] IMAGE  = 0b0001,

    SURROGATE                    = 0b0010,
    [ExcludeFlag] HIGH_SURROGATE = 0b0010,
    [ExcludeFlag] LOW_SURROGATE  = 0b0011,

    CLUSTER                        = 0b1000,
    [ExcludeFlag] CLUSTER_START    = 0b1000,
    [ExcludeFlag] CLUSTER_CONTINUE = 0b1100,
    
    /// <summary>
    /// This flag indicates that the cell is transparent, meaning the cell will
    /// not be drawn onto the canvas.
    /// </summary>
    TRANSPARENT = 0b10000,
}

public static partial class NKCellFlagsExtensions {
    extension(NKCellFlags val) {

        public bool GetIsSingle()              => ((ushort)val & 0b0011) == (ushort)SINGLE;
        public bool GetIsImage()               => ((ushort)val & 0b0011) == (ushort)IMAGE;
        public bool GetIsHighSurrogate()       => ((ushort)val & 0b0011) == (ushort)HIGH_SURROGATE;
        public bool GetIsLowSurrogate()        => ((ushort)val & 0b0011) == (ushort)LOW_SURROGATE;
        public bool GetIsClusterStart()        => ((ushort)val & 0b1100) == (ushort)CLUSTER_START;
        public bool GetIsClusterContinuation() => ((ushort)val & 0b1100) == (ushort)CLUSTER_CONTINUE;

        #if NET10_0_OR_GREATER

        public bool IsSingle              => ((ushort)val & 0b0011) == (ushort)SINGLE;
        public bool IsImage               => ((ushort)val & 0b0011) == (ushort)IMAGE;
        public bool IsHighSurrogate       => ((ushort)val & 0b0011) == (ushort)HIGH_SURROGATE;
        public bool IsLowSurrogate        => ((ushort)val & 0b0011) == (ushort)LOW_SURROGATE;
        public bool IsClusterStart        => ((ushort)val & 0b1100) == (ushort)CLUSTER_START;
        public bool IsClusterContinuation => ((ushort)val & 0b1100) == (ushort)CLUSTER_CONTINUE;

        #endif
    }
}