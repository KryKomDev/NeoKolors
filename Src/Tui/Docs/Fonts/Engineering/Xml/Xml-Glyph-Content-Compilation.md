+++
Written for NeoKolors.Tui.Fonts version 2026.3 (NKFont V3)
by KryKom with love and care :)
+++

# Glyph Content Compilation

Glyphs are compiled in the following steps:

1. align point marker detection
2. masking
3. size reduction

## 1 Align point marker detection

The align-point markers are detected and then replaced by the
character or cell specified for the marker.

## 2 Masking 

The masking step replaces the characters with the characters or cells
specified in the [`Mask`](Xml-Glyph-Definition-Properties.md#masking) 
property.

## 3 Size reduction

Size is reduced by removing rows and columns on the sides of the glyph
containing only the background cell.