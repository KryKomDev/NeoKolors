+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'NKStyle'
+++

```C#
[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
public struct NKStyle : ICloneable, IEquatable<NKStyle>, IFormattable;
```

Structure `NKStyle` contains data for string styling. It compresses 2 `NKColor` instances
(one for text, one for background) and a `TextStyles` instance into a single `long` making
it a pretty optimized data type.

## Raw

```C#
[field: FieldOffset(0)]
public ulong Raw { get; private set; }
```

The actual compressed styles.
Bit 0 represents the most significant bit and bit 63 the least significant bit.

### Layout

| bits       | meaning                                                                                            |
|------------|----------------------------------------------------------------------------------------------------|
| bits 0-23  | foreground color                                                                                   |
| bits 24-47 | background color                                                                                   |
| bits 56-61 | text style bitmap (strikethrough, negative, faint, underline, italic and bold respectively)        |
| bit 62     | toggles terminal color palette mode for text (uses custom color if 1, palette colors else)         |
| bit 63     | toggles terminal color palette mode for a background (uses custom color if 1, palette colors else) |

---

## FColor

```C#
public NKColor FColor { get; set; }
```

Represents the color of text.

---

## FColor

```C#
public NKColor BColor { get; set; }
```

Represents the color of background.

---

## Styles

```C#
public TextStyles Styles { get; set; }
```

The styles of the text.

---

## SetFColor

```C#
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetFColor(UInt24 color);

public void SetFColor(NKColor color);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetFColor(NKConsoleColor color);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetFColor(DefaultColor color);

[MethodImpl(MethodImplOptions.AggressiveInlining)] 
public void SetFColor(InheritColor color);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetFColor();
```

Sets the foreground (text) color.
Overload `SetFColor()` sets the color to `DefaultColor`.

---

## SetBColor

```C#
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetBColor(UInt24 color);

public void SetBColor(NKColor color);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetBColor(NKConsoleColor color);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetBColor(DefaultColor color);

[MethodImpl(MethodImplOptions.AggressiveInlining)] 
public void SetBColor(InheritColor color);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetBColor();
```

Sets the foreground (text) color.
Overload `SetBColor()` sets the color to `DefaultColor`.

---

## SafeSetFColor

```C#
public void SafeSetFColor(NKColor color);
```

Safely overrides `this` text color.
If `color` is not `InheritColor` sets `this` instance, else does nothing.

---

## SafeSetBColor

```C#
public void SafeSetBColor(NKColor color);
```

Safely overrides `this` background color.
If `color` is not `InheritColor` sets `this` instance, else does nothing.

---

## GetFColor

```C#
public NKColor GetFColor();
```

Returns text color of `this` instance.

---

## GetBColor

```C#
public NKColor GetBColor();
```

Returns background color of `this` instance.

---

## SetStyles

```C#
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public void SetStyles(TextStyles styles);
```

Sets `this` instance's styles.

---

## GetStyles

```C#
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public TextStyles GetStyles();
```

Returns `this` instance's text styles.

---

## Override

```C#
public void Override(NKStyle other);
```

Safely sets `this` instance's background and foreground color to `other`'s and 
sets `this` instance's styles to `other`'s styles.

---

## Constructors

```C#
public NKStyle(NKColor f, NKColor b);
public NKStyle(NKColor f, TextStyles s);
public NKStyle(NKColor f);
public NKStyle(TextStyles s);
public NKStyle();
```

Colors are by default set to `NKColor.Default`, styles are by default set to `TextStyles.NONE`.

---

## &lt;&lt; operator

```C#
public static NKStyle operator <<(NKStyle overriden, NKStyle overrider);
```

Calls `Override` on `overriden` with `overrider` as parameter.

It is advised to use this feature as shown below:

```C# {hl_lines=[4]}
NKStyle overriden = new NKStyle(0x123456, 0x789abc, TextStyles.ITALIC);
NKStyle overrider = new NKStyle(NKColor.Default, NKColor.Inherit, TextStyles.BOLD);

overriden <<= overrider;

// overriden:
//   FColor: DefaultColor
//   BColor: 0x789abc
//   Styles: TextStyles.BOLD
```

---

## Helper Properties

```C#
public bool IsFColorCustom { get; set; }
public bool IsBColorCustom { get; set; }
public bool IsFColorDefault { get; set; }
public bool IsBColorDefault { get; set; }
public bool IsFColorInherit { get; set; }
public bool IsBColorInherit { get; set; }
```