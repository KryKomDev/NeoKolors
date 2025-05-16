//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Events;

/// <summary>
/// Provides a centralized static event bus for managing application events within the NeoKolors framework,
/// such as key events, resize events, application start events, and application stop events.
/// </summary>
/// <remarks>
/// This class facilitates subscription and unsubscription to events exposed by the IApplication interface and
/// ensures that event handlers are managed in relation to the source application instance.
/// </remarks>
public static class AppEventBus {
    private static IApplication? SOURCE_APPLICATION;

    /// <summary>
    /// Sets the source application instance for the event bus, allowing event handlers to be
    /// subscribed or unsubscribed in relation to this source application.
    /// </summary>
    /// <param name="sourceApplication">The application instance that will serve as the source of events.</param>
    public static void SetSourceApplication(IApplication sourceApplication) {
        SOURCE_APPLICATION = sourceApplication;
    }

    /// <summary>
    /// Subscribes a handler method to the key event of the source application, enabling it to
    /// process key-related events as they occur.
    /// </summary>
    /// <param name="handler">The method to be invoked when a key event is triggered.</param>
    public static void SubscribeToKeyEvent(KeyEventHandler handler) {
        if (SOURCE_APPLICATION != null)
            SOURCE_APPLICATION.KeyEvent += handler;
    }

    /// <summary>
    /// Unsubscribes a previously registered key event handler from the source application's key event.
    /// </summary>
    /// <param name="handler">The key event handler to be removed from the event subscription.</param>
    public static void UnsubscribeFromKeyEvent(KeyEventHandler handler) {
        if (SOURCE_APPLICATION != null)
            SOURCE_APPLICATION.KeyEvent -= handler;
    }

    /// <summary>
    /// Subscribes to the resize event of the current source application.
    /// This allows the provided event handler to be invoked when a resize event occurs within the application.
    /// </summary>
    /// <param name="handler">The handler to be invoked when the resize event is triggered.</param>
    public static void SubscribeToResizeEvent(ResizeEventHandler handler) {
        if (SOURCE_APPLICATION != null)
            SOURCE_APPLICATION.ResizeEvent += handler;
    }

    /// <summary>
    /// Unsubscribes the specified handler from the resize event of the source application.
    /// </summary>
    /// <param name="handler">The event handler to be unsubscribed from the resize event.</param>
    public static void UnsubscribeFromResizeEvent(ResizeEventHandler handler) {
        if (SOURCE_APPLICATION != null)
            SOURCE_APPLICATION.ResizeEvent -= handler;
    }

    /// <summary>
    /// Subscribes a handler to the application start event, enabling custom logic to be executed
    /// when the application starts.
    /// </summary>
    /// <param name="handler">The delegate that will handle the start event.</param>
    public static void SubscribeToStartEvent(AppStartEventHandler handler) {
        if (SOURCE_APPLICATION != null)
            SOURCE_APPLICATION.StartEvent += handler;
    }

    /// <summary>
    /// Unsubscribes an event handler from the application start event of the source application,
    /// preventing it from receiving further start event notifications.
    /// </summary>
    /// <param name="handler">The event handler to unsubscribe from the application start event.</param>
    public static void UnsubscribeFromStartEvent(AppStartEventHandler handler) {
        if (SOURCE_APPLICATION != null)
            SOURCE_APPLICATION.StartEvent -= handler;
    }

    /// <summary>
    /// Subscribes a handler method to the stop event of the source application, allowing it to
    /// execute when the application stop event is triggered.
    /// </summary>
    /// <param name="handler">The method to be invoked when the stop event occurs.</param>
    public static void SubscribeToStopEvent(EventHandler handler) {
        if (SOURCE_APPLICATION != null)
            SOURCE_APPLICATION.StopEvent += handler;
    }

    /// <summary>
    /// Unsubscribes a handler method from the stop event of the source application, preventing it
    /// from being invoked when a stop event occurs.
    /// </summary>
    /// <param name="handler">The method to be removed from the invocation list of the stop event.</param>
    public static void UnsubscribeFromStopEvent(EventHandler handler) {
        if (SOURCE_APPLICATION != null)
            SOURCE_APPLICATION.StopEvent -= handler;
    }
}