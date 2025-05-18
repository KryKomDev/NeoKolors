+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'NeoKolors.Common'
+++

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-89b4fa?style=for-the-badge&labelColor=6c7086)
![.NET 5](https://img.shields.io/badge/.NET-5.0-cba6f7?style=for-the-badge&labelColor=6c7086)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Common?color=f5c2e7&style=for-the-badge&labelColor=6c7086)](https://www.nuget.org/packages/NeoKolors.Common)
![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Common?color=a6e3a1&style=for-the-badge&labelColor=6c7086)
![GitHub License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%236c7086&color=%23f9e2af)

NeoKolors.Common library contains basic common tools, structures and classes
for working with colors and ANSI escape sequences. It also offers some other 
helpful utilities.

---

## Features
- **Color structures**: Structures for storing, converting and manipulating colors.
- **Utility Classes**: Things like `List2D` or `UInt24` can simplify and boost the performance of the code. 
- **High Performance**: Optimized code for speed and efficiency. Or at least I tried to :)
- **Cross-Platform Support**: Built to work seamlessly across frameworks like `net5.0`, `net8.0`, `.NET Standard 2.0`, and more.

---

## Getting Started

### Installation

To use `NeoKolors.Common`, add a reference to the project in your solution. You can use one of the following methods:

#### NuGet CLI

``` bash
Install-Package NeoKolors.Common
```

#### .NET CLI

``` bash
dotnet add package NeoKolors.Common
```

Or, manually reference in a project using `<PackageReference>` in your `.csproj` file:

``` xml
<ItemGroup>
    <!-- ... -->
    <!-- change the version when a newer one is available -->
    <PackageReference Include="NeoKolors.Common" Version="0.25.20"/> 
    <!-- ... -->
</ItemGroup>
```

### Requirements

- **.NET Targets**: `net5.0` and higher (explicit builds also for `net8.0` and `net9.0`) 
- **.NET Standard Targets**: `.NETStandard 2.0` and higher
- **Programming Language**: C# 12.0 or later

---

## Main Classes

### [NKColor](nkcolor)

`NKColor` structure combines the worlds of classic color data and console color data.
It is able to hold the classic 24-bit (+ 8-bit alpha channel) color, one of the predefined 
console colors, the default console color, or it can be in state defining that the color
should be inherited (something like transparent).

### [NKConsoleColor](nkconsolecolor)

Enum `NKConoleColor` contains 256 colors. It basically extends the `System.ConsoleColor` enum 
(although some values do not match). Apart from the basic 16 console colors, it also 
contains a 6x6x6 RGB color cube and 24 grayscale colors.

### [NKStyle](nkstyle)

Structure `NKStyle` contains data for string styling. It compresses 2 `NKColor` instances
(one for text, one for background) and a `TextStyles` instance into a single `long` making
it a pretty optimized data type.

### [TextStyles](textstyles)

Flag enum `TextStyles` contains six basic styles for text: bold, italic, underline, faint, 
negative and strikethrough.

### [NKColorPalette](palette)

Works as a collection of colors. New cohesive color palettes can also be generated using the
`NKColorPalette.GeneratePalette` method.

### NKPalettes 

A collection of palettes

---

## Utilities

### [List2D](util/list2d)

A 2-dimensional dynamically sized array. Also contains useful methods 
for working with 2D arrays like `Resize` or `Fill`. 

### [StringUtils](util/stringutils)

Contains useful methods for working with strings.

### NameConvertor

Converts between different naming conventions, e.g. "hello_world" to "HelloWorld" or "Hello world".

### UInt24

Just a 24-bit unsigned integer.

### Extensions

Some other extension methods.