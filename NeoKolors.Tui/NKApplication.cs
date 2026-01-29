// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Events;
using NeoKolors.Tui.Events;
using System.Diagnostics;
using NeoKolors.Console.Mouse;
using NeoKolors.Tui.Extensions;
using NeoKolors.Tui.Global;
using NeoKolors.Tui.Rendering;

namespace NeoKolors.Tui;

public class NKApplication : IMouseSupportingApplication {

    private static readonly NKLogger LOGGER = NKDebug.GetLogger<NKApplication>();
    
    // --- Screen ---
    private readonly NKCharScreen _screen = new(Stdio.BufferSize);
    private Size _lastSize     = Size.Zero;
    private Size _lasPixelSize = Size.Zero;

    public NKCharScreen Screen => _screen;
    
    // --- IApp impl ---
    public IRenderable Base { get; set; }
    public NKAppConfig Config { get; }
    
    // --- Flow control ---
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }
    private readonly SemaphoreSlim _pausedSignal = new(0);
    
    // --- Events ---
    public event KeyEventHandler      KeyEvent;
    public event MouseEventHandler    MouseEvent;
    public event ResizeEventHandler   ResizeEvent;
    public event AppStartEventHandler StartEvent;
    public event AppStopEventHandler  StopEvent;
    public event Action               OnRender;

    private void InvokeKeyEvent(ConsoleKeyInfo k) => KeyEvent.Invoke(k);
    private void InvokeMouseEvent(MouseEventArgs m) => MouseEvent.Invoke(m);
    private void InvokeResizeEvent(ResizeEventArgs r) => ResizeEvent.Invoke(r);

    public NKApplication(NKAppConfig config, IRenderable @base) {
        Config = config;
        Base   = @base;

        KeyEvent    +=  _     => { };
        MouseEvent  +=  _     => { };
        ResizeEvent +=  _     => { };
        StartEvent  += (_, _) => { };
        StopEvent   +=  _     => { };
        OnRender    += ( )    => { };
    }
    
    public void Start() {
        
        // configure console
        NKConsole.MouseReportProtocol = Config.MouseReportProtocol;
        NKConsole.MouseReportLevel    = Config.MouseReportLevel;
        NKConsole.BracketedPasteMode  = Config.BracketedPaste;
        
        if (Config.PauseOnFocusLost) {
            NKConsole.ReportFocus    = true;
            NKConsole.FocusOutEvent += Pause;
            NKConsole.FocusInEvent  += Unpause;
        }

        NKConsole.KeyEvent   += InvokeKeyEvent;
        NKConsole.MouseEvent += InvokeMouseEvent;

        Stdio.TreatControlCAsInput = !Config.CtrlCForceQuits;

        if (Config.KeepCursorDisabled) {
            NKConsole.HideCursor();
        }
        
        NKConsole.EnableAltBuffer();
        NKConsole.EnableMouseEvents();
        NKConsole.StartInputInterception();

        // configure app
        IsRunning = true;
        StartEvent.Invoke(this, new AppStartEventArgs(Config.Rendering.IsLazy));
        
        NKConsole.KeyEvent += CheckQuit;
        
        if (Config.Rendering.IsLazy) 
            RunLazy();
        else if (Config.Rendering.IsUnlimited)
            RunUnlimited();
        else
            RunLimited();
        
        FinalizeRun();
    }
    
    public void Stop() {
        IsRunning = false;
        StopEvent.Invoke(this);
    }

    private void FinalizeRun() {
        NKConsole.MouseReportLevel = MouseReportLevel.NONE;
        
        if (Config.PauseOnFocusLost) {
            NKConsole.ReportFocus    = false;
            NKConsole.FocusOutEvent -= Pause;
            NKConsole.FocusInEvent  -= Unpause;
        }

        NKConsole.KeyEvent   -= InvokeKeyEvent;
        NKConsole.MouseEvent -= InvokeMouseEvent;

        if (Config.KeepCursorDisabled) {
            NKConsole.ShowCursor();
        }
        
        NKConsole.DisableAltBuffer();
        NKConsole.StopInputInterception();

        NKConsole.KeyEvent -= CheckQuit;
    }

    private void RunLazy() {
        
        // configure lazy 
        var semaphore = new SemaphoreSlim(0);
        NKConsole.MouseEvent += SignalRender;
        NKConsole.KeyEvent   += SignalRender;
        
        while (IsRunning) {
            Render();
            semaphore.Wait();
        }

        NKConsole.MouseEvent -= SignalRender;
        NKConsole.KeyEvent   -= SignalRender;
        
        return;

        void SignalRender<T>(T? _) {
            if (semaphore.CurrentCount == 0) {
                semaphore.Release();
            }
        }
    }

    private void RunUnlimited() {
        var sw = Stopwatch.StartNew();
        long fc = 0;
        
        while (IsRunning) {
            if (IsPaused) {
                sw.Stop();
                _pausedSignal.Wait();
                sw.Start();
            }
            
            Render();
            fc++;
        }
        
        sw.Stop();
        if (fc <= 0 || !(sw.Elapsed.TotalSeconds > 0)) return;
        
        double avgFps = fc / sw.Elapsed.TotalSeconds;
        LOGGER.Info($"Average FPS: {avgFps}");
    }
    
    private void RunLimited() {
        int targetFps = Config.Rendering.Limit; 
        var frameTime = TimeSpan.FromSeconds(1.0 / targetFps);
        var stopwatch = new Stopwatch();
        ulong delayedCount = 0;
        var totalDelay = TimeSpan.Zero;

        while (IsRunning) {
            if (IsPaused) {
                _pausedSignal.Wait();
            }
            
            stopwatch.Restart();
            
            Render();

            var elapsed = stopwatch.Elapsed;

            if (elapsed <= frameTime) {
                Thread.Sleep(frameTime - elapsed);
            }
            else {
                delayedCount++;
                totalDelay += elapsed - frameTime;
            }
        }
        
        LOGGER.Info($"\nTotal frames delayed: {delayedCount}" +
                    $"\nTotal delay: {totalDelay}" +
                    $"\nAverage delay per frame: {totalDelay / delayedCount}");
    }
    
    private void Render() {
        if (!Config.KeepCursorDisabled) {
            NKConsole.HideCursor();
            NKConsole.SaveCursor();
        }
        
        if (_lastSize != Stdio.BufferSize) {
            _lastSize = Stdio.BufferSize;
            _screen.Resize(_lastSize.Width, _lastSize.Height);
            InvokeResizeEvent(new ResizeEventArgs(_lastSize.Width, _lastSize.Height));
            _lasPixelSize = NKConsole.GetScreenSizePx();
            ScreenSizeTracker.SetScreenSizeCh(_lastSize);
            ScreenSizeTracker.SetScreenSizePx(_lasPixelSize);
        }
        
        OnRender.Invoke();
        Base.Render(_screen);
        _screen.Render();
        
        if (!Config.KeepCursorDisabled) {
            NKConsole.RestoreCursor();
            NKConsole.ShowCursor();
        }
    }

    private void Pause() {
        IsPaused = true;
    }

    private void Unpause() {
        IsPaused = false;

        if (_pausedSignal.CurrentCount == 0) {
            _pausedSignal.Release();
        }
    }

    private void CheckQuit(ConsoleKeyInfo keyInfo) {
        if (keyInfo.Key       == Config.InterruptCombination.Key &&
            keyInfo.Modifiers == Config.InterruptCombination.Modifiers)
        {
            IsRunning = false;
            Stop();
        }
    }
}