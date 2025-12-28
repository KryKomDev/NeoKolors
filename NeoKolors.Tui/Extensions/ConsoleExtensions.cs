// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Extensions;

public static class ConsoleExtensions {
    extension(Stdio) {
        public static Size WindowSize => new(Stdio.WindowWidth, Stdio.WindowHeight);
        public static Size BufferSize => new(Stdio.BufferWidth, Stdio.BufferHeight);
    }
}