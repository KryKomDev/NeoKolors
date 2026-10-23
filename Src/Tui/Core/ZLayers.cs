// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Core;

/// <summary>
/// Provides predefined constants for z-index layers used to manage rendering order of visual elements.
/// </summary>
/// <remarks>
/// The constants in this class define rendering priorities for common UI elements,
/// ensuring a consistent stacking order across the application.
/// </remarks>
public static class ZLayers {
    public const int BACKGROUND = 0;
    public const int CONTENT    = 1_000;
    public const int ELEVATED   = 2_000;  // Focused windows, floating panels
    public const int POPUP      = 10_000; // Dropdowns, context menus
    public const int MODAL      = 20_000; // Dialogs
    public const int TOOLTIP    = 30_000; // Tooltips, drag-and-drop ghosts
}