+++
Written for NeoKolors.Tui.Fonts version 2026.3 (NKFont V3)
by KryKom with love and care :)
+++

# String Tokenization

This document describes how the font renderer tokenizes text and resolves
the tokens to individual glyphs.

## 1 Tokens

The V3 tokenizer has the following token types:

- Letters (single characters, e.g. `a`, `.`, `1`...)
- Ligatures (multiple characters, e.g. `->`, `!=`...)
- Spaces 
- Newlines
- Unknown

> **Todo**: 
> Add support for tabs.

> **Note**:
> Besides from the token contents, the styles of the characters are also 
> stored if the source string is a styled string.

### 1.1 Resolution order

The tokenizer resolves the tokens in this order:

1. Newlines
2. Spaces
3. Ligatures
4. Letters

### 1.2 Newlines

The newline tokens can span the line feed character (`\n`) but also the line feed and 
carriage return combination (`\r\n`). The tokenizer, however, produces the same token for
both variants.

### 1.3 Spaces

Spaces tokens are created so that the token spans through all neighboring space characters.
The count of the neighboring space characters is stored inside the token.

> **Note:**
> If there is a style change in a string of consecutive space characters, 
> the tokenizer creates multiple tokens for every style change.

### 1.4 Ligatures

Ligatures are resolved so that the longest ligatures are prioritized before the shorter ones.
For example, the string `!==` would be resolved with the ligature spanning three characters 
(`!==`) instead of the one spanning only two (`!=`) 

> **Note:**
> If there is a style change inside a ligature string, the tokenizer will
> not produce a ligature token for that string.

### 1.5 Letters

The letter tokens are created from characters not recognized as any other token.

## 2 Symbol resolution

The tokenizer first tries to match the symbol exactly. If it fails, it tries to find the 
closest match. It finds all symbols with the same text value and then counts how many 
styles are the same. The symbol with the most matching styles is chosen. If no symbol
with the same exact string value is found, the tokenizer returns the Unknown token.

> **Note:**
> Colors are not accounted for during the symbol resolution process; only 
> individual text styles are compared.