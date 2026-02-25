// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Common;

namespace NeoKolors.Console.Ansi;

public readonly record struct AnsiRequest {
    public int Mode { get; }
    public bool IsOsc { get; }

    public AnsiRequest(EscapeCodes.DecMode mode) {
        Mode = (int)mode;
        IsOsc = false;
    }

    public AnsiRequest(EscapeCodes.OscMode mode) {
        Mode = (int)mode;
        IsOsc = true;
    }
    
    public AnsiRequest(int mode, bool isOsc) {
        Mode = mode;
        IsOsc = isOsc;
    }
}