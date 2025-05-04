//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui;

/// <summary>
/// base class for all renderable classes
/// </summary>
public interface IRenderable {
    public void Render(in IConsoleScreen target);
}