// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Game2D;

/// <summary>
///     A 2D camera viewport managing coordinate transformations and viewport culling.
/// </summary>
public class Camera2D {
    private float _zoom = 1.0f;

    /// <summary>
    ///     Gets or sets the world-space X coordinate of the camera center.
    /// </summary>
    public float X { get; set; } = 0.0f;

    /// <summary>
    ///     Gets or sets the world-space Y coordinate of the camera center.
    /// </summary>
    public float Y { get; set; } = 0.0f;

    /// <summary>
    ///     Gets or sets the zoom scale. Must be greater than 0. Default is 1.0.
    /// </summary>
    public float Zoom {
        get => _zoom;
        set => _zoom = value <= 0.0001f ? 0.0001f : value;
    }

    /// <summary>
    ///     Gets or sets the width of the display viewport in screen pixels or character cells.
    /// </summary>
    public int ViewportWidth { get; set; } = 80;

    /// <summary>
    ///     Gets or sets the height of the display viewport in screen pixels or character cells.
    /// </summary>
    public int ViewportHeight { get; set; } = 25;

    /// <summary>
    ///     Gets the minimum world-space X coordinate visible inside the viewport.
    /// </summary>
    public float VisibleWorldMinX => X - ViewportWidth / (2.0f * _zoom);

    /// <summary>
    ///     Gets the maximum world-space X coordinate visible inside the viewport.
    /// </summary>
    public float VisibleWorldMaxX => X + ViewportWidth / (2.0f * _zoom);

    /// <summary>
    ///     Gets the minimum world-space Y coordinate visible inside the viewport.
    /// </summary>
    public float VisibleWorldMinY => Y - ViewportHeight / (2.0f * _zoom);

    /// <summary>
    ///     Gets the maximum world-space Y coordinate visible inside the viewport.
    /// </summary>
    public float VisibleWorldMaxY => Y + ViewportHeight / (2.0f * _zoom);

    /// <summary>
    ///     Converts a point from world coordinates to viewport screen coordinates.
    /// </summary>
    public void WorldToScreen(float worldX, float worldY, out float screenX, out float screenY) {
        screenX = (worldX - X) * _zoom + ViewportWidth  / 2.0f;
        screenY = (worldY - Y) * _zoom + ViewportHeight / 2.0f;
    }

    /// <summary>
    ///     Converts a point from viewport screen coordinates to world coordinates.
    /// </summary>
    public void ScreenToWorld(float screenX, float screenY, out float worldX, out float worldY) {
        worldX = (screenX - ViewportWidth  / 2.0f) / _zoom + X;
        worldY = (screenY - ViewportHeight / 2.0f) / _zoom + Y;
    }

    /// <summary>
    ///     Performs an axis-aligned bounding box (AABB) intersection test to determine if an
    ///     entity is within the visible camera area.
    /// </summary>
    /// <param name="minX">The minimum world-space X coordinate of the bounding box.</param>
    /// <param name="minY">The minimum world-space Y coordinate of the bounding box.</param>
    /// <param name="maxX">The maximum world-space X coordinate of the bounding box.</param>
    /// <param name="maxY">The maximum world-space Y coordinate of the bounding box.</param>
    /// <returns>
    ///     <c>true</c> if the box overlaps the visible camera bounds; otherwise, <c>false</c>.
    /// </returns>
    public bool IsAabbVisible(float minX, float minY, float maxX, float maxY) {
        return maxX >= VisibleWorldMinX &&
            minX    <= VisibleWorldMaxX &&
            maxY    >= VisibleWorldMinY &&
            minY    <= VisibleWorldMaxY;
    }
}