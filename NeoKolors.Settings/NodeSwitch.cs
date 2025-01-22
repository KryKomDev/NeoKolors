//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings;

public struct NodeSwitch {
    public List<string> Names { get; }
    public int Index { get; private set; }

    public NodeSwitch(ISettingsNode[] nodes) {
        Names = new string[nodes.Length].ToList();

        for (int i = 0; i < nodes.Length; i++) {
            Names[i] = nodes[i].Name;
        }
    }

    public NodeSwitch Select(string name) {
        for (int i = 0; i < Names.Count; i++) {
            if (Names[i] == name) {
                Index = i;
                return this;
            }
        }
        
        throw SettingsBuilderException.SwitchInvalidNodeName(name);
    }

    public NodeSwitch Select(int index) {
        if (index < 0 || index >= Names.Count) throw SettingsBuilderException.SwitchNodeIndexOutOfRange(index, Names.Count);
        
        Index = index;
        return this;
    }

    public override string ToString() => $"{{\"index\": {Index}, \"names\": [{string.Join(", ", Names)}]}}";

}