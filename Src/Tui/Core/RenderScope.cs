// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Core;

/// <summary>
/// A disposable scope handle that restores the previous <see cref="RenderContext"/> state upon disposal.
/// </summary>
public readonly struct RenderScope : IDisposable {
    private readonly RenderContext _context;
    private readonly int           _expectedDepth;

    internal RenderScope(RenderContext context, int expectedDepth) {
        _context       = context;
        _expectedDepth = expectedDepth;
    }

    /// <summary>
    /// Restores the render context to the state saved when this scope was created.
    /// </summary>
    public void Dispose() {
        if (_context != null && _context.StateDepth >= _expectedDepth) {
            _context.RestoreToDepth(_expectedDepth - 1);
        }
    }
}