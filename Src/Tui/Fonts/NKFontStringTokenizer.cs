// NeoKolors
// Copyright (c) 2026 KryKom

using TokenData = OneOf.OneOf<
    NeoKolors.Tui.Fonts.NKFontStringTokenizer.SimpleToken,
    NeoKolors.Tui.Fonts.NKFontStringTokenizer.LigatureToken, 
    NeoKolors.Tui.Fonts.NKFontStringTokenizer.AutoCompoundToken,
    NeoKolors.Tui.Fonts.NKFontStringTokenizer.SpaceToken
>?;

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// Provides functionality to tokenize a string into glyph-based tokens such as simple, ligature,
/// and auto-compound glyphs, based on a specified instance of <see cref="NKFont"/>.
/// </summary>
public static class NKFontStringTokenizer {

    public static Token[] Tokenize(AnsiString str, NKFont font) {
        var tokens = new List<Token>();
        int i = 0;
        
        var chars = str.ToArray();

        while (i < chars.Length) {
            
            // skip spaces
            int spaces = 0;

            for (int j = i; j < chars.Length; j++) {
                if (chars[j].Char != ' ') break;
                spaces += 1;
            }
            
            if (spaces > 0) {
                tokens.Add(Token.Space(spaces, chars[i].Style));
                i += spaces;
                continue;
            }

            // skip newlines
            if (chars[i].Char == '\n') {
                tokens.Add(Token.Newline(chars[i].Style));
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

            for (int j = Math.Min(chars.Length - i, font.MaxLigatureLength); j >= 0; j--) {
                var candidateChars = chars[i..(i + j)];
                var candidate = new string(candidateChars.Select(c => c.Char).ToArray());

                if (!font.HasLigature(candidate)) continue;
                
                token = Token.Ligature(candidate, chars[i].Style);
                add = j;
                break;
            }

            if (token != null) {
                tokens.Add(token.Value);
                i += add;
                continue;
            }
            
            // auto-compound detection
            
            if (i != chars.Length - 1) {
                char st = chars[i].Char;
                char nd = chars[i + 1].Char;
                bool success = false;
                
                if (font.HasAutoCompound(nd, st, false)) {
                    token = Token.AutoCompound(st, nd, false, chars[i].Style);
                    success = true;
                }
                else if (font.HasAutoCompound(st, nd, true)) {
                    token = Token.AutoCompound(st, nd, true, chars[i].Style);
                    success = true;
                }

                if (success) {
                    tokens.Add(token!.Value);
                    i += 2;
                    continue;
                }
            }
            
            // simple glyph detection

            if (font.HasSimple(chars[i].Char)) {
                token = Token.Simple(chars[i].Char, chars[i].Style);
                tokens.Add(token.Value);
                i += 1;
                continue;
            }
            
            // split character into two tokens
            
            var split = char.SeparateDiacritics(chars[i].Char);

            if (split.Diacritics is not null) {
                var st = split.BaseChar;
                var nd = split.Diacritics.Value;
                bool success = false;
                
                if (font.HasAutoCompound(nd, st, false)) {
                    token = Token.AutoCompound(st, nd, false, chars[i].Style);
                    success = true;
                }
                else if (font.HasAutoCompound(st, nd, true)) {
                    token = Token.AutoCompound(st, nd, true, chars[i].Style);
                    success = true;
                }

                if (success) {
                    tokens.Add(token!.Value);
                    i += 1;
                    continue;
                }
                
                // fallback to simple glyph if auto-compound fails
                if (font.HasSimple(st)) {
                    tokens.Add(Token.Simple(st, chars[i].Style));
                    i += 1;
                    continue;
                }
            }
            
            tokens.Add(Token.Invalid(chars[i].Style));
            i += 1;
        }
        
        return tokens.ToArray();
    }

    public static Token[] Tokenize(string str, NKFont font) => Tokenize(new AnsiString(str), font);

    public readonly struct Token {
        
        public TokenData Data { get; }
        public TokenType Type { get; }
        public NKStyle   Style { get; }
        
        internal Token(TokenData data, TokenType type, NKStyle style = default) {
            Data = data;
            Type = type;
            Style = style;
        }
        
        public static Token Invalid(NKStyle style = default) => new(null, TokenType.INVALID, style);
        public static Token Simple(char content, NKStyle style = default) => new(new SimpleToken(content), TokenType.SIMPLE, style);
        public static Token Ligature(string content, NKStyle style = default) => new(new LigatureToken(content), TokenType.LIGATURE, style);
        public static Token AutoCompound(char main, char nd, bool mainFirst, NKStyle style = default) 
            => new(new AutoCompoundToken(main, nd, mainFirst), TokenType.AUTO_COMPOUND, style);
        public static Token Space(int width, NKStyle style = default) => new(new SpaceToken(width), TokenType.SPACE, style);
        public static Token Newline(NKStyle style = default) => new(null, TokenType.NEWLINE, style);

        public override string ToString() {
            return Type switch {
                TokenType.INVALID => "Invalid",
                TokenType.LIGATURE => Data!.Value.AsT1.Ligature,
                TokenType.SIMPLE => Data!.Value.AsT0.Character.ToString(),
                TokenType.AUTO_COMPOUND => Data!.Value.AsT2.Main.ToString() + Data!.Value.AsT2.Second,
                TokenType.SPACE => "Space",
                TokenType.NEWLINE => "Newline",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public SimpleToken AsSimple() 
            => Data is { IsT0: true } ? Data.Value.AsT0 : throw new InvalidOperationException();
        
        public LigatureToken AsLigature()
            => Data is { IsT1: true} ? Data.Value.AsT1 : throw new InvalidOperationException();
        
        public AutoCompoundToken AsAutoCompound()
            => Data is { IsT2: true } ? Data.Value.AsT2 : throw new InvalidOperationException();
        
        public SpaceToken AsSpace()
            => Data is { IsT3: true } ? Data.Value.AsT3 : throw new InvalidOperationException();
    }

    public readonly struct SimpleToken {
        public char Character { get; }
        
        internal SimpleToken(char character) {
            Character = character;
        }
    }
    
    public readonly struct LigatureToken {
        public string Ligature { get; }
        public int Length => Ligature.Length;
        
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
    
    public readonly struct SpaceToken {
        public int Width { get; }
        
        internal SpaceToken(int width) {
            Width = width;
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