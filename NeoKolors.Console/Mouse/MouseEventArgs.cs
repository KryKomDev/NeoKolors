// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;

namespace NeoKolors.Console.Mouse;

public readonly struct MouseEventArgs {
    public MouseButton      Button    { get; }
    public ConsoleModifiers Modifiers { get; }
    public bool             Move      { get; }
    public Point2D          Position  { get; }
    public bool             Release   { get; }
    
    public MouseEventArgs(
        MouseButton      button, 
        ConsoleModifiers modifiers,
        bool             move, 
        Point2D          position,
        bool             release    = false) 
    {
        Button    = button;
        Modifiers = modifiers;
        Move      = move;
        Position  = position;
        Release   = release;
    }
}