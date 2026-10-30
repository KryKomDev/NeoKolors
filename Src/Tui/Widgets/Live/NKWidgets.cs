// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Widgets.Elements;
using NeoKolors.Tui.Widgets.Lifecycle;
using NeoKolors.Tui.Widgets.Rendering;

namespace NeoKolors.Tui.Widgets.Live;

/// <summary>
/// Convenient entry points and helper methods for rendering inline terminal widgets and live elements.
/// </summary>
public static class NKWidgets {
    /// <summary>
    /// Executes an asynchronous operation while rendering an animated inline spinner and status message.
    /// </summary>
    /// <param name="initialText">The initial message displayed beside the spinner.</param>
    /// <param name="action">The asynchronous workload to execute.</param>
    /// <param name="spinnerStyle">Optional custom spinner frame preset.</param>
    /// <param name="persistOnExit">Whether to preserve the final status line on console upon completion.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    public static async Task StatusAsync(
        string                    initialText,
        Func<StatusContext, Task> action,
        SpinnerStyle?             spinnerStyle      = null,
        bool                      persistOnExit     = true,
        CancellationToken         cancellationToken = default
    ) {
        ArgumentNullException.ThrowIfNull(action);

        var statusElement = new StatusElement(initialText, spinnerStyle);

        var host = new InlineLifecycleHost {
            PersistOnExit = persistOnExit
        };

        await host.RunAsync(
            statusElement,
            async inlineCtx => {
                var statusCtx = new StatusContext(statusElement, inlineCtx);

                try {
                    await action(statusCtx).ConfigureAwait(false);

                    if (!statusElement.IsCompleted.AsT && !statusElement.IsFailed.AsT)
                        statusCtx.Success();
                }
                catch (Exception) {
                    statusCtx.Fail();

                    throw;
                }
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes a synchronous operation while rendering an animated inline spinner and status message.
    /// </summary>
    public static void Status(
        string                initialText,
        Action<StatusContext> action,
        SpinnerStyle?         spinnerStyle  = null,
        bool                  persistOnExit = true
    ) {
        ArgumentNullException.ThrowIfNull(action);

        StatusAsync(
            initialText,
            ctx => {
                action(ctx);

                return Task.CompletedTask;
            },
            spinnerStyle,
            persistOnExit
        ).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Executes an asynchronous operation while rendering an inline progress bar and descriptive title.
    /// </summary>
    /// <param name="title">The title text displayed above or beside the progress bar.</param>
    /// <param name="action">The asynchronous workload to execute.</param>
    /// <param name="persistOnExit">Whether to preserve the progress bar on console upon completion.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    public static async Task ProgressAsync(
        string                      title,
        Func<ProgressContext, Task> action,
        bool                        persistOnExit     = true,
        CancellationToken           cancellationToken = default
    ) {
        ArgumentNullException.ThrowIfNull(action);

        var descSignal = new Signal<string>(title);
        var bar        = new ProgressBarElement();

        var layout = Flex.Column(
            new Text((Func<AnsiString>)(() => new AnsiString(descSignal.Value))),
            bar
        );

        var host = new InlineLifecycleHost {
            PersistOnExit = persistOnExit
        };

        await host.RunAsync(
            layout,
            async inlineCtx => {
                var progCtx = new ProgressContext(bar, descSignal, inlineCtx);
                await action(progCtx).ConfigureAwait(false);
            },
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes a synchronous operation while rendering an inline progress bar and descriptive title.
    /// </summary>
    public static void Progress(
        string                  title,
        Action<ProgressContext> action,
        bool                    persistOnExit = true
    ) {
        ArgumentNullException.ThrowIfNull(action);

        ProgressAsync(
            title,
            ctx => {
                action(ctx);

                return Task.CompletedTask;
            },
            persistOnExit
        ).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Executes an asynchronous operation while continuously rendering an arbitrary declarative UI element hierarchy inline.
    /// </summary>
    /// <param name="root">The root visual element (e.g. composed of Flex, Grid, Border, Table, Text, or custom widgets).</param>
    /// <param name="action">The workload to execute while the UI is live.</param>
    /// <param name="fps">Target frame rate for redrawing.</param>
    /// <param name="persistOnExit">Whether the rendered UI remains visible on the console after exit.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    public static async Task LiveAsync(
        IElement                  root,
        Func<InlineContext, Task> action,
        int                       fps               = 20,
        bool                      persistOnExit     = true,
        CancellationToken         cancellationToken = default
    ) {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(action);

        var host = new InlineLifecycleHost {
            TargetFps     = fps,
            PersistOnExit = persistOnExit
        };

        await host.RunAsync(root, action, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes a synchronous operation while continuously rendering an arbitrary declarative UI element hierarchy inline.
    /// </summary>
    public static void Live(
        IElement              root,
        Action<InlineContext> action,
        int                   fps           = 20,
        bool                  persistOnExit = true
    ) {
        ArgumentNullException.ThrowIfNull(action);

        LiveAsync(
            root,
            ctx => {
                action(ctx);

                return Task.CompletedTask;
            },
            fps,
            persistOnExit
        ).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Renders an arbitrary declarative UI element hierarchy once directly to the console inline stream.
    /// </summary>
    /// <param name="root">The root visual element to render.</param>
    public static void Render(IElement root) {
        ArgumentNullException.ThrowIfNull(root);

        var terminalWidth = Math.Max(10, NKConsole.WindowSize.X);
        var layoutCtx     = new LayoutContext(new Size2D(terminalWidth, 1000), new Size2D(terminalWidth, 1000));
        var measured      = root.Measure(layoutCtx);
        var targetHeight  = Math.Max(1, measured.Y);

        var screen = new NKInlineScreen(terminalWidth, targetHeight);
        screen.Clean();
        var renderCtx = new RenderContext(screen);
        root.Render(renderCtx, screen.Size);

        NKConsole.OutputDriver.Write(EscapeCodes.GetDecRst(EscapeCodes.DecMode.AUTOWRAP));
        try {
            screen.Render();
            screen.Finish(persist: true);
        }
        finally {
            NKConsole.OutputDriver.Write(EscapeCodes.GetDecSet(EscapeCodes.DecMode.AUTOWRAP));
        }
    }
}