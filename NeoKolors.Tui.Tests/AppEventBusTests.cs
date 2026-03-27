//
// NeoKolors.Test
// Copyright (c) 2026 KryKom
//

using Metriks;
using NeoKolors.Console;
using NeoKolors.Console.Events;
using NeoKolors.Console.Input;
using NeoKolors.Tui.Events;

namespace NeoKolors.Tui.Tests;

public class MockApplication : IApplication, IMouseSupportingApplication {
    public IRenderable Base => null!;
    public void Start() => StartEvent?.Invoke(this, new AppStartEventArgs(false));
    public void Stop() => StopEvent?.Invoke(this);

    public event KeyEventHandler KeyEvent = delegate { };
    public event ResizeEventHandler ResizeEvent = delegate { };
    public event AppStartEventHandler StartEvent = delegate { };
    public event AppStopEventHandler StopEvent = delegate { };
    public event MouseEventHandler MouseEvent = delegate { };

    public void TriggerKeyEvent(KeyEventArgs e) => KeyEvent?.Invoke(e);
    public void TriggerMouseEvent(MouseEventArgs e) => MouseEvent?.Invoke(e);
    public void TriggerResizeEvent(ResizeEventArgs e) => ResizeEvent?.Invoke(e);
}

public class AppEventBusTests {

    [Fact]
    public void SetSourceApplication_ShouldLinkEvents() {
        var app = new MockApplication();
        AppEventBus.SetSourceApplication(app);
        
        Assert.True(AppEventBus.IsSourceSet);
        Assert.Equal(app, AppEventBus.Application);
    }

    [Fact]
    public void KeyEvent_ShouldBePropagated() {
        var app = new MockApplication();
        AppEventBus.SetSourceApplication(app);
        
        bool received = false;
        KeyEventHandler handler = (e) => received = true;
        
        AppEventBus.SubscribeToKeyEvent(handler);
        app.TriggerKeyEvent(new KeyEventArgs(KeyCode.A, KeyModifiers.NONE, 'a'));
        
        Assert.True(received);
        
        received = false;
        AppEventBus.UnsubscribeFromKeyEvent(handler);
        app.TriggerKeyEvent(new KeyEventArgs(KeyCode.A, KeyModifiers.NONE, 'a'));
        
        Assert.False(received);
    }

    [Fact]
    public void MouseEvent_ShouldBePropagated() {
        var app = new MockApplication();
        AppEventBus.SetSourceApplication(app);
        
        bool received = false;
        MouseEventHandler handler = (e) => received = true;
        
        AppEventBus.SubscribeToMouseEvent(handler);
        app.TriggerMouseEvent(new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(0, 0), false, false));
        
        Assert.True(received);
        
        received = false;
        AppEventBus.UnsubscribeFromMouseEvent(handler);
        app.TriggerMouseEvent(new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(0, 0), false, false));
        
        Assert.False(received);
    }

    [Fact]
    public void StartStopEvents_ShouldBePropagated() {
        var app = new MockApplication();
        AppEventBus.SetSourceApplication(app);
        
        bool started = false;
        bool stopped = false;
        
        AppStartEventHandler startHandler = (s, e) => started = true;
        AppStopEventHandler stopHandler = (s) => stopped = true;
        
        AppEventBus.SubscribeToStartEvent(startHandler);
        AppEventBus.SubscribeToStopEvent(stopHandler);
        
        app.Start();
        app.Stop();
        
        Assert.True(started);
        Assert.True(stopped);
        
        started = false;
        stopped = false;
        AppEventBus.UnsubscribeFromStartEvent(startHandler);
        AppEventBus.UnsubscribeFromStopEvent(stopHandler);
        
        app.Start();
        app.Stop();
        
        Assert.False(started);
        Assert.False(stopped);
    }
}
