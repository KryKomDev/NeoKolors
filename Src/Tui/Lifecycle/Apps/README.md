# NeoKolors.Tui.Lifecycle.Apps

[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Lifecycle.Apps?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Apps) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Lifecycle.Apps?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Apps) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui.Lifecycle.Apps` provides the application hosting infrastructure, execution loop strategies, window focus management, and view binding for interactive TUI applications.

---

## Key Components

### 1. Application Host (`AppLifecycleHost`)

`AppLifecycleHost` coordinates application states:
- **Activation State**: `AppActivationState` (`ACTIVE`, `INACTIVE`, `SUSPENDED`).
- **Focus Management**: Automatically manages whether the terminal has OS window focus (`HasFocus`).
- **Terminal Resizing**: Responds to terminal resize events (`NotifyWindowResized(width, height)`).
- **Graceful Termination**: Handles close requests (`RequestClose()`, `CloseRequested` event with cancellation option).

```csharp
using NeoKolors.Tui.Lifecycle.Apps;

var host = new AppLifecycleHost();

host.CloseRequested += (sender, e) => {
    if (HasUnsavedChanges()) {
        e.Cancel = true; // Prevent shutdown
        ShowUnsavedDialog();
    }
};

host.Start();
```

---

### 2. View Presenter (`NKAppView`)

`NKAppView` connects a root `IElement` layout tree to an `ICharScreen` and `HitMap`:
- Manages dirty tracking (`Invalidate()`, `IsDirty`, `ResetDirty()`).
- Fires lifecycle events: `BeforeRender`, `Render`, `AfterRender`.
- Replaces or updates the active UI hierarchy seamlessly (`view.RootElement = BuildUi()`).

```csharp
using NeoKolors.Tui.Lifecycle.Apps;

var view = new NKAppView(rootElement, screen, hitMap);

// Trigger re-render whenever state changes
stateSignal.Changed += () => {
    view.RootElement = BuildUpdatedUi();
    view.Invalidate();
};
```

---

### 3. Rendering Loop Strategies (`IAppLoopStrategy`)

#### `ThrottledAppLoopStrategy`
Maintains high frame rates when the application is focused, and throttles down when the application loses focus:
- `ActiveFps`: Target FPS when focused (e.g. 60 FPS).
- `BackgroundFps`: Target FPS when inactive (e.g. 10 FPS) to drastically reduce CPU usage.
- `RenderOnlyWhenDirty`: Only renders frames if `view.IsDirty` is true.
- `SignalWakeup()`: Thread-safe signal to immediately awaken the loop from sleeping on user interaction.

```csharp
var loopStrategy = new ThrottledAppLoopStrategy {
    ActiveFps = 60,
    BackgroundFps = 10,
    RenderOnlyWhenDirty = true
};

while (host.State != LifecycleState.STOPPED) {
    loopStrategy.Tick(host, view);
}
```

#### `LazyAppLoopStrategy`
Designed for classic terminal utilities:
- Remains completely asleep until an input event, resize notification, or wakeup signal is dispatched.
- Uses 0% CPU while idle.

---

## Complete Boilerplate Example

See [`Examples/ExampleApp.cs`](https://github.com/KryKomDev/NeoKolors/blob/main/Examples/ExampleApp.cs) for a complete production-grade application loop using `AppLifecycleHost`, `NKAppView`, and `ThrottledAppLoopStrategy`.
