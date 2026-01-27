// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Elements;

public record ElementInfo {
    private HashSet<string> _classes = [];
    
    public string? Id { get; set; }
    public string[] Classes {
        get => _classes.ToArray();
        set => _classes = value.ToHashSet();
    }
    
    public ElementInfo(string id, string[] classes) {
        Id = id;
        _classes = classes.ToHashSet();
    }

    public ElementInfo() {
        Id = null;
        _classes = [];
    }

    public static ElementInfo Default => new();

    public void AddClass(string className) => _classes.Add(className);
    public void RemoveClass(string className) => _classes.Remove(className);
    public bool IsOfClass(string className) => _classes.Contains(className);
}