// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

public interface INotifyOnRender {
    public event OnRenderEventHandler OnRender;
}