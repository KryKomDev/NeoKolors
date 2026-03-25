// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

namespace NeoKolors.Console.Ansi;

/// <summary>
/// Represents various types of VT (Video Terminal) models with specific characteristics
/// and capabilities. These values correspond to different terminal generations, each
/// offering distinct features and levels of compatibility.
/// </summary>
public enum VTType {
    VT05,
    VT50,
    VT52,
    VT100,
    VT101,
    VT102,
    VT125,
    VT131,
    VT132,
    VT220,
    VT240,
    VT251,
    VT320,
    VT330,
    VT340,
    VT420,
    VT510,
    VT520,
    VT525,
}