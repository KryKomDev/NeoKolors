//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Command;

public delegate CommandNodeCollection<TResult> CommandNodeCollectionSupplier<TResult>(in CommandNodeCollection<TResult> nodes);
public delegate CommandNodeCollection CommandNodeCollectionSupplier(in CommandNodeCollection nodes);