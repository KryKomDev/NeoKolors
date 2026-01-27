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
        
        var oneOf = element.GetChildNode();
        
        if (oneOf is not IElement[] children) return null;

        for (var i = 0; i < children.Length; i++) {
            var c = children[i];
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
        
        var node = element.GetChildNode();
        
        if (node is not IElement[] children) return;

        for (var i = 0; i < children.Length; i++) {
            var c = children[i];
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
        
        var node = element.GetChildNode();
        
        if (node is not IElement[] children) return;

        for (var i = 0; i < children.Length; i++) {
            var c = children[i];
            GetElementsByType(type, c, result);
        }
    }

    public IEnumerable<IElement> All() {
        var l = new List<IElement>();
        All(BaseElement, l);
        return l;
    }

    public static void All(IElement element, List<IElement> result) {
        result.Add(element);
        
        var node = element.GetChildNode();
        
        if (node is not IElement[] children) return;

        for (int i = 0; i < children.Length; i++) {
            var c = children[i];
            All(c, result);
        }
    }
}