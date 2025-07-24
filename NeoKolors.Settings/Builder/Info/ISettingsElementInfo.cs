// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder.Info;

public interface ISettingsElementInfo<out TValue> : ISettingsElementInfo {
    public new TValue Value { get; }
}

public interface ISettingsElementInfo {
    public string Name { get; }
    public object? Value { get; }
    public string? Description { get; }
    public string ToXsd();
}