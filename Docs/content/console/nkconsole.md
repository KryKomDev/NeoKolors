+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'NKConsole'
+++

```c# {lineNos=false}
public static partial class NKConsole;
```

`NKConsole` provides I/O methods. 
Output methods can be used to color strings in console.
Input methods provide an easy way to get user input, 
without having to define much of the boilerplate code.

---

## Write

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, int hex);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, byte r, byte g, byte b);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, System.Drawing.Color c);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, ConsoleColor c);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, NKColor c);
```

Writes string `s` to the standard output stream 
colored by the color specified by the other arguments.

---

## WriteF

```c# {lineNos=false}
[StringFormatMethod(nameof(s))]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteF(string s, params NKColor[] colors);
```

Writes the format string `s` colored by the colors specified in `colors`.

---

## WriteLine

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, int hex);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, byte r, byte g, byte b);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, System.Drawing.Color c);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, ConsoleColor c);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, NKColor c);
```

Writes string `s` to the standard output stream
colored by the color specified by the other arguments 
and appends a newline to the end.


---

## WriteLineF

```c# {lineNos=false}
[StringFormatMethod(nameof(s))]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLineF(string s, params NKColor[] colors);
```

Writes the format string `s` colored by the colors specified in `colors` 
and adds a newline to the end.

---

## WriteB

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteB(string s, int hex);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteB(string s, byte r, byte g, byte b);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteB(string s, System.Drawing.Color c);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteB(string s, ConsoleColor c);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteB(string s, NKColor c);
```

Writes string `s` to the standard output stream with background
colored by the color specified by the other arguments.

---

## WriteBF

```c# {lineNos=false}
[StringFormatMethod(nameof(s))]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteBF(string s, params NKColor[] colors);
```

Writes the format string `s` with background colored by the colors specified in `colors`.

---

## WriteLineB

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLineB(string s, int hex);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLineB(string s, byte r, byte g, byte b);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLineB(string s, System.Drawing.Color c);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLineB(string s, ConsoleColor c);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLineB(string s, NKColor c);
```

Writes string `s` to the standard output stream with background
colored by the color specified by the other arguments
and appends a newline to the end.


---

## WriteLineBF

```c# {lineNos=false}
[StringFormatMethod(nameof(s))]
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLineBF(string s, params NKColor[] colors);
```

Writes the format string `s` with background colored by the colors
specified in `colors` and adds a newline to the end.

---

## Write(string, &lt;forg&gt;, &lt;bckg&gt;)

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, int text, int background);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, byte tr, byte tg, byte tb, byte br, byte bg, byte bb);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, System.Drawing.Color text, System.Drawing.Color background);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, ConsoleColor text, ConsoleColor background);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, NKColor text, NKColor background);
```

Writes string `s` to the standard output stream with background and text
colored by the color specified by the other arguments.

---

## WriteLine(string, &lt;forg&gt;, &lt;bckg&gt;)

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, int text, int background);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, byte tr, byte tg, byte tb, byte br, byte bg, byte bb);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, System.Drawing.Color text, System.Drawing.Color background);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, ConsoleColor text, ConsoleColor background);

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, NKColor text, NKColor background);
```

Writes string `s` to the standard output stream with background and text
colored by the color specified by the other arguments and appends a newline.

---

## Write(string, NKStyle)

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(string s, NKStyle st);
```

Writes the string `s` styled using the `st` argument to the standard output stream. 

---

## WriteLine(string, NKStyle)

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(string s, NKStyle st);
```

Writes the string `s` styled using the `st` argument to the standard output stream and adds a newline.

---

## Write(char, NKStyle)

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void Write(char c, NKStyle st);
```

Writes the char `c` styled using the `st` argument to the standard output stream. 

---

## WriteLine(char, NKStyle)

```c# {lineNos=false}
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static void WriteLine(char c, NKStyle st);
```

Writes the char `c` styled using the `st` argument to the standard output stream and adds a newline. 

---

## WriteTable&lt;T&gt;(IEnumerable&lt;string&gt;, IEnumerable&lt;T&gt;, Func&lt;T, string[]&gt;)

```c# {lineNos=false}
public static void WriteTable<T>(
        IEnumerable<string> header, 
        IEnumerable<T> data, 
        Func<T, string[]> rowSelector);
```

Writes a table to the standard output stream.

### Params
- **`header`** - collection of strings to be used as a header for the table
- **`data`** - the data to be displayed in the table
- **`rowSelector`** - converts a data object into a row of individual strings

### Exceptions
- **`ArgumentException`** - thrown when any row's column count does not match the 
    number of columns specified by the header

---

## WriteTable&lt;T&gt;(IEnumerable&lt;string&gt;, IEnumerable&lt;IEnumerable&lt;T&gt;&gt;, Func&lt;T, string&gt;)

```c# {lineNos=false}
public static void WriteTable<T>(
        IEnumerable<string> header, 
        IEnumerable<IEnumerable<T>> data,
        Func<T, string>? stringifier = null);
```

Writes a table to the standard output stream.

### Params
- **`header`** - collection of strings to be used as a header for the table
- **`data`** - a 2D `IEnumerable` representing the data of the table
- **`stringifier`** - converts a single data point to a string

### Exceptions
- **`ArgumentException`** - thrown when any row's column count does not match the
  number of columns specified by the header

---

## WriteTable&lt;T&gt;(string[], T[])

```c# {lineNos=false}
public static void WriteTable<T>(string[] header, params T[] data);
```

Automatically collects values of properties of objects from `data` and
writes a table to the standard output stream.

### Params 
- **`header`** - collection of string to be used as a header for the table
- **`data`** - the data to be displayed in the table

### Exceptions
- **`ArgumentException`** - thrown when any row's column count does not match the
  number of columns specified by the header **or** some properties defined in header were not found

### Remarks

Automatically collects values of properties with the same name as 
defined in `header` using reflections.