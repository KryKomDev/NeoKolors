// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Core;

/// <summary>
/// A stack-based scene manager supporting stacked scenes (e.g. pause menus over gameplay)
/// and full scene transitions.
/// </summary>
/// <typeparam name="TContext">The context type passed during scene transitions and updates.</typeparam>
public class SceneStack<TContext> : ISceneManager<TContext> {
    private readonly Stack<IScene<TContext>> _stack = new();
    private readonly Lock _syncRoot = new();

    /// <inheritdoc/>
    public IScene<TContext>? CurrentScene {
        get {
            lock (_syncRoot) {
                return _stack.Count > 0 ? _stack.Peek() : null;
            }
        }
    }

    /// <inheritdoc/>
    public int SceneCount {
        get {
            lock (_syncRoot) {
                return _stack.Count;
            }
        }
    }

    /// <inheritdoc/>
    public void PushScene(IScene<TContext> scene, TContext context) {
        ArgumentNullException.ThrowIfNull(scene);

        lock (_syncRoot) {
            if (_stack.Count > 0) {
                var previous = _stack.Peek();
                previous.OnPause(context);
            }

            _stack.Push(scene);
            scene.OnEnter(context);
        }
    }

    /// <inheritdoc/>
    public IScene<TContext>? PopScene(TContext context) {
        lock (_syncRoot) {
            if (_stack.Count == 0) {
                return null;
            }

            var popped = _stack.Pop();
            popped.OnExit(context);

            if (_stack.Count > 0) {
                var resumed = _stack.Peek();
                resumed.OnResume(context);
            }

            return popped;
        }
    }

    /// <inheritdoc/>
    public void SwitchScene(IScene<TContext> scene, TContext context) {
        ArgumentNullException.ThrowIfNull(scene);

        lock (_syncRoot) {
            if (_stack.Count > 0) {
                var current = _stack.Pop();
                current.OnExit(context);
            }

            _stack.Push(scene);
            scene.OnEnter(context);
        }
    }

    /// <inheritdoc/>
    public void Update(TContext context, DeltaTime time) {
        var activeScene = CurrentScene;
        activeScene?.OnUpdate(context, time);
    }
}
