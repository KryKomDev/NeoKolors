+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'NeoKolors.Console'
+++

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-89b4fa?style=for-the-badge&labelColor=6c7086)
![.NET 5](https://img.shields.io/badge/.NET-5.0-cba6f7?style=for-the-badge&labelColor=6c7086)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Console?color=f5c2e7&style=for-the-badge&labelColor=6c7086)](https://www.nuget.org/packages/NeoKolors.Console)
![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Console?color=a6e3a1&style=for-the-badge&labelColor=6c7086)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build-console.yml?style=for-the-badge&labelColor=%236c7086&color=%23f9e2af)
![GitHub License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%236c7086&color=%23fab387)

NeoKolors.Console offers a full logger and utilities for working 
with colors in console. It also offers automatic exception formatter.

---

## Features
- **Logger**: NeoKolors.Console offers a highly configurable and easy to use logger
- **Console utilities**: console string coloring, styling, writing tables and more
- **Configurability**: everything is fully configurable
- **Easy to use**: everything is ready for use out of the box, no configuration needed

---

## Getting Started

### Installation

To use `NeoKolors.Console`, add a reference to the project in your solution. 
You can use one of the following methods:

#### NuGet CLI

``` bash
Install-Package NeoKolors.Console
```

#### .NET CLI

``` bash
dotnet add package NeoKolors.Console
```

Or, manually reference in a project using `<PackageReference>` in your `.csproj` file:

``` xml
<ItemGroup>
    <!-- ... -->
    <!-- change the version when a newer one is available -->
    <PackageReference Include="NeoKolors.Console" Version="0.25.20"/> 
    <!-- ... -->
</ItemGroup>
```

### Requirements

- **.NET Targets**: `net5.0` and higher (explicit builds also for `net8.0` and `net9.0`)
- **.NET Standard Targets**: `.NETStandard 2.0` and higher
- **Programming Language**: C# 12.0 or later

---

## Main classes

### [NKConsole](nkconsole)

Static class offering colored and styled output messages. 
It also offers input methods for some basic types.

### [NKDebug](nkdebug)

Static class containing a global static instance of `NKLogger` and `ExceptionFormatter`.
It also manages automatic unhandled exception formatting.

### [NKLogger](nklogger) 

Class with all the logger functionalities. 
It can be fully configured with `LoggerConfig`.

### [ExceptionFormatter](exceptionformatter)

A class for formatting exceptions.
Configuration can be managed using `ExceptionFormat`.