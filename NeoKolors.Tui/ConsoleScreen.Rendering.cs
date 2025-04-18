//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;

namespace NeoKolors.Tui;

/// <summary>
/// virtual console that saves all the styles, colors and characters
/// </summary>
public partial class ConsoleScreen {
    
    public void Render() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                System.Console.SetCursorPosition(x, y);
                StandardOutput.Write(Chars[x, y].AddStyle(Styles[x, y]));
            }
        }
        
        // for (int y = 0; y < Height; y++) {
        //     bool isBehind = true;
        //     ulong prevStyle = 0;
        //     for (int x = 0; x < Width; x++) {
        //         if (PixelChanges[x, y]) {
        //             RenderSingle(x, y, isBehind, ref prevStyle);
        //             isBehind = false;
        //         }
        //         else {
        //             isBehind = true;
        //         }
        //     }
        // }
    }

    private void RenderSingle(int x, int y, bool isBehind, ref ulong prevStyle) {
        if (isBehind) System.Console.SetCursorPosition(x, y);
        
        if (Styles[x, y].Raw == prevStyle) {
            StandardOutput.Write(Chars[x, y]);
        }
        else {
            StandardOutput.Write(Chars[x, y].AddStyle(Styles[x, y]));
            prevStyle = Styles[x, y].Raw;
        } 
    }
}