﻿// 
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Console;

/// <summary>
/// a wrapper exception for fancy formatting
/// </summary>
public sealed class FancyException<TInner> : Exception, IFancyException<TInner> where TInner : Exception {
    public TInner OriginalException { get; }
    public override string ToString() => Debug.ToString(OriginalException);
    public FancyException(TInner e) => OriginalException = e;

    /// <summary>
    /// creates a new fancy exception from an exception
    /// </summary>
    internal static IFancyException<Exception> Create(Exception e) {
        var type = typeof(FancyException<>).MakeGenericType(e.GetType());
        return (IFancyException<Exception>)Activator.CreateInstance(type, e)!;
    }
    
    public static implicit operator TInner(FancyException<TInner> e) => e.OriginalException;
}

/// <summary>
/// covariant interface for fancy exceptions
/// </summary>
/// <typeparam name="TInner">the wrapped exception type</typeparam>
public interface IFancyException<out TInner> where TInner : Exception {
    public TInner OriginalException { get; }
}