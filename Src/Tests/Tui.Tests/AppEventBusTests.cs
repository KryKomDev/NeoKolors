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

public class MockApplication : IMouseSupportingApplication {
    public IRenderable Base => null!;
    public void Start() => StartEvent(this, new AppStartEventArgs(false));
    public void Stop() => StopEvent(this);

    public event KeyEventHandler KeyEvent = delegate { };
    public event ResizeEventHandler ResizeEvent = delegate { };
    public event AppStartEventHandler StartEvent = delegate { };
    public event AppStopEventHandler StopEvent = delegate { };
    public event MouseEventHandler MouseEvent = delegate { };

    public void TriggerKeyEvent(KeyEventArgs e) => KeyEvent(e);
    public void TriggerMouseEvent(MouseEventArgs e) => MouseEvent(e);
    public void TriggerResizeEvent(ResizeEventArgs e) => ResizeEvent(e);
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
        KeyEventHandler handler = _ => received = true;
        
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
        MouseEventHandler handler = _ => received = true;
        
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
        
        AppStartEventHandler startHandler = (_, _) => started = true;
        AppStopEventHandler stopHandler = _ => stopped = true;
        
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
