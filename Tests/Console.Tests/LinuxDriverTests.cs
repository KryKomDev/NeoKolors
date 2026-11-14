//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

using System.Text;
using NeoKolors.Console.Ansi;

namespace NeoKolors.Console.Tests;

[Collection("ConsoleTests")]
public class LinuxDriverTests {

    [LinuxFact]
    public unsafe void LinuxImports_StructSizes_MatchLinuxAbi() {
        Assert.Equal(60, sizeof(LinuxImports.Termios));
        Assert.Equal(8,  sizeof(LinuxImports.WinSize));
        Assert.Equal(8,  sizeof(LinuxImports.PollFd));
        Assert.Equal(8,  sizeof(LinuxImports.PipeFds));
    }

    [LinuxTheory]
    [InlineData('\r',      KeyCode.RETURN,    KeyModifiers.NONE)]
    [InlineData('\n',      KeyCode.RETURN,    KeyModifiers.NONE)]
    [InlineData('\t',      KeyCode.TAB,       KeyModifiers.NONE)]
    [InlineData('\b',      KeyCode.BACKSPACE, KeyModifiers.NONE)]
    [InlineData((char)127, KeyCode.BACKSPACE, KeyModifiers.NONE)]
    [InlineData(' ',       KeyCode.SPACE,     KeyModifiers.NONE)]
    [InlineData('\e',      KeyCode.ESCAPE,    KeyModifiers.NONE)]
    [InlineData('a',       KeyCode.A,         KeyModifiers.NONE)]
    [InlineData('z',       KeyCode.Z,         KeyModifiers.NONE)]
    [InlineData('A',       KeyCode.A,         KeyModifiers.SHIFT)]
    [InlineData('Z',       KeyCode.Z,         KeyModifiers.SHIFT)]
    [InlineData('0',       KeyCode.D0,        KeyModifiers.NONE)]
    [InlineData('9',       KeyCode.D9,        KeyModifiers.NONE)]
    [InlineData((char)1,   KeyCode.A,         KeyModifiers.LEFT_CTRL)]
    [InlineData((char)26,  KeyCode.Z,         KeyModifiers.LEFT_CTRL)]
    public void LinuxInput_CharToKeyEventArgs_MapsAccurately(char input, KeyCode expectedKey, KeyModifiers expectedModifiers) {
        var args = LinuxInput.CharToKeyEventArgs(input);

        Assert.Equal(expectedKey,       args.Key);
        Assert.Equal(expectedModifiers, args.Modifiers);
    }

    [LinuxFact]
    public void LinuxPipe_OutputAndInput_DirectTransfer() {
        var ret = LinuxImports.pipe(out var pipeFds);
        Assert.Equal(0, ret);

        try {
            using var output = new LinuxOutput(pipeFds.WriteFd);
            using var input  = new LinuxInput(pipeFds.ReadFd);

            const string message = "Hello Native Linux!";
            output.Write(message);

            // Close write fd so EOF can be reached after reading
            LinuxImports.close(pipeFds.WriteFd);
            pipeFds.WriteFd = -1;

            var sb = new StringBuilder();

            while (input.PollAndBuffer(100)) {
                var key = input.ReadKey(100);
                sb.Append(key.Char);
            }

            Assert.Equal(message, sb.ToString());
        }
        finally {
            if (pipeFds.ReadFd > 0)
                LinuxImports.close(pipeFds.ReadFd);

            if (pipeFds.WriteFd > 0)
                LinuxImports.close(pipeFds.WriteFd);
        }
    }

    [LinuxFact]
    public void LinuxTerminal_TryGetWindowSize_DoesNotThrow() {
        var success = LinuxTerminal.TryGetWindowSize(out var size);

        if (success) {
            Assert.True(size.X >= 0);
            Assert.True(size.Y >= 0);
        }
    }

    [LinuxFact]
    public void LinuxTerminal_TryGetWindowPixelSize_DoesNotThrow() {
        var success = LinuxTerminal.TryGetWindowPixelSize(out var pixelSize);

        if (success) {
            Assert.True(pixelSize.X >= 0);
            Assert.True(pixelSize.Y >= 0);
        }
    }

    [LinuxFact]
    public void LinuxInputDriverConfig_Defaults_AreValid() {
        var config = new LinuxInputDriverConfig();

        Assert.True(config.ReportFocus);
        Assert.True(config.ReportResize);
        Assert.False(config.BracketedPaste);
        Assert.Equal(ReportedMouseEvents.NONE, config.MouseReportLevel);
        Assert.Equal(MouseReportProtocol.SGR,  config.MouseReportProtocol);
        Assert.True(config.CtrlCForceQuits);
    }

    [LinuxFact]
    public void LinuxInputDriverConfig_WithExpressions_CreateUpdatedCopies() {
        var config = new LinuxInputDriverConfig();
        var updated = config with {
            MouseReportLevel = ReportedMouseEvents.ALL,
            MouseReportProtocol = MouseReportProtocol.X10
        };

        Assert.Equal(ReportedMouseEvents.ALL, updated.MouseReportLevel);
        Assert.Equal(MouseReportProtocol.X10, updated.MouseReportProtocol);
        Assert.NotEqual(config, updated);
        Assert.Equal(ReportedMouseEvents.NONE, config.MouseReportLevel);
    }

    [LinuxFact]
    public void LinuxInputDriver_ConstructsAndDisposes_WithoutThrowing() {
        using var driver = new LinuxInputDriver();
        Assert.NotNull(driver);
        Assert.False(driver.IsRunning);
    }

    [LinuxFact]
    public void LinuxOutputDriver_ConstructsAndDisposes_WithoutThrowing() {
        using var driver = new LinuxOutputDriver();
        Assert.NotNull(driver);
    }
}

#endif