// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Lifecycle.Core;

namespace NeoKolors.Tui.Lifecycle.Inline;

/// <summary>
/// A specialized lifecycle host that coordinates running and rendering inline widgets and element hierarchies
/// in the standard console stream.
/// </summary>
public class InlineLifecycleHost : LifecycleController {
    private readonly SemaphoreSlim            _wakeSignal = new(0, 1);
    private readonly object                   _hostLock   = new();
    private          CancellationTokenSource? _sessionCts;
    private          IInlineScreen?           _screen;
    private volatile bool                     _isDirty    = true;

    /// <summary>
    /// Gets or sets the target frames per second for the background rendering loop.
    /// Default is 20 FPS.
    /// </summary>
    public int TargetFps { get; set; } = 20;

    /// <summary>
    /// Gets or sets whether the cursor should automatically be hidden while the widget runs.
    /// Default is <c>true</c>.
    /// </summary>
    public bool AutoHideCursor { get; set; } = true;

    /// <summary>
    /// Gets or sets whether terminal autowrap should temporarily be disabled to avoid layout overflow.
    /// Default is <c>true</c>.
    /// </summary>
    public bool DisableAutoWrap { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the widget remains rendered on the console after execution finishes.
    /// Default is <c>true</c>.
    /// </summary>
    public bool PersistOnExit { get; set; } = true;

    /// <summary>
    /// Gets or sets the screen presenter used to render frames.
    /// If null, a default <see cref="NKInlineScreen"/> is allocated automatically.
    /// </summary>
    public IInlineScreen? Screen {
        get => _screen;
        set => _screen = value;
    }

    /// <summary>
    /// Occurs on every frame tick before layout measurement and rendering occurs.
    /// </summary>
    public event Action<TimeSpan>? Tick;

    /// <summary>
    /// Requests an immediate frame redraw without waiting for the next periodic timer interval.
    /// </summary>
    public void Invalidate() {
        _isDirty = true;
        if (_wakeSignal.CurrentCount == 0) {
            try {
                _wakeSignal.Release();
            }
            catch (SemaphoreFullException) {
                // Ignore if already signaled
            }
        }
    }

    /// <summary>
    /// Requests the inline session to exit gracefully.
    /// </summary>
    public void RequestExit() {
        lock (_hostLock) {
            _sessionCts?.Cancel();
        }
    }

    /// <summary>
    /// Executes the specified asynchronous workload while continuously rendering the visual element hierarchy inline.
    /// </summary>
    /// <param name="root">The root visual element to render.</param>
    /// <param name="action">The asynchronous workload to execute.</param>
    /// <param name="cancellationToken">An optional external cancellation token.</param>
    public async Task RunAsync(IElement root, Func<InlineContext, Task> action, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(action);

        lock (_hostLock) {
            if (IsRunning) {
                throw new InvalidOperationException("This inline lifecycle host is already running a session.");
            }

            _sessionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        var cts   = _sessionCts;
        var token = cts.Token;

        var context = new InlineContext(this, token) {
            PersistOnExit = PersistOnExit
        };

        var initialWidth = Math.Max(10, NKConsole.WindowSize.X);
        var screen       = _screen ??= new NKInlineScreen(initialWidth, 1);

        if (AutoHideCursor) {
            NKConsole.HideCursor();
        }

        if (DisableAutoWrap) {
            NKConsole.OutputDriver.Write(EscapeCodes.GetDecRst(EscapeCodes.DecMode.AUTOWRAP));
        }

        Initialize();
        Start();

        var fpsInterval  = TimeSpan.FromMilliseconds(1000.0 / Math.Clamp(TargetFps, 1, 120));
        var stopwatch    = Stopwatch.StartNew();
        var lastElapsed  = TimeSpan.Zero;
        var hasTickables = HasTickable(root);
        _isDirty = true;

        // Background rendering task
        var renderTask = Task.Run(
            async () => {
                while (!token.IsCancellationRequested && IsRunning) {
                    var currentElapsed = stopwatch.Elapsed;
                    var delta          = currentElapsed - lastElapsed;
                    lastElapsed = currentElapsed;

                    // 1. Tick animations and time-based states
                    if (hasTickables) {
                        TickTree(root, delta);
                        _isDirty = true;
                    }

                    if (Tick is not null) {
                        Tick.Invoke(delta);
                        _isDirty = true;
                    }

                    // 2. Render frame
                    if (_isDirty || screen.RenderedHeight == 0) {
                        _isDirty = false;
                        RenderFrame(root, screen);
                    }

                    // 3. Wait for next frame or immediate invalidation
                    try {
                        await _wakeSignal.WaitAsync(fpsInterval, token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) {
                        break;
                    }
                }

                // Render final state upon completion if dirty or not yet rendered
                if (_isDirty || screen.RenderedHeight == 0) {
                    _isDirty = false;
                    RenderFrame(root, screen);
                }
            },
            CancellationToken.None
        );

        try {
            await action(context).ConfigureAwait(false);
        }
        finally {
            cts.Cancel();

            try {
                await renderTask.ConfigureAwait(false);
            }
            catch (Exception) {
                // Suppress background loop cancellation errors
            }

            Stop();

            screen.Finish(context.PersistOnExit);

            if (DisableAutoWrap) {
                NKConsole.OutputDriver.Write(EscapeCodes.GetDecSet(EscapeCodes.DecMode.AUTOWRAP));
            }

            if (AutoHideCursor) {
                NKConsole.ShowCursor();
            }

            lock (_hostLock) {
                _sessionCts?.Dispose();
                _sessionCts = null;
            }
        }
    }

    /// <summary>
    /// Executes the specified synchronous workload while continuously rendering the visual element hierarchy inline.
    /// </summary>
    /// <param name="root">The root visual element to render.</param>
    /// <param name="action">The synchronous workload to execute.</param>
    public void Run(IElement root, Action<InlineContext> action) {
        ArgumentNullException.ThrowIfNull(action);

        RunAsync(
            root,
            ctx => {
                action(ctx);

                return Task.CompletedTask;
            }
        ).GetAwaiter().GetResult();
    }

    private static void RenderFrame(IElement root, IInlineScreen screen) {
        var terminalWidth = Math.Max(10, NKConsole.WindowSize.X);
        var layoutCtx     = new LayoutContext(new Size2D(terminalWidth, 1000), new Size2D(terminalWidth, 1000));
        var measured      = root.Measure(layoutCtx);
        var targetHeight  = Math.Max(1, measured.Y);

        if (screen.Width != terminalWidth || screen.Height != targetHeight) {
            screen.Resize(new Size2D(terminalWidth, targetHeight));
        }

        screen.Clean();
        var renderCtx = new RenderContext(screen);
        root.Render(renderCtx, screen.Size);
        screen.Render();
    }

    private static void TickTree(IElement? element, TimeSpan delta) {
        if (element is null)
            return;

        if (element is ITickableElement tickable) {
            tickable.Tick(delta);
        }

        if (element is Flex flex) {
            var children = flex.Children.AsT;

            if (children is not null) {
                for (int i = 0; i < children.Count; i++) {
                    TickTree(children[i], delta);
                }
            }
        }
        else if (element is Border border) {
            TickTree(border.Element.AsT, delta);
        }
    }

    private static bool HasTickable(IElement? element) {
        if (element is null)
            return false;

        if (element is ITickableElement)
            return true;

        if (element is Flex flex) {
            var children = flex.Children.AsT;

            if (children is not null) {
                for (int i = 0; i < children.Count; i++) {
                    if (HasTickable(children[i]))
                        return true;
                }
            }
        }
        else if (element is Border border) {
            return HasTickable(border.Element.AsT);
        }

        return false;
    }
}