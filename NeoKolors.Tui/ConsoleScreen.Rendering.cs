//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Common.Util;
using NeoKolors.Console;

namespace NeoKolors.Tui;

/// <summary>
/// virtual console that saves all the styles, colors and characters
/// </summary>
public partial class ConsoleScreen {
    
    public void Render() {
        NKColor fp = default;
        NKColor bp = default;
        TextStyles sp = default;
        bool isBehind = true;

        for (int y = 0; y < Height; y++) {

            TryMoveCursor(0, y);
            
            for (int x = 0; x < Width; x++) {
            
                if (!_changes[x, y]) {
                    isBehind = true;
                    continue;
                }

                if (isBehind) {
                    TryMoveCursor(x, y);
                    isBehind = false;
                }

                if (fp != _styles[x, y].FColor) {
                    fp = _styles[x, y].FColor;
                    StdOut.Write(fp.Text);
                }

                if (bp != _styles[x, y].BColor) {
                    bp = _styles[x, y].BColor;
                    StdOut.Write(bp.Bckg);
                }

                if (sp != _styles[x, y].Styles) {
                    sp = _styles[x, y].Styles;
                    StdOut.Write(Chars[x, y].AddCStyle(sp));
                }
                else {
                    StdOut.Write(_chars[x, y]);
                }
            }
        }
        
        System.Console.Write(EscapeCodes.FORMATTING_RESET);
        List2D.Fill(_changes, false);
    }
    
    private void TryMoveCursor(int x, int y) {
        try {
            System.Console.SetCursorPosition(x, y);
        }
        catch (ArgumentOutOfRangeException) {
            // terminal is probably being resized, ignore this error
            NKDebug.Warn("Cursor out of bounds! Ignoring...");
        }
    }
}