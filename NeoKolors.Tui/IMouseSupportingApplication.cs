// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Events;

namespace NeoKolors.Tui;

public interface IMouseSupportingApplication : IApplication {
    public event MouseEventHandler MouseEvent;
}