// NeoKolors
// Copyright (c) 2026 KryKom

using Implyzer;

namespace NeoKolors.Tui.Styles.Properties;

public interface IPartialStyleProperty<out TValue, TSelf, TBase> 
    : IStyleProperty<TValue, TSelf>, IPartialStyleProperty
    where TValue : notnull 
    where TSelf  : IPartialStyleProperty<TValue, TSelf, TBase>, IStyleProperty<TValue, TSelf>, new()
    where TBase  : IStyleProperty, new() 
{
    public TBase Combine(TBase baseProperty);
    public TSelf Extract(TBase baseProperty);

    Type IPartialStyleProperty.BaseType => typeof(TBase);
    Type IStyleProperty.ValueType => typeof(TValue);

    IStyleProperty IPartialStyleProperty.Combine(IStyleProperty baseProperty) {
        if (baseProperty is TBase baseTBase)
            return Combine(baseTBase);
        
        throw new ArgumentException($"baseProperty must be of type '{typeof(TBase)}'");
    }

    IPartialStyleProperty IPartialStyleProperty.Extract(IStyleProperty baseProperty) {
        if (baseProperty is TBase baseTBase)
            return Extract(baseTBase);
        
        throw new ArgumentException($"baseProperty must be of type '{typeof(TBase)}'");
    }
}

[ImplType(ImplKind.ValueType)]
[IndirectImpl(typeof(IPartialStyleProperty<,,>))]
public interface IPartialStyleProperty : IStyleProperty {
    public IStyleProperty Combine(IStyleProperty baseProperty);
    public IPartialStyleProperty Extract(IStyleProperty baseProperty);
    public Type BaseType { get; }
}