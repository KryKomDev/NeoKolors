# Font Configuration

## Elements

| Name            | Description                                                                  | Required                         |
|-----------------|------------------------------------------------------------------------------|----------------------------------|
| `Name`          | The name of the font.                                                        | True                             |
| `LetterSpacing` | An integer specifying the spacing between letters.                           | True                             |
| `Leading`       | An integer specifying the vertical distance between baselines of text.       | True                             |
| `Ligatures`     | A boolean specifying whether the font uses ligatures.                        | False (default is true)          |
| `IntrinsicBold` | A boolean specifying whether the font renders the bold style instrinsically. | False (default is false)         |
| `Monospaced`    | Configuration of the monospaced font. Cannot be combined with `Variable`.    | True (if not using `Variable`)   |
| `Variable`      | Configuration of the variable font. Cannot be combined with `Monospaced`.    | True (if not using `Monospaced`) |
| `Defaults`      | Default configuration of the font.                                           | False                            |

### Name 

The name of the font in a string value satisfying the regex: `(\w|\.|_|-|)+`.

### LetterSpacing

An integer specifying the spacing between letters. When the font is monospaced
or variable with `Kerning` set to false, the letter spacing is computed between the
rectangular bounds of the glyphs. When the font is variable and `Kerning` is set to
true, the letter spacing is computed as the minimal horizontal distance between
the glyph characters.

### Leading

An integer specifying the vertical distance between baselines of text.

### Ligatures

A boolean specifying whether the font uses ligatures.

### IntrinsicBold

If `true` the font renderer applies the effect intrinsically, that means the bold 
style is applied to the glyph characters directly. If `false` the font uses custom
glyphs for bold characters.

### Monospaced

The `Monospaced` element specifies the font is monospaced.

| Name           | Description                                    | Required                 |
|----------------|------------------------------------------------|--------------------------|
| `LetterWidth`  | The width of a letter.                         | True                     |
| `LetterHeight` | The height of a letter.                        | True                     |
| `AlignToGrid`  | If true, the glyphs will be aligned to a grid. | False (default is false) |

#### LetterWidth

An integer specifying the width of characters.

#### LetterHeight

An integer specifying the height of characters.

#### AlignToGrid

If true, the glyphs will be aligned to a grid.

### Variable

The `Variable` element specifies the font is variable.

| Name          | Description                                      | Required                |
|---------------|--------------------------------------------------|-------------------------|
| `WordSpacing` | An integer specifying the spacing between words. | True                    |
| `Kerning`     | If true, the font renderer will use kerning.     | False (default is true) |

#### Kerning

If true, the font renderer will use kerning.

#### WordSpacing

An integer specifying the spacing between words.

### Defaults

The `Defaults` element contains default configuration for glyph definitions.

| Name                | Description                                             | Required |
|---------------------|---------------------------------------------------------|----------|
| `Align`             | The default value for the `Align` property.             | False    |
| `Mask`              | The default value for the `Mask` property.              | False    |
| `AlignPointReplace` | The default value for the `AlignPointReplace` property. | False    |
| `AlignPointMarkers`   | The default value for the `AlignPointMarkers` property.   | False    |

#### Align

The default value for the `Align` property of [Compound](Font-Mapping.md) and
[Auto-Compound](Font-Mapping.md) glyphs.

#### Mask

The default value for the `Mask` property. For more see: 
[Mask](Glyph-Properties.md).

#### AlignPointReplace

The default value for the `AlignPointReplace` property. For more 
see: [AlignPointReplace](Glyph-Properties.md).

#### AlignPointMarkers

The default value for the `AlignPointMarkers` property. For more see:
[AlignPointMarkers](Glyph-Properties.md).
