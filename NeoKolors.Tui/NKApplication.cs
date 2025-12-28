// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Events;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Events;
using System.Diagnostics;
using NeoKolors.Console.Mouse;
using NeoKolors.Tui.Extensions;

namespace NeoKolors.Tui;

public class NKApplication : IMouseSupportingApplication {

    private static readonly NKLogger LOGGER = NKDebug.GetLogger<NKApplication>();
    
    // --- Screen ---
    private readonly NKCharScreen _screen = new(Stdio.WindowSize);
    private Size _lastSize = Stdio.WindowSize;
    
    // --- IApp impl ---
    public IDom Dom { get; }
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

    private void InvokeKeyEvent(ConsoleKeyInfo k) => KeyEvent.Invoke(k);
    private void InvokeMouseEvent(MouseEventInfo m) => MouseEvent.Invoke(m);
    private void InvokeResizeEvent(ResizeEventArgs r) => ResizeEvent.Invoke(r);

    public NKApplication(NKAppConfig config, IDom dom) {
        Config = config;
        Dom = dom;

        KeyEvent    +=  _     => { };
        MouseEvent  +=  _     => { };
        ResizeEvent +=  _     => { };
        StartEvent  += (_, _) => { };
        StopEvent   +=  _     => { };
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
    }
    
    public void Stop() {
        IsRunning = false;
        StopEvent.Invoke(this);
        
        NKConsole.MouseReportLevel = MouseReportLevel.NONE;
        
        if (Config.PauseOnFocusLost) {
            NKConsole.ReportFocus    = false;
            NKConsole.FocusOutEvent -= Pause;
            NKConsole.FocusInEvent  -= Unpause;
        }

        NKConsole.KeyEvent   -= InvokeKeyEvent;
        NKConsole.MouseEvent -= InvokeMouseEvent;
        
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

        while (IsRunning) {
            if (IsPaused) {
                _pausedSignal.Wait();
            }
            
            stopwatch.Restart();
            
            Render();

            var elapsed = stopwatch.Elapsed;

            if (elapsed < frameTime) {
                Thread.Sleep(frameTime - elapsed);
            }
        }
    }

    private void Render() {
        Dom.BaseElement.Render(_screen);
        _screen.Render();
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