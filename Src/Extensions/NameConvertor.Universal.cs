// NeoKolors
// Copyright (c) 2025 KryKom


namespace NeoKolors.Extensions;


public static partial class NameConvertor {

    public static string Convert(this string s, NamingCase source, NamingCase target) => 
        FromTokens(Tokenize(s, source), target);

    private static string[] Tokenize(string s, NamingCase source) {
        return source switch {
            NamingCase.CAMEL => TokenizeCamel(s),
            NamingCase.PASCAL => TokenizePascal(s),
            NamingCase.SNAKE => TokenizeSnake(s),
            NamingCase.SCREAMING_SNAKE => TokenizeScreamingSnake(s),
            NamingCase.KEBAB => TokenizeKebab(s),
            NamingCase.SPACED_CAMEL => TokenizeSpacedCamel(s),
            NamingCase.TRAIN => TokenizeTrain(s),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    private static string FromTokens(string[] tokens, NamingCase target) {
        return target switch {
            NamingCase.CAMEL => FromTokensCamel(tokens),
            NamingCase.PASCAL => FromTokensPascal(tokens),
            NamingCase.SNAKE => FromTokensSnake(tokens),
            NamingCase.SCREAMING_SNAKE => FromTokensScreamingSnake(tokens),
            NamingCase.KEBAB => FromTokensKebab(tokens),
            NamingCase.SPACED_CAMEL => FromTokensSpacedCamel(tokens),
            NamingCase.TRAIN => FromTokensTrain(tokens),
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };
    }

    private static string[] TokenizeCamel(string str) {
        List<string> tokens = [];
      
        string currentToken = "";


        foreach (var c in str) {
            if (char.IsUpper(c)) {
                tokens.Add(currentToken);
                currentToken = "";
            }
          
            currentToken += char.ToLower(c);
        }
      
        tokens.Add(currentToken);
        return tokens.ToArray();
    }

    private static string[] TokenizePascal(string str) {
        List<string> tokens = [];
        string currentToken = "";


        foreach (var c in str) {
            if (char.IsUpper(c) && currentToken.Length > 0) {
                tokens.Add(currentToken);
                currentToken = "";
            }
          
            currentToken += char.ToLower(c);
        }
      
        if (currentToken.Length > 0) {
            tokens.Add(currentToken);
        }
      
        return tokens.ToArray();
    }
    
    private static string[] TokenizeSnake(string str) {
        return str.Split('_')
            .Select(s => s.ToLower())
            .Where(s => s.Length > 0)
            .ToArray();
    }

    private static string[] TokenizeScreamingSnake(string str) {
        return str.Split('_')
            .Select(s => s.ToLower())
            .Where(s => s.Length > 0)
            .ToArray();
    }
    
    private static string[] TokenizeKebab(string str) {
        return str.Split('-')
            .Select(s => s.ToLower())
            .Where(s => s.Length > 0)
            .ToArray();
    }

    private static string[] TokenizeSpacedCamel(string str) {
        return str.Split(' ')
            .Select(s => s.ToLower())
            .Where(s => s.Length > 0)
            .ToArray();
    }

    private static string[] TokenizeTrain(string str) {
        return str.Split('-')
            .Select(s => s.ToLower())
            .Where(s => s.Length > 0)
            .ToArray();
    }

    private static string FromTokensCamel(string[] tokens) {
        if (tokens.Length == 0) return string.Empty;
      
        string result = tokens[0].ToLower();
      
        for (int i = 1; i < tokens.Length; i++) {
            if (tokens[i].Length > 0) {
                result += char.ToUpper(tokens[i][0]) + tokens[i].Substring(1).ToLower();
            }
        }
      
        return result;
    }

    private static string FromTokensPascal(string[] tokens) {
        if (tokens.Length == 0) return string.Empty;
      
        string result = "";
      
        foreach (var token in tokens) {
            if (token.Length > 0) {
                result += char.ToUpper(token[0]) + token.Substring(1).ToLower();
            }
        }
      
        return result;
    }

    private static string FromTokensSnake(string[] tokens) {
        return string.Join("_", tokens.Select(t => t.ToLower()));
    }

    private static string FromTokensScreamingSnake(string[] tokens) {
        return string.Join("_", tokens.Select(t => t.ToUpper()));
    }

    private static string FromTokensKebab(string[] tokens) {
        return string.Join("-", tokens.Select(t => t.ToLower()));
    }

    private static string FromTokensSpacedCamel(string[] tokens) {
        if (tokens.Length == 0) return string.Empty;
      
        string result = tokens[0].ToLower();
      
        for (int i = 1; i < tokens.Length; i++) {
            if (tokens[i].Length > 0) {
                result += " " + char.ToUpper(tokens[i][0]) + tokens[i].Substring(1).ToLower();
            }
        }
      
        return result;
    }

    private static string FromTokensTrain(string[] tokens) {
        return string.Join("-", tokens.Select(t => t.ToUpper()));
    }
}