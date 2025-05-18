+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'TextStyles'
+++

```C#
[Flags]
public enum TextStyles : byte {
    NONE = 0,
    BOLD = 1,
    ITALIC = 2,
    UNDERLINE = 4,
    FAINT = 8,
    NEGATIVE = 16,
    STRIKETHROUGH = 32
}
```

Contains text styling types.