// 
// NeoKolors
// Copyright (c) 2026 KryKom
// 

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using HasFlagExtension;
using NeoKolors.Extensions;

namespace NeoKolors.Console.Driver.Windows;

internal partial class WinImports {
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/input-record-str#members"/>
    public enum WinEventType : ushort {
        KEY    = 0x1,
        MOUSE  = 0x2,
        RESIZE = 0x4,
        MENU   = 0x8,
        FOCUS  = 0x10,
    }

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/focus-event-record-str"/>
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public readonly record struct WinFocusEvent(bool FocusSet) {
        
        [field: FieldOffset(0)] 
        public bool FocusSet { get; } = FocusSet;

        public override string ToString() => $"FocusEvent({FocusSet})";
    }

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/key-event-record-str#members"/>
    [Flags]
    [FlagGroup("Alt",  "Has")]
    [FlagGroup("Ctrl", "Has")]
    internal enum WinKeyModifiers : uint {
        NONE        = 0x0000,
        
        [FlagGroup("Alt") ] RIGHT_ALT  = 0x0001,
        [FlagGroup("Alt") ] LEFT_ALT   = 0x0002,
        [FlagGroup("Ctrl")] RIGHT_CTRL = 0x0004,
        [FlagGroup("Ctrl")] LEFT_CTRL  = 0x0008,
        
        SHIFT       = 0x0010,
        NUMLOCK     = 0x0020,
        SCROLL_LOCK = 0x0040,
        CAPS_LOCK   = 0x0080,
        ENHANCED    = 0x0100,
    }

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/key-event-record-str"/>
    [StructLayout(LayoutKind.Explicit, Size = 16, CharSet = CharSet.Unicode)]
    public readonly record struct WinKeyEvent(
        bool              Down,
        ushort            RepeatCount, 
        WinVirtualKeyCode VirtualKeyCode, 
        ushort            VirtualScanCode,
        char              Char,
        WinKeyModifiers   Modifiers)
    {
        [field: FieldOffset(0)]  
        [field: MarshalAs(UnmanagedType.Bool)]
        public bool Down { get; } = Down;
        
        [field: FieldOffset(4)]
        [field: MarshalAs(UnmanagedType.U2)]
        public ushort RepeatCount { get; } = RepeatCount;
        
        [field: FieldOffset(6)]  
        [field: MarshalAs(UnmanagedType.U2)]
        public WinVirtualKeyCode VirtualKeyCode { get; } = VirtualKeyCode;
        
        [field: FieldOffset(8)]  
        [field: MarshalAs(UnmanagedType.U2)]
        public ushort VirtualScanCode { get; } = VirtualScanCode;
        
        [field: FieldOffset(10)]
        public char Char { get; } = Char;
        
        [field: FieldOffset(12)]
        [field: MarshalAs(UnmanagedType.U4)]
        public WinKeyModifiers Modifiers { get; } = Modifiers;

        public override string ToString() 
            => $"KeyEvent(" +
                $"{(Down ? "down" : "up")}, " +
                $"{RepeatCount}x, " +
                $"{VirtualKeyCode}, " +
                $"{VirtualScanCode}, " +
                $"{char.ToDisplay(Char)}, " +
                $"{Modifiers})";
    }
    
    /// <see href="https://learn.microsoft.com/en-us/windows/console/menu-event-record-str"/>
    [StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
    public readonly record struct WinMenuEvent(uint CommandId) {
        
        [field: FieldOffset(0)]
        public uint CommandId { get; } = CommandId;
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/console/coord-str"/>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public readonly record struct WinCoord(short X, short Y) {
        [field: FieldOffset(0)] public short X { get; } = X;
        [field: FieldOffset(2)] public short Y { get; } = Y;

        public override string ToString() => $"[{X}, {Y}]";

        public void Deconstruct(out short x, out short y) {
            x = X;
            y = Y;
        }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/console/mouse-event-record-str#members"/>
    [Flags]
    public enum WinMouseButtonState {
        LMB = 0x01,
        RMB = 0x02,
        MB2 = 0x04,
        MB3 = 0x08,
        MB4 = 0x10,
    }

    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/mouse-event-record-str#members"/>
    [Flags]
    public enum WinMouseEventFlags : uint {
        MOVED   = 0x01,
        DOUBLE  = 0x02,
        WHEEL   = 0x04,
        H_WHEEL = 0x08,
    }
    
    /// <see href="http://learn.microsoft.com/en-us/windows/console/mouse-event-record-str"/>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public readonly record struct WinMouseEvent(
        WinCoord            Coord, 
        WinMouseButtonState Button, 
        WinKeyModifiers     Modifiers, 
        WinMouseEventFlags  Flags) 
    {
        [field: FieldOffset(0)]  public WinCoord            Coord     { get; } = Coord;
        [field: FieldOffset(4)]  public WinMouseButtonState Button    { get; } = Button;
        [field: FieldOffset(8)]  public WinKeyModifiers     Modifiers { get; } = Modifiers;
        [field: FieldOffset(12)] public WinMouseEventFlags  Flags     { get; } = Flags;

        public override string ToString() {
            return $"MouseEvent({Coord}, {Button:x}, {Modifiers}, {Flags})";
        }
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/console/window-buffer-size-record-str"/>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public readonly record struct WinResizeEvent(WinCoord Size) {
        [field: FieldOffset(0)] public WinCoord Size { get; } = Size;
    }
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/console/input-record-str"/>
    [StructLayout(LayoutKind.Explicit)]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public readonly record struct WinInputRecord {
        
        [field: FieldOffset(0)] public WinEventType   Type   { get; }
        [field: FieldOffset(4)] public WinFocusEvent  Focus  { get; }
        [field: FieldOffset(4)] public WinKeyEvent    Key    { get; }
        [field: FieldOffset(4)] public WinMenuEvent   Menu   { get; }
        [field: FieldOffset(4)] public WinMouseEvent  Mouse  { get; }
        [field: FieldOffset(4)] public WinResizeEvent Resize { get; }

        public override string ToString() {
            return Type switch {
                WinEventType.KEY    => Key   .ToString(),
                WinEventType.MOUSE  => Mouse .ToString(),
                WinEventType.RESIZE => Resize.ToString(),
                WinEventType.MENU   => Menu  .ToString(),
                WinEventType.FOCUS  => Focus .ToString(),
                _                => "Unknown event type"
            };
        }
    }
    
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/gdi/colorref"/>
    [StructLayout (LayoutKind.Explicit, Size = 4)]
    public struct WinColorRef {
        
        [FieldOffset (0)] public uint Value;
        [FieldOffset (0)] public byte R;
        [FieldOffset (1)] public byte G;
        [FieldOffset (2)] public byte B;

        public WinColorRef(byte r, byte g, byte b) {
            Value = 0;
            R = r;
            G = g;
            B = b;
        }

        public WinColorRef(uint value) {
            R = 0;
            G = 0;
            B = 0;
            Value = value & 0x00FFFFFF;
        }
    }
}