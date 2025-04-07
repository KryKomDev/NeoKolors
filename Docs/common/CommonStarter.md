# Welcome to NeoKolors.Common

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-512bd4)
![.NET 5](https://img.shields.io/badge/.NET-5.0-682a7b)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Common?color=a53c7a)](https://www.nuget.org/packages/NeoKolors.Common)
![Downloads](https://img.shields.io/nuget/dt/NeoKolors.Common?color=a31c35)

Offers some utilities for working with colors and basic ansi string manipulation.

## NKColor

Color structure.
Handy when working with custom and palette colors in console.
See [](NKColor.md) for more info.

## ColorFormat

Offers methods for casting between different color formats
(e.g.
[System.Drawing.Color](https://learn.microsoft.com/cs-cz/dotnet/api/system.drawing.color?view=net-8.0), 
[SKColor](https://learn.microsoft.com/en-us/dotnet/api/skiasharp.skcolor?view=skiasharp-2.88),
int as hex, rgb)

## ColorPalette

Holds a color palette. Contains a method for random palette generation.

## EscapeCodes

Contains ANSI escape sequences.

## StringEffects

Methods for applying ANSI escape sequences to strings.

## NKStyle

Structure that holds all ANSI styles of a string (foreground and background color, 
bold, italic, strikethrough, faint, underline, inverse).

## NKConsoleColor

The extended color palette for console.

## UInt24

An unsigned 24 bit integer.