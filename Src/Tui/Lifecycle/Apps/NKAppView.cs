// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Lifecycle.Apps;

/// <summary>
/// Default implementation of <see cref="IAppView"/> that renders an <see cref="IElement"/> hierarchy
/// onto an <see cref="ICharScreen"/> using <see cref="RenderContext"/>.
/// </summary>
public class NKAppView : IAppView {
    private readonly object _syncRoot = new();
    private IElement _rootElement;
    private ICharScreen _screen;
    private bool _isDirty = true;

    /// <summary>
    /// Gets or sets the root visual element rendered by this view.
    /// </summary>
    public IElement RootElement {
        get {
            lock (_syncRoot) {
                return _rootElement;
            }
        }
        set {
            ArgumentNullException.ThrowIfNull(value);
            lock (_syncRoot) {
                _rootElement = value;
            }
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the output character screen target.
    /// </summary>
    public ICharScreen Screen {
        get {
            lock (_syncRoot) {
                return _screen;
            }
        }
        set {
            ArgumentNullException.ThrowIfNull(value);
            lock (_syncRoot) {
                _screen = value;
            }
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the interactive hit map used to register click and mouse regions.
    /// </summary>
    public HitMap? HitMap { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the canvas buffer is cleaned before rendering. Default is <c>true</c>.
    /// </summary>
    public bool ClearBeforeRender { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the dirty flag is automatically reset upon successful rendering. Default is <c>true</c>.
    /// </summary>
    public bool AutoResetDirty { get; set; } = true;

    /// <inheritdoc/>
    public bool IsDirty {
        get {
            lock (_syncRoot) {
                return _isDirty;
            }
        }
        private set {
            lock (_syncRoot) {
                _isDirty = value;
            }
        }
    }

    /// <summary>
    /// Occurs immediately before rendering commences.
    /// </summary>
    public event Action? BeforeRender;

    /// <summary>
    /// Occurs immediately after rendering and screen flush completes.
    /// </summary>
    public event Action? AfterRender;

    /// <summary>
    /// Occurs whenever the view is invalidated.
    /// </summary>
    public event Action? Invalidated;

    /// <summary>
    /// Initializes a new instance of the <see cref="NKAppView"/> class.
    /// </summary>
    /// <param name="rootElement">The root UI element.</param>
    /// <param name="screen">The character screen destination.</param>
    /// <param name="hitMap">Optional hit map for mouse interaction dispatch.</param>
    public NKAppView(IElement rootElement, ICharScreen screen, HitMap? hitMap = null) {
        _rootElement = rootElement ?? throw new ArgumentNullException(nameof(rootElement));
        _screen = screen ?? throw new ArgumentNullException(nameof(screen));
        HitMap = hitMap;
    }

    /// <inheritdoc/>
    public void Invalidate() {
        IsDirty = true;
        Invalidated?.Invoke();
    }

    /// <inheritdoc/>
    public void ResetDirty() {
        IsDirty = false;
    }

    /// <inheritdoc/>
    public void Render() {
        lock (_syncRoot) {
            BeforeRender?.Invoke();

            if (ClearBeforeRender) {
                _screen.Clean();
            }

            Size2D screenSize = _screen.Size;
            var layoutContext = new LayoutContext(screenSize, screenSize);
            _rootElement.Measure(layoutContext);

            HitMap?.Clear();
            var renderContext = new RenderContext(_screen, HitMap);
            _rootElement.Render(renderContext, screenSize);
            HitMap?.Commit();

            _screen.Render();

            if (AutoResetDirty) {
                _isDirty = false;
            }

            AfterRender?.Invoke();
        }
    }
}
