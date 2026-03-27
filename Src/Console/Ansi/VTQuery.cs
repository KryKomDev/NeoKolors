//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Runtime.InteropServices;
using NeoKolors.Common;
using NeoKolors.Console.Events;

namespace NeoKolors.Console.Ansi;

/// <summary>
/// Represents a response or a query to a Virtual Terminal (VT).
/// This struct encapsulates the type of the response, data associated with the response,
/// and specific mode information when applicable.
/// </summary>
public readonly struct VTQuery {
    
    private readonly VTQueryType _type;
    private readonly int         _mode;
    private readonly IntPtr      _response;
    
    private object? GetResponse() {
        if (_response == IntPtr.Zero)
            return null;
            
        var handle = GCHandle.FromIntPtr(_response);
        return handle.Target;
    }

    
    private object? Response {
        get => GetResponse();
        init {
            if (value == null) {
                _response = IntPtr.Zero;
            }
            else {
                var handle = GCHandle.Alloc(value);
                _response  = GCHandle.ToIntPtr(handle);
            }
        }
    }

    public VTQueryType Type => _type;

    public DecReqResponseType DecResponse {
        get =>
            Type is VTQueryType.DEC
                ? (DecReqResponseType)(Response ?? throw new InvalidOperationException("The query has no response."))
                : throw new InvalidOperationException($"Cannot convert a {_type} query to a DEC query response.");
        init {
            if (Type != VTQueryType.DEC)
                throw new InvalidOperationException($"Cannot convert a {_type} query to a DEC query response.");

            Response = value;
        }
    }

    public EscapeCodes.DecMode DecMode => 
        Type == VTQueryType.DEC 
            ? (EscapeCodes.DecMode)_mode 
            : throw new InvalidOperationException($"Cannot convert a {_type} query to a DEC query response.");

    public object? OscResponse {
        get =>
            Type is VTQueryType.OSC
                ? _response
                : throw new InvalidOperationException($"Cannot convert a {_type} query to an OSC query response.");
        init {
            if (Type != VTQueryType.OSC)
                throw new InvalidOperationException($"Cannot convert a {_type} query to an OSC query response.");

            Response = value;
        }
    }

    public EscapeCodes.OscMode OscMode => 
        Type == VTQueryType.OSC 
            ? (EscapeCodes.OscMode)_mode
            : throw new InvalidOperationException($"Cannot convert a {_type} query to an OSC query response.");

    
    public (int X, int Y) WinResponse {
        get =>
            Type is VTQueryType.WIN
                ? ((int X, int Y))(Response ?? throw new InvalidOperationException("The query has no response."))
                : throw new InvalidOperationException($"Cannot convert a {_type} query to a WinOpt query response.");
        init {
            if (Type != VTQueryType.WIN)
                throw new InvalidOperationException($"Cannot convert a {_type} query to a WinOpt query response.");

            Response = value;
        }
    }

    public EscapeCodes.WinOpts WinMode => 
        Type == VTQueryType.WIN 
            ? (EscapeCodes.WinOpts)_mode
            : throw new InvalidOperationException($"Cannot convert a {_type} query to an WinOpt query response.");
    
    public PDAResponse? PrimaryDAResponse =>
        Type == VTQueryType.PRIMARY_DA
            ? (PDAResponse?)(Response ?? throw new InvalidOperationException("The query has no response."))
            : throw new InvalidOperationException($"Cannot convert a {_type} query to a PrimaryDA query response.");

    
    public VTQuery() {
        _type     = VTQueryType.INVALID;
        _mode     = 0;
        _response = IntPtr.Zero;
    }

    private VTQuery(VTQueryType type, int mode, object? data = null) {
        _type    = type;
        _mode    = mode;
        Response = data;
    }

    private VTQuery(VTQueryType type, object? data = null) {
        _type    = type;
        _mode    = 0;
        Response = data;
    }

    private T As<T>(VTQueryType type) =>
        _type == type 
            ? (T)(Response ?? throw new InvalidOperationException("The query has no response.")) 
            : throw new InvalidOperationException($"Cannot convert a {_type} query to a {_type} query response.");

    public bool   AsWinState  => As<bool>   (VTQueryType.WIN_STATE);
    public string AsWinTitle  => As<string> (VTQueryType.WIN_TITLE);
    public string AsIconTitle => As<string> (VTQueryType.ICON_TITLE);

    public string GetEscSeq() {
        return _type switch {
            VTQueryType.INVALID      => throw new InvalidOperationException(),
            VTQueryType.DEC          => $"\e[?{_mode}$p",
            VTQueryType.OSC          => $"\e]{_mode};?\a",
            VTQueryType.WIN          => $"\e[{_mode}t",
            VTQueryType.WIN_STATE    =>  "\e[11t",
            VTQueryType.WIN_TITLE    =>  "\e]l",
            VTQueryType.ICON_TITLE   =>  "\e]L",
            VTQueryType.PRIMARY_DA   =>  "\e[c",
            VTQueryType.SECONDARY_DA =>  "\e[>c",
            _                        => throw new ArgumentOutOfRangeException()
        };
    }

    public override string ToString() {
        return $"Type: {_type}, Mode: {_mode}, Data: {_response}";
    }


    // ---------------- FACTORY METHODS ---------------- // 
    
    // --- requests ---
    
    public static VTQuery RequestDec(EscapeCodes.DecMode mode) => 
        new(VTQueryType.DEC, (int)mode);

    public static VTQuery RequestDec(int mode) =>
        new(VTQueryType.DEC, mode);

    public static VTQuery RequestOsc(EscapeCodes.OscMode mode) =>
        new(VTQueryType.OSC, (int)mode);

    public static VTQuery RequestOsc(int mode) =>
        new(VTQueryType.OSC, mode);
    
    public static VTQuery RequestWin(EscapeCodes.WinOpts mode) =>
        new(VTQueryType.WIN, (int)mode);

    public static VTQuery RequestWin(int mode) =>
        new(VTQueryType.WIN, mode);

    public static VTQuery RequestWinState    => new(VTQueryType.WIN_STATE);
    public static VTQuery RequestWinTitle    => new(VTQueryType.WIN_TITLE);
    public static VTQuery RequestIconTitle   => new(VTQueryType.ICON_TITLE);
    public static VTQuery RequestPrimaryDA   => new(VTQueryType.PRIMARY_DA);
    public static VTQuery RequestSecondaryDA => new(VTQueryType.SECONDARY_DA);
    
    
    // --- responses ---
    
    public static VTQuery Dec(EscapeCodes.DecMode mode, DecReqResponseType data) => 
        new(VTQueryType.DEC, (int)mode, data);

    public static VTQuery Dec(int mode, DecReqResponseType data) => 
        new(VTQueryType.DEC, mode, data);

    public static VTQuery Osc(EscapeCodes.OscMode mode, object? data) => 
        new(VTQueryType.OSC, (int)mode, data);

    public static VTQuery Osc(int mode, object? data) => 
        new(VTQueryType.OSC, mode, data);
    
    public static VTQuery Win(EscapeCodes.WinOpts mode, (int X, int Y) data) => 
        new(VTQueryType.WIN, (int)mode, data);

    public static VTQuery Win(int mode, (int X, int Y) data) => 
        new(VTQueryType.WIN, mode, data);

    /// <summary>
    /// Creates a new VT query response for a window state query.
    /// </summary>
    /// <param name="state">If true, the VT window is minimized, otherwise not.</param>
    public static VTQuery WinState(bool state) => new(VTQueryType.WIN_STATE, state);
    public static VTQuery WinTitle(string title) => new(VTQueryType.WIN_TITLE, title);
    public static VTQuery IconTitle(string title) => new(VTQueryType.ICON_TITLE, title);

    /// <summary>
    /// Creates a new VT query response for a Primary Device Attribute (PrimaryDA) query.
    /// </summary>
    /// <param name="response">The PDAResponse object representing the device's primary attribute information.</param>
    /// <returns>A VTQuery instance representing the PrimaryDA query response.</returns>
    public static VTQuery PrimaryDA(PDAResponse response) => new(VTQueryType.PRIMARY_DA, response);
}