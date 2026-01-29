//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Console.Events;

namespace NeoKolors.Tui.Events;

/// <summary>
/// Provides a centralized static event bus for managing application events within the NeoKolors framework,
/// such as key events, resize events, application start events, and application stop events.
/// </summary>
/// <remarks>
/// This class facilitates subscription and unsubscription to events exposed by the IApplicationOld interface and
/// ensures that event handlers are managed in relation to the source application instance.
/// </remarks>
public static class AppEventBus {
    private static IApplication? SOURCE_APPLICATION;

    public static bool IsSourceSet => SOURCE_APPLICATION != null;
    public static IApplication Application =>
        SOURCE_APPLICATION ?? throw new NullReferenceException("Source Application not set!");

    public static event KeyEventHandler      KeyEvent    = delegate { };
    public static event MouseEventHandler    MouseEvent  = delegate { };
    public static event ResizeEventHandler   ResizeEvent = delegate { };
    public static event AppStartEventHandler StartEvent  = delegate { };
    public static event AppStopEventHandler  StopEvent   = delegate { };
    
    /// <summary>
    /// Sets the source application instance for the event bus, allowing event handlers to be
    /// subscribed or unsubscribed in relation to this source application.
    /// </summary>
    /// <param name="sourceApplication">The application instance that will serve as the source of events.</param>
    public static void SetSourceApplication(IApplication sourceApplication) {
        if (SOURCE_APPLICATION != null) {
            SOURCE_APPLICATION.StartEvent  -= StartEvent;
            SOURCE_APPLICATION.StopEvent   -= StopEvent;
            SOURCE_APPLICATION.ResizeEvent -= ResizeEvent;
            SOURCE_APPLICATION.KeyEvent    -= KeyEvent;

            if (SOURCE_APPLICATION is IMouseSupportingApplication om) {
                om.MouseEvent -= MouseEvent;
            }
        }
        
        SOURCE_APPLICATION = sourceApplication;
        
        SOURCE_APPLICATION.StartEvent  += StartEvent;
        SOURCE_APPLICATION.StopEvent   += StopEvent;
        SOURCE_APPLICATION.ResizeEvent += ResizeEvent;
        SOURCE_APPLICATION.KeyEvent    += KeyEvent;

        if (SOURCE_APPLICATION is IMouseSupportingApplication nm) {
            nm.MouseEvent += MouseEvent;
        }
    }

    /// <summary>
    /// Subscribes a handler method to the key event of the source application, enabling it to
    /// process key-related events as they occur.
    /// </summary>
    /// <param name="handler">The method to be invoked when a key event is triggered.</param>
    public static void SubscribeToKeyEvent(KeyEventHandler handler) {
        KeyEvent += handler;
    }

    /// <summary>
    /// Unsubscribes a previously registered key event handler from the source application's key event.
    /// </summary>
    /// <param name="handler">The key event handler to be removed from the event subscription.</param>
    public static void UnsubscribeFromKeyEvent(KeyEventHandler handler) {
        KeyEvent -= handler;
    }
    
    /// <summary>
    /// Subscribes a handler method to the key event of the source application, enabling it to
    /// process key-related events as they occur.
    /// </summary>
    /// <param name="handler">The method to be invoked when a key event is triggered.</param>
    public static void SubscribeToMouseEvent(MouseEventHandler handler) {
        MouseEvent += handler;
    }

    /// <summary>
    /// Unsubscribes a previously registered key event handler from the source application's key event.
    /// </summary>
    /// <param name="handler">The key event handler to be removed from the event subscription.</param>
    public static void UnsubscribeFromMouseEvent(MouseEventHandler handler) {
        MouseEvent -= handler;
    }

    /// <summary>
    /// Subscribes to the resize event of the current source application.
    /// This allows the provided event handler to be invoked when a resize event occurs within the application.
    /// </summary>
    /// <param name="handler">The handler to be invoked when the resize event is triggered.</param>
    public static void SubscribeToResizeEvent(ResizeEventHandler handler) {
        ResizeEvent += handler;
    }

    /// <summary>
    /// Unsubscribes the specified handler from the resize event of the source application.
    /// </summary>
    /// <param name="handler">The event handler to be unsubscribed from the resize event.</param>
    public static void UnsubscribeFromResizeEvent(ResizeEventHandler handler) {
        ResizeEvent -= handler;
    }

    /// <summary>
    /// Subscribes a handler to the application start event, enabling custom logic to be executed
    /// when the application starts.
    /// </summary>
    /// <param name="handler">The delegate that will handle the start event.</param>
    public static void SubscribeToStartEvent(AppStartEventHandler handler) {
        StartEvent += handler;
    }

    /// <summary>
    /// Unsubscribes an event handler from the application start event of the source application,
    /// preventing it from receiving further start event notifications.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe from the application start event.</param>
    public static void UnsubscribeFromStartEvent(AppStartEventHandler handler) {
        StartEvent -= handler;
    }

    /// <summary>
    /// Subscribes a handler method to the stop event of the source application, allowing it to
    /// execute when the application stop event is triggered.
    /// </summary>
    /// <param name="handler">The method to be invoked when the stop event occurs.</param>
    public static void SubscribeToStopEvent(AppStopEventHandler handler) {
        StopEvent += handler;
    }

    /// <summary>
    /// Unsubscribes a handler method from the stop event of the source application, preventing it
    /// from being invoked when a stop event occurs.
    /// </summary>
    /// <param name="handler">The method to be removed from the invocation list of the stop event.</param>
    public static void UnsubscribeFromStopEvent(AppStopEventHandler handler) {
        StopEvent -= handler;
    }
}