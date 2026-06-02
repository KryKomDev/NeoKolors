// NeoKolors
// Copyright (c) 2026 KryKom

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
        
        var node = element.GetChildNode();
        if (node == null) return null;

        if (node is IElement child) {
            var res = GetElementById(id, child);
            if (res != null) return res;
        }
        else if (node is IElement[] array) {
            for (int i = 0; i < array.Length; i++) {
                var c = array[i];
                if (c != null) {
                    var res = GetElementById(id, c);
                    if (res != null) return res;
                }
            }
        }
        else if (node is List<IElement> list) {
            for (int i = 0; i < list.Count; i++) {
                var c = list[i];
                if (c != null) {
                    var res = GetElementById(id, c);
                    if (res != null) return res;
                }
            }
        }
        else if (node is IReadOnlyList<IElement> readOnlyList) {
            for (int i = 0; i < readOnlyList.Count; i++) {
                var c = readOnlyList[i];
                if (c != null) {
                    var res = GetElementById(id, c);
                    if (res != null) return res;
                }
            }
        }
        else if (node is IList<IElement> iList) {
            for (int i = 0; i < iList.Count; i++) {
                var c = iList[i];
                if (c != null) {
                    var res = GetElementById(id, c);
                    if (res != null) return res;
                }
            }
        }
        else if (node is IEnumerable<IElement> children) {
            foreach (var c in children) {
                if (c != null) {
                    var res = GetElementById(id, c);
                    if (res != null) return res;
                }
            }
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
        if (node == null) return;

        if (node is IElement child) {
            GetElementsByClass(className, child, result);
        }
        else if (node is IElement[] array) {
            for (int i = 0; i < array.Length; i++) {
                var c = array[i];
                if (c != null) GetElementsByClass(className, c, result);
            }
        }
        else if (node is List<IElement> list) {
            for (int i = 0; i < list.Count; i++) {
                var c = list[i];
                if (c != null) GetElementsByClass(className, c, result);
            }
        }
        else if (node is IReadOnlyList<IElement> readOnlyList) {
            for (int i = 0; i < readOnlyList.Count; i++) {
                var c = readOnlyList[i];
                if (c != null) GetElementsByClass(className, c, result);
            }
        }
        else if (node is IList<IElement> iList) {
            for (int i = 0; i < iList.Count; i++) {
                var c = iList[i];
                if (c != null) GetElementsByClass(className, c, result);
            }
        }
        else if (node is IEnumerable<IElement> children) {
            foreach (var c in children) {
                if (c != null) GetElementsByClass(className, c, result);
            }
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
        if (node == null) return;

        if (node is IElement child) {
            GetElementsByType(type, child, result);
        }
        else if (node is IElement[] array) {
            for (int i = 0; i < array.Length; i++) {
                var c = array[i];
                if (c != null) GetElementsByType(type, c, result);
            }
        }
        else if (node is List<IElement> list) {
            for (int i = 0; i < list.Count; i++) {
                var c = list[i];
                if (c != null) GetElementsByType(type, c, result);
            }
        }
        else if (node is IReadOnlyList<IElement> readOnlyList) {
            for (int i = 0; i < readOnlyList.Count; i++) {
                var c = readOnlyList[i];
                if (c != null) GetElementsByType(type, c, result);
            }
        }
        else if (node is IList<IElement> iList) {
            for (int i = 0; i < iList.Count; i++) {
                var c = iList[i];
                if (c != null) GetElementsByType(type, c, result);
            }
        }
        else if (node is IEnumerable<IElement> children) {
            foreach (var c in children) {
                if (c != null) GetElementsByType(type, c, result);
            }
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
        if (node == null) return;

        if (node is IElement child) {
            All(child, result);
        }
        else if (node is IElement[] array) {
            for (int i = 0; i < array.Length; i++) {
                var c = array[i];
                if (c != null) All(c, result);
            }
        }
        else if (node is List<IElement> list) {
            for (int i = 0; i < list.Count; i++) {
                var c = list[i];
                if (c != null) All(c, result);
            }
        }
        else if (node is IReadOnlyList<IElement> readOnlyList) {
            for (int i = 0; i < readOnlyList.Count; i++) {
                var c = readOnlyList[i];
                if (c != null) All(c, result);
            }
        }
        else if (node is IList<IElement> iList) {
            for (int i = 0; i < iList.Count; i++) {
                var c = iList[i];
                if (c != null) All(c, result);
            }
        }
        else if (node is IEnumerable<IElement> children) {
            foreach (var c in children) {
                if (c != null) All(c, result);
            }
        }
    }
}