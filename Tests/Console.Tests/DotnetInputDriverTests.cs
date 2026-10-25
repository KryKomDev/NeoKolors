using System.Text;
using NeoKolors.Common;

namespace NeoKolors.Console.Tests;

[Collection("ConsoleTests")]
public class DotnetInputDriverTests {

    private class RecordingOutputDriver : IOutputDriver {
        private readonly StringBuilder _sb = new();
        public string Output => _sb.ToString();

        public void Write(ReadOnlySpan<char> value) => _sb.Append(value.ToString());
        public void Write(string value) => _sb.Append(value);
        public void Clear() => _sb.Clear();
        public void Dispose() { }
    }

    [Fact]
    public void SettingMouseReportLevel_AppliesLevelAndProtocolSequences() {
        var rec = new RecordingOutputDriver();
        var prevOutput = NKConsole.OutputDriver;
        NKConsole.OutputDriver = rec;

        try {
            var driver = new DotnetInputDriver();
            driver.Config.MouseReportLevel = ReportedMouseEvents.ALL;

            // Must have enabled ALL mouse events (\e[?1003h) and SGR protocol (\e[?1006h)
            Assert.Contains(EscapeCodes.MOUSE_EV_ON_ALL_ON, rec.Output);
            Assert.Contains(EscapeCodes.MOUSE_EV_SGR_ON, rec.Output);

            rec.Clear();
            driver.Config.MouseReportLevel = ReportedMouseEvents.NONE;

            // Must have disabled ALL mouse events (\e[?1003l) and SGR protocol (\e[?1006l)
            Assert.Contains(EscapeCodes.MOUSE_EV_ON_ALL_OFF, rec.Output);
            Assert.Contains(EscapeCodes.MOUSE_EV_SGR_OFF, rec.Output);
        }
        finally {
            NKConsole.OutputDriver = prevOutput;
        }
    }

    [Fact]
    public void DriverStartAndStop_AppliesAndResetsMouseReporting() {
        var rec = new RecordingOutputDriver();
        var prevOutput = NKConsole.OutputDriver;
        NKConsole.OutputDriver = rec;

        try {
            var config = new DotnetInputDriverConfig(mouseConfig: ReportedMouseEvents.ALL);
            var driver = new DotnetInputDriver(config);

            // Pre-configured mouse level is applied on initialization
            Assert.Contains(EscapeCodes.MOUSE_EV_ON_ALL_ON, rec.Output);
            Assert.Contains(EscapeCodes.MOUSE_EV_SGR_ON, rec.Output);

            rec.Clear();
            driver.Stop();

            // Stop must reset mouse reporting
            Assert.Contains(EscapeCodes.MOUSE_EV_ON_ALL_OFF, rec.Output);
            Assert.Contains(EscapeCodes.MOUSE_EV_SGR_OFF, rec.Output);
        }
        finally {
            NKConsole.OutputDriver = prevOutput;
        }
    }

    [LinuxFact]
    public void LinuxInputDriver_InheritsDotnetInputDriverCorrectly() {
        var rec = new RecordingOutputDriver();
        var prevOutput = NKConsole.OutputDriver;
        NKConsole.OutputDriver = rec;

        try {
            var driver = new LinuxInputDriver();
            driver.Config.MouseReportLevel = ReportedMouseEvents.DRAG;

            Assert.Contains(EscapeCodes.MOUSE_EV_ON_PRD_ON, rec.Output);
            Assert.Contains(EscapeCodes.MOUSE_EV_SGR_ON, rec.Output);

            driver.Stop();
        }
        finally {
            NKConsole.OutputDriver = prevOutput;
        }
    }
}
