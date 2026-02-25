// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Common;
using NeoKolors.Console.Events;

namespace NeoKolors.Console.Driver;

/// <summary>
/// Defines an interface that represents an input driver for processing console input events.
/// </summary>
public interface IInputDriver : IDisposable {

    public event MouseEventHandler? Mouse;
    public event KeyEventHandler? Key;
    public event FocusInEventHandler? FocusIn;
    public event FocusOutEventHandler? FocusOut;
    public event ResizeEventHandler? Resize;
    public event PasteEventHandler? Paste;
    public event VTQueryResponseHandler? VTQuery;

    /// <summary>
    /// Requests a specific DEC (Digital Equipment Corporation) private mode to be set or reset in the console.
    /// </summary>
    /// <param name="mode">
    /// The DEC private mode to be requested. This parameter is represented by the
    /// <see cref="EscapeCodes.DecMode"/> enumeration and specifies the desired
    /// functionality to enable or disable, such as mouse reporting, alternate buffer,
    /// or focus reporting.
    /// </param>
    public void RequestDecMode(EscapeCodes.DecMode mode);

    /// <summary>
    /// Requests a specific OSC (Operating System Command) mode to be set or executed in the console.
    /// </summary>
    /// <param name="mode">
    /// The OSC mode to be requested. This parameter is represented by the
    /// <see cref="EscapeCodes.OscMode"/> enumeration and specifies the desired
    /// command for interacting with the console, such as clipboard operations
    /// or setting the window title.
    /// </param>
    public void RequestOscMode(EscapeCodes.OscMode mode);

    public bool IsRunning { get; }
    
    public void Start();
    public void Stop();
}