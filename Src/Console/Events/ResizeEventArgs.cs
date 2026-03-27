// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;

namespace NeoKolors.Console.Events;

public class ResizeEventArgs {
    public int Width { get; }
    public int Height { get; }
    
    public ResizeEventArgs(Size2D size) {
        Width  = size.X;
        Height = size.Y;
    }
    
    public ResizeEventArgs(int width, int height) {
        Width  = width;
        Height = height;
    }
}