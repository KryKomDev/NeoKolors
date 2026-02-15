// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;

namespace NeoKolors.Common.Util;

public class TypeSet<TBase> : ITypeSet<TBase> {
    
    protected readonly Dictionary<Type, TBase> _dict = new();
    
    public int Count => _dict.Count;

    public bool Contains(Type type) => _dict.ContainsKey(type);
    public bool Contains<T>() where T : TBase => _dict.ContainsKey(typeof(T));

    public void Set(TBase instance) {
        var type = instance?.GetType();
        if (type == null)
            throw new ArgumentNullException(nameof(instance));
        
        _dict[type] = instance;
    }
    
    public void Set<T>(T instance) where T : TBase => _dict[typeof(T)] = instance;

    public TBase Get(Type type) => _dict[type];
    public T Get<T>() where T : TBase => (T)_dict[typeof(T)]! ?? throw new InvalidOperationException();

    public void Remove(Type type) => _dict.Remove(type);
    public void Remove<T>() where T : TBase => _dict.Remove(typeof(T));

    public IEnumerator<TBase> GetEnumerator() => ((IEnumerable<TBase>)_dict.Values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}