+++
Written for NeoKolors.Tui.Fonts version 2026.3 (NKFont V3)
by KryKom with love and care :)
+++

# Font Rendering

The actual glyphs in NeoKolors are represented by the `NKGlyph` class.
At the base of the glyph there is a 2D array of cells that can contain either
a character, a foreground value or a background value. These individual cells
are represented by the `NKGlyphCell` struct. By rendering a glyph is meant 
copying the cell grid onto a canvas. 

The rendering process has the following steps:
1. [Tokenization](#1-tokenization)
2. [Measuring](#2-measuring)
3. [Alignment](#3-alignment)
4. [Placement](#4-placing-the-glyph)
5. [Styling](#5-styling-the-letters)

## 1 Tokenization

The tokenization process is described in the [String Tokenization](String-Tokenization.md) document.

## 2 Measuring

The layout computation depends on whether the font is monospaced or has 
variable spacing.

### 2.1 Monospaced fonts

For monospaced fonts the layout is computed by equal increments specified 
in the font configuration. 

For the x-axis (horizontal) the following configurations apply:

- letter width
- letter spacing
- word spacing (mostly the same as letter width but can be changed)

For the y-axis (vertical) the following configurations apply:

- letter height
- leading

The following figure shows a visual representation of the dimensions
taken into account by the font renderer when rendering a monospaced font:

```
     ┌─────────────────────> letter offset
  ┌──┴──┐
  ┏━┓   ┏┓          ┏━╸  ┐
  ┣━┫   ┣┻┓         ┃    ├─> letter height 
 .╹ ╹...┗━┛.........┗━╸. ┘
  └┬┘└┬┘└┬┘└───┬───┘ 
   │  │  │     └───────────> word spacing
   │  │  └─────────────────> letter width
   │  └────────────────────> letter spacing
   └───────────────────────> letter width

  ┏━┓ ┏┓  
  ┣━┫ ┣┻┓
 .╹ ╹.┗━┛. ┐
           │
  ┏━╸      ├──> leading
  ┃        │
 .┗━╸..... ┘

```

### 2.2 Variable fonts

Computation of horizontal letter offsets can be affected by the usage of 
kerning. If kerning is disabled, the value is computed by adding the previous 
letters width to letter spacing (or word spacing if the letters are separated by 
a space character). The following figure shows a visual example of the offset 
computation:

```
        ┌───────────────> letter offset
 ┌──────┴─────┐ 
 .  __  .     __    __
 . /  \ .     \ \  / /
 ./ __ \.     .\ \/ /.
 /_/  \_\     . \__/ .
 └───┬──┘└─┬─┘
     │     └────────────> letter spacing
     └──────────────────> letter width
```

The horizontal offset from the previous letter for fonts using kerning is 
computed by making the horizontal distance between the individual letters' 
contents at least the value specified by letter spacing. By the content of 
a glyph are meant the cells containing a non-background value. 
The following figure shows a visual example of offset computation with kerning
enabled (the dots between the letters symbolize the distance between the letters'
contents):

```
       ┌──────────────> letter offset
 ┌─────┴────┐ 
    __......__    __
   /  \.....\ \  / /
  / __ \.....\ \/ /
 /_/  \_\.....\__/
 └───┬──┘└─┬─┘
     │     └──────────> letter spacing
     └────────────────> letter width
```

## 3 Alignment

In the alignment phase, the widths of the lines and the offsets of the glyphs 
are computed. The following word-wrap options influence the process:
- None - the text is rendered in a single line.
- Hard - the text is broken exactly where the next letter cannot fit
- Word - the text is split in a way its flow stays as natural as possible

## 4 Placement

### 4.1 Translating the cell data

The cell data should be translated as follows:

1. background and foreground cells are replaced by a space character,
2. character cells are copied directly.

### 4.2 Placing the glyph

The glyph is placed so that its baseline aligns with the line's baseline `y`
coordinate. 

Figure 4.1 shows the definition of a glyph for the letter 'y' with its baseline
offset set to -1. Figure 4.2 then shows the placed glyph, the dotted line 
represents the line's baseline.

```
fig. 4.1     |  fig. 4.2
  _  _       |       _  _      
 | || |      |      | || |     
  \_, |      |   . . \_, | . .  
  |__/       |       |__/     
```

## 5 Styling

Text styles can be divided into several groups: 
- coloring (negative, colors) 
- letter glyph exclusive (bold, faint, italic)
- dynamic (blink, invisible)
- underline + strikethrough

### 5.1 Applying coloring styles

The letters are colored by simply applying the colors or the negative style 
in the region of the letter's bounds. 

> Note: The negative style is not applied if the rendered glyph is defined with the 
> style.

> Note: The negative style can also be used as a glyph exclusive style.

### 5.2 Applying glyph exclusive styles

Glyph exclusive styles must be defined for the individual letters as their own glyphs.

> Note: The negative style can also be used as a glyph exclusive style.

### 5.3 Applying dynamic styles

To apply the invisible style, do not render the glyph content.
To apply the blink style, switch between rendering and not rendering the glyph content.
When to render or not to render the glyph content can be determined, for example, by 
the local time (for example, switch every second).

### 5.4 Applying underline and strikethrough

The strikethrough and underline styles are defined as standalone glyphs.
These glyphs are then continuously placed under or above the letter glyphs (before or
after the letter glyphs have been rendered).

> Note: The styles are not applied if the rendered glyph is defined with the
> style.
