// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Rendering;

namespace NeoKolors.Tui;

public interface IRenderable {
    public void Render(ICharCanvas canvas);
}