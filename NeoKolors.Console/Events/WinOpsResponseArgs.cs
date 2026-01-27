// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using static NeoKolors.Console.Events.WinOpsResponseType;

namespace NeoKolors.Console.Events;

public readonly struct WinOpsResponseArgs {
    public WinOpsResponseType Type { get; }
    public object? Value { get; }
    
    private WinOpsResponseArgs(WinOpsResponseType type, object value) {
        Type = type;
        Value = value;
    }

    public WinOpsResponseArgs() {
        Type = NONE;
        Value = null;
    }
    
    public bool IsNone()      => Type == NONE;
    public bool IsIconLabel() => Type == ICON_LABEL;
    public bool IsWinSize()   => Type == WIN_SIZE;
    public bool IsWinSizePx() => Type == WIN_SIZE_PX;
    public bool IsWinState()  => Type == WIN_STATE;
    public bool IsWinPos()    => Type == WIN_POS;
    public bool IsScrSize()   => Type == SCREEN_SIZE;
    public bool IsWinTitle()  => Type == WIN_TITLE;
    
    public string AsIconLabel() 
        => Type == ICON_LABEL ? (string)Value! : throw new InvalidOperationException("Not an icon label.");
    public Size2D AsWinSize() 
        => Type == WIN_SIZE ? (Size2D)Value! : throw new InvalidOperationException("Not a win size.");
    public Size2D AsWinSizePx() 
        => Type == WIN_SIZE_PX ? (Size2D)Value! : throw new InvalidOperationException("Not a win size px.");
    public bool AsWinState() 
        => Type == WIN_STATE ? (bool)Value! : throw new InvalidOperationException("Not a win state.");
    public (int X, int Y) AsWinPos() 
        => Type == WIN_POS ? ((int, int))Value! : throw new InvalidOperationException("Not a win pos.");
    public Size2D AsScrSize() 
        => Type == SCREEN_SIZE ? (Size2D)Value! : throw new InvalidOperationException("Not a screen size.");
    public string AsWinTitle()                 
        => Type == WIN_TITLE ? (string)Value! : throw new InvalidOperationException("Not a win title.");
    
    public static WinOpsResponseArgs IconLabel(string label) => new(ICON_LABEL, label);
    public static WinOpsResponseArgs WinSize(Size2D size) => new(WIN_SIZE, size);
    public static WinOpsResponseArgs WinSizePx(Size2D size) => new(WIN_SIZE_PX, size);
    public static WinOpsResponseArgs WinState(bool open) => new(WIN_STATE, open);
    public static WinOpsResponseArgs WinPos((int X, int Y) pos) => new(WIN_POS, pos);
    public static WinOpsResponseArgs ScrSize((int Width, int Height) size) => new(SCREEN_SIZE, size);
    public static WinOpsResponseArgs WinTitle(string title) => new(WIN_TITLE, title);
}