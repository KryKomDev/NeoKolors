// NeoKolors
// Copyright (c) 2025 KryKom

using Implyzer;
using Metriks;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Events;

namespace NeoKolors.Console.Driver;

/// <summary>
/// Defines an interface that represents an input driver for processing console input events.
/// </summary>
public interface IInputDriver<TConfig> : IInputDriver where TConfig : InputDriverConfig {
    public new TConfig Config { get; set; }
    
    InputDriverConfig IInputDriver.Config {
        get => Config;
        set => Config = value as TConfig 
            ?? throw new InvalidCastException($"Config must be of type {typeof(TConfig).FullName}"); 
    }
}

/// <summary>
/// Defines an interface that represents an input driver for processing console input events.
/// </summary>
[IndirectImpl(typeof(IInputDriver<>))]
public interface IInputDriver : IDisposable {

    public event MouseEventHandler?      Mouse;
    public event KeyEventHandler?        Key;
    public event FocusInEventHandler?    FocusIn;
    public event FocusOutEventHandler?   FocusOut;
    public event ResizeEventHandler?     Resize;
    public event PasteEventHandler?      Paste;
    public event VTQueryResponseHandler? VTQuery;


    public bool IsRunning { get; }

    public InputDriverConfig Config { get; set; }

    /// <summary>
    /// Sends a VT (Virtual Terminal) query request to the driver.
    /// This method initiates a request for specific terminal states or features based on the provided query type.
    /// </summary>
    /// <param name="request">The <see cref="VTQuery"/> object defining the type of query to send to the terminal.</param>
    public void RequestVTQuery(VTQuery request);

    public void Start();
    public void Stop();

    /// <summary>
    /// Retrieves the current size of the console or terminal associated with the input driver.
    /// This method provides the dimensions as a 2D-size object, typically representing rows and columns.
    /// </summary>
    /// <returns>A <see cref="Size2D"/> object representing the current width and height of the terminal.</returns>
    public Size2D GetSize();
}