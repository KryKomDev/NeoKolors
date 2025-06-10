//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;
using NeoKolors.Console;
using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

public class Application : IApplication {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(Application));
    
    /// <summary>
    /// stack of 'pop-ups'
    /// </summary>
    private List<Window> Windows { get; } = new();

    
    /// <summary>
    /// adds a new window to the stack of windows of the application
    /// </summary>
    public void PushWindow(Window w) {
        KeyEvent += w.HandleKeyPress;
        ResizeEvent += w.HandleResize;
        StopEvent += w.HandleAppStop;
        StartEvent += w.HandleAppStart;
        Windows.Add(w);
    }

    
    /// <summary>
    /// removes a window from the stack of windows of the application 
    /// </summary>
    public void PopWindow() => Windows.RemoveAt(Windows.Count - 1);


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
        KeyEvent = (_, _) => { };
        ResizeEvent = _ => { };
        StartEvent = (_, _) => { };
        StopEvent = (_, _) => { };

        // subscribe events
        ResizeEvent += Screen.Resize;
        KeyEvent += _baseView.HandleKeyPress;
        ResizeEvent += _baseView.HandleResize;
        StartEvent += _baseView.HandleAppStart;
        StopEvent += _baseView.HandleAppStop;
    }
    
    
    /// <inheritdoc cref="IApplication.Render"/>
    public void Render() {
        
        // render the base view
        _baseView.Render(Screen);

        // render the window stack
        foreach (var w in Windows) {
            w.Render(Screen);
        }
    }
    
    
    /// <inheritdoc cref="IApplication.Start"/>
    public void Start() {
        
        LOGGER.Info($"Starting application with config: {Config.ToString()}");
        
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
        
        while (IsRunning) {
            var ki = System.Console.ReadKey(true);

            // interrupt combination triggered -> stop application
            if (ki.Key == Config.InterruptCombination.Key && 
                ki.Modifiers == Config.InterruptCombination.Modifiers)
            {
                Stop();
                return;
            }
            
            if (ki.Key == Config.InterruptCombination.Key && 
                ki.Modifiers == Config.InterruptCombination.Modifiers)
            {
                Screen.ToggleScreenMode();
            }

            // the terminal has been resized
            if (Screen.Width != System.Console.WindowWidth ||
                Screen.Height != System.Console.WindowHeight) 
            {
                ResizeEvent.Invoke(new ResizeEventArgs(System.Console.WindowWidth, System.Console.WindowHeight));
            }

            KeyEvent.Invoke(this, new KeyEventArgs(ki));
            
            LOGGER.Trace($"Pressed key: {Extensions.ToString(ki)}");
            
            Render();
            Screen.Render();
        }
    }

    
    /// <summary>
    /// runs the application in dynamic render mode
    /// </summary>
    private void RunDynamic() {
        
        // the first frame should be rendered immediately, so last frame was rendered yesterday this time :) 
        DateTime lastFrame = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
        
        // minimal time span between frames
        TimeSpan minDelta = new TimeSpan(0, 0, 0, 0, 1 / Config.MaxUpdatesPerSecond * 1000);
        
        while (IsRunning) {

            ConsoleKeyInfo? ki = null;
            
            // if a key is available for reading, read it
            if (System.Console.KeyAvailable) {
                ki = System.Console.ReadKey(true);
            }
            
            // interrupt combination triggered -> stop application
            if (ki != null && 
                ki.Value.Key == Config.InterruptCombination.Key && 
                ki.Value.Modifiers == Config.InterruptCombination.Modifiers) 
            {
                Stop();
                return;
            }
            
            // terminal has been resized
            if (Screen.Width != System.Console.WindowWidth ||
                Screen.Height != System.Console.WindowHeight) 
            {
                ResizeEvent.Invoke(new ResizeEventArgs(System.Console.WindowWidth, System.Console.WindowHeight));
            }
            
            // invoke all registered subscribers to key events
            if (ki != null) {
                KeyEvent.Invoke(this, new KeyEventArgs((ConsoleKeyInfo)ki));
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
    }
}