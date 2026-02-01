// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Events;

namespace NeoKolors.Console.Driver;

public interface IInputDriver : IDisposable {

    public event MouseEventHandler? Mouse;
    public event KeyEventHandler? Key;
    public event FocusInEventHandler? FocusIn;
    public event FocusOutEventHandler? FocusOut;
    public event PasteEventHandler? Paste;
    public event WinOpsResponseEventHandler? WinOpsResponse;
    public event DecReqResponseEventHandler? DecReqResponse;

    public void Start();
    public void Stop();

}