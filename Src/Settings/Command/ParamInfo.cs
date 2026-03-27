//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using OneOf;

namespace NeoKolors.Settings.Command;

public readonly struct ParamInfo {
    
    public OneOf<ArgumentInfo, FlagInfo, SwitchInfo, NodeInfo> Info { get; }
    
    public ParamInfo(OneOf<ArgumentInfo, FlagInfo, SwitchInfo, NodeInfo> info) {
        Info = info;
    }

    public override string ToString() => Info.Match(
        a => a.ToString(),
        f => f.ToString(),
        s => s.ToString(),
        n => n.ToString()
    );
}