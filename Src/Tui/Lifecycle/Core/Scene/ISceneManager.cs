// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Core;

/// <summary>
/// Manages the activation, transitions, and execution flow of scenes.
/// </summary>
/// <typeparam name="TContext">The context type passed during scene transitions and updates.</typeparam>
public interface ISceneManager<TContext> {
    /// <summary>
    /// Gets the current foreground active scene, or <c>null</c> if no scenes are active.
    /// </summary>
    IScene<TContext>? CurrentScene { get; }

    /// <summary>
    /// Gets the number of scenes currently residing in the stack.
    /// </summary>
    int SceneCount { get; }

    /// <summary>
    /// Pushes a new scene on top of the stack, pausing the currently active scene.
    /// </summary>
    void PushScene(IScene<TContext> scene, TContext context);

    /// <summary>
    /// Pops the active scene from the stack, calling its exit routine, and resumes the previous scene.
    /// </summary>
    /// <returns>The scene that was popped, or <c>null</c> if the stack was empty.</returns>
    IScene<TContext>? PopScene(TContext context);

    /// <summary>
    /// Replaces the current scene entirely with a new scene, exiting the old and entering the new.
    /// </summary>
    void SwitchScene(IScene<TContext> scene, TContext context);

    /// <summary>
    /// Updates the currently active scene with the frame timing information.
    /// </summary>
    void Update(TContext context, DeltaTime time);
}
