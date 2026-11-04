// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Lifecycle.Core;

namespace NeoKolors.Tui.Lifecycle.Tests;

public class LifecycleControllerTests {
    [Fact]
    public void Controller_InitialState_IsCreated() {
        var controller = new LifecycleController();
        Assert.Equal(LifecycleState.CREATED, controller.State);
        Assert.False(controller.IsRunning);
        Assert.False(controller.IsPaused);
    }

    [Fact]
    public void Controller_Initialize_TransitionsToInitialized() {
        var controller = new LifecycleController();
        var transitions = new List<LifecycleState>();

        controller.StateChanged += (_, args) => transitions.Add(args.NewState);
        controller.Initialize();

        Assert.Equal(LifecycleState.INITIALIZED, controller.State);
        Assert.Equal([LifecycleState.INITIALIZING, LifecycleState.INITIALIZED], transitions);
    }

    [Fact]
    public void Controller_Start_TransitionsToRunning() {
        var controller = new LifecycleController();
        var transitions = new List<LifecycleState>();

        controller.StateChanged += (_, args) => transitions.Add(args.NewState);
        controller.Start();

        Assert.Equal(LifecycleState.RUNNING, controller.State);
        Assert.True(controller.IsRunning);
        Assert.Contains(LifecycleState.RUNNING, transitions);
    }

    [Fact]
    public void Controller_PauseAndResume_TransitionsCorrectly() {
        var controller = new LifecycleController();
        controller.Start();

        controller.Pause();
        Assert.Equal(LifecycleState.PAUSED, controller.State);
        Assert.True(controller.IsPaused);
        Assert.False(controller.IsRunning);

        controller.Resume();
        Assert.Equal(LifecycleState.RUNNING, controller.State);
        Assert.True(controller.IsRunning);
        Assert.False(controller.IsPaused);
    }

    [Fact]
    public void Controller_Stop_TransitionsToStopped() {
        var controller = new LifecycleController();
        controller.Start();

        controller.Stop();
        Assert.Equal(LifecycleState.STOPPED, controller.State);
        Assert.False(controller.IsRunning);
    }

    [Fact]
    public void Controller_Dispose_TransitionsToDisposed() {
        var controller = new LifecycleController();
        controller.Start();

        controller.Dispose();
        Assert.Equal(LifecycleState.DISPOSED, controller.State);
    }

    [Fact]
    public void Controller_Pause_WhenNotRunning_DoesNothing() {
        var controller = new LifecycleController();
        controller.Pause();
        Assert.Equal(LifecycleState.CREATED, controller.State);
    }

    [Fact]
    public void Controller_Resume_WhenNotPaused_DoesNothing() {
        var controller = new LifecycleController();
        controller.Start();
        controller.Resume();
        Assert.Equal(LifecycleState.RUNNING, controller.State);
    }
}
