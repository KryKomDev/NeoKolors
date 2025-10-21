// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions;

public static class Numeric {
    
    extension(int value) {

        #if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        
        /// <summary>
        /// Determines whether the current numeric value is within the specified range, inclusive of the range's boundaries.
        /// If the range's boundaries are from the end of the range, the range is considered to be open on that side.
        /// </summary>
        /// <param name="range">The range to check the numeric value against.</param>
        /// <returns>True if the numeric value lies within the range; otherwise, false.</returns>
        public bool Belongs(Range range) {
            range = new Range(
                range.Start.IsFromEnd
                    ? range.Start.Value + 1 
                    : range.Start.Value, 
                range.End.IsFromEnd 
                    ? range.End.Value - 1 
                    : range.End.Value
            );
            
            return value >= range.Start.Value && value <= range.End.Value;
        }

        #endif
    }
}