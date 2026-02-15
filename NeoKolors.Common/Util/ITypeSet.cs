// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Common.Util;

public interface ITypeSet<TBase> : IEnumerable<TBase> {
    public int Count { get; }
    
    public bool Contains(Type type);
    public bool Contains<T>() where T : TBase;
    
    public void Set(TBase instance);
    public void Set<T>(T instance) where T : TBase;
    
    public TBase Get(Type type);
    public T Get<T>() where T : TBase;
    
    public void Remove(Type type);
    public void Remove<T>() where T : TBase;
}