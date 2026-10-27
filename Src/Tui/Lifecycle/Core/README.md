# NeoKolors.Tui.Lifecycle.Core

[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Lifecycle.Core?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Core) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Lifecycle.Core?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Core) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui.Lifecycle.Core` provides the core lifecycle management, frame timing, deterministic time-step accumulation, and scene management for NeoKolors applications and games.

---

## Core Primitives

### 1. State Machine (`ILifecycle` & `LifecycleController`)

The `LifecycleController` manages execution states:
- `INITIALIZED`: Application created, ready to start.
- `RUNNING`: Main execution and render loop active.
- `PAUSED`: Execution suspended (e.g. window minimized or backgrounded).
- `STOPPED`: Application terminated and resources cleaned up.

```csharp
using NeoKolors.Tui.Lifecycle.Core;

var controller = new LifecycleController();
controller.StateChanged += (sender, e) => {
    Console.WriteLine($"Lifecycle changed: {e.OldState} -> {e.NewState}");
};

controller.Start();
// ...
controller.Pause();
// ...
controller.Resume();
// ...
controller.Stop();
```

---

### 2. High-Precision Frame Timing (`FrameClock`)

`FrameClock` calculates exact delta time between ticks, monitors frame rates, and computes smoothed FPS averages:

```csharp
using NeoKolors.Tui.Lifecycle.Core;

var clock = new FrameClock();
clock.Start();

while (isRunning) {
    DeltaTime dt = clock.Tick();
    
    double fps = clock.FramesPerSecond;
    double smoothedFps = clock.SmoothedFps;
    long totalFrames = clock.TotalFrames;
}
```

---

### 3. Deterministic Physics (`FixedTimestepAccumulator`)

When developing games or deterministic animations, render frame rates may fluctuate while physics/logic must run at an exact constant interval (e.g. 60 Hz). `FixedTimestepAccumulator` consumes variable frame delta times and executes exact fixed updates:

```csharp
using NeoKolors.Tui.Lifecycle.Core;

var accumulator = new FixedTimestepAccumulator(targetTps: 60);

// Inside your main loop:
accumulator.Accumulate(variableDeltaTime);

while (accumulator.ShouldUpdate()) {
    // Exactly 1/60th second step:
    physicsWorld.Step(accumulator.FixedDeltaTime);
}

// Alpha blend factor for interpolation between physics frames:
double alpha = accumulator.Alpha;
```

---

### 4. Scene Stack (`SceneStack` & `IScene`)

Manages multi-screen applications and game states (e.g. Main Menu, Settings, Gameplay, Game Over):

```csharp
using NeoKolors.Tui.Lifecycle.Core;

var sceneStack = new SceneStack();
sceneStack.Push(new MainMenuScene());

// Transition to new game:
sceneStack.Push(new GameplayScene());

// Return to menu:
sceneStack.Pop();
```
