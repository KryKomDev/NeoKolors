// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Settings.Builder.Collection;

namespace NeoKolors.Settings.Builder.Delegate;

public delegate ISettingsNodeCollection<TResult> SettingsNodeCollectionSupplier<TResult>(in ISettingsNodeCollection<TResult> nodes);
public delegate ISettingsNodeCollection SettingsNodeCollectionSupplier(in ISettingsNodeCollection nodes);