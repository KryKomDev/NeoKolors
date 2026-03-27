// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Events;

namespace NeoKolors.Tui;

public interface IMouseSupportingApplication : IApplication {
    public event MouseEventHandler MouseEvent;
}