# Glyph Properties

## Aligning

Glyphs can be aligned in two ways: using align-points, or an align-direction.

### Align-points

Glyphs can contain align-points that can be used to align two glyphs.
The aligning is done by aligning the glyphs on top of each other in a way
where the points have the same result coordinates.

Consider the following glyphs with the align-points marked with `x`:

```
 x __       |      //
  /  \      |    x  
 / __ \     |    
/_/  \_\    |   
```

When the two glyphs are aligned using the align-points, the result will be 
(with the original align-points):

```
   //
 x __
  /  \
 / __ \
/_/  \_\
```

### Configuration

Configuration of align-points can be done using three properties: `AlignPoints`,
`AlignPointMarkers` and `AlignPointReplace`.

#### AlignPoints

`AlignPoints` offers a direct way to input align-points. It takes input in format
`c(x, y), ...` where `c` is the align-point ID and `x` and `y` are the coordinates.
The align-points are separated by comma.

#### AlignPointMarkers

`AlignPointMarkers` defines what characters in the glyph file should be interpreted as
align-points. Characters in the input string are not separated.

#### AlignPointReplace

`AlignPointReplace` defines how the characters interpreted as align-points in the glyph
file should be erased. It has five types of input. 

There are five options to choose from:
- `forg`
- `bckg`
- `none`
- single character
- replacement character pairs

##### Using `forg`, `bckg` and `none`

`forg` means foreground should replace the characters. `bckg` marks that the character should 
be replaced by background. The third option is `none` which signifies that the align 
point character should not be replaced. 

You can also specify which characters those options apply to by adding a colon after the expression
and inserting a single-quoted c# valid string. For example `forg: 'xyz\u1234'` replaces the characters
`x`, `y` and `z` and the Unicode character with code U+1234 with foreground.

##### Using single characters

When a single character is inputted, that character will replace all align-point characters.
The character can be inputted in the form of the character itself or as any valid c# escape code.

##### Using replacement character pairs

Replacement character pairs define which align-point character will be replaced by which character.
You can insert a character pair by using the following format: `custom: 'a' 'b'`, where `a` is the
character to be replaced and `b` is the character to replace with. You can also input multiple character
pairs at once, either by inserting multiple of those formats separated by a comma (e.g. 
`custom: 'a' 'b', custom: 'c' 'd'`) or by inserting multiple ordered characters inside the quotes
(e.g. `custom: 'ac' 'bd'`).

##### Combining the values

You can combine the formats specified above by inserting a comma between the formats. However,
you cannot combine multiple of `forg`, `bckg` and `none` without the replaced characters specified.
For example, the following value would be valid: `forg: 'a', bckg: 'b', none`, and this one would 
be invalid: `forg: 'a', bckg, none`.

### Align

`Align` property controls how to align two glyphs. You can input an align-point ID
or a pre-defined direction. 

#### Pre-defined Directions

The following four values place the secondary glyph to a side of the main glyph
so that the glyphs are centered around an axis:

- `top`
- `left`
- `bottom`
- `right`
- `center`

The values described in the table below align the corners of the two glyphs. 

| Value                 | Main Glyph Corner | Secondary Glyph Corner |
|-----------------------|-------------------|------------------------|
| `top-left`            | top-left          | top-right              |
| `top-right`           | top-right         | top-left               |
| `left-top`            | top-left          | top-right              |
| `left-bottom`         | bottom-left       | bottom-right           |
| `bottom-left`         | bottom-left       | top-left               |
| `bottom-right`        | bottom-right      | top-right              |
| `right-top`           | top-right         | top-left               |
| `right-bottom`        | bottom-right      | bottom-right           |
| `corner-top-left`     | top-left          | bottom-right           |
| `corener-top-right`   | top-right         | bottom-left            |
| `corener-bottom-left` | bottom-left       | top-right              |
| `corner-bottom-right` | bottom-right      | top-left               |

The values' alignment can be visualized using the diagram below.

```
    corner-top-left  .  top-left        top       top-right  .  corner-top-right
                     .                                       .
 . . . . . . . . . . +---------------------------------------+ . . . . . . . . 
           left-top  |                                       |  right-top
                     |                                       |
               left  |                 center                |  right
                     |              (main glyph)             | 
        left-bottom  |                                       |  right-bottom
 . . . . . . . . . . +---------------------------------------+ . . . . . . . . .
                     .                                       .
 corner-bottom-left  .  bottom-left   bottom   bottom-right  .  corner-bottom-right
```

The value `custom: x` can be used to align the glyphs using an align-point
whose ID is specified after the colon (marked as `x` in the example).

## Masking

Masking defines how a glyph's background will be rendered. It can be set through
the `Mask` property.

The `Mask` property can have the following values:
- `bckg` - all space characters are interpreted as the background
- `forg` - all space characters are interpreted as the foreground
- `space` - all space characters are interpreted as spaces
- `custom-bckg: 'x'` - all characters matching the character placeheld by `x` and
  surrounded by single quotes are interpreted as the background (multiple characters 
  are allowed)
- `custom-forg: 'x'` - all characters matching the character placeheld by `x` and
  surrounded by single quotes are interpreted as the foreground (multiple characters
  are allowed)

You can also combine the values. In that case, comma separates the formats 
described above. For example `custom-forg: 'xyz', custom-bckg: 'abc', bckg` uses the 
characters `x`, `y` and `z` as foreground, the characters `a`, `b` and `c` as
foreground and spaces as background. Note that the `bckg` and `forg` values
cannot be combined.

**Note**: To input a single quote simply add `\'`. The backward slash also works with
other c# escapes and Unicode and hexadecimal codes.

## Baseline

The `Baseline` property defines the vertical offset of a glyph to the baseline.
If the value is positive, the glyph is moved up (above the baseline). If the value 
is negative, the glyph is moved down (below the baseline).

### Example

Consider the following glyph:

```
  (_)
  | |
 _| |
|__/
```
The `Baseline` property is set to `0` in the case of fig. 1, and to `-1` in the
case of fig. 2. The dotted line represents the baseline.

```
1.            .  2.
      (_)     .
      | |     .      (_)
     _| |     .      | |
....|__/............_| |....
              .    |__/
              .
```

## Styles 

To set the styles the glyph uses, use the `Styles` property. You can use the following 
values:
- `bold`
- `italic`
- `negative` 
- `faint`
- `underline`
- `strikethrough`

You can also combine the values by separating them with a comma, e.g. `bold, italic`. 

**Note**:
The `negative`, `faint`, `underline` and `strikethrough` are handled automatically by 
the renderer by default, though the rendering can be customized. 