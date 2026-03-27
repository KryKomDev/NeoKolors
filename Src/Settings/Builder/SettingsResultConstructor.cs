// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder;

public delegate TResult SettingsResultConstructor<TResult>(in Context context);
public delegate object? SettingsResultConstructor(in Context context);