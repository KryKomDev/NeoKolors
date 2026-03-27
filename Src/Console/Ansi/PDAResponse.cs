// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using static NeoKolors.Console.Ansi.VTType;

namespace NeoKolors.Console.Ansi;

/// <summary>
/// Represents a response for Primary Device Attributes (PDA) within
/// the NeoKolors ANSI Console framework. The PDAResponse struct encapsulates
/// details about the terminal's VT type and its supported capabilities.
/// </summary>
public readonly struct PDAResponse {
    
    public VTType          Type         { get; }
    public VTCapabilities? Capabilities { get; }
    
    public PDAResponse(VTType type, VTCapabilities? capabilities = null) {
        Type         = type;
        Capabilities = capabilities;
    }

    public PDAResponse(int type, int[]? capabilities = null) {
        Type = GetVTType(
            type, 
            capabilities is { Length: > 0 } 
                ? capabilities[0] 
                : null
        );
        
        Capabilities = capabilities != null 
            ? GetVTCapabilities(capabilities) 
            : null;
    }

    public override string ToString() => 
        $"Primary DA {{ Type: {Type}, Capabilities: {Capabilities?.ToString() ?? "N/A"} }}";

    /// <summary>
    /// Determines the specific VT (Video Terminal) type based on the provided main type and optional subtype.
    /// This method maps specific combinations of type and subtype to their corresponding VTType enumeration values.
    /// </summary>
    /// <param name="type">An integer representing the main type of the Video Terminal.</param>
    /// <param name="subtype">An optional integer representing the subtype of the Video Terminal. Can be null if no
    /// subtype is applicable.</param>
    /// <returns>A <see cref="VTType"/> value corresponding to the given type and subtype.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided type does not correspond to any
    /// supported VTType.</exception>
    public static VTType GetVTType(int type, int? subtype) {
        return type switch {
            1 when subtype == 2 => VT100,
            1 when subtype == 0 => VT101,
            1                   => VT100,
            4 when subtype == 6 => VT132,
            6                   => VT102,
            7                   => VT131,
            12                  => VT125,
            62                  => VT220,
            63                  => VT320,
            64                  => VT420,
            65                  => VT510,
            _                   => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    /// Retrieves the combined Virtual Terminal (VT) capabilities from an array of individual capabilities.
    /// Each capability in the array is processed and aggregated to form the final set of VT capabilities.
    /// </summary>
    /// <param name="capabilities">An array representing individual VT capabilities as integers.</param>
    /// <returns>A <see cref="VTCapabilities"/> value representing the aggregated capabilities derived from
    /// the input array.</returns>
    public static VTCapabilities GetVTCapabilities(int[] capabilities) {
        var result = VTCapabilities.NONE;

        for (int i = 0; i < capabilities.Length; i++) {
            result |= GetVTCapability(capabilities[i]);
        }
        
        return result;
    }

    private static VTCapabilities GetVTCapability(int capability) {
        return capability switch {
            1  => VTCapabilities.COL_132,
            2  => VTCapabilities.PRINTER,
            3  => VTCapabilities.REGIS,
            4  => VTCapabilities.SIXEL,
            6  => VTCapabilities.SELECTIVE_ERASE,
            8  => VTCapabilities.USER_DEFINED_KEYS,
            15 => VTCapabilities.TECHNICAL_CHARS,
            16 => VTCapabilities.LOCATOR_PORT,
            17 => VTCapabilities.VT_STATE_REPORT,
            18 => VTCapabilities.USER_WINDOWS,
            21 => VTCapabilities.HORIZONTAL_SCROLL,
            22 => VTCapabilities.ANSI_COLOR,
            28 => VTCapabilities.RECTANGULAR_EDITING,
            29 => VTCapabilities.ANSI_TEXT_LOCATOR,
            _  => throw new ArgumentOutOfRangeException(nameof(capability), capability, null)
        };
    } 
}