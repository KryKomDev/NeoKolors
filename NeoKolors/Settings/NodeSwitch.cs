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

    public void Select(string name) {
        for (int i = 0; i < Names.Count; i++) {
            if (Names[i] == name) {
                Index = i;
                return;
            }
        }
        
        throw SettingsBuilderException.SwitchInvalidNodeName(name);
    }

    public void Select(int index) {
        if (index >= 0 && index < Names.Count) Index = index;
        
        throw SettingsBuilderException.SwitchNodeIndexOutOfRange(index, Names.Count);
    }
}