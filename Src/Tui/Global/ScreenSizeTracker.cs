// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Global;

internal static class ScreenSizeTracker {

    private static Size2D PX = new(0, 0);
    private static Size2D CH = new(0, 0);
    
    public static void SetScreenSizePx(Size2D screenSize) => PX = screenSize;
    public static void SetScreenSizeCh(Size2D screenSize) => CH = screenSize;
    public static Size2D GetScreenSizePx() => PX;
    public static Size2D GetScreenSizeCh() => CH;
}