// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Lifecycle.Core;

namespace NeoKolors.Tui.Lifecycle.Apps;

/// <summary>
/// A concrete application lifecycle host coordinating state, focus, activation, and graceful close requests.
/// </summary>
public class AppLifecycleHost : LifecycleController, IAppLifecycleHost {
    private AppActivationState _activationState = AppActivationState.INACTIVE;
    private bool               _hasFocus        = true;
    private int                _currentWidth    = 0;
    private int                _currentHeight   = 0;

    /// <inheritdoc/>
    public AppActivationState ActivationState {
        get {
            lock (SyncRoot) {
                return _activationState;
            }
        }
        private set {
            lock (SyncRoot) {
                if (_activationState == value)
                    return;

                _activationState = value;
            }

            ActivationChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public bool HasFocus {
        get {
            lock (SyncRoot) {
                return _hasFocus;
            }
        }
    }

    /// <inheritdoc/>
    public event EventHandler<bool>? FocusChanged;

    /// <inheritdoc/>
    public event EventHandler<AppActivationState>? ActivationChanged;

    /// <inheritdoc/>
    public event EventHandler<CloseCancelEventArgs>? CloseRequested;

    /// <inheritdoc/>
    public event EventHandler<SizeChangedEventArgs>? WindowResized;

    /// <inheritdoc/>
    public bool RequestClose(CloseReason reason = CloseReason.USER_REQUEST) {
        var args = new CloseCancelEventArgs(reason);
        CloseRequested?.Invoke(this, args);

        if (args.Cancel) {
            return false;
        }

        Stop();

        return true;
    }

    /// <inheritdoc/>
    public void Suspend() {
        Pause();
        ActivationState = AppActivationState.SUSPENDED;
    }

    /// <inheritdoc/>
    public void ResumeApp() {
        Resume();
        ActivationState = _hasFocus ? AppActivationState.ACTIVE : AppActivationState.INACTIVE;
    }

    /// <inheritdoc/>
    public void NotifyFocusChanged(bool hasFocus) {
        lock (SyncRoot) {
            if (_hasFocus == hasFocus)
                return;

            _hasFocus = hasFocus;
        }

        FocusChanged?.Invoke(this, hasFocus);

        if (IsRunning) {
            ActivationState = hasFocus ? AppActivationState.ACTIVE : AppActivationState.INACTIVE;
        }
    }

    /// <inheritdoc/>
    public void NotifyWindowResized(int width, int height) {
        int prevWidth, prevHeight;

        lock (SyncRoot) {
            if (_currentWidth == width && _currentHeight == height)
                return;

            prevWidth      = _currentWidth;
            prevHeight     = _currentHeight;
            _currentWidth  = width;
            _currentHeight = height;
        }

        WindowResized?.Invoke(this, new SizeChangedEventArgs(width, height, prevWidth, prevHeight));
    }

    protected override void OnStarted() {
        base.OnStarted();
        ActivationState = _hasFocus ? AppActivationState.ACTIVE : AppActivationState.INACTIVE;
    }

    protected override void OnStopped() {
        base.OnStopped();
        ActivationState = AppActivationState.INACTIVE;
    }
}