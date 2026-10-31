using System.Runtime.CompilerServices;
using Metriks;
using NeoKolors.Common;
using SkiaSharp;

namespace NeoKolors.Tui.Core;

/// <summary>
/// Provides a scoped immediate-mode rendering context with an internal state stack architecture for drawing onto <see cref="ICharCanvas"/>.
/// Supports coordinate translations, hierarchical clipping, z-index layering, style cascaded overrides, and writing outside element context areas.
/// </summary>
public class RenderContext : ICharDrawer {
    private readonly ICharCanvas        _canvas;
    private readonly Stack<RenderState> _stateStack = new();
    private          RenderState        _current    = new();

    /// <summary>
    /// Gets the underlying canvas that this render context writes to.
    /// </summary>
    public ICharCanvas Canvas => _canvas;

    /// <summary>
    /// Gets the width of the underlying canvas.
    /// </summary>
    public int CanvasWidth => _canvas.Width;

    /// <summary>
    /// Gets the height of the underlying canvas.
    /// </summary>
    public int CanvasHeight => _canvas.Height;

    /// <summary>
    /// Gets the size of the underlying canvas.
    /// </summary>
    public Size2D CanvasSize => _canvas.Size;

    /// <summary>
    /// Gets the full boundary area of the underlying canvas.
    /// </summary>
    public Rect2D CanvasBounds => new(0, 0, _canvas.Width, _canvas.Height);

    /// <summary>
    /// Gets the current depth of the internal state stack.
    /// </summary>
    public int StateDepth => _stateStack.Count;

    /// <summary>
    /// Gets the current depth of the internal state stack (alias for StateDepth).
    /// </summary>
    public int SaveCount => _stateStack.Count;

    /// <summary>
    /// Gets the current render state.
    /// </summary>
    public RenderState CurrentState => _current;

    /// <summary>
    /// Gets or sets the current coordinate origin translation in global canvas space.
    /// </summary>
    public Point2D Origin {
        get => _current.Origin;
        set => _current = _current with { Origin = value };
    }

    /// <summary>
    /// Gets or sets the current clip rectangle in global canvas space.
    /// </summary>
    public Rect2D? Clip {
        get => _current.Clip;
        set => _current = _current with { Clip = value };
    }

    /// <summary>
    /// Gets or sets the current z-index priority for drawing operations.
    /// </summary>
    public int ZIndex {
        get => _current.ZIndex;
        set => _current = _current with { ZIndex = value };
    }

    /// <summary>
    /// Gets or sets the current active styling applied to rendered elements and text.
    /// </summary>
    public NKStyle CurrentStyle {
        get => _current.Style;
        set => _current = _current with { Style = value };
    }

    /// <summary>
    /// Gets or sets whether drawing operations are permitted to write outside the current element's context area / clip boundaries.
    /// When set to true, clipping to <see cref="Clip"/> or <see cref="ContextArea"/> is bypassed, bounded only by the physical canvas dimensions.
    /// </summary>
    public bool AllowOverflow {
        get => _current.AllowOverflow;
        set => _current = _current with { AllowOverflow = value };
    }

    /// <summary>
    /// Gets or sets the designated context area bounds of the current element (in global canvas space).
    /// </summary>
    public Rect2D? ContextArea {
        get => _current.ContextArea;
        set => _current = _current with { ContextArea = value };
    }

    /// <summary>
    /// Gets or sets the active hit map used to record interactive spatial regions during the render pass.
    /// </summary>
    public HitMap? HitMap { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="RenderContext"/> for the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to render to.</param>
    /// <param name="hitMap">Optional hit map to record interactive targets during render.</param>
    public RenderContext(ICharCanvas canvas, HitMap? hitMap = null) {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        HitMap  = hitMap;
    }

    // =========================================================================
    //                            STATE STACK MANAGEMENT
    // =========================================================================

    /// <summary>
    /// Pushes a copy of the current state onto the state stack and returns a disposable scope.
    /// </summary>
    /// <returns>A <see cref="RenderScope"/> that restores the previous state when disposed.</returns>
    public RenderScope Save() {
        _stateStack.Push(_current);

        return new RenderScope(this, _stateStack.Count);
    }

    /// <summary>
    /// Restores the most recently saved state from the state stack.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the state stack is empty.</exception>
    public void Restore() {
        if (_stateStack.Count == 0) {
            throw new InvalidOperationException("Cannot restore render state: state stack is empty.");
        }

        _current = _stateStack.Pop();
    }

    /// <summary>
    /// Restores the state stack to the specified target depth.
    /// </summary>
    /// <param name="targetDepth">The target state depth.</param>
    public void RestoreToDepth(int targetDepth) {
        while (_stateStack.Count > targetDepth) {
            _current = _stateStack.Pop();
        }
    }

    /// <summary>
    /// Resets the context to its initial default state, clearing the entire state stack.
    /// </summary>
    public void Reset() {
        _stateStack.Clear();
        _current = new RenderState();
    }

    // =========================================================================
    //                              SCOPED HELPERS
    // =========================================================================

    /// <summary>
    /// Pushes a new state with custom parameter overrides and returns a disposable scope.
    /// </summary>
    public RenderScope PushScope(
        Point2D? offset        = null,
        Rect2D?  clip          = null,
        int?     zIndex        = null,
        NKStyle? style         = null,
        bool?    allowOverflow = null,
        Rect2D?  contextArea   = null,
        bool     intersectClip = true
    ) {
        var scope = Save();

        var newOrigin = _current.Origin + (offset ?? Point2D.Zero);

        Rect2D? newClip = _current.Clip;

        if (clip.HasValue) {
            var globalClip = clip.Value + newOrigin;

            newClip = (intersectClip && _current.Clip.HasValue)
                ? IntersectAreas(_current.Clip.Value, globalClip)
                : globalClip;
        }

        Rect2D? newContextArea = contextArea.HasValue ? (contextArea.Value + newOrigin) : _current.ContextArea;

        _current = new RenderState(
            origin: newOrigin,
            clip: newClip,
            zIndex: zIndex ?? _current.ZIndex,
            style: style.HasValue ? _current.Style.With(style.Value) : _current.Style,
            allowOverflow: allowOverflow ?? _current.AllowOverflow,
            contextArea: newContextArea
        );

        return scope;
    }

    /// <summary>
    /// Pushes a coordinate translation offset relative to the current origin.
    /// </summary>
    public RenderScope PushOffset(Point2D offset) {
        var scope = Save();
        _current = _current with { Origin = _current.Origin + offset };

        return scope;
    }

    /// <summary>
    /// Pushes a coordinate translation offset relative to the current origin.
    /// </summary>
    public RenderScope PushOffset(int dx, int dy) => PushOffset(new Point2D(dx, dy));

    /// <summary>
    /// Pushes an absolute origin in canvas coordinate space.
    /// </summary>
    public RenderScope PushOrigin(Point2D absoluteOrigin) {
        var scope = Save();
        _current = _current with { Origin = absoluteOrigin };

        return scope;
    }

    /// <summary>
    /// Pushes a local clipping rectangle (relative to current origin).
    /// </summary>
    public RenderScope PushClip(Rect2D localClip, bool intersect = true) {
        var scope      = Save();
        var globalClip = localClip + _current.Origin;

        _current = _current with {
            Clip = intersect && _current.Clip.HasValue
                ? IntersectAreas(_current.Clip.Value, globalClip)
                : globalClip
        };

        return scope;
    }

    /// <summary>
    /// Pushes an absolute clipping rectangle in global canvas coordinates.
    /// </summary>
    public RenderScope PushGlobalClip(Rect2D globalClip, bool intersect = true) {
        var scope = Save();

        _current = _current with {
            Clip = intersect && _current.Clip.HasValue
                ? IntersectAreas(_current.Clip.Value, globalClip)
                : globalClip
        };

        return scope;
    }

    /// <summary>
    /// Pushes a scope that enables writing outside of the current element's context area / clip boundaries.
    /// </summary>
    public RenderScope PushUnclipped() => AllowOverflowScope(true);

    /// <summary>
    /// Pushes a scope with the specified <see cref="AllowOverflow"/> setting.
    /// </summary>
    public RenderScope AllowOverflowScope(bool allow = true) {
        var scope = Save();
        _current = _current with { AllowOverflow = allow };

        return scope;
    }

    /// <summary>
    /// Pushes a specific z-index value onto the stack.
    /// </summary>
    public RenderScope PushZIndex(int zIndex) {
        var scope = Save();
        _current = _current with { ZIndex = zIndex };

        return scope;
    }

    /// <summary>
    /// Pushes a z-index delta added to the current z-index.
    /// </summary>
    public RenderScope PushZIndexDelta(int delta) {
        var scope = Save();
        _current = _current with { ZIndex = _current.ZIndex + delta };

        return scope;
    }

    /// <summary>
    /// Pushes a new style, overriding active styles.
    /// </summary>
    public RenderScope PushStyle(NKStyle style, bool overrideCurrent = true) {
        var scope = Save();
        _current = _current with { Style = overrideCurrent ? _current.Style.With(style) : style };

        return scope;
    }

    /// <summary>
    /// Pushes a foreground text color modifier.
    /// </summary>
    public RenderScope PushForeground(NKColor color) {
        var scope = Save();
        _current = _current with { Style = _current.Style.WithFColor(color) };

        return scope;
    }

    /// <summary>
    /// Pushes a background color modifier.
    /// </summary>
    public RenderScope PushBackground(NKColor color) {
        var scope = Save();
        _current = _current with { Style = _current.Style.WithBColor(color) };

        return scope;
    }

    /// <summary>
    /// Pushes a text style modifier flags.
    /// </summary>
    public RenderScope PushTextStyle(NKTextStyles textStyles) {
        var scope = Save();
        _current = _current with { Style = _current.Style.WithStyles(textStyles) };

        return scope;
    }

    /// <summary>
    /// Pushes a global scope (Origin = (0,0), Clip = null, AllowOverflow = true) for unconstrained canvas drawing.
    /// </summary>
    public RenderScope PushGlobal() {
        var scope = Save();
        _current = _current with { Origin = Point2D.Zero, Clip = null, AllowOverflow = true };

        return scope;
    }

    /// <summary>
    /// Pushes a context scope configured for the designated element context area.
    /// </summary>
    /// <param name="elementBounds">The render bounds of the element.</param>
    /// <param name="contentArea">The content box of the element relative to its bounds.</param>
    /// <param name="zIndex">The z-index of the element.</param>
    /// <param name="clipToContent">Whether to restrict clipping to the element's content box.</param>
    /// <param name="allowOverflow">Whether to allow writing outside the element's context area.</param>
    public RenderScope PushElementContext(
        Rect2D elementBounds,
        Rect2D contentArea,
        int    zIndex        = 0,
        bool   clipToContent = false,
        bool   allowOverflow = false
    ) {
        var scope         = Save();
        var origin        = elementBounds.Lower + contentArea.Lower;
        var globalContent = contentArea         + elementBounds.Lower;

        _current = new RenderState(
            origin: origin,
            clip: clipToContent ? globalContent : null,
            zIndex: zIndex,
            style: _current.Style,
            allowOverflow: allowOverflow,
            contextArea: globalContent
        );

        return scope;
    }

    // Fluent aliases for Push helpers
    public RenderScope WithOffset(Point2D         offset)     => PushOffset(offset);
    public RenderScope WithOffset(int             dx, int dy) => PushOffset(dx, dy);
    public RenderScope WithOrigin(Point2D         absoluteOrigin)                    => PushOrigin(absoluteOrigin);
    public RenderScope WithClip(Rect2D            localClip,  bool intersect = true) => PushClip(localClip, intersect);
    public RenderScope WithGlobalClip(Rect2D      globalClip, bool intersect = true) => PushGlobalClip(globalClip, intersect);
    public RenderScope WithZIndex(int             zIndex)     => PushZIndex(zIndex);
    public RenderScope WithStyle(NKStyle          style)      => PushStyle(style);
    public RenderScope WithForeground(NKColor     color)      => PushForeground(color);
    public RenderScope WithBackground(NKColor     color)      => PushBackground(color);
    public RenderScope WithTextStyle(NKTextStyles textStyles) => PushTextStyle(textStyles);
    public RenderScope WithUnclipped()                        => PushUnclipped();
    public RenderScope WithOverflow(bool allow = true)        => AllowOverflowScope(allow);
    public RenderScope WithGlobal()                           => PushGlobal();

    // =========================================================================
    //                     COORDINATE & VISIBILITY UTILITIES
    // =========================================================================

    /// <summary>
    /// Converts a local coordinate point to global canvas coordinates.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point2D ToCanvas(Point2D localPoint) => _current.Origin + localPoint;

    /// <summary>
    /// Converts local coordinates to global canvas coordinates.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point2D ToCanvas(int localX, int localY) => new(_current.Origin.X + localX, _current.Origin.Y + localY);

    /// <summary>
    /// Converts a global canvas coordinate point to local space.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point2D ToLocal(Point2D canvasPoint) => new(canvasPoint.X - _current.Origin.X, canvasPoint.Y - _current.Origin.Y);

    /// <summary>
    /// Converts global canvas coordinates to local space.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point2D ToLocal(int canvasX, int canvasY) => new(canvasX - _current.Origin.X, canvasY - _current.Origin.Y);

    /// <summary>
    /// Converts a local rectangular area to global canvas space.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rect2D ToCanvas(Rect2D localArea) => new(localArea.Lower + _current.Origin, localArea.Higher + _current.Origin);

    /// <summary>
    /// Converts a global canvas rectangular area to local coordinate space.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rect2D ToLocal(Rect2D canvasArea) => new(canvasArea.Lower - _current.Origin, canvasArea.Higher - _current.Origin);

    /// <summary>
    /// Checks whether the local coordinate is currently visible under active clipping and canvas boundaries.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsPointVisible(Point2D localPoint) => IsCanvasPointVisible(_current.Origin.X + localPoint.X, _current.Origin.Y + localPoint.Y);

    /// <summary>
    /// Checks whether the local coordinates are currently visible under active clipping and canvas boundaries.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsPointVisible(int localX, int localY) => IsCanvasPointVisible(_current.Origin.X + localX, _current.Origin.Y + localY);

    /// <summary>
    /// Checks whether the global canvas coordinate point is currently visible under active clipping and canvas boundaries.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsCanvasPointVisible(Point2D canvasPoint) => IsCanvasPointVisible(canvasPoint.X, canvasPoint.Y);

    /// <summary>
    /// Checks whether the global canvas coordinates are currently visible under active clipping and canvas boundaries.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsCanvasPointVisible(int canvasX, int canvasY) {
        if (canvasX < 0 || canvasX >= _canvas.Width || canvasY < 0 || canvasY >= _canvas.Height)
            return false;

        if (_current is { AllowOverflow: false, Clip: not null }) {
            var clip = _current.Clip.Value;

            if (canvasX < clip.LowerX || canvasX >= clip.HigherX || canvasY < clip.LowerY || canvasY >= clip.HigherY)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns the effective clipping region in global canvas space.
    /// </summary>
    public Rect2D GetEffectiveClip() {
        var canvasArea = CanvasBounds;

        if (_current.AllowOverflow || !_current.Clip.HasValue)
            return canvasArea;

        return IntersectAreas(canvasArea, _current.Clip.Value);
    }

    /// <summary>
    /// Calculates the intersection of two rectangular areas.
    /// </summary>
    internal static Rect2D IntersectAreas(Rect2D a, Rect2D b) {
        int lx = Math.Max(a.LowerX, b.LowerX);
        int ly = Math.Max(a.LowerY, b.LowerY);
        int hx = Math.Min(a.HigherX, b.HigherX);
        int hy = Math.Min(a.HigherY, b.HigherY);

        if (hx <= lx || hy <= ly)
            return new Rect2D(lx, ly, lx, ly);

        return new Rect2D(lx, ly, hx, hy);
    }

    /// <summary>
    /// Registers an interactive hit target for mouse event handling within the current render scope.
    /// The bounds are converted to global coordinates and intersected with the effective clipping region.
    /// </summary>
    public void RegisterHitTarget(
        object?                        element,
        Rect2D                         localBounds,
        HitTestBehavior                behavior     = HitTestBehavior.OPAQUE,
        Action<ElementMouseEventArgs>? onClick      = null,
        Action<ElementMouseEventArgs>? onMouseEnter = null,
        Action<ElementMouseEventArgs>? onMouseLeave = null,
        Action<ElementMouseEventArgs>? onMouseDown  = null,
        Action<ElementMouseEventArgs>? onMouseUp    = null,
        Action<ElementMouseEventArgs>? onMouseMove  = null,
        Action<ElementMouseEventArgs>? onScroll     = null,
        Bindable<bool>?                isHovered    = null,
        Bindable<bool>?                isPressed    = null
    ) {
        if (HitMap == null || behavior == HitTestBehavior.NONE)
            return;

        var globalBounds  = localBounds + _current.Origin;
        var effectiveClip = GetEffectiveClip();
        var clippedBounds = IntersectAreas(globalBounds, effectiveClip);

        if (clippedBounds.HigherX <= clippedBounds.LowerX || clippedBounds.HigherY <= clippedBounds.LowerY)
            return;

        var target = new HitTarget {
            Element      = element,
            GlobalBounds = clippedBounds,
            LocalOrigin  = _current.Origin + new Point2D(localBounds.LowerX, localBounds.LowerY),
            ZIndex       = _current.ZIndex,
            PaintOrder   = HitMap.NextPaintOrder(),
            Behavior     = behavior,
            OnClick      = onClick,
            OnMouseEnter = onMouseEnter,
            OnMouseLeave = onMouseLeave,
            OnMouseDown  = onMouseDown,
            OnMouseUp    = onMouseUp,
            OnMouseMove  = onMouseMove,
            OnScroll     = onScroll,
            IsHovered    = isHovered,
            IsPressed    = isPressed
        };

        HitMap.Register(target);
    }

    // =========================================================================
    //                        CELL LEVEL DRAWING PRIMITIVES
    // =========================================================================
    public Size2D                  Size => CanvasSize;
    public IReadOnlyList2D<NKCell>? Data { get; }

    public void Draw(ICharCanvas canvas, int xOffset, int yOffset, int z) {
        _canvas.Draw(canvas, _current.Origin.X + xOffset, _current.Origin.Y + yOffset, _current.ZIndex + z);
    }

    public void Draw(AnsiChar c, int xOffset, int yOffset, int z) {
        var cx = _current.Origin.X + xOffset;
        var cy = _current.Origin.Y + yOffset;

        if (!IsCanvasPointVisible(cx, cy))
            return;

        var style = _current.Style.With(c.Style);
        _canvas.Draw(new AnsiChar(c.Char, style), cx, cy, _current.ZIndex + z);
    }

    public void Draw(AnsiString str, int xOffset, int yOffset, int z) {
        _canvas.Draw(str, _current.Origin.X + xOffset, _current.Origin.Y + yOffset, _current.ZIndex + z);
    }

    public void Draw(SKImage image, int xOffset, int yOffset, int xSize, int ySize, int zLayer, SKSamplingOptions samplingOptions = default) {
        _canvas.Draw(image, _current.Origin.X + xOffset, _current.Origin.Y + yOffset, xSize, ySize, _current.ZIndex + zLayer, samplingOptions);
    }

    public void DrawVerticalLine(AnsiChar c, int x, int yOffset, int length, int z) {
        _canvas.DrawVerticalLine(c, _current.Origin.X + x, _current.Origin.Y + yOffset, length, _current.ZIndex + z);
    }

    public void DrawHorizontalLine(AnsiChar c, int xOffset, int y, int length, int z) {
        _canvas.DrawHorizontalLine(c, _current.Origin.X + xOffset, _current.Origin.Y + y, length, _current.ZIndex + z);
    }

    public void DrawRect(AnsiChar c, int xOffset, int yOffset, int width, int height, int z) {
        _canvas.DrawRect(c, _current.Origin.X + xOffset, _current.Origin.Y + yOffset, width, height, _current.ZIndex + z);
    }
    
    public void Clean() {
        _canvas.Clean();
    }
}