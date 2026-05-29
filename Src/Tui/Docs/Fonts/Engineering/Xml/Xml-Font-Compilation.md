+++
Written for NeoKolors.Tui.Fonts version 2026.3 (NKFont V3)
by KryKom with love and care :)
+++

# XML Font Compilation

This document describes how the font deserializer handles the deserialization,
linking and combining process.

The compilation follows the stages below:
1. Deserialization
2. Linking
3. Combining
4. Assembling font data

## 1 Deserialization

The deserialization process uses the classes defined in the 
`NeoKolors.Tui.Fonts.Serialization.Xml.V3` namespace. After the data is deserialized
from XML, the glyph contents are loaded and stored alongside their corresponding
glyph definitions. 

## 2 Linking

To link the defined glyphs follow the steps below:

1. create symbol (SyRT) and ID reference tables (IDRT)
2. fill the IDRT with all known IDs and their corresponding glyph definitions
3. fill SyRT with all known symbols and their corresponding glyph definitions
4. use three color DFS algorithm to compile all glyphs

### 2.1 SyRT and IDRT

The symbol and ID reference tables are a way to know what to link to.
The code below is an example definition of the two tables.

```C#
using XmlDefTuple = (XmlGlyphDef Def, string[] lines);

Dictionary<NKGlyphSymbol, (XmlDefTuple Def, NKGlyph? Glyph)> syrt;
Dictionary<string,        (XmlDefTuple Def, NKGlyph? Glyph)> idrt;
```

The tables also contain the glyph data alongside the glyph definition to store 
the compiled glyphs. If the `Glyph` value is null the glyph has not been compiled
yet.

> **Important**: 
> If at any point there are multiple definitions with the same ID/symbol
> the serializer should fail.

### 2.2 Reference resolution

When a compound or auto-compound glyph references a glyph, the serializer
first tries to retrieve the value from the ID table. If the ID does not exist, the 
serializer creates a symbol from the reference letter and the styles from the (auto-)compound 
glyph definition and searches it inside the symbol table. If it fails again, it looks up all
glyphs with the letter in its symbol and matches it to the symbol that has the most matching 
style; if no symbols with the matching letter exist, the serializer fails.

## 3 Combining


## 4 Assembling font data