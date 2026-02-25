//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using Metriks;
using NeoKolors.Common;

namespace NeoKolors.Console.Events;

/// <summary>
/// Represents a response to a Virtual Terminal (VT) query.
/// This struct encapsulates the type of the response, data associated with the response,
/// and specific mode information when applicable.
/// </summary>
public readonly struct VTQueryResponse {
    private readonly VTQueryResponseType _type;
    private readonly int _mode;
    private readonly object? _data;

    public VTQueryResponseType Type => _type;
    
    public object? OscData => 
        Type is VTQueryResponseType.OSC
            ? _data
            : throw new InvalidOperationException();
    
    public DecReqResponseType DecData =>
        Type is VTQueryResponseType.DEC
            ? (DecReqResponseType)(_data ?? throw new InvalidOperationException())
            : throw new InvalidOperationException();
    
    public EscapeCodes.DecMode DecMode => 
        Type == VTQueryResponseType.DEC 
            ? (EscapeCodes.DecMode)_mode 
            : throw new InvalidOperationException();
    
    public EscapeCodes.OscMode OscMode => 
        Type == VTQueryResponseType.OSC 
            ? (EscapeCodes.OscMode)_mode
            : throw new InvalidOperationException();

    public VTQueryResponse() {
        _type = VTQueryResponseType.INVALID;
        _mode = 0;
        _data = null;
    }

    private VTQueryResponse(VTQueryResponseType type, int mode, object? data) {
        _type = type;
        _mode = mode;
        _data = data;
    }

    private VTQueryResponse(VTQueryResponseType type, object? data) {
        _type = type;
        _mode = 0;
        _data = data;
    }

    private T As<T>(VTQueryResponseType type) =>
        _type == type 
            ? (T)(_data ?? throw new InvalidOperationException()) 
            : throw new InvalidOperationException();

    public bool    AsWinState   => As<bool>   (VTQueryResponseType.WIN_STATE);
    public Point2D AsWinPos     => As<Point2D>(VTQueryResponseType.WIN_POS);
    public Size2D  AsWinSize    => As<Size2D> (VTQueryResponseType.WIN_SIZE);
    public Size2D  AsWinSizePx  => As<Size2D> (VTQueryResponseType.WIN_SIZE_PX);
    public Size2D  AsScreenSize => As<Size2D> (VTQueryResponseType.SCREEN_SIZE);
    public string  AsLabel      => As<string> (VTQueryResponseType.LABEL);
    public string  AsWinTitle   => As<string> (VTQueryResponseType.WIN_TITLE);
    public string  AsIconTitle  => As<string> (VTQueryResponseType.ICON_TITLE);
    
    // ---------------- FACTORY METHODS ---------------- // 
    
    public static VTQueryResponse Dec(EscapeCodes.DecMode mode, DecReqResponseType data) 
        => new(VTQueryResponseType.DEC, (int)mode, data);
    
    public static VTQueryResponse Osc(EscapeCodes.OscMode mode, object? data) 
        => new(VTQueryResponseType.OSC, (int)mode, data);
    
    
    public static VTQueryResponse Dec(int mode, DecReqResponseType data) 
        => new(VTQueryResponseType.DEC, mode, data);
    
    public static VTQueryResponse Osc(int mode, object? data) 
        => new(VTQueryResponseType.OSC, mode, data);
    
    public static VTQueryResponse WinState  (bool state)   => new(VTQueryResponseType.WIN_STATE,   state);
    public static VTQueryResponse WinPos    (Point2D pos)  => new(VTQueryResponseType.WIN_POS,     pos);
    public static VTQueryResponse WinSize   (Size2D size)  => new(VTQueryResponseType.WIN_SIZE,    size);
    public static VTQueryResponse WinSizePx (Size2D size)  => new(VTQueryResponseType.WIN_SIZE_PX, size);
    public static VTQueryResponse ScreenSize(Size2D size)  => new(VTQueryResponseType.SCREEN_SIZE, size);
    public static VTQueryResponse Label     (string label) => new(VTQueryResponseType.LABEL,       label);
    public static VTQueryResponse WinTitle  (string title) => new(VTQueryResponseType.WIN_TITLE,   title);
    public static VTQueryResponse IconTitle (string title) => new(VTQueryResponseType.ICON_TITLE,  title);
}