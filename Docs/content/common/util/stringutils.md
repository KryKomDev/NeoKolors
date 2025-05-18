+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'StringUtils'
+++

```C#
public static class StringUtils;
```

Contains useful methods for working with strings.

---

## VisibleLength

```C#
[Pure]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static int VisibleLength(this string text);
```

Returns the total count of the printable / visible characters contained by the string
(for example, ansi escape characters are not counted).

---

## Capitalize

```C#
[Pure]
public static string Capitalize(this string s);
```

Capitalizes all the words within the string.

### Exceptions
- `ArgumentNullException` - string `s` is null

---

## CapitalizeFirst

```C#
[Pure]
public static string CapitalizeFirst(this string input, CultureInfo? cultureInfo = null);
```

Capitalizes the first character of the string.

### Exceptions
- `ArgumentNullException` - string `s` is null

---

## DecapitalizeFirst

```C#
[Pure]
public static string DecapitalizeFirst(this string input, CultureInfo? cultureInfo = null);
```

Decapitalizes the first character of the string.

### Exceptions
- `ArgumentNullException` - string `s` is null

---

## Format

```C#
[Pure]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static string Format(
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)] this string format, 
    params object[] args);
```

Formats the string using the `string.Format(System.IFormatProvider?,string,object?` method.

### Params
- `format` - the string to format
- `args` - the arguments for the string

---

## InRange

```C#
#if NET5_0_OR_GREATER
[Obsolete("This method can be substituted with range indexer.")]
#endif
public static string InRange(this string s, int startIndex, int endIndex);
```

Creates a substring between the indices, including `startIndex`, excluding `endIndex`.

### Exceptions
- `ArgumentOutOfRangeException` - `startIndex` or `endIndex` is out of range **or** 
    `startIndex` is greater than `endIndex`.

> [!WARNING]
> This method is obsolete in .NET 5.0 and higher, and it is provided only to make the NeoKolors codebase cleaner.

---

## InRange&lt;T&gt;

```C#
#if NET5_0_OR_GREATER
[Obsolete("This method can be substituted with range indexer.")]
#endif
public static IEnumerable<T> InRange<T>(this IEnumerable<T> enumerable, int startIndex, int endIndex);
```

Extracts a range of elements from the given enumerable collection starting from the specified
start index to the specified end index, inclusive of the start index but exclusive of the end index.

### Params
- `enumerable` - The enumerable collection from which to extract the range of elements.
- `startIndex` - The zero-based starting index of the range to extract.
- `endIndex` - The zero-based ending index of the range to extract (exclusive).

### Returns
An enumerable containing the elements from the specified range.

### Exceptions
- `ArgumentOutOfRangeException` - Thrown if <paramref `startIndex` or `endIndex` is out of the array's bounds,
    **or** if `startIndex` is greater than `endIndex`.

> [!WARNING]
> This method is obsolete in .NET 5.0 and higher, and it is provided only to make the NeoKolors codebase cleaner.

---

## Chop

```C#
[Pure]
public static string[] Chop(this string s, int maxLength);
```

Chops the string into multiple strings of a specified maximum length.

### Params
- `s` - the string to be chopped
- `maxLength` - the maximum length of a single string

---

## Join

```C#
public static string Join(this IEnumerable collection, string separator);
```

Joins the stringified objects from the collection using the provided separator.

---

## Join&lt;T&gt;

```C#
public static string Join<TSource>(
        this IEnumerable<TSource> collection,
        string separator,
        Func<TSource?, string?> stringifier);
```

Concatenates the elements of a specified collection using the specified separator.

### Params
- `collection` - The collection whose elements will be concatenated.
- `separator` - The string separator to insert between each element of the collection.
- `stringifier` - A function to convert each element of the collection to a string.

### Returns
A string consisting of the elements of the collection concatenated using the specified separator.

### Exceptions
- `ArgumentNullException` - Thrown if the collection or stringifier is null.

---

## ToRoman

```C#
[Pure]
public static string ToRoman(this int number, bool lowercase = false);
```

Converts an integer to its Roman numeral representation.
Supports values from 1 to 3999.

### Params
- `number` - The integer to be converted to Roman numerals.
- `lowercase` - Specifies whether to return the result in lowercase. Default is false.

### Returns
A string representing the Roman numeral equivalent of the given number.

### Exceptions
- `ArgumentOutOfRangeException` - Thrown when the supplied number is less than 1 or greater than 3999.