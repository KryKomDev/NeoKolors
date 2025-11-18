//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;

namespace NeoKolors.Settings.Command;

public readonly struct Usage {
    public List<Usage> NodeUsages { get; }
    public List<ParamInfo> ThisUsage { get; }
    
    public Usage() {
        NodeUsages = [];
        ThisUsage = [];
    }

    public override string ToString() {
        string s = ThisUsage.Join(" ");
        s += NodeUsages.Join(" ");
        return s;
    }
}