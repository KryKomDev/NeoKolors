//
// NeoKolors.Test
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console;
using NeoKolors.Console.Ansi.Mouse;
using NeoKolors.Console.Input;

namespace NeoKolors.Tui.Tests;

public class NKAppConfigTests {
    
    [Fact]
    public void DefaultConstructor_ShouldSetDefaultValues() {
        var config = new NKAppConfig();
        
        Assert.False(config.CtrlCForceQuits);
        Assert.Equal(24, config.Rendering.Limit);
        Assert.Equal(MouseReportProtocol.SGR, config.MouseReportProtocol);
        Assert.Equal(MouseReportLevel.ALL, config.MouseReportLevel);
        Assert.True(config.PauseOnFocusLost);
        Assert.True(config.KeepCursorDisabled);
        Assert.False(config.BracketedPaste);
        Assert.Equal(KeyCode.Q, config.InterruptCombination.Key);
        Assert.Equal(KeyModifiers.LEFT_CTRL, config.InterruptCombination.Modifiers);
    }

    [Fact]
    public void ParameterizedConstructor_ShouldSetValues() {
        var rendering = RenderingConfig.Limited(60);
        var interrupt = new KeyEventArgs(KeyCode.ESCAPE, KeyModifiers.NONE, '\x1b');
        
        var config = new NKAppConfig(
            rendering: rendering,
            ctrlCForceQuits: true,
            interruptCombination: interrupt,
            mouseReportProtocol: MouseReportProtocol.SGR,
            mouseReportLevel: MouseReportLevel.PRESS,
            bracketedPaste: true,
            pauseOnFocusLost: false,
            keepCursorDisabled: false
        );
        
        Assert.True(config.CtrlCForceQuits);
        Assert.Equal(60, config.Rendering.Limit);
        Assert.Equal(MouseReportProtocol.SGR, config.MouseReportProtocol);
        Assert.Equal(MouseReportLevel.PRESS, config.MouseReportLevel);
        Assert.True(config.BracketedPaste);
        Assert.False(config.PauseOnFocusLost);
        Assert.False(config.KeepCursorDisabled);
        Assert.Equal(KeyCode.ESCAPE, config.InterruptCombination.Key);
    }

    [Fact]
    public void ToString_ShouldReturnCorrectFormat() {
        var config = new NKAppConfig();
        var str = config.ToString();
        
        Assert.Contains("Rendering", str);
        Assert.Contains("Interrupt", str);
        Assert.Contains("Mouse Protocol", str);
        Assert.Contains("Mouse Level", str);
    }
}
