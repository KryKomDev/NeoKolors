// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Dom;

public class NKDom : IDom {
    
    public IElement BaseElement { get; set; }

    public NKDom(IElement baseElement) {
        BaseElement = baseElement;
    }
    
    public IElement? GetElementById(string id) => GetElementById(id, BaseElement);

    public static IElement? GetElementById(string id, IElement element) {
        if (element.Info.Id == id) return element;
        
        var oneOf = element.GetChildren();
        
        if (oneOf.IsT1) return null;

        var t0 = oneOf.AsT0;
        for (var i = 0; i < t0.Length; i++) {
            var c = t0[i];
            var res = GetElementById(id, c);
            if (res != null) return res;
        }

        return null;
    }
    
    public IElement[] GetElementsByClass(string className) {
        var l = new List<IElement>();
        GetElementsByClass(className, BaseElement, l);
        return l.ToArray();
    }

    public static void GetElementsByClass(string className, IElement element, List<IElement> result) {
        if (element.Info.IsOfClass(className)) result.Add(element);
        
        var children = element.GetChildren();
        
        if (children.IsT1) return;

        var t0 = children.AsT0;
        for (var i = 0; i < t0.Length; i++) {
            var c = t0[i];
            GetElementsByClass(className, c, result);
        }
    }
    
    public IElement[] GetElementsByType(Type type) {
        var l = new List<IElement>();
        GetElementsByType(type, BaseElement, l);
        return l.ToArray();
    }

    public static void GetElementsByType(Type type, IElement element, List<IElement> result) {
        if (element.GetType() == type) result.Add(element);
        
        var children = element.GetChildren();
        
        if (children.IsT1) return;

        var t0 = children.AsT0;
        for (var i = 0; i < t0.Length; i++) {
            var c = t0[i];
            GetElementsByType(type, c, result);
        }
    }
}