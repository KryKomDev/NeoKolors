// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Lifecycle.Core;

namespace NeoKolors.Tui.Lifecycle.Game2D;

/// <summary>
///     Contract representing a scene in a 2D game, integrating fixed physics updates,
///     variable logic updates, and interpolated rendering passes.
/// </summary>
public interface IGame2DScene : IScene<Game2DUpdateContext> {

    /// <summary>
    ///     Executes deterministic fixed-step simulation (physics, collision) for this scene.
    /// </summary>
    void FixedUpdate(Game2DFixedContext context);

    /// <summary>
    ///     Renders the scene visual elements using the interpolated alpha factor and camera.
    /// </summary>
    void Render(Game2DRenderContext context);
}

/// <summary>
///     A scene manager specialized for 2D games, orchestrating fixed updates and rendering
///     passes.
/// </summary>
public class Game2DSceneStack : SceneStack<Game2DUpdateContext> {
    
    /// <summary>
    ///     Gets the current active 2D game scene, or <c>null</c> if no 2D scene is active.
    /// </summary>
    public IGame2DScene? Current2DScene => CurrentScene as IGame2DScene;

    /// <summary>
    ///     Dispatches a fixed simulation step to the currently active scene.
    /// </summary>
    public void FixedUpdate(Game2DFixedContext context) {
        Current2DScene?.FixedUpdate(context);
    }

    /// <summary>
    ///     Dispatches a rendering pass to the currently active scene.
    /// </summary>
    public void Render(Game2DRenderContext context) {
        Current2DScene?.Render(context);
    }
}