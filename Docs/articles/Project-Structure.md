# Project Structure

This document describes the solution structure.

```mermaid
flowchart TD
    e[Extensions]
    m[Common]
    n[Console]
    t[Tui]
    tc[Tui.Core]
    tf[Tui.Fonts]
    tfa[Tui.Fonts.Assets]
    tg[Tui.Generators]
    
    m --> e
    n --> e
    n --> m
    tc --> n
    tc --> m
    tc --> e
    tf --> tc
```