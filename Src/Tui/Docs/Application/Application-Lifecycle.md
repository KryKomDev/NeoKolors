# TUI Application Lifecycle and Bootstrapping

The **[NKApplication](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/NKApplication.cs)** class is the controller for a NeoKolors TUI application. It manages the boot sequence, hooks native console input interceptors, routes events, and controls the main rendering thread.

---

## 1. Application Configuration (`NKAppConfig`)

Bootstrapping is configured via **[NKAppConfig](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/NKAppConfig.cs)**:

```csharp
var config = new NKAppConfig {
    // True: exit command will not trigger on Ctrl+C, allowing custom interception
    CtrlCForceQuits = false,
    
    // Auto-pause the render loop if console window loses OS focus
    PauseOnFocusLost = true,
    
    // Automatically hide terminal cursor blinking
    KeepCursorDisabled = true,

    // Rendering configuration mode
    Rendering = new RenderingConfig {
        IsLazy = true // True: renders only when events trigger visual updates
    }
};
```

---

## 2. Boot Sequence Flow

When `NKApplication.Start()` is invoked, the following operations run:

```
[Start()]
   │
   ▼
[Configure Console Modes] (Mouse reporting, Alt screen buffer, paste mode)
   │
   ▼
[Subscribe Drivers] (NKConsole input event delegates -> App event handlers)
   │
   ▼
[Set Event Bus Host] (AppEventBus.SetSourceApplication)
   │
   ▼
[Trigger StartEvent] (Invoke Start handlers)
   │
   ▼
[Enter Run Loop] (RunLazy or RunUnlimited)
```

---

## 3. Render Loop Execution

NeoKolors supports two rendering models, configured via `RenderingConfig`:

### 3.1 Lazy Rendering Loop (`IsLazy = true`)
The engine sits idle waiting for system inputs. When a keypress, mouse click, window resize, or manual notification triggers (calling `UpdateLayout()` or updating element states), the loop schedules a single frame render. This keeps CPU usage at 0% when idle.

### 3.2 Unlimited/Thread-Driven Loop (`IsLazy = false`)
The engine renders at a maximum frame rate or continuously inside an asynchronous thread loop. This is suitable for games or rich real-time animations.

---

## 4. Lifecycle Event Subscriptions

You can hook into application lifecycle milestones to initialize services:

```csharp
using NeoKolors.Tui;
using NeoKolors.Tui.Events;

var app = new NKApplication(config, baseViewElement);

// Triggered when application starts rendering
app.StartEvent += (sender, args) => {
    NKDebug.Info("TUI started, lazy mode: {Lazy}", args.IsLazy);
};

// Triggered when app window resizes
app.ResizeEvent += (args) => {
    NKDebug.Debug("Console resized to {Width}x{Height}", args.NewSize.Width, args.NewSize.Height);
};

// Start executing
app.Start();
```
