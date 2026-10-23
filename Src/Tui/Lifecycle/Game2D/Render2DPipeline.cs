// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Game2D;

/// <summary>
///     Defines the sequential layers in a 2D rendering pipeline.
/// </summary>
public enum Render2DLayer {
    /// <summary>
    ///     Background tiles, parallax scrolling skyboxes, or backdrop color.
    /// </summary>
    BACKGROUND = 0,

    /// <summary>
    ///     Main terrain, static obstacles, or world tilemaps.
    /// </summary>
    WORLD = 100,

    /// <summary>
    ///     Dynamic game objects, players, NPCs, and Y-sorted sprites.
    /// </summary>
    ENTITIES = 200,

    /// <summary>
    ///     Visual effects, explosions, smoke, and 2D particle emitters.
    /// </summary>
    PARTICLES = 300,

    /// <summary>
    ///     Lighting masks, weather overlays, and foreground decorative layers.
    /// </summary>
    FOREGROUND = 400,

    /// <summary>
    ///     Screen-space HUD, inventory, health bars, and user interface.
    /// </summary>
    UI = 500
}

/// <summary>
///     Contract representing an individual rendering pass in the 2D pipeline.
/// </summary>
public interface IRender2DStage {
    /// <summary>
    ///     Gets the unique identifier of this stage.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the target layer to which this stage belongs.
    /// </summary>
    Render2DLayer Layer { get; }

    /// <summary>
    ///     Gets the execution priority within the target layer. Lower numbers execute first.
    /// </summary>
    int OrderWithinLayer { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether this stage is active and should be executed.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    ///     Executes the rendering logic for this stage.
    /// </summary>
    void Render(Game2DRenderContext context);
}

/// <summary>
///     An extensible multi-stage 2D rendering pipeline that executes stages strictly ordered
///     by layer and priority.
/// </summary>
public class Render2DPipeline {
    private readonly List<IRender2DStage> _stages   = [];
    private readonly object               _syncRoot = new();

    /// <summary>
    ///     Gets the number of registered stages in the pipeline.
    /// </summary>
    public int StageCount {
        get {
            lock (_syncRoot) {
                return _stages.Count;
            }
        }
    }

    /// <summary>
    ///     Registers a rendering stage and maintains layer ordering.
    /// </summary>
    public void AddStage(IRender2DStage stage) {
        ArgumentNullException.ThrowIfNull(stage);

        lock (_syncRoot) {
            _stages.Add(stage);
            SortStages();
        }
    }

    /// <summary>
    ///     Removes a stage by name.
    /// </summary>
    /// <returns><c>true</c> if found and removed; otherwise <c>false</c>.</returns>
    public bool RemoveStage(string name) {
        lock (_syncRoot) {
            int index = _stages.FindIndex(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));

            if (index >= 0) {
                _stages.RemoveAt(index);

                return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     Executes all enabled stages in sequential order.
    /// </summary>
    public void Execute(Game2DRenderContext context) {
        lock (_syncRoot) {
            for (int i = 0; i < _stages.Count; i++) {
                var stage = _stages[i];

                if (stage.IsEnabled) {
                    stage.Render(context);
                }
            }
        }
    }

    /// <summary>
    ///     Clears all stages from the pipeline.
    /// </summary>
    public void Clear() {
        lock (_syncRoot) {
            _stages.Clear();
        }
    }

    private void SortStages() {
        _stages.Sort(
            (a, b) => {
                int layerComparison = a.Layer.CompareTo(b.Layer);

                return layerComparison != 0 ? layerComparison : a.OrderWithinLayer.CompareTo(b.OrderWithinLayer);
            }
        );
    }
}