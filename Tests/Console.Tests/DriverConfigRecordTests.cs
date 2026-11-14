//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console.Ansi;

namespace NeoKolors.Console.Tests;

[Collection("ConsoleTests")]
public class DriverConfigRecordTests {

    [Fact]
    public void InputDriverConfig_Defaults_AreStandardized() {
        var config = new InputDriverConfig();

        Assert.True(config.ReportFocus);
        Assert.True(config.ReportResize);
        Assert.False(config.BracketedPaste);
        Assert.Equal(ReportedMouseEvents.NONE, config.MouseReportLevel);
        Assert.Equal(MouseReportProtocol.SGR, config.MouseReportProtocol);
        Assert.True(config.CtrlCForceQuits);
        Assert.Equal(TimeSpan.FromMilliseconds(10), config.RefreshInterval);
        Assert.Equal(TimeSpan.FromMilliseconds(100), config.VTRequestTimeout);
        Assert.Equal(1, config.VTRequestMaxRetries);
    }

    [Fact]
    public void InputDriverConfig_RecordEquality_And_WithExpressions() {
        var c1 = new InputDriverConfig {
            MouseReportLevel = ReportedMouseEvents.ALL,
            VTRequestTimeout = TimeSpan.FromMilliseconds(300)
        };

        var c2 = new InputDriverConfig {
            MouseReportLevel = ReportedMouseEvents.ALL,
            VTRequestTimeout = TimeSpan.FromMilliseconds(300)
        };

        var c3 = c1 with { MouseReportLevel = ReportedMouseEvents.NONE };

        Assert.Equal(c1, c2);
        Assert.True(c1 == c2);
        Assert.Equal(c1.GetHashCode(), c2.GetHashCode());

        Assert.NotEqual(c1, c3);
        Assert.True(c1 != c3);
        Assert.Equal(ReportedMouseEvents.NONE, c3.MouseReportLevel);
        Assert.Equal(TimeSpan.FromMilliseconds(300), c3.VTRequestTimeout);
    }

    [Fact]
    public void WinInputDriverConfig_Defaults_And_OverriddenValues() {
        var winConfig = new WinInputDriverConfig();

        // Windows defaults mouse to ALL
        Assert.Equal(ReportedMouseEvents.ALL, winConfig.MouseReportLevel);
        Assert.True(winConfig.ReportFocus);
        Assert.True(winConfig.ReportResize);

        var updated = winConfig with { MouseReportLevel = ReportedMouseEvents.DOWN };
        Assert.Equal(ReportedMouseEvents.DOWN, updated.MouseReportLevel);
        Assert.IsType<WinInputDriverConfig>(updated);
    }

    [Fact]
    public void DotnetInputDriverConfig_Defaults_And_OverriddenValues() {
        var dotnetConfig = new DotnetInputDriverConfig();

        // Dotnet defaults resize to false
        Assert.False(dotnetConfig.ReportResize);
        Assert.Equal(ReportedMouseEvents.NONE, dotnetConfig.MouseReportLevel);

        var updated = dotnetConfig with { ReportResize = true };
        Assert.True(updated.ReportResize);
        Assert.IsType<DotnetInputDriverConfig>(updated);
    }

    [Fact]
    public void LinuxInputDriverConfig_Defaults_And_OverriddenValues() {
        var linuxConfig = new LinuxInputDriverConfig();

        Assert.True(linuxConfig.ReportFocus);
        Assert.True(linuxConfig.ReportResize);
        Assert.False(linuxConfig.BracketedPaste);

        var updated = linuxConfig with { BracketedPaste = true };
        Assert.True(updated.BracketedPaste);
        Assert.IsType<LinuxInputDriverConfig>(updated);
    }

    [Fact]
    public void Polymorphism_BaseRecordWith_PreservesDerivedType() {
        InputDriverConfig baseRef = new WinInputDriverConfig();

        // 'with' on base record variable invokes virtual clone and creates the concrete derived record
        var updated = baseRef with { VTRequestTimeout = TimeSpan.FromMilliseconds(500) };

        Assert.IsType<WinInputDriverConfig>(updated);
        Assert.Equal(TimeSpan.FromMilliseconds(500), updated.VTRequestTimeout);
        Assert.Equal(ReportedMouseEvents.ALL, updated.MouseReportLevel);
    }

    [Fact]
    public void NKConsole_ConfigureInput_UpdatesDriverConfigSeamlessly() {
        var prevDriver = NKConsole.InputDriver;
        var testDriver = new DotnetInputDriver();
        NKConsole.InputDriver = testDriver;

        try {
            NKConsole.ConfigureInput(c => c with {
                MouseReportLevel = ReportedMouseEvents.RELEASE,
                VTRequestTimeout = TimeSpan.FromMilliseconds(222)
            });

            Assert.Equal(ReportedMouseEvents.RELEASE, NKConsole.InputDriver.Config.MouseReportLevel);
            Assert.Equal(TimeSpan.FromMilliseconds(222), NKConsole.InputDriver.Config.VTRequestTimeout);
            Assert.IsType<DotnetInputDriverConfig>(NKConsole.InputDriver.Config);
        }
        finally {
            NKConsole.InputDriver = prevDriver;
        }
    }

    [Fact]
    public void NKConsole_ConfigureInputTyped_UpdatesTypedDriverConfig() {
        var prevDriver = NKConsole.InputDriver;
        var testDriver = new DotnetInputDriver();
        NKConsole.InputDriver = testDriver;

        try {
            NKConsole.ConfigureInput<DotnetInputDriverConfig>(c => c with {
                MouseReportProtocol = MouseReportProtocol.SGR_PIXELS,
                BracketedPaste = true
            });

            var config = (DotnetInputDriverConfig)NKConsole.InputDriver.Config;
            Assert.Equal(MouseReportProtocol.SGR_PIXELS, config.MouseReportProtocol);
            Assert.True(config.BracketedPaste);
        }
        finally {
            NKConsole.InputDriver = prevDriver;
        }
    }
}
