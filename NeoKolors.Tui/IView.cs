//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

/// <summary>
/// base class for rendered sections
/// </summary>
public interface IView : IRenderable {

    /// <summary>
    /// Handles the key press event for the view.
    /// </summary>
    /// <param name="sender">The source of the event, typically the object that triggered the key press.</param>
    /// <param name="args">The event arguments containing information about the pressed key.</param>
    public void HandleKeyPress(object? sender, KeyEventArgs args);

    /// <summary>
    /// Handles the resize event for the view.
    /// </summary>
    /// <param name="args">The event arguments containing the new width and height of the view.</param>
    public void HandleResize(ResizeEventArgs args);

    /// <summary>
    /// Handles the application start event for the view.
    /// </summary>
    /// <param name="sender">The source of the event, typically the object that triggered the application start.</param>
    /// <param name="args">The event arguments containing details about the application's start state.</param>
    public void HandleAppStart(object? sender, AppStartEventArgs args);

    /// <summary>
    /// Handles the application stop event for the view.
    /// </summary>
    /// <param name="sender">The source of the event, typically the object that triggered the application stop.</param>
    /// <param name="args">The event arguments containing details related to the application's stop event.</param>
    public void HandleAppStop(object? sender, EventArgs args);
}