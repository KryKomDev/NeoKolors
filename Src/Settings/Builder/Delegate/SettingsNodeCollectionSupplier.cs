// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder;

public delegate ISettingsNodeCollection<TResult> SettingsNodeCollectionSupplier<TResult>(in ISettingsNodeCollection<TResult> nodes);
public delegate ISettingsNodeCollection SettingsNodeCollectionSupplier(in ISettingsNodeCollection nodes);