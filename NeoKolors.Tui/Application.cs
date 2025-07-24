//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;
using NeoKolors.Console;
using NeoKolors.Console.Events;
using NeoKolors.Console.Mouse;
using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

public class Application : IApplication {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(Application));
    
    /// <summary>
    /// stack of 'pop-ups'
    /// </summary>
    private readonly List<Window> _windows = new();

    
    /// <summary>
    /// adds a new window to the stack of windows of the application
    /// </summary>
    public void PushWindow(Window w) {
        KeyEvent += w.HandleKeyPress;
        ResizeEvent += w.HandleResize;
        StopEvent += w.HandleAppStop;
        StartEvent += w.HandleAppStart;
        MouseEvent += w.HandleMouseEvent;
        _windows.Add(w);
    }

    
    /// <summary>
    /// removes a window from the stack of windows of the application 
    /// </summary>
    public void PopWindow() => _windows.RemoveAt(_windows.Count - 1);


    /// <summary>
    /// Represents the main view at the base level of the application, serving as the foundational interface for
    /// rendering and managing the application's UI components.
    /// </summary>
    private readonly IView _baseView;


    /// <summary>
    /// called when a key is pressed
    /// </summary>
    public event KeyEventHandler KeyEvent;

    
    /// <summary>
    /// Event handler for mouse interaction events within the application.
    /// </summary>
    public event MouseEventHandler MouseEvent;


    /// <summary>
    /// Represents an event triggered when the application or one of its elements gains focus.
    /// </summary>
    public event FocusInEventHandler FocusInEvent;


    /// <summary>
    /// Event triggered when a loss of keyboard focus occurs within the application.
    /// </summary>
    public event FocusOutEventHandler FocusOutEvent;


    /// <summary>
    /// Represents an event triggered when a paste operation occurs in the application.
    /// </summary>
    public event PasteEventHandler PasteEvent;
    
    
    /// <summary>
    /// called when terminal is resized
    /// </summary>
    public event ResizeEventHandler ResizeEvent;
    
    
    /// <summary>
    /// called when the application is started
    /// </summary>
    public event AppStartEventHandler StartEvent;
    
    
    /// <summary>
    /// called when the application is stopped
    /// </summary>
    public event EventHandler StopEvent;

    
    /// <summary>
    /// holds the configuration of the application
    /// </summary>
    public AppConfig Config { get; set; }

    
    /// <summary>
    /// determines whether the application is running
    /// </summary>
    public bool IsRunning { get; private set; } = false;
    
    
    /// <summary>
    /// the console that will be used to display 
    /// </summary>
    public ConsoleScreen Screen { get; private set; }
    
    
    /// <summary>
    /// creates a new tui application with the default configuration
    /// </summary>
    public Application(IView baseView, AppConfig? config = null, ConsoleScreen? screen = null) {
        
        // set up fields
        _baseView = baseView;
        Config = new AppConfig();
        Screen = screen ?? new ConsoleScreen(System.Console.Out);
        Config = config ?? new AppConfig();

        // set up events
        KeyEvent = _ => { };
        MouseEvent = _ => { };
        ResizeEvent = _ => { };
        StartEvent = (_, _) => { };
        StopEvent = (_, _) => { };
        FocusInEvent = () => { };
        FocusOutEvent = () => { };
        PasteEvent = _ => { };

        // subscribe events
        ResizeEvent += Screen.Resize;
    }
    
    
    /// <inheritdoc cref="IApplication.Render"/>
    public void Render() {
        
        // render the base view
        _baseView.Render(Screen);

        // render the window stack
        foreach (var w in _windows) {
            w.Render(Screen);
        }
    }
    
    
    /// <inheritdoc cref="IApplication.Start"/>
    public void Start() {
        
        LOGGER.Info($"Starting application with config: {Config.ToString()}");

        NKConsole.EnableMouseEvents();
        NKConsole.MouseReportProtocol = Config.MouseReportProtocol;
        NKConsole.EnableBracketedPasteMode();
        NKConsole.EnableFocusReporting();
            
        NKConsole.FocusInEvent += () => FocusInEvent.Invoke();
        NKConsole.FocusOutEvent += () => FocusOutEvent.Invoke();
        NKConsole.MouseEvent += info => MouseEvent.Invoke(info);
        NKConsole.KeyEvent += info => KeyEvent.Invoke(info);
        NKConsole.PasteEvent += content => PasteEvent.Invoke(content);
        
        NKConsole.StartInputInterception();
        
        IsRunning = true;
        StartEvent.Invoke(this, new AppStartEventArgs(Config.LazyRender));
        System.Console.SetOut(Screen);

        if (Config.LazyRender) {
            RunLazy();
        }
        else {
            RunDynamic();
        }
    }

    
    /// <summary>
    /// runs the application in lazy render mode
    /// </summary>
    private void RunLazy() {
        Render();
        Screen.Render();

        NKConsole.KeyEvent += info => {
            Render();
            Screen.Render();
            
            if (info.Key == Config.InterruptCombination.Key &&
                info.Modifiers == Config.InterruptCombination.Modifiers) 
            {
                Stop();
            }
        };

        NKConsole.MouseEvent += _ => {
            Render();
            Screen.Render();
        };

        int prevWidth = System.Console.WindowWidth;
        int prevHeight = System.Console.WindowHeight;
        
        while (IsRunning) {
            
            if (prevWidth == System.Console.BufferWidth &&
                prevHeight == System.Console.BufferHeight) 
            {
                continue;
            }

            ResizeEvent.Invoke(new ResizeEventArgs(System.Console.BufferWidth, System.Console.BufferHeight));
            
            prevWidth = System.Console.BufferWidth;
            prevHeight = System.Console.BufferHeight;
            
            Render();
            Screen.Render();
        }
    }
    
    
    /// <summary>
    /// runs the application in dynamic render mode
    /// </summary>
    private void RunDynamic() {
        
        // the first frame should be rendered immediately, so last frame was rendered yesterday this time :) 
        var lastFrame = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
        
        // minimal time span between frames
        var minDelta = new TimeSpan(0, 0, 0, 0, 1 / Config.FpsLimit.Framerate * 1000);

        NKConsole.KeyEvent += info => {
            if (info.Key == Config.InterruptCombination.Key &&
                info.Modifiers == Config.InterruptCombination.Modifiers) 
            {
                Stop();
            }
        };
        
        while (IsRunning) {
            
            // terminal has been resized
            if (Screen.Width != System.Console.WindowWidth ||
                Screen.Height != System.Console.WindowHeight) 
            {
                ResizeEvent.Invoke(new ResizeEventArgs(System.Console.WindowWidth, System.Console.WindowHeight));
            }

            // if the last frame has been rendered in less than minDelta skip rendering for this cycle
            if ((DateTime.Now - lastFrame).TotalMilliseconds < minDelta.TotalMilliseconds) continue;
            
            lastFrame = DateTime.Now;
            Render();
            Screen.Render();
        }
    }
    
    
    /// <inheritdoc cref="IApplication.Stop"/>
    public void Stop() {
        LOGGER.Info("Stopping application...");
        IsRunning = false;
        StopEvent.Invoke(this, EventArgs.Empty);
        Screen.ScreenMode = false;
        NKConsole.StopInputInterception();
    }
}