// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts.V2;

public readonly struct MonospaceInfo {
    public bool IsMonospace { get; }

    public int CharacterWidth {
        get {
            if (!IsMonospace) 
                throw new InvalidOperationException("Non-monospace fonts do not have fixed character width.");

            return field;
        }
    }
    
    public int CharacterHeight { 
        get {
            if (!IsMonospace) 
                throw new InvalidOperationException("Non-monospace fonts do not have fixed character width.");

            return field;
        } 
    }

    private MonospaceInfo(bool isMonospace, int charWidth, int charHeight) {
        IsMonospace = isMonospace;
        CharacterWidth = charWidth;
        CharacterHeight = charHeight;   
    }

    public static MonospaceInfo Mono(int width, int height) => new(true, width, height);
    public static MonospaceInfo Var() => new(false, 0, 0);
}