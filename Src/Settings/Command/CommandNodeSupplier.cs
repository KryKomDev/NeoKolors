//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Command;

public delegate ICommandNode<TResult> CommandNodeSupplier<TResult>(in ICommandNode<TResult> node);
public delegate ICommandNode CommandNodeSupplier(in ICommandNode node);