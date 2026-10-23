// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Core;

/// <summary>
/// Defines a discrete application or game state/screen that participates in a managed scene lifecycle.
/// </summary>
/// <typeparam name="TContext">The context type passed during lifecycle transitions and updates.</typeparam>
public interface IScene<in TContext> {
    /// <summary>
    /// Gets the human-readable identifier of this scene.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the scene is currently the active foreground scene.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Called when the scene becomes active and enters the foreground.
    /// </summary>
    void OnEnter(TContext context);

    /// <summary>
    /// Called when the scene is removed from the active stack and terminated.
    /// </summary>
    void OnExit(TContext context);

    /// <summary>
    /// Called when another scene is pushed on top of this scene, causing it to become inactive.
    /// </summary>
    void OnPause(TContext context);

    /// <summary>
    /// Called when a scene above this one is popped, restoring this scene to the foreground.
    /// </summary>
    void OnResume(TContext context);

    /// <summary>
    /// Updates the scene logic during each variable frame tick.
    /// </summary>
    void OnUpdate(TContext context, DeltaTime time);
}
