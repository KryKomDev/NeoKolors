// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

/// <summary>
/// Maintains the spatial index of interactive hit targets generated during a render pass.
/// Enables spatial hit-testing with z-index ordering and painter's order resolution.
/// </summary>
public class HitMap {

    private readonly object          _lock = new();
    private readonly List<HitTarget> _targets = new();
    private readonly List<HitTarget> _staging = new();
    private          bool            _isStaging;
    private          int             _nextPaintOrder;

    /// <summary>
    /// Gets all registered hit targets in this hit map.
    /// </summary>
    public IReadOnlyList<HitTarget> Targets {
        get {
            lock (_lock) {
                return _targets.Count > 0 ? _targets.ToList() : _staging.ToList();
            }
        }
    }

    /// <summary>
    /// Returns the next sequential paint order number for the current render pass.
    /// </summary>
    public int NextPaintOrder() => Interlocked.Increment(ref _nextPaintOrder);

    /// <summary>
    /// Registers a new hit target into the spatial map.
    /// </summary>
    public void Register(HitTarget target) {
        if (target != null) {
            lock (_lock) {
                if (_isStaging) {
                    _staging.Add(target);
                }
                else {
                    _targets.Add(target);
                }
            }
        }
    }

    /// <summary>
    /// Clears all registered targets and resets the paint order sequence.
    /// In staging mode, retains active targets until <see cref="Commit"/> is called.
    /// </summary>
    public void Clear() {
        lock (_lock) {
            _staging.Clear();
            _isStaging = true;
            _nextPaintOrder = 0;
        }
    }

    /// <summary>
    /// Commits staging targets to active targets atomically.
    /// </summary>
    public void Commit() {
        lock (_lock) {
            _targets.Clear();
            _targets.AddRange(_staging);
            _staging.Clear();
            _isStaging = false;
        }
    }

    /// <summary>
    /// Performs hit-testing at the specified screen coordinate.
    /// Returns matching targets ordered from topmost (highest ZIndex, highest PaintOrder) downward.
    /// Respects <see cref="HitTestBehavior.OPAQUE"/> occlusion and <see cref="HitTestBehavior.NONE"/>.
    /// </summary>
    public List<HitTarget> HitTest(Point2D screenPoint) {
        var candidates = new List<HitTarget>();

        lock (_lock) {
            var source = (_targets.Count > 0 || !_isStaging) ? _targets : _staging;

            for (int i = 0; i < source.Count; i++) {
                var target = source[i];
                if (target.Behavior == HitTestBehavior.NONE)
                    continue;

                var bounds = target.GlobalBounds;
                if (screenPoint.X >= bounds.LowerX && screenPoint.X < bounds.HigherX &&
                    screenPoint.Y >= bounds.LowerY && screenPoint.Y < bounds.HigherY) {
                    candidates.Add(target);
                }
            }
        }

        // Sort topmost first: highest ZIndex first, then highest PaintOrder (rendered later) first
        candidates.Sort((a, b) => {
            int zComp = b.ZIndex.CompareTo(a.ZIndex);
            return zComp != 0 ? zComp : b.PaintOrder.CompareTo(a.PaintOrder);
        });

        // Filter for OPAQUE occlusion: an opaque target blocks targets rendered behind it
        var result = new List<HitTarget>();
        foreach (var target in candidates) {
            result.Add(target);
            if (target.Behavior == HitTestBehavior.OPAQUE) {
                break;
            }
        }

        return result;
    }
}
