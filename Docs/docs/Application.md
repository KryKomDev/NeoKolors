# Application

```C#
public class Application : IApplication;
```

The basic `IApplication` implementation.

## Fields and properties
* [Windows](#windows)
* [Views](#views)
* [KeyPressedEvent](#keypressedevent)
* [ResizeEvent](#resizeevent)
* [StartEvent](#startevent)
* [StopEvent](#stopevent)
* [Config](#config)
* [IsRunning](#isrunning)

### Windows

```C#
private List<Window> Windows { get; }
```

Stack of 'pop-ups'. 

Push and remove from the window stack using the `PushWindow(Window)` and `PopWindow()` methods.


### Views

```C#
private List<View> Views { get; }
```

Contains the base rendered elements of the application.

Add a `View` to the application using the `AddView(View)` method. 


### KeyPressedEvent

```C#
private event EventHandler KeyPressedEvent;
```

Called when a key is pressed.


### ResizeEvent

```C#
private event EventHandler ResizeEvent;
```

Called when terminal is resized.


### StartEvent

```C#
private event EventHandler StartEvent;
```

Called when the application is started.


### StopEvent

```C#
private event EventHandler StopEvent;
```

Called when the application is stopped.


### Config

```C#
public AppConfig Config { get; set; }
```

Holds the configuration of the application.
See `AppConfig` for more info.


### IsRunning

```C#
public bool IsRunning { get; private set; }
```

Determines whether the application is running.


## Methods
* [Start](#start)
* [Stop](#stop)
* [Render](#render)
* [PushWindow](#pushwindow)
* [PopWindow](#popwindow)
* [AddView](#addview)

### Start

```C#
public void Start();
```

Implementation of `IApplication.Start()`. 

Sets `IsRunning` to `true` and invokes `StartEvent`. 
Then starts `RunLazy` if `Config.LazyRender` is `true` else `RunDynamic`.

### Stop

```C#
public void Stop();
```

Implementation of `IApplication.Stops()`.

Sets `IsRunning` to `false` and invokes `StopEvent`.

### Render

```C#
public void Render();
```

Implementation of `IApplication.Render()`.

Renders `View`s and then `Window`s. 

### PushWindow

```C#
public void PushWindow(Window w) => Windows.Add(w);
```

Adds a new window to the stack of windows of the application.

### PopWindow

```C#
public void PopWindow() => Windows.RemoveAt(Windows.Count - 1);
```

Removes a window from the stack of windows of the application.


### AddView

```C#
public void AddView(View v);
```

Adds a new `View` to the `Application` and automatically subscribes its 
event handlers (if available) to the events.