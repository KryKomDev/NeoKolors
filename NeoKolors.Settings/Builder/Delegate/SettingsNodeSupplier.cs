// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Settings.Builder.Delegate;

public delegate ISettingsNode<TResult> SettingsNodeSupplier<TResult>(ISettingsNode<TResult> node);
public delegate ISettingsNode SettingsNodeSupplier(ISettingsNode node);