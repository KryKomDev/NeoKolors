// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using HasFlagExtension;
using NeoKolors.Extensions;

namespace NeoKolors.Tui.Fonts.Serialization.Xml.V3;

public abstract class XmlAutoCompoundApplicable {
    public abstract HashSet<char> ApplicableChars { get; }
}

public class XmlAutoCompoundApplicableGroup : XmlAutoCompoundApplicable {

    [XmlIgnore]
    public ApplicableGroups Group { get; set; }

    [XmlIgnore]
    public override HashSet<char> ApplicableChars {
        get {
            var chars = new HashSet<char>();
            
            if (Group.GetHasBasicVowelsUpper())        chars.UnionWith(char.BasicVowelsUpper); 
            if (Group.GetHasBasicVowelsLower())        chars.UnionWith(char.BasicVowelsLower); 
            if (Group.GetHasExtendedVowelsUpper())     chars.UnionWith(char.ExtendedVowelsUpper); 
            if (Group.GetHasExtendedVowelsLower())     chars.UnionWith(char.ExtendedVowelsLower);
            if (Group.GetHasBasicConsonantsUpper())    chars.UnionWith(char.BasicConsonantsUpper); 
            if (Group.GetHasBasicConsonantsLower())    chars.UnionWith(char.BasicConsonantsLower); 
            if (Group.GetHasExtendedConsonantsUpper()) chars.UnionWith(char.ExtendedConsonantsUpper); 
            if (Group.GetHasExtendedConsonantsLower()) chars.UnionWith(char.ExtendedConsonantsLower);
            if (Group.GetHasNumbers())                 chars.UnionWith(char.Digits);
            
            return chars;
        }
    }

    [XmlText]
    public string GroupString {
        get => Group.ToString();
        set => Group = TryParse(value, out var group) ? group.Value : ApplicableGroups.NONE;
    }

    public XmlAutoCompoundApplicableGroup() {
        Group = ApplicableGroups.NONE;
    }
    
    public XmlAutoCompoundApplicableGroup(ApplicableGroups group) {
        Group = group;
    }

    public static bool TryParse(string s, [NotNullWhen(true)] out ApplicableGroups? output) {
        if (string.IsNullOrEmpty(s)) {
            NKFontSerializer.LOGGER.Error("The value for ApplicableGroup is empty or null.");
            output = null;
            return false;
        }

        if (Enum.TryParse<ApplicableGroups>(s.Replace('-', '_'), true, out var result)) {
            output = result;
            return true;
        }

        output = null;
        return false;
    }

    [Flags]
    public enum ApplicableGroups {
        [ExcludeFlag] NONE = 0,
        
        BASIC_VOWELS_UPPER        = 1 << 0,
        BASIC_VOWELS_LOWER        = 1 << 1,
        EXTENDED_VOWELS_UPPER     = 1 << 2,
        EXTENDED_VOWELS_LOWER     = 1 << 3,
        BASIC_CONSONANTS_UPPER    = 1 << 4,
        BASIC_CONSONANTS_LOWER    = 1 << 5,
        EXTENDED_CONSONANTS_UPPER = 1 << 6,
        EXTENDED_CONSONANTS_LOWER = 1 << 7,
        NUMBERS                   = 1 << 8,
        
        BASIC_VOWELS    = BASIC_VOWELS_UPPER    | BASIC_VOWELS_LOWER,
        EXTENDED_VOWELS = EXTENDED_VOWELS_UPPER | EXTENDED_VOWELS_LOWER,
        VOWELS_UPPER    = BASIC_VOWELS_UPPER    | EXTENDED_VOWELS_UPPER,
        VOWELS_LOWER    = BASIC_VOWELS_LOWER    | EXTENDED_VOWELS_LOWER,
        VOWELS          = VOWELS_LOWER          | VOWELS_UPPER,
        
        BASIC_CONSONANTS    = BASIC_CONSONANTS_UPPER    | BASIC_CONSONANTS_LOWER,
        EXTENDED_CONSONANTS = EXTENDED_CONSONANTS_UPPER | EXTENDED_CONSONANTS_LOWER,
        CONSONANTS_UPPER    = BASIC_CONSONANTS_UPPER    | EXTENDED_CONSONANTS_UPPER,
        CONSONANTS_LOWER    = BASIC_CONSONANTS_LOWER    | EXTENDED_CONSONANTS_LOWER,
        CONSONANTS          = CONSONANTS_LOWER          | CONSONANTS_UPPER,
        
        LETTERS_UPPER = VOWELS_UPPER  | CONSONANTS_UPPER,
        LETTERS_LOWER = VOWELS_LOWER  | CONSONANTS_LOWER,
        LETTERS       = LETTERS_LOWER | LETTERS_UPPER,
    }

}

public class XmlAutoCompoundApplicableChars : XmlAutoCompoundApplicable {

    [XmlIgnore]
    public override HashSet<char> ApplicableChars => Chars.ToHashSet();

    [XmlText]
    public string Chars { get; set; }

    public XmlAutoCompoundApplicableChars() {
        Chars = string.Empty;
    }
}