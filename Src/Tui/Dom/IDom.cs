// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Dom;

public interface IDom {
    public IElement BaseElement { get; set; }
    
    public IElement? GetElementById(string id);
    public IElement[] GetElementsByClass(string className);
    public IElement[] GetElementsByType(Type type);
}