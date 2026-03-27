// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder;

public static class GenericSettingsFactory<TResult> {
    public static DefaultNodeProvider<TResult> DefaultNodeProvider { get; set; } = () => new SettingsNode<TResult>();
}

public delegate ISettingsNode<TResult> DefaultNodeProvider<TResult>();
