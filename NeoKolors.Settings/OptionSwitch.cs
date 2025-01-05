//
// NeoKolors
// Copyright (c) 2024 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings;

public struct OptionSwitch {
    public List<string> Names { get; }
    public int Index { get; private set; }

    public OptionSwitch(SettingsGroupOption[] options) {
        Names = new string[options.Length].ToList();

        for (int i = 0; i < options.Length; i++) {
            Names[i] = options[i].Name;
        }
    }

    public OptionSwitch(params string[] names) {
        Names = [..names];
    }

    public OptionSwitch() {
        Names = new List<string>();
    }

    public void Add(string name) {
        Names.Add(name);
    }

    public void Add(SettingsGroupOption option) {
        Names.Add(option.Name);
    }

    public OptionSwitch Select(string name) {
        for (int i = 0; i < Names.Count; i++) {
            if (Names[i] != name) continue;
            
            Index = i;
            return this;
        }
        
        throw SettingsGroupException.SwitchInvalidOptionName(name);
    }

    public OptionSwitch Select(int index) {
        if (0 > index || index >= Names.Count)
            throw SettingsGroupException.SwitchOptionIndexOutOfRange(index, Names.Count);

        Index = index;
        return this;
    }
}