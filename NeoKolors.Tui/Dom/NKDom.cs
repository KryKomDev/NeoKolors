// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Dom;

public class NKDom : IDom {
    
    public IElement BaseElement { get; set; }

    public NKDom(IElement baseElement) {
        BaseElement = baseElement;
    }
    
    public IElement GetElementById(string id) {
        throw new NotImplementedException();
    }
    
    public IElement[] GetElementsByClass(string className) {
        throw new NotImplementedException();
    }
    
    public IElement[] GetElementsByType(Type type) {
        throw new NotImplementedException();
    }
}