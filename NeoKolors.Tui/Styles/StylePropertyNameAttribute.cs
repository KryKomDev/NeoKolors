//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Styles;

[AttributeUsage(AttributeTargets.Struct)]
public class StylePropertyNameAttribute : Attribute {
    public string Name { get; }
    
    public StylePropertyNameAttribute(string name) {
        Name = name;
    }
}