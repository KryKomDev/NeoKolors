//
// NeoKolors
// Copyright (c) 2025 KryKom
//

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
    /// the main views of the application 
    /// </summary>
    private List<View> Views { get; } = new();


    /// <summary>
    /// adds a view to the base of the application
    /// </summary>
    /// <param name="v"></param>
    public void AddView(View v) {
        Views.Add(v);
        KeyPressedHandler += v.InteractKey;
    }


    /// <summary>
    /// event handler for base view key events
    /// </summary>
    private event EventHandler KeyPressedHandler;

    
    /// <summary>
    /// holds the configuration of the application
    /// </summary>
    public AppConfig Config { get; set; }

    
    /// <summary>
    /// character width of the terminal window
    /// </summary>
    public int WindowWidth { get; private set; } = 1;
    
    
    /// <summary>
    /// character height of the terminal window 
    /// </summary>
    public int WindowHeight { get; private set; } = 1;

    
    /// <summary>
    /// determines whether the application is running
    /// </summary>
    public bool IsRunning { get; private set; } = false;
    
    
    /// <summary>
    /// creates a new tui application with the default configuration
    /// </summary>
    public Application() {
        Config = new AppConfig();
        KeyPressedHandler = (_, _) => { };
    }
    
    
    /// <summary>
    /// creates a new tui application with a custom configuration
    /// </summary>
    public Application(AppConfig config) {
        Config = config;
        KeyPressedHandler = (_, _) => { };
    }
    
    
    /// <inheritdoc cref="IApplication.Render"/>
    public void Render() {
        
        // render the base views
        foreach (var v in Views) {
            v.Render();
        }

        // render the window stack
        foreach (var w in Windows) {
            w.Render();
        }
    }
    
    
    /// <inheritdoc cref="IApplication.Start"/>
    public void Start() {
        IsRunning = true;

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
            if (ki == Config.InterruptCombination) {
                Stop();
                return;
            }
            
            KeyPressedHandler.Invoke(this, new KeyEventArgs(ki));
            
            Render();
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
            if (ki == Config.InterruptCombination) {
                Stop();
                return;
            }
            
            // invoke all registered subscribers to key events
            if (ki != null) {
                KeyPressedHandler.Invoke(this, new KeyEventArgs((ConsoleKeyInfo)ki));
            }

            // if the last frame has been rendered in less than minDelta skip rendering for this cycle
            if ((DateTime.Now - lastFrame).TotalMilliseconds < minDelta.TotalMilliseconds) continue;
            
            lastFrame = DateTime.Now;
            Render();
        }
    }
    
    
    /// <inheritdoc cref="IApplication.Stop"/>
    public void Stop() {
        IsRunning = false;
    }
}