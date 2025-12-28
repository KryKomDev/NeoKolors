// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Events;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

public interface IApplication {
    
    /// <summary>
    /// Represents the Document Object Model (DOM) of the application.
    /// Provides capabilities for accessing and manipulating the application's
    /// visual hierarchy and elements.
    /// </summary>
    public IDom Dom { get; }

    /// <summary>
    /// starts the application life-cycle
    /// </summary>
    public void Start();
    
    /// <summary>
    /// stops the application life-cycle
    /// </summary>
    public void Stop();

    /// <summary>
    /// Represents a delegate type for handling key press events within the application.
    /// This event is triggered when a user interacts with the application using a keyboard.
    /// </summary>
    public event KeyEventHandler KeyEvent;

    /// <summary>
    /// Represents a delegate type for handling resize events within the application.
    /// This event is triggered when the application's dimensions or layout are altered,
    /// requiring adjustments to the UI or other components.
    /// </summary>
    public event ResizeEventHandler ResizeEvent;

    /// <summary>
    /// Represents a delegate type for handling events triggered during the start of the application's life-cycle.
    /// This event is invoked when the application initializes and begins execution.
    /// </summary>
    public event AppStartEventHandler StartEvent;

    /// <summary>
    /// Represents an event triggered when the application stop process is initiated.
    /// This event occurs at the conclusion of the application's life-cycle.
    /// </summary>
    public event AppStopEventHandler StopEvent;
}