//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Common.Util;
using NeoKolors.Tui.Exceptions;
using OneOf;

namespace NeoKolors.Tui.Fonts;

public readonly struct GlyphDistribution {
    public OneOf<DistroStrings, char>[] Distribution { get; }

    public char this[int index] => GetChars()[index];
    
    public string GetChars() {
        string chars = "";
        foreach (var d in Distribution) {
            chars += d.Match(s => s switch {
                    DistroStrings.UABC => "ABCDEFGHIJKLMNOPQRSTUVWXYZ", 
                    DistroStrings.LABC => "abcdefghijklmnopqrstuvwxyz",
                    DistroStrings.DIA => "´ˇ¨ ̊",
                    DistroStrings.DIG => "0123456789",
                    DistroStrings.SPC => "!\"#$%&'()*+,-./\\:;<=>?@^_`{|}~",
                    _ => throw new ArgumentOutOfRangeException()
                }, 
                c => c + ""
            );
        }

        return chars;
    }
    
    public GlyphDistribution(OneOf<DistroStrings, char>[] distribution) {
        Distribution = distribution;
    }

    public GlyphDistribution(string distribution) {
        string[] split = distribution.Split(' ');
        List<OneOf<DistroStrings, char>> list = new();

        foreach (string s in split) {
            if (s.Length == 1) {
                list.Add(s[0]);
            } 
            else switch (s.ToUpperInvariant()) {
                case "UABC":
                    list.Add(DistroStrings.UABC);
                    break;
                case "LABC":
                    list.Add(DistroStrings.LABC);
                    break;
                case "DIG":
                    list.Add(DistroStrings.DIG);
                    break;
                case "SPC":
                    list.Add(DistroStrings.SPC);
                    break;
                case "DIA":
                    list.Add(DistroStrings.DIA);
                    break;
                default:
                    throw FontReaderException.InvalidGlyphDistribution(s);
            }
        }
        
        Distribution = list.ToArray();
    }

    public GlyphDistribution() {
        Distribution = [
            DistroStrings.UABC,
            DistroStrings.LABC, 
            DistroStrings.DIG,
            DistroStrings.SPC,
            DistroStrings.DIA
        ];
    }
    
    public override string ToString() => 
        Distribution.Join(" ", 
            d => d.Match(
                s => Enum.GetName(typeof(DistroStrings), s)!.ToLowerInvariant(),
                c => c.ToString()
            )
        );

    public int GetGlyphCount() {
        int length = 0;
        
        foreach (var d in Distribution) {
            length += d.Match(
                s => s switch {
                    DistroStrings.UABC => 26,
                    DistroStrings.LABC => 26,
                    DistroStrings.DIG => 10,
                    DistroStrings.SPC => 30,
                    DistroStrings.DIA => 4,
                    _ => throw new ArgumentException("invalid distribution string")
                },
                _ => 1
            );
        }
        
        return length;
    }
    
    public int GlyphCount => GetGlyphCount();
}

public enum DistroStrings {
    
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