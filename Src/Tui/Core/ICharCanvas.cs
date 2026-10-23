// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

public interface ICharCanvas : ICharDrawer, IDisposable {
    
    /// <summary>
    /// Gets the 2D grid of <see cref="NKCell" /> representing the contents of the character canvas.
    /// </summary>
    /// <remarks>
    /// This property provides access to the underlying data structure of the canvas, which contains
    /// individual cells responsible for rendering and styling characters. Each cell holds a character
    /// and its associated visual attributes. Modifications to this data will be reflected in the
    /// canvas rendering, allowing for dynamic updates and custom drawing operations.
    /// </remarks>
    public IReadOnlyList2D<NKCell> Data { get; }
    
    /// <summary>
    /// Resizes the current canvas to the specified dimensions.
    /// </summary>
    /// <param name="size">The new dimensions to resize the canvas to.</param>
    public void Resize(Size2D size);

    /// <summary>
    /// Resets the z-index of all elements within the current canvas to their default state,
    /// removing any previously applied z-index transformations.
    /// </summary>
    public void ResetZ();
}