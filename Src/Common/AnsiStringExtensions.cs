// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Common;

public static class AnsiStringExtensions {
    
    public static AnsiString ToAnsiString(this object o) {
        var s = o?.ToString();

        return s != null ? new AnsiString(s) : AnsiString.Empty;
    }

}