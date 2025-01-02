//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings;

public struct NodeSwitch {
    public string[] Names { get; }
    public int Index { get; private set; }

    public NodeSwitch(ISettingsNode<object>[] nodes) {
        Names = new string[nodes.Length];

        for (int i = 0; i < nodes.Length; i++) {
            Names[i] = nodes[i].Name;
        }
    }

    public void Select(string name) {
        for (int i = 0; i < Names.Length; i++) {
            if (Names[i] == name) {
                Index = i;
                return;
            }
        }
        
        throw SettingsBuilderException.InvalidNodeName(name);
    }

    public void Select(int index) {
        if (index >= 0 && index < Names.Length) Index = index;
        
        throw SettingsBuilderException.NodeIndexOutOfRange(index, Names.Length);
    }
}