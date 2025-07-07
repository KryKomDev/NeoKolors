// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder;

public static class SettingsFactory {
    public static DefaultNodeProvider DefaultNodeProvider { get; set; } = () => new SettingsNode<object>();
}

public delegate ISettingsNode DefaultNodeProvider();