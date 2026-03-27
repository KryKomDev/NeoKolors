//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using Metriks;

namespace NeoKolors.Console.Input;

public readonly record struct MouseEventArgs {
    public MouseButton  Button    { get; init; }
    public Point2D      Position  { get; init; }
    public KeyModifiers Modifiers { get; init; }
    public bool         Released  { get; init; }
    public bool         Moved     { get; init; }

    public bool IsHover   => Button == MouseButton.RELEASE && Moved;
    public bool IsPress   => !Released && !Moved;
    public bool IsRelease =>  Released && !Moved;
    
    public MouseEventArgs(MouseButton button, KeyModifiers modifiers, Point2D position, bool released, bool moved) {
        Button    = button;
        Position  = position;
        Modifiers = modifiers;
        Released  = released;
        Moved     = moved;
    }
    
    public override string ToString() =>
        $"{(Released ? "Released " : "")}" +
        $"{(Moved && Button == MouseButton.RELEASE ? "Moved" : Moved ? $"Dragged {Button}" : Button)}" +
        $" at {Position} with {Modifiers}";
}