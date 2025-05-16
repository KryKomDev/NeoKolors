//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Events;

public class ResizeEventArgs : EventArgs {
    public int NewWidth { get; }
    public int NewHeight { get; }
    
    public ResizeEventArgs(int newWidth, int newHeight) {
        NewWidth = newWidth;
        NewHeight = newHeight;
    }
}