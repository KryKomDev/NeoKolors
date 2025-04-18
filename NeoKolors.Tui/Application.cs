//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Console;
using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

public class Application : IApplication {
    
    /// <summary>
    /// stack of 'pop-ups'
    /// </summary>
    private List<Window> Windows { get; } = new();

    
    /// <summary>
    /// adds a new window to the stack of windows of the application
    /// </summary>
    public void PushWindow(Window w) => Windows.Add(w);

    
    /// <summary>
    /// removes a window from the stack of windows of the application 
    /// </summary>
    public void PopWindow() => Windows.RemoveAt(Windows.Count - 1);

    
    /// <summary>
    /// contains the base rendered elements of the application
    /// </summary>
    private List<View> Views { get; } = new();


    /// <summary>
    /// adds a view to the base of the application
    /// </summary>
    public void AddView(View v) {
        Views.Add(v);
        if (v.OnKeyPress != null) KeyEvent += new KeyEventHandler(v.OnKeyPress);
        if (v.OnResize != null) ResizeEvent += new ResizeEventHandler(v.OnResize);
        if (v.OnStart != null) StartEvent += new AppStartEventHandler(v.OnStart);
        if (v.OnStop != null) StopEvent += new EventHandler(v.OnStop);
    }


    /// <summary>
    /// called when a key is pressed
    /// </summary>
    private event KeyEventHandler KeyEvent;
    
    
    /// <summary>
    /// called when terminal is resized
    /// </summary>
    private event ResizeEventHandler ResizeEvent;
    
    
    /// <summary>
    /// called when the application is started
    /// </summary>
    private event AppStartEventHandler StartEvent;
    
    
    /// <summary>
    /// called when the application is stopped
    /// </summary>
    private event EventHandler StopEvent;

    
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
    public Application() {
        Config = new AppConfig();
        KeyEvent = (_, _) => { };
        ResizeEvent = () => { };
        StartEvent = (_, _) => { };
        StopEvent = (_, _) => { };
        Screen = new ConsoleScreen(System.Console.Out);
        ResizeEvent += Screen.Resize;
    }
    
    
    /// <summary>
    /// creates a new tui application with a custom configuration
    /// </summary>
    public Application(AppConfig config) {
        Config = config;
        KeyEvent = (_, _) => { };
        ResizeEvent = () => { };
        StartEvent = (_, _) => { };
        StopEvent = (_, _) => { };
        Screen = new ConsoleScreen(System.Console.Out);
        ResizeEvent += Screen.Resize;
    }
    
    
    /// <inheritdoc cref="IApplication.Render"/>
    public void Render() {
        
        // render the base views
        foreach (var v in Views) {
            v.Render(Screen);
        }

        // render the window stack
        foreach (var w in Windows) {
            w.Render(Screen);
        }
    }
    
    
    /// <inheritdoc cref="IApplication.Start"/>
    public void Start() {
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

            // terminal has been resized
            if (Screen.Width != System.Console.WindowWidth ||
                Screen.Height != System.Console.WindowHeight) 
            {
                ResizeEvent.Invoke();
            }

            if (!Screen.ScreenMode)
                KeyEvent.Invoke(this, new KeyEventArgs(ki));
            
            NKDebug.Debug($"Key ?: {ki.KeyChar}");
            
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
                ResizeEvent.Invoke();
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
        IsRunning = false;
        StopEvent.Invoke(this, EventArgs.Empty);
        Screen.ScreenMode = false;
    }
}