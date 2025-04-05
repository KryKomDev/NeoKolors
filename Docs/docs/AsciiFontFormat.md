# Ascii Font Formats

This document describes the supported ascii font formats.

## NeoKolors Ascii Font

NeoKolors Ascii Font is a custom font file format from NeoKolors, usually with file
extension `.nkaf`.

### Header {id="k_header"}

**Contents:**
1. file format version (e.g. `nkaf 1`)
2. [unknown glyph mode](#unknown-glyph-modes) 
3. letter spacing, word spacing and line spacing
4. the glyph start marker
5. the glyph end marker
6. [whitespace mode](#whitespace-mode)

**For example:**

```
nkaf 1 
glyph X 
1 2 1 
## 
@@ 
mask . 
```

#### Unknown Glyph Modes

Sets the substitute characters for the missing characters.

**Modes:**
1. `default` - uses the character itself as a substitute
2. `glyph _` - uses the glyph at `_` as the substitute (e.g. `glyph A` uses the glyph for A)
3. `skip` - skips the character

#### Whitespace Mode

Determines how will whitespaces affect font rendering.

**Modes:**
1. `transparent` - whitespaces do not replace characters below the glyph
2. `overlap` - whitespaces replace characters below glyph
3. `mask _` - character at `_` is treated like a whitespace and replaces 
   characters below the glyph, other whitespaces are transparent

### Glyph Definitions {id="k_character-definitions"}

**Contents:**
1. the glyph start marker and the character itself
2. X and Y offset of the glyph
3. the glyph itself
4. glyph end marker

### Examples

<tabs>
<tab id="windows-install" title="Simple A">
<code-block>
## A
0 0
▄▄▄
█▄█
█.█
@@
</code-block>
</tab>
<tab id="macos-install" title="Italic A">
<code-block>
## A
8 5
0 0
    ___
   /...|
  /./|.|
 /.___.|
/_/  |_|
@@
</code-block>
</tab>
<tab id="linux-install" title="Linux">
        How to install on Linux.
</tab>
</tabs>

The header from example above is applied to this example. 
The dots represent non-transparent spaces.

### Compound Glyph Definition

**Contents:**
1. the glyph start marker and the character itself
2. the `compound` keyword
3. the two glyphs to mix together
4. glyph end marker 

---

## FIGLet Fonts

### Header {id="f_font"}

**Contents:**
1. version (e.g. `flf2a`)
2. newline marker
3. character height

For example:

```
flf2a $ 6 10 2 1 1 0 10 32 127
```

Let's break down what some of these numbers and symbols might mean (don't worry, you 
don't need to memorize all of them!):

* **`flf2a`**: This is like the name of the recipe format. It tells the computer, "Hey,
  this is an FLF file!" The `2a` might mean a specific version of the format.
* **`$`**: This is a special character that often marks the end of a line in the font's design.
* **`6`**: This might tell you how many lines of characters are used to make up each letter.
* **`10`**: This could be the height of the characters in the font (how tall they are).
* **The other numbers**: These numbers give more details about the font, like how much 
  space is between characters, how wide they are, and other technical stuff.

### Character Definitions: {id="f_character-definitions"}

After the header, the file contains the actual "recipes" for each character (like the 
letter "A", the number "1", etc.). Each character has its own section in the file.

These sections are made up of lines of text that use the regular keyboard characters
to draw the shape of the letter.

In a real FLF file, you would see many more lines of these characters, and they would 
be more detailed to make the letters look nicer. Each character would be separated from 
the next in the file.

### End Marker:

At the very end of the FLF file, there's usually a special marker to tell the computer 
that it has reached the end of the font information. This is often just a line with a 
special character or a blank line.
