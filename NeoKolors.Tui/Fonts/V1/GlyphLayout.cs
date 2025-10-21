//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Fonts.V1;

public readonly struct GlyphLayout {
    public OneOf<LayoutStrings, char>[] Distribution { get; }

    public char this[int index] => GetChars()[index];
    
    public string GetChars() {
        string chars = "";
        foreach (var d in Distribution) {
            chars += d.Match(s => s switch {
                    LayoutStrings.UABC => "ABCDEFGHIJKLMNOPQRSTUVWXYZ", 
                    LayoutStrings.LABC => "abcdefghijklmnopqrstuvwxyz",
                    LayoutStrings.DIA => "´ˇ¨ ̊",
                    LayoutStrings.DIG => "0123456789",
                    LayoutStrings.SPC => "!\"#$%&'()*+,-./\\:;<=>?@^_`{|}~",
                    _ => throw new ArgumentOutOfRangeException()
                }, 
                c => c + ""
            );
        }

        return chars;
    }
    
    public GlyphLayout(params OneOf<LayoutStrings, char>[] distribution) {
        Distribution = distribution;
    }

    public GlyphLayout(string distribution) {
        string[] split = distribution.Split(' ');
        List<OneOf<LayoutStrings, char>> list = new();

        foreach (string s in split) {
            if (s.Length == 1) {
                list.Add(s[0]);
            } 
            else switch (s.ToUpperInvariant()) {
                case "UABC":
                    list.Add(LayoutStrings.UABC);
                    break;
                case "LABC":
                    list.Add(LayoutStrings.LABC);
                    break;
                case "DIG":
                    list.Add(LayoutStrings.DIG);
                    break;
                case "SPC":
                    list.Add(LayoutStrings.SPC);
                    break;
                case "DIA":
                    list.Add(LayoutStrings.DIA);
                    break;
                default:
                    throw FontReaderException.InvalidGlyphDistribution(s);
            }
        }
        
        Distribution = list.ToArray();
    }

    public GlyphLayout() {
        Distribution = [
            LayoutStrings.UABC,
            LayoutStrings.LABC, 
            LayoutStrings.DIG,
            LayoutStrings.SPC,
            LayoutStrings.DIA
        ];
    }
    
    public override string ToString() => 
        Distribution.Join(" ", 
            d => d.Match(
                s => Enum.GetName(typeof(LayoutStrings), s)!.ToLowerInvariant(),
                c => c.ToString()
            )
        );

    public int GetGlyphCount() {
        int length = 0;
        
        foreach (var d in Distribution) {
            length += d.Match(
                s => s switch {
                    LayoutStrings.UABC => 26,
                    LayoutStrings.LABC => 26,
                    LayoutStrings.DIG => 10,
                    LayoutStrings.SPC => 30,
                    LayoutStrings.DIA => 4,
                    _ => throw new ArgumentException("invalid distribution string")
                },
                _ => 1
            );
        }
        
        return length;
    }
    
    public int GlyphCount => GetGlyphCount();
    
    public static implicit operator GlyphLayout(string s) => new(s);
}

public enum LayoutStrings {
    
    /// <summary>
    /// uppercase English alphabet characters
    /// </summary>
    UABC,
    
    /// <summary>
    /// lowercase English alphabet characters
    /// </summary>
    LABC,
    
    /// <summary>
    /// 0 - 9 characters
    /// </summary>
    DIG,
    
    /// <summary>
    /// special characters (! " # $ % &amp; &#39; ( ) * + , - . / : &#59; &lt; = &gt; ? @ ^ _ ` { | } ~)
    /// </summary>
    SPC,
    
    /// <summary>
    /// diacritics characters (´, ˇ, ¨, `) 
    /// </summary>
    DIA
}