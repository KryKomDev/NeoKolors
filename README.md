<div align="center">

<h1>NeoKolors</h1>

[![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)](https://github.com/KryKomDev/NeoKolors/blob/main/LICENSE)
[![Latest Stable](https://img.shields.io/github/v/release/KryKomDev/NeoKolors?sort=semver&style=for-the-badge&label=Latest%20Stable&labelColor=2a313c&color=e051c6)](https://img.shields.io/github/v/release/KryKomDev/NeoKolors?sort=semver&style=for-the-badge&label=Latest%20Stable&labelColor=2a313c&color=e051c6)
[![Latest](https://img.shields.io/github/v/release/KryKomDev/NeoKolors?include_prereleases&sort=semver&style=for-the-badge&label=Latest&labelColor=2a313c&color=e051c6)](https://img.shields.io/github/v/release/KryKomDev/NeoKolors?include_prereleases&sort=semver&style=for-the-badge&label=Latest&labelColor=2a313c&color=e051c6)
[![Commits per month](https://img.shields.io/github/commit-activity/m/KryKomDev/NeoKolors/main?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://img.shields.io/github/commit-activity/m/KryKomDev/NeoKolors/main?style=for-the-badge&labelColor=%232a313c&color=%2358a6ff)

</div>

## About

NeoKolors is a C# library ecosystem with a number of color utilities, console graphics 
classes, and a modern, web-inspired Text User Interface (TUI) framework.

---

## Status

[![Build & Test](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/ci.yml?branch=main&style=for-the-badge&label=Build%20%26%20Test&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/ci.yml)
[![Documentation](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/docs.yml?branch=main&style=for-the-badge&label=Docs&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/docs.yml)
[![Release](https://img.shields.io/github/actions/workflow/status/KryKomDev/NeoKolors/release.yml?style=for-the-badge&label=Release&labelColor=%232a313c)](https://github.com/KryKomDev/NeoKolors/actions/workflows/release.yml)

---

## NeoKolors.Common

[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Common?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Common) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Common?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Common) 

NeoKolors.Common contains basic common tools, structures, and classes for working 
with colors and ANSI escape sequences. It also offers some other helpful utilities, 
optimized primitives, and Sixel graphics support.

---

## NeoKolors.Extensions

[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Extensions?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Extensions) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Extensions?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Extensions) 

NeoKolors.Extensions is a high-performance productivity library providing utility 
extensions for standard .NET and C# types. It includes word wrapping (`Chop`), plain 
text visual length calculators, collection helpers, and programmable name casing 
transformations (`NamingCase`).

---

## NeoKolors.Console

[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Console?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Console) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Console?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Console) 

NeoKolors.Console offers a fully configurable logger with the option of coloring the 
messages when printing to console. It also offers automatic unhandled exception formatting. 
One of the most basic features is output text styling.

### Logs
![Logs](Docs/static/images/logs.png)

### Exceptions
![Exception](Docs/static/images/exception.png)

---

## NeoKolors.Settings

[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Settings?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Settings) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Settings?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Settings) 

NeoKolors.Settings is a framework for transferring configurations and automation of 
instance-from-settings creation. It offers a simple syntax to create solid settings 
configuration options.

```c# 
var builder = SettingsBuilder<Result>.Build("c-builder",
    SettingsNode<Result>.New("default")
        .Argument("field", Arguments.Integer(-10, 10))
        .Constructs(context => new Result((int)context["field"].Get()))
);

var result = builder.GetResult();
```

---

## NeoKolors.Tui

[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui) 

NeoKolors.Tui is a framework for creating highly customizable TUI's in console emulators. 
It is made to be as similar as possible to web development.

Picture below is an example of a Ncurses-style like Tui application.

![Tui Application](Docs/static/images/tui.png)

### Presentations

NeoKolors.Tui framework can also be used to create interactive presentations loaded from 
Markdown files. This feature is, however, still in the works.

> [!IMPORTANT]
> NeoKolors.Tui is still in early development. It can be unstable or have unexpected behavior. 
> Use at your own risk.

---

## NeoKolors.Tui.Fonts

[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Fonts?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Fonts?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts) 

NeoKolors.Tui.Fonts is the core font engine for the NeoKolors TUI framework. It provides 
parsing, serializing, measuring, and rendering capabilities for stylized ASCII and ANSI fonts.

---

## NeoKolors.Tui.Fonts.Assets

[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tui.Fonts.Assets?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts.Assets) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tui.Fonts.Assets?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tui.Fonts.Assets) 

NeoKolors.Tui.Fonts.Assets contains the default built-in fonts (`Bytesized`, 
`Future`, and `Dummy`) and the MSBuild compilation pipeline task to package 
them as embedded resources.

---

## NeoKolors.Tools.NKFont

[![NuGet Version](https://img.shields.io/nuget/v/NeoKolors.Tools.NKFont?style=for-the-badge&labelColor=%232a313c&color=%23e051c6)](https://www.nuget.org/packages/NeoKolors.Tools.NKFont) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/NeoKolors.Tools.NKFont?style=for-the-badge&labelColor=%232a313c&color=%23d69a00)](https://www.nuget.org/packages/NeoKolors.Tools.NKFont) 

NKFontTool (`nkfont`) is a command-line interface (CLI) tool designed to manage, 
compile, display, inspect, and create custom TUI fonts for the NeoKolors framework.

---

## Installation

To use one of NeoKolors' packages, add a reference to the project in your solution. 
You can use one of the following methods:

### NuGet CLI

```bash {{ copy = true }}
Install-Package NeoKolors.Common
Install-Package NeoKolors.Extensions
Install-Package NeoKolors.Console
Install-Package NeoKolors.Tui
Install-Package NeoKolors.Tui.Fonts
Install-Package NeoKolors.Tui.Fonts.Assets
```

### .NET CLI

```bash
dotnet add package NeoKolors.Common
dotnet add package NeoKolors.Extensions
dotnet add package NeoKolors.Console
dotnet add package NeoKolors.Tui
dotnet add package NeoKolors.Tui.Fonts
dotnet add package NeoKolors.Tui.Fonts.Assets
```

### Installing the NKFont CLI Tool

```bash
dotnet tool install --global NeoKolors.Tools.NKFont
```

### Manual Reference

Or, manually reference in a project using `<PackageReference>` in your `.csproj` file:

```xml
<ItemGroup>
    <!-- ... -->
    <!-- change the version when a newer one is available -->
    <PackageReference Include="NeoKolors.Common" Version="*"/>
    <PackageReference Include="NeoKolors.Extensions" Version="*"/>
    <PackageReference Include="NeoKolors.Console" Version="*"/>
    <PackageReference Include="NeoKolors.Tui" Version="*"/>
    <PackageReference Include="NeoKolors.Tui.Fonts" Version="*"/>
    <PackageReference Include="NeoKolors.Tui.Fonts.Assets" Version="*"/>
    <!-- ... -->
</ItemGroup>
```

> [!NOTE]
> You do not need to copy all the lines. Select only the packages you want to import.

```mermaid
flowchart TD
        subgraph Triggers
            PR[Pull Request]
            PushMain[Push to main]
            PushTag[Push Tag v*.*.*]
            Manual[workflow_dispatch]
        end

        subgraph CI["1. CI & Validation (ci.yml)"]
            TestMatrix["Build & Test Matrix<br/>(Linux + Windows/macOS sanity)"]
            DocsLint["DocFX Validation (Dry-run)"]
            TestReport["GitHub Step Summary & Test Report"]
        end

        subgraph Docs["2. Docs Deployment (docs.yml)"]
            BuildDocs["DocFX Build (API & Guides)"]
            DeployPages["Deploy to GitHub Pages"]
        end

        subgraph ReleaseAndPublish["3. Release & Publish (release.yml)"]
            PackRelease["Build & Pack NuGet (.nupkg + .snupkg)"]
            CreateGHRelease["GitHub Release (Auto-Changelog)"]
            PushNuGet["Publish to NuGet.org (OIDC / API Key)"]
        end

        PR --> CI
        PushMain --> CI
        PushMain --> Docs
        PushTag --> ReleaseAndPublish
        ReleaseAndPublish --> Docs
        Manual --> CI
        Manual --> ReleaseAndPublish
```
