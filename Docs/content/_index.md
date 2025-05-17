+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'Home'
+++

# Welcome to the official NeoKolors Documentation

![GitHub License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%236c7086&color=%23fab387)

NeoKolors is a C# library with a number of color utilities and console graphics classes.

---

## NeoKolors.Common

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-89b4fa?style=for-the-badge&labelColor=6c7086)
![.NET 5](https://img.shields.io/badge/.NET-5.0-cba6f7?style=for-the-badge&labelColor=6c7086)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Common?color=f5c2e7&style=for-the-badge&labelColor=6c7086)](https://www.nuget.org/packages/NeoKolors.Common)
![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Common?color=a6e3a1&style=for-the-badge&labelColor=6c7086)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build-common.yml?style=for-the-badge&labelColor=%236c7086&color=%23f9e2af)

NeoKolors.Common library contains basic common tools, structures and classes
for working with colors and ANSI escape sequences. It also offers some other
helpful utilities.

---

## NeoKolors.Console

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-89b4fa?style=for-the-badge&labelColor=6c7086)
![.NET 5](https://img.shields.io/badge/.NET-5.0-cba6f7?style=for-the-badge&labelColor=6c7086)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Console?color=f5c2e7&style=for-the-badge&labelColor=6c7086)](https://www.nuget.org/packages/NeoKolors.Common)
![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Console?color=a6e3a1&style=for-the-badge&labelColor=6c7086)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build-console.yml?style=for-the-badge&labelColor=%236c7086&color=%23f9e2af)

NeoKolors.Console offers a fully configurable logger with the option of 
coloring the messages when printing to console. It also offers automatic 
unhandled exception formatting. One of the most basic features is output text styling.

### Logs
![Logs](images/logs.png)

### Exceptions
![Exception](images/exception.png)

---

## NeoKolors.Settings

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-89b4fa?style=for-the-badge&labelColor=6c7086)
![.NET 5](https://img.shields.io/badge/.NET-5.0-cba6f7?style=for-the-badge&labelColor=6c7086)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Settings?color=f5c2e7&style=for-the-badge&labelColor=6c7086)](https://www.nuget.org/packages/NeoKolors.Common)
![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Settings?color=a6e3a1&style=for-the-badge&labelColor=6c7086)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build-settings.yml?style=for-the-badge&labelColor=%236c7086&color=%23f9e2af)

NeoKolors.Settings is a framework for transferring configurations and automation
of instance-from-settings creation.
It offers a simple syntax to create solid settings configuration options.

```c# 
var builder = SettingsBuidler<Result>.Build("c-builder",
    SettingsNode<Result>.New("default")
        .Argument("field", Arguments.Integer(-10, 10))
        .Constructs(context => new Result((int)context["field"].Get()))
);

var result = builder.GetResult();
```

---

## NeoKolors.Tui

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.1-89b4fa?style=for-the-badge&labelColor=6c7086)
![.NET 5](https://img.shields.io/badge/.NET-5.0-cba6f7?style=for-the-badge&labelColor=6c7086)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Tui?color=f5c2e7&style=for-the-badge&labelColor=6c7086)](https://www.nuget.org/packages/NeoKolors.Common)
![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui?color=a6e3a1&style=for-the-badge&labelColor=6c7086)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/build-tui.yml?style=for-the-badge&labelColor=%236c7086&color=%23f9e2af)

NeoKolors.Tui is a framework for creating highly customizable TUI's in console emulators.
It is made to be as similar as possible to web development. 

Picture below is an example of a Ncurses-style like Tui application.

![Tui Application](images/tui.png)

### Presentations

NeoKolors.Tui framework can also be used to create interactive presentations loaded
from Markdown files.

This feature is, however, still in the works.

> [!IMPORTANT]
> NeoKolors.Tui is still in early development. It can be unstable or have unexpected
> behavior. Use at your own risk.

---

## Installation

To use one of NeoKolors' packages, add a reference to the project in your solution.
You can use one of the following methods:

### NuGet CLI

```bash {{ copy = true }}
Install-Package NeoKolors.Common
Install-Package NeoKolors.Console
Install-Package NeoKolors.Settings
Install-Package NeoKolors.Tui
```

### .NET CLI

```bash
dotnet add package NeoKolors.Common
dotnet add package NeoKolors.Console
dotnet add package NeoKolors.Settings
dotnet add package NeoKolors.Tui
```

Or, manually reference in a project using `<PackageReference>` in your `.csproj` file:

``` xml
<ItemGroup>
    <!-- ... -->
    <!-- change the version when a newer one is available -->
    <PackageReference Include="NeoKolors.Common" Version="0.25.20"/>
    <PackageReference Include="NeoKolors.Console" Version="0.25.20"/>
    <PackageReference Include="NeoKolors.Settings" Version="0.25.4"/>
    <PackageReference Include="NeoKolors.Tui" Version="0.25.19-alpha"/>
    <!-- ... -->
</ItemGroup>
```

> [!NOTE]
> You do not need to copy all the lines. Select only the packages you want to import.