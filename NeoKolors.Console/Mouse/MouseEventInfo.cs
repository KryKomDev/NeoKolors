// NeoKolors
// Copyright (c) 2025 KryKom

using System;
using NeoKolors.Common.Util;
using NeoKolors.Extensions;

namespace NeoKolors.Console.Mouse;

public readonly struct MouseEventInfo {
    public MouseEventType Type { get; }
    public ConsoleModifiers Modifiers { get; }
    public int X { get; }
    public int Y { get; }
    
    public MouseEventInfo(MouseEventType type, ConsoleModifiers modifiers, int x, int y) {
        Type = type;
        Modifiers = modifiers;
        X = x;
        Y = y;
    }
    
    public override string ToString() => $"{Type.ToString().EnumToSpace()}{(Modifiers != 0 ? $" with {Modifiers}" : "")} at ({X}, {Y})";
}