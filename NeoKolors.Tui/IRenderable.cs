// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Rendering;

namespace NeoKolors.Tui;

public interface IRenderable {
    public void Render(ICharCanvas canvas);
}