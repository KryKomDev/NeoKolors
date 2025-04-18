//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Events;

/// <summary>
/// arguments for when the application is being started
/// </summary>
public class AppStartEventArgs : EventArgs {
    
    /// <summary>
    /// determines whether the application is rendered lazily
    /// (if true application is rerendered only on key press)
    /// </summary>
    public bool IsAppLazyRendered { get; }

    /// <param name="isAppLazyRendered">
    /// determines whether the application is rendered lazily
    /// (if true application is rerendered only on key press)
    /// </param>
    public AppStartEventArgs(bool isAppLazyRendered) {
        IsAppLazyRendered = isAppLazyRendered;
    }
}