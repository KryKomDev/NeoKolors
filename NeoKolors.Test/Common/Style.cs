// 
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Console;

namespace NeoKolors.Test.Common;

public class StyleTests {
    
    [Fact]
    public void SetFColorCustom_ShouldWorkProperly() {
        var s = new Style(0x01_23_45_67_89_ab_cd_e0ul);
        s.SetFColor((UInt24)0xabcdef);
        Assert.Equal(0xab_cd_ef_67_89_ab_cd_e2ul, s.Raw);
        s.SetFColor((UInt24)0x000000);
        Assert.Equal(0x00_00_00_67_89_ab_cd_e2ul, s.Raw);
    }

    [Fact]
    public void SetBColorCustom_ShouldWorkProperly() {
        var s = new Style(0x01_23_45_67_89_ab_cd_e0ul);
        s.SetBColor((UInt24)0xabcdef);
        Assert.Equal(0x01_23_45_ab_cd_ef_cd_e1ul, s.Raw);
        s.SetBColor((UInt24)0x000000);
        Assert.Equal(0x01_23_45_00_00_00_cd_e1ul, s.Raw);
    }

    [Fact]
    public void IsFColorCustom_ShouldReturnCorrectValue() {
        var s = new Style(0x0000000000000002ul);
        Assert.True(s.IsFColorCustom);
        s = new Style(0x0000000000000001ul);
        Assert.False(s.IsFColorCustom);
    }
    
    [Fact]
    public void IsBColorCustom_ShouldReturnCorrectValue() {
        var s = new Style(0x0000000000000002ul);
        Assert.False(s.IsBColorCustom);
        s = new Style(0x0000000000000001ul);
        Assert.True(s.IsBColorCustom);
    }

    [Fact]
    public void GetFColor_ShouldReturnCorrectValue() {
        var s = new Style(0xab_cd_ef_00_00_00_00_02ul);
        var c = s.GetFColor();
        Assert.Equal(new Color(0xab_cd_ef), c);
        s = new Style(0x00_00_00_00_00_00_c0_00ul);
        c = s.GetFColor();
        Assert.Equal(new Color(ConsoleColor.Red), c); // Red is equal to 0xc
    }
    
    [Fact]
    public void GetBColor_ShouldReturnCorrectValue() {
        var s = new Style(0x00_00_00_ab_cd_ef_00_01ul);
        var c = s.GetBColor();
        Assert.Equal(new Color(0xab_cd_ef), c);
        s = new Style(0x00_00_00_00_00_00_0c_00ul);
        c = s.GetBColor();
        Debug.Log(c);
        Assert.Equal(new Color(ConsoleColor.Red), c); // Red is equal to 0xc
    }

    [Fact]
    public void SetStyles_ShouldWorkProperly() {
        var s = new Style(0x00_00_00_00_00_00_00_00ul);
        s.SetStyles(0b101010);
        Assert.Equal(0x00_00_00_00_00_00_00_a8ul, s.Raw);
    }

    [Fact]
    public void GetStyles_ShouldReturnCorrectValue() {
        var s = new Style(0x00_00_00_00_00_00_00_a8ul);
        var t = s.GetStyles();
        Assert.Equal(0b101010, t);
    }
}