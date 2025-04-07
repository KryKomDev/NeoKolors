# UInt24

```C#
#if NET5_0_OR_GREATER && !NET5_0

public readonly struct UInt24 : 
    IMinMaxValue<UInt24>,
    IBinaryInteger<UInt24>;
#else 
public readonly struct UInt24 :
    IComparable, 
    IComparable<UInt24>,
    IEquatable<UInt24>;
#endif
```

An unsigned 24 bit integer;