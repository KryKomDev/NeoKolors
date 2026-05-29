// NeoKolors
// Copyright (c) krystof 2026

using static NeoKolors.Tui.Fonts.FontProportionsInfo;
using VarCfg  = NeoKolors.Tui.Fonts.NKFontVariableConfig;
using MonoCfg = NeoKolors.Tui.Fonts.NKFontMonospacedConfig;

namespace NeoKolors.Tui.Fonts;

public readonly record struct NKFontInfo {
    
    
    public string         Name           { get; init; }
    public bool           Ligatures      { get; init; }
    public int            Leading        { get; init; }
    public int            LetterSpacing  { get; init; }
    public int            WordSpacing    { get; init; }
    public FontProportionsInfo FontPropoInfo { get; init; }
    public ProportionType ProportionType => FontPropoInfo.Type;

    public NKFontInfo(
        string name,
        bool   ligatures,
        int    leading,
        int    letterSpacing,
        int    wordSpacing,
        VarCfg variable)
    {
        Name           = name;
        Ligatures      = ligatures;
        Leading        = leading;
        LetterSpacing  = letterSpacing;
        WordSpacing    = wordSpacing;
        FontPropoInfo = new FontProportionsInfo(variable);
    }
    
    public NKFontInfo(
        string  name,
        bool    ligatures,
        int     leading,
        int     letterSpacing,
        int     wordSpacing,
        MonoCfg monospaced)
    {
        Name           = name;
        Ligatures      = ligatures;
        Leading        = leading;
        LetterSpacing  = letterSpacing;
        WordSpacing    = wordSpacing;
        FontPropoInfo = new FontProportionsInfo(monospaced);
    }

    public override string ToString() => 
        $"NKFontInfo {{ " +
        $"Name: '{Name}', " +
        $"Ligatures: {Ligatures}, " +
        $"Leading: {Leading}, " +
        $"LetterSpacing: {LetterSpacing}, " +
        $"WordSpacing: {WordSpacing}, " +
        $"Proportions: {ProportionType switch {
            ProportionType.VARIABLE   => FontPropoInfo.AsVar .ToString(), 
            ProportionType.MONOSPACED => FontPropoInfo.AsMono.ToString(), 
            _                         => throw new IOp()
        }} " +
        $"}}";
}