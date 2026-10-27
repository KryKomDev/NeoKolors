# NeoKolors.Tui.Lifecycle.Game2D

[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml) [![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Lifecycle.Game2D?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Game2D) [![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Lifecycle.Game2D?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Lifecycle.Game2D) [![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)

`NeoKolors.Tui.Lifecycle.Game2D` is a dedicated 2D terminal game engine providing fixed-timestep game loops, 2D camera viewport transformations, and modular multi-stage rendering pipelines.

---

## Architecture

```
                       ┌─────────────────────────┐
                       │       Game2DLoop        │
                       └────────────┬────────────┘
                                    │
            ┌───────────────────────┴────────────────────────┐
            ▼                                                ▼
┌────────────────────────┐                       ┌───────────────────────┐
│ FixedTimestep Tick     │                       │ Variable Render Tick  │
│  - Physics & Logic     │                       │  - Render2DPipeline   │
│  - 60 Hz deterministic │                       │  - Camera2D transform │
└────────────────────────┘                       └───────────┬───────────┘
                                                             │
                              ┌──────────────────────────────┼──────────────────────────────┐
                              ▼                              ▼                              ▼
                     ┌──────────────────┐           ┌──────────────────┐           ┌──────────────────┐
                     │ Background Stage │           │  Entities Stage  │           │    HUD Stage     │
                     └──────────────────┘           └──────────────────┘           └──────────────────┘
```

---

## Core Components

### 1. Game Loop (`Game2DLoop`)

`Game2DLoop` drives the game execution, decoupling physics updates from rendering:
- `TargetFps`: Target render framerate (e.g. 60 or 120 FPS).
- `FixedTicksPerSecond`: Fixed simulation rate (default 60 Hz).
- `OnFixedUpdate`: Dispatched for fixed physics/logic updates.
- `OnVariableUpdate`: Dispatched per render frame with actual delta time.

```csharp
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Lifecycle.Game2D;

var screen = new NKCharScreen(new Size2D(120, 35));
var camera = new Camera2D(new Rect2D(0, 0, 120, 35));
var pipeline = new Render2DPipeline();

var loop = new Game2DLoop(screen, camera, pipeline) {
    TargetFps = 60,
    FixedTicksPerSecond = 60
};

loop.OnFixedUpdate += dt => {
    // 60 Hz physics update
    player.Update(dt);
    enemyManager.Update(dt);
};
```

---

### 2. 2D Camera (`Camera2D`)

Translates world coordinates to terminal character screen cells, supporting viewport scrolling, zooming, and tracking targets:

```csharp
using NeoKolors.Tui.Lifecycle.Game2D;

var camera = new Camera2D(viewport: new Rect2D(0, 0, 80, 24));
camera.Position = player.Position; // Follow player

// Convert world point to screen coordinates
Point2D screenPos = camera.WorldToScreen(enemy.Position);
```

---

### 3. Modular Rendering Pipeline (`Render2DPipeline` & `IGame2DStage`)

Render passes are organized into cleanly isolated stages:

```csharp
public class WorldEntitiesStage(GameState state) : IGame2DStage {
    public void Render(Game2DRenderContext context) {
        var screen = context.Screen;
        var camera = context.Camera;

        foreach (var entity in state.Entities) {
            var pos = camera.WorldToScreen(entity.Position);
            if (camera.IsVisible(pos)) {
                screen.Place(entity.Sprite, pos);
            }
        }
    }
}

// Register stages
pipeline.AddStage(new StarfieldStage());
pipeline.AddStage(new WorldEntitiesStage(gameState));
pipeline.AddStage(new ParticleEffectsStage());
pipeline.AddStage(new HudOverlayStage());
```

---

## Complete Game Example

Check out [`Examples/ExampleGame2D.cs`](https://github.com/KryKomDev/NeoKolors/blob/main/Examples/ExampleGame2D.cs) for *Cosmic Invaders*, a complete, playable 2D arcade terminal game built with this engine.
