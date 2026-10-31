// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics;
using NeoKolors.Tui.Lifecycle.Core;

namespace NeoKolors.Tui.Lifecycle.Game2D;

/// <summary>
///     A high-performance 2D game loop orchestrating decoupled fixed physics ticks,
///     variable logic updates, and interpolated multi-stage rendering passes.
/// </summary>
public class Game2DLoop : LifecycleController {
    private ulong _fixedTickCount = 0;

    /// <summary>
    ///     Gets the fixed-timestep accumulator used to decouple simulation from rendering.
    /// </summary>
    public FixedTimestepAccumulator Accumulator { get; } = new();

    /// <summary>
    ///     Gets the precision clock measuring delta times and tracking frame rate metrics.
    /// </summary>
    public FrameClock Clock { get; } = new();

    /// <summary>
    ///     Gets the main 2D camera viewport.
    /// </summary>
    public Camera2D Camera { get; } = new();

    /// <summary>
    ///     Gets the layered 2D rendering pipeline.
    /// </summary>
    public Render2DPipeline Pipeline { get; } = new();

    /// <summary>
    ///     Gets the scene manager coordinating active 2D game states.
    /// </summary>
    public Game2DSceneStack SceneStack { get; } = new();

    /// <summary>
    ///     Gets or sets the target frames-per-second. Set to -1 for unlimited frame rate. Default
    ///     is 60.
    /// </summary>
    public int TargetFps { get; set; } = 60;

    /// <summary>
    ///     Gets or sets optional user state passed into update and render contexts.
    /// </summary>
    public object? UserState { get; set; }

    /// <summary>
    ///     Gets the cumulative count of fixed simulation ticks processed.
    /// </summary>
    public ulong FixedTickCount => _fixedTickCount;

    /// <summary>
    ///     Occurs each frame before simulation steps to poll user input.
    /// </summary>
    public event Action? InputPolling;

    /// <summary>
    ///     Occurs during each fixed simulation step.
    /// </summary>
    public event Action<Game2DFixedContext>? FixedUpdating;

    /// <summary>
    ///     Occurs each frame during the variable gameplay logic update.
    /// </summary>
    public event Action<Game2DUpdateContext>? Updating;

    /// <summary>
    ///     Occurs each frame when rendering is executed.
    /// </summary>
    public event Action<Game2DRenderContext>? Rendering;

    /// <summary>
    ///     Runs the 2D game loop continuously until stopped or canceled.
    /// </summary>
    public void Run(CancellationToken cancellationToken = default) {
        if (!IsRunning) {
            Start();
        }

        Clock.Reset();
        Accumulator.Reset();
        var frameStopwatch = new Stopwatch();

        while (IsRunning && !cancellationToken.IsCancellationRequested) {
            if (IsPaused) {
                Thread.Sleep(16);
                Clock.Reset();

                continue;
            }

            frameStopwatch.Restart();

            var time = Clock.Tick();
            ProcessSingleFrame(time);

            if (TargetFps <= 0)
                continue;

            var targetFrameTime = TimeSpan.FromSeconds(1.0 / TargetFps);
            var elapsed         = frameStopwatch.Elapsed;

            if (elapsed >= targetFrameTime)
                continue;

            var remaining = targetFrameTime - elapsed;
            int sleepMs   = (int)remaining.TotalMilliseconds;

            if (sleepMs > 0) {
                Thread.Sleep(sleepMs);
            }
        }
    }

    /// <summary>
    ///     Advances the loop by a single simulated delta interval, useful for headless testing or
    ///     manual frame stepping.
    /// </summary>
    public void StepSimulation(TimeSpan delta) {
        DeltaTime time = new(delta, Clock.TotalTime + delta, Clock.FrameCount + 1);
        ProcessSingleFrame(time);
    }

    private void ProcessSingleFrame(DeltaTime time) {
        // 1. Poll input
        InputPolling?.Invoke();

        // 2. Accumulate fixed simulation time
        Accumulator.Accumulate(time.Delta);

        // 3. Process fixed simulation steps
        while (Accumulator.Step(out TimeSpan stepDelta)) {
            _fixedTickCount++;
            var fixedCtx = new Game2DFixedContext(stepDelta, _fixedTickCount, UserState);
            FixedUpdating?.Invoke(fixedCtx);
            SceneStack.FixedUpdate(fixedCtx);
        }

        // 4. Process variable gameplay update
        var updateCtx = new Game2DUpdateContext(time, UserState);
        Updating?.Invoke(updateCtx);
        SceneStack.Update(updateCtx, time);

        // 5. Execute 2D rendering passes with interpolation alpha
        var renderCtx = new Game2DRenderContext(Accumulator.Alpha, time, Camera, UserState);
        Pipeline.Execute(renderCtx);
        SceneStack.Render(renderCtx);
        Rendering?.Invoke(renderCtx);
    }
}