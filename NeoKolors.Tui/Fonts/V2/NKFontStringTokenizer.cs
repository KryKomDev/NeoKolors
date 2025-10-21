// NeoKolors
// Copyright (c) 2025 KryKom

using System.Text.RegularExpressions;
using NeoKolors.Extensions;

using TokenData = OneOf.OneOf<
    NeoKolors.Tui.Fonts.V2.NKFontStringTokenizer.SimpleToken,
    NeoKolors.Tui.Fonts.V2.NKFontStringTokenizer.LigatureToken, 
    NeoKolors.Tui.Fonts.V2.NKFontStringTokenizer.AutoCompoundToken
>?;

namespace NeoKolors.Tui.Fonts.V2;

/// <summary>
/// Provides functionality to tokenize a string into glyph-based tokens such as simple, ligature,
/// and auto-compound glyphs, based on a specified instance of <see cref="NKFont"/>.
/// </summary>
public static class NKFontStringTokenizer {

    public static Token[] Tokenize(string str, NKFont font) {
        var tokens = new List<Token>();
        int i = 0;
        
        str = Regex.Replace(str, @"\s+\n", "\n");

        while (i < str.Length) {
            
            // skip spaces
            if (str[i] == ' ') {
                tokens.Add(Token.Space());
                i += 1;
                continue;
            }

            // skip newlines
            if (str[i] == '\n') {
                tokens.Add(Token.Newline());
                i += 1;
                continue;
            }
            
            Token? token = null;
            int add = 0;
            
            // try to create a ligature, if possible. If not, try to create an auto-compound glyph from
            // the two next characters. If that is not possible, try to find a matching simple glyph.
            // If not possible, normalize the character and create an auto compound glyph from the letter
            // and diacritics. If not possible, add an invalid character token.
            
            // ligature detection

            for (int j = Math.Min(str.Length - i, font.MaxLigatureLength); j >= 0; j--) {
                string candidate = str.Substring(i, j);

                if (!font.HasLigature(candidate)) continue;
                
                token = Token.Ligature(candidate);
                add = j;
                break;
            }

            if (token != null) {
                tokens.Add(token.Value);
                i += add;
                continue;
            }
            
            // auto-compound detection
            
            if (i != str.Length - 1) {
                char st = str[i];
                char nd = str[i + 1];
                bool success = false;
                
                if (font.HasAutoCompound(nd, st, false)) {
                    token = Token.AutoCompound(st, nd, false);
                    success = true;
                }
                else if (font.HasAutoCompound(st, nd, true)) {
                    token = Token.AutoCompound(st, nd, true);
                    success = true;
                }

                if (success) {
                    tokens.Add(token!.Value);
                    i += 2;
                    continue;
                }
            }
            
            // simple glyph detection

            if (font.HasSimple(str[i])) {
                token = Token.Simple(str[i]);
                tokens.Add(token.Value);
                i += 1;
                continue;
            }
            
            // split character into two tokens
            
            var split = char.SeparateDiacritics(str[i]);

            if (split.Diacritics is not null) {
                var st = split.BaseChar;
                var nd = split.Diacritics.Value;
                bool success = false;
                
                if (font.HasAutoCompound(nd, st, false)) {
                    token = Token.AutoCompound(st, nd, false);
                    success = true;
                }
                else if (font.HasAutoCompound(st, nd, true)) {
                    token = Token.AutoCompound(st, nd, true);
                    success = true;
                }

                if (!success) continue;
                
                tokens.Add(token!.Value);
                i += 1;
                continue;
            }
            
            tokens.Add(Token.Invalid());
            i += 1;
        }
        
        return tokens.ToArray();
    }
    
    // what is the best way I can split a string into tokens that represent simple, ligature and auto-compound glyphs?
    // public static Token[] TokenizeS(string str, NKFont font) {
    //     var tokens = new List<Token>();
    //     int i = 0;
    //
    //     while (i < str.Length) {
    //         Token? token = null;
    //         int maxLength = 0;
    //
    //         // Try to find the longest matching sequence starting at position i
    //         // Check ligatures first (they usually have priority)
    //         for (int length = Math.Min(str.Length - i, GetMaxLigatureLength(font)); length > 0; length--) {
    //             string candidate = str.Substring(i, length);
    //
    //             if (font.HasLigature(candidate)) {
    //                 token = new Token(candidate, TokenType.LIGATURE);
    //                 maxLength = length;
    //                 break;
    //             }
    //         }
    //
    //         // If no ligature found, check auto-compound sequences
    //         if (token == null) {
    //             for (int length = Math.Min(str.Length - i, 2); length > 0; length--) {
    //                 string candidate = str.Substring(i, length);
    //
    //                 if (font.HasAutoCompound(candidate[1], ' ')) {
    //                     token = new Token(candidate, TokenType.AUTO_COMPOUND);
    //                     maxLength = length;
    //                     break;
    //                 }
    //             }
    //         }
    //
    //         // If no special sequence found, treat as simple glyph
    //         if (token == null) {
    //             token = new Token(str[i].ToString(), TokenType.SIMPLE);
    //             maxLength = 1;
    //         }
    //
    //         tokens.Add(token.Value);
    //         i += maxLength;
    //     }
    //
    //     return tokens.ToArray();
    // }

    public readonly struct Token {
        
        public TokenData TokenData { get; }
        public TokenType Type { get; }
        
        internal Token(TokenData data, TokenType type) {
            TokenData = data;
            Type = type;
        }
        
        public static Token Invalid() => new(null, TokenType.INVALID);
        public static Token Simple(char content) => new(new SimpleToken(content), TokenType.SIMPLE);
        public static Token Ligature(string content) => new(new LigatureToken(content), TokenType.LIGATURE);
        public static Token AutoCompound(char main, char nd, bool mainFirst) 
            => new(new AutoCompoundToken(main, nd, mainFirst), TokenType.AUTO_COMPOUND);
        public static Token Space() => new(null, TokenType.SPACE);
        public static Token Newline() => new(null, TokenType.NEWLINE);
    }

    public readonly struct SimpleToken {
        public char Character { get; }
        
        internal SimpleToken(char character) {
            Character = character;
        }
    }
    
    public readonly struct LigatureToken {
        public string Ligature { get; }
        
        internal LigatureToken(string ligature) {
            Ligature = ligature;
        }
    }
    
    public readonly struct AutoCompoundToken {
        public char Main { get; }
        public char Second { get; }
        public bool MainFirst { get; }
        
        internal AutoCompoundToken(char first, char second, bool mainFirst) {
            Main = first;
            Second = second;
            MainFirst = mainFirst;
        }
    }
    
    public enum TokenType : byte {
        INVALID = 0,
        SIMPLE = 1,
        LIGATURE = 2,
        AUTO_COMPOUND = 3,
        SPACE = 4,
        NEWLINE = 5,
    }
}