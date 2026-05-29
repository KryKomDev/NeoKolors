# Font Mapping

Font mapping is done in a font mappings file.

## Component Glyphs

Defines a glyph that uses a text file as a source of its visual content and
represents a single character.

### Properties

| Name                | Description                                                                                                             | Required                             |
|---------------------|-------------------------------------------------------------------------------------------------------------------------|--------------------------------------|
| `Symbol`            | Single character specifying the meaning of the glyph.                                                                   | True                                 |
| `File`              | The path to the source file of the glyph.                                                                               | True                                 |
| `Id`                | The ID of the glyph.                                                                                                    | False (default is value of `Symbol`) |
| `AlignPoints`       | Specifies a set of align-points. For more see: [Aligning](Xml-Glyph-Definition-Properties.md)                           | False (default is empty list)        |
| `AlignPointMarkers` | Specifies a set of characters interpreted as align-points. For more see: [Aligning](Xml-Glyph-Definition-Properties.md) | False (default is empty list)        |
| `AlignPointReplace` | Defines how will the align-points be replaced. For more see: [Aligning](Xml-Glyph-Definition-Properties.md)             | False (default is `bckg`)            |
| `Mask`              | Defines how the glyph will be masked. For more see: [Masking](Xml-Glyph-Definition-Properties.md)                       | False (default is `bckg`)            |
| `Baseline`          | Defines the vertical offset of the glyph. For more see: [Baseline](Xml-Glyph-Definition-Properties.md)                  | False (default is `0`)               |
| `Styles`            | Defines what styles does the glyph use. For more see: [Styles](Xml-Glyph-Definition-Properties.md)                      | False (default is `none`)            |

### Example Definition

```xml
<Component 
    Symbol="A"
    File="uppercase/A-bold.nkg"
    Id="A" 
    AlignPoints="x(1, 2), y(4, 5)"
    AlignPointMarkers="abc"
    AlignPointReplace="bckg"
    Mask="bckg"
    Baseline="0"
    Styles="bold"
/>
```

## Ligature Glyphs

Defines a glyph that uses a text file as a source of its visual content and
represents multiple characters.

### Properties

| Name                | Description                                                                                                             | Required                              |
|---------------------|-------------------------------------------------------------------------------------------------------------------------|---------------------------------------|
| `Symbol`            | String specifying the meaning of the glyph.                                                                             | True                                  |
| `File`              | The path to the source file of the glyph.                                                                               | True                                  |
| `Id`                | The ID of the glyph.                                                                                                    | False (defaults to value of `Symbol`) |
| `AlignPoints`       | Specifies a set of align points. For more see: [Aligning](Xml-Glyph-Definition-Properties.md)                           | False (default is empty list)         |
| `AlignPointMarkers` | Specifies a set of characters interpreted as align points. For more see: [Aligning](Xml-Glyph-Definition-Properties.md) | False (default is empty list)         |
| `AlignPointReplace` | Defines how will the align points be replaced. For more see: [Aligning](Xml-Glyph-Definition-Properties.md)             | False (default is `bckg`)             |
| `Mask`              | Defines how the glyph will be masked. For more see: [Masking](Xml-Glyph-Definition-Properties.md)                       | False (default is `bckg`)             |
| `Baseline`          | Defines the vertical offset of the glyph. For more see: [Baseline](Xml-Glyph-Definition-Properties.md)                  | False (default is `0`)                |
| `Styles`            | Defines what styles does the glyph use. For more see: [Styles](Xml-Glyph-Definition-Properties.md)                      | False (default is `none`)             |

### Example Definition

```xml
<Ligature 
    Symbol="->"
    File="ligatures/arrow-right-short.nkg"
    Id="arrow-right-short" 
    Mask="bckg"
    Baseline="0"
/>
```

## Compound Glyphs

Defines a glyph that uses two combined glyphs as a source of its visual content and
represents simple characters.

### Properties

| Name          | Description                                                                                              | Required                              |
|---------------|----------------------------------------------------------------------------------------------------------|---------------------------------------|
| `Symbol`      | Single character specifying the meaning of the glyph.                                                    | True                                  |
| `Main`        | ID of the main glyph.                                                                                    | True                                  |
| `Secondary`   | ID of the secondary glyph.                                                                               | True                                  |
| `Align`       | Defines how the base glyphs should be aligned. For more see: [Align](Xml-Glyph-Definition-Properties.md) | True                                  |
| `Id`          | The ID of the glyph.                                                                                     | False (defaults to value of `Symbol`) |
| `AlignPoints` | Specifies a set of align points. For more see: [Aligning](Xml-Glyph-Definition-Properties.md)            | False (default is empty list)         |
| `Styles`      | Defines what styles does the glyph use. For more see: [Styles](Xml-Glyph-Definition-Properties.md)       | False (default is `none`)             |

### Example Definition

```xml
<Compound
    Symbol="ü"
    Main="u"
    Secondary="¨"
    Align="top"
/>
```

## Auto-Compound Glyphs

Defines a family of glyphs that use two combined glyphs as a source of their visual content.
The glyphs are automatically constructed from a specified glyph and glyphs of a group of characters
that are specified to be applicable to the `Symbol` character. 

### Properties

| Name                 | Description                                                                                              | Required                      |
|----------------------|----------------------------------------------------------------------------------------------------------|-------------------------------|
| `Symbol`             | Single character specifying one of the components of the letter represented by the glyph.                | True                          |
| `Base`               | ID of one of the base glyphs.                                                                            | True                          |
| `Align`              | Defines how the base glyphs should be aligned. For more see: [Align](Xml-Glyph-Definition-Properties.md) | True                          |
| `IsBaseMain`         | Defines whether the specified base glyph is the main glyph.                                              | True                          |
| `SecondaryAfterMain` | `true`, if the secondary characters comes after the base character (defined in `Symbol`).                | False (default is `false`)    |
| `AlignPoints`        | Specifies a set of align points. For more see: [Aligning](Xml-Glyph-Definition-Properties.md)            | False (default is empty list) |
| `Styles`             | Defines what styles does the glyph use. For more see: [Styles](Xml-Glyph-Definition-Properties.md)       | False (default is `none`)     |

### Sub-elements

| Name              | Description                                                                    |
|-------------------|--------------------------------------------------------------------------------|
| `ApplicableChars` | Defines a set of secondary characters that can be used to create the glyph.    |
| `ApplicableGroup` | Selects a group of applicable characters that can be used to create the glyph. |

#### ApplicableGroup Values

| Name                        | Description                                                 |
|-----------------------------|-------------------------------------------------------------|
| `basic-vowels-upper`        | Uppercase basic vowels (A, E, I, O, U, Y).                  |
| `basic-vowels-lower`        | Lowercase basic vowels (a, e, i, o, u, y).                  |
| `extended-vowels-upper`     | Uppercase accented vowels (e.g., Á, É, Í).                  |
| `extended-vowels-lower`     | Lowercase accented vowels (e.g., á, é, í).                  |
| `basic-consonants-upper`    | Uppercase basic consonants (B, C, D, F, etc.).              |
| `basic-consonants-lower`    | Lowercase basic consonants (b, c, d, f, etc.).              |
| `extended-consonants-upper` | Uppercase accented consonants (e.g., Č, Ř, Š).              |
| `extended-consonants-lower` | Lowercase accented consonants (e.g., č, ř, š).              |
| `numbers`                   | Digits from 0 to 9.                                         |
| `basic-vowels`              | Both uppercase and lowercase basic vowels.                  |
| `extended-vowels`           | Both uppercase and lowercase accented vowels.               |
| `vowels-upper`              | All uppercase vowels (basic and extended).                  |
| `vowels-lower`              | All lowercase vowels (basic and extended).                  |
| `vowels`                    | All vowels (uppercase, lowercase, basic, and extended).     |
| `basic-consonants`          | Both uppercase and lowercase basic consonants.              |
| `extended-consonants`       | Both uppercase and lowercase accented consonants.           |
| `consonants-upper`          | All uppercase consonants (basic and extended).              |
| `consonants-lower`          | All lowercase consonants (basic and extended).              |
| `consonants`                | All consonants (uppercase, lowercase, basic, and extended). |
| `letters-upper`             | All uppercase letters (vowels and consonants).              |
| `letters-lower`             | All lowercase letters (vowels and consonants).              |
| `letters`                   | All letters (vowels, consonants, upper, and lower).         |


### Example Definitions

```xml
<AutoCompound Base="ˇ" Align="top" IsBaseMain="false" BaseAfterSecondary="false">
    <ApplicableChars>cdenrszCDENRSZ</ApplicableChars>
</AutoCompound>

<AutoCompound Base="´" Align="top" IsBaseMain="false" BaseAfterSecondary="false">
    <ApplicableGroup>vowels</ApplicableGroup>
</AutoCompound>
```

## Strikethrough Glyph

Defines the underline glyph used by the font renderer to create the strikethrough effect.

### Properties

| Name           | Description                                                                                                                                    | Required                   |
|----------------|------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------|
| `File`         | The path to the source file of the glyph.                                                                                                      | True                       |
| `Baseline`     | Defines the vertical offset of the glyph. For more see: [Baseline](Xml-Glyph-Definition-Properties.md)                                         | False (default is `0`)     |
| `Mask`         | Defines how the glyph will be masked. For more see: [Masking](Xml-Glyph-Definition-Properties.md)                                              | False (default is `bckg`)  |
| `AboveLetters` | Defines whether the effect glyph will be placed above or below letter glyphs. For more see: [Font Rendering](../Engineering/Font-Rendering.md) | False (default is `false`) |

### Example Definition

```xml
<Strikethrough File="effects/strikethrough.nkg"/>
```

## Underline Glyph

Defines the underline glyph used by the font renderer to create the underline effect.

### Properties

| Name           | Description                                                                                                                                    | Required                   |
|----------------|------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------|
| `File`         | The path to the source file of the glyph.                                                                                                      | True                       |
| `Baseline`     | Defines the vertical offset of the glyph. For more see: [Baseline](Xml-Glyph-Definition-Properties.md)                                         | False (default is `0`)     |
| `Mask`         | Defines how the glyph will be masked. For more see: [Masking](Xml-Glyph-Definition-Properties.md)                                              | False (default is `bckg`)  |
| `AboveLetters` | Defines whether the effect glyph will be placed above or below letter glyphs. For more see: [Font Rendering](../Engineering/Font-Rendering.md) | False (default is `false`) |

### Example Definition

```xml
<Underline File="effects/underline.nkg"/>
```

## Notes

