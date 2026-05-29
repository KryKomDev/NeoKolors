# AnsiString

The `AnsiString` class provides a way of storing styled strings.
The class is immutable.

## 1 Style encoding

The styles are stored as markers, e.g., a list of start indices of 
the styles and the styles themselves. The optimal data structure is 
evaluated to be the `List` as it allows for builtin binary search.
The markers are boundary-based and do not store the length of their 
span.

## 2 Parsing

The parsed string can contain markers in the format `{:<somestyle>}` 
where `<somestyle>` can be a combination of the following:
- `b` - bold
- `i` - italic
- `u` - underline
- `f` - faint
- `l` - blink
- `n` - negative
- `v` - invisible
- `s` - strikethrough

Or it can contain a color marker in the format `{:f#<somecolor>}`
for text color or `{:b#<somecolor>}` for background color where 
`<somecolor>` can be either any element of the `NKConsoleColor` 
enum in the original or kebab case or a hexadecimal representation 
of an RGB color. The following examples show some color markers:
- `{:b#red}`
- `{:f#012345}`
- `{:b#dark-cyan}`

> **Note:**
> When trying to convert a preset color string call `Replace('-', '_')`
> and `ToUpper` on the section so the parser doesn't have to check the case.  

It is also possible to escape the markers using the double curly bracket
(`{{`, `}}`) syntax.