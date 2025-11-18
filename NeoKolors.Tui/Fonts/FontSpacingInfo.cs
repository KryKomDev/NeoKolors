// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Tui.Fonts;

public class FontSpacingInfo : OneOfBase<MonospaceInfo, VariableInfo> {
    protected FontSpacingInfo(OneOf<MonospaceInfo, VariableInfo> input) : base(input) { }

    public static implicit operator FontSpacingInfo(MonospaceInfo m) => new(m);
    public static implicit operator FontSpacingInfo(VariableInfo v) => new(v);

    public bool IsMonospace => IsT0;
    public bool IsVariable => IsT1;
    
    public MonospaceInfo AsMonospace => AsT0;
    public VariableInfo AsVariable => AsT1;
}