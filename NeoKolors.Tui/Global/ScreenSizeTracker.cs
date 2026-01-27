// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Global;

internal static class ScreenSizeTracker {

    private static Size PX = new(0, 0);
    private static Size CH = new(0, 0);
    
    public static void SetScreenSizePx(Size screenSize) => PX = screenSize;
    public static void SetScreenSizeCh(Size screenSize) => CH = screenSize;
    public static Size GetScreenSizePx() => PX;
    public static Size GetScreenSizeCh() => CH;
}