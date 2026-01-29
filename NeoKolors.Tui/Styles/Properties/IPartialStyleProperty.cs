// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Styles.Properties;

public interface IPartialStyleProperty<out TValue, in TSelf, TBase> 
    : IStyleProperty<TValue, TSelf>, IPartialStyleProperty
    where TValue : notnull 
    where TSelf  : IStyleProperty<TValue, TSelf>, new()
    where TBase  : IStyleProperty, new() 
{
    public TBase Combine(TBase baseProperty);

    Type IPartialStyleProperty.BaseType => typeof(TBase);
    
    IStyleProperty IPartialStyleProperty.Combine(IStyleProperty baseProperty) {
        if (baseProperty is TBase baseTBase)
            return Combine(baseTBase);
        
        throw new ArgumentException($"baseProperty must be of type '{typeof(TBase)}'");
    }
}

public interface IPartialStyleProperty : IStyleProperty {
    public IStyleProperty Combine(IStyleProperty baseProperty);
    public Type BaseType { get; }
}