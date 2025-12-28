// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Events;

namespace NeoKolors.Tui;

public interface INotifyOnRender {
    public event OnRenderEventHandler OnRender;
}