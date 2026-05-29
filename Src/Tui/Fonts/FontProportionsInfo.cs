// NeoKolors
// Copyright (c) krystof 2026

using HasFlagExtension;
using OneOf;
using VarCfg  = NeoKolors.Tui.Fonts.NKFontVariableConfig;
using MonoCfg = NeoKolors.Tui.Fonts.NKFontMonospacedConfig;

namespace NeoKolors.Tui.Fonts;

public record FontProportionsInfo {
    
    [Flags]
    [HasFlagPrefix("Is")]
    [FlagGroup("Kerning", "Uses")]
    public enum ProportionType {
        MONOSPACED = 0,
        VARIABLE   = 1,

        [ExcludeFlag]
        [FlagGroup("Kerning")]
        KERNING    = 2
    }

    public ProportionType Type { get; private init; }

    public OneOf<VarCfg, MonoCfg> Info {
        get;
        init {
            field = value;
            Type  = GetProportionType(value);
        }
    }

    public VarCfg  AsVar  => Type.GetIsVariable()   ? Info.AsT0 : throw new IOp();
    public MonoCfg AsMono => Type.GetIsMonospaced() ? Info.AsT1 : throw new IOp();

    private static ProportionType GetProportionType(OneOf<VarCfg, MonoCfg> info) {
        return info.IsT0
            ? ProportionType.VARIABLE | (info.AsT0.Kerning ? ProportionType.KERNING : 0)
            : ProportionType.MONOSPACED;
    }

    public FontProportionsInfo(MonoCfg monospaced) {
        Info = monospaced;
        Type = ProportionType.MONOSPACED;
    }

    public FontProportionsInfo(VarCfg variable) {
        Info = variable;
        Type = ProportionType.VARIABLE | (variable.Kerning ? ProportionType.KERNING : 0);
    }
    
    public FontProportionsInfo(OneOf<VarCfg, MonoCfg> info) {
        Info = info;
        Type = GetProportionType(info.AsT0);
    }
}