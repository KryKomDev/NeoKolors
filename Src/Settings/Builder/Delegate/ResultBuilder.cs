//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Builder.Delegate;

public delegate TResult ResultBuilder<out TResult>(in Context context);
public delegate object? ResultBuilder(in Context context);