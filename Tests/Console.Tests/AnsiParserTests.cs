using NeoKolors.Console.Ansi;
using NeoKolors.Extensions;

namespace NeoKolors.Console.Tests;

public class AnsiParserTests {

    private class MockAnsiParser : AnsiParser {
        
        private readonly Queue<KeyEventArgs> _input = new();
        public bool Released { get; private set; }
        public bool ReleaseSuccess { get; private set; }

        public MockAnsiParser(params string[] sequences) {
            foreach (var seq in sequences) {
                foreach (var c in seq) {
                    _input.Enqueue(new KeyEventArgs(ConsoleKeyInfo.FromChar(c)));
                }
            }
        }

        public override bool Peek() => _input.Count > 0;

        public override KeyEventArgs Read() {
            return _input.Count == 0 
                ? new KeyEventArgs(KeyCode.NONE, KeyModifiers.NONE, '\0') 
                : _input.Dequeue();
        }

        public override void ReleaseRead(bool success) {
            Released = true;
            ReleaseSuccess = success;
        }
    }

    [Fact]
    public void Parse_MouseSGR_Press_ReturnsCorrectRecord() {
        // \e[<0;10;20M -> Button 0 (Left), X=10, Y=20, Press (M)
        var parser = new MockAnsiParser("\e[<0;10;20M");
        var record = parser.Parse();

        Assert.NotNull(record);
        Assert.Equal(AnsiRecordType.MOUSE, record.Value.Type);
        Assert.Equal(MouseButton.LEFT, record.Value.Mouse.Button);
        Assert.Equal(9, record.Value.Mouse.Position.X);
        Assert.Equal(19, record.Value.Mouse.Position.Y);
        Assert.True(record.Value.Mouse.IsPress);
        Assert.False(record.Value.Mouse.Released);
        Assert.False(record.Value.Mouse.Moved);
    }

    [Fact]
    public void Parse_MouseSGR_Release_ReturnsCorrectRecord() {
        // \e[<0;10;20m -> Button 0 (Left), X=10, Y=20, Release (m)
        var parser = new MockAnsiParser("\e[<0;10;20m");
        var record = parser.Parse();

        Assert.NotNull(record);
        Assert.Equal(AnsiRecordType.MOUSE, record.Value.Type);
        Assert.Equal(MouseButton.LEFT, record.Value.Mouse.Button);
        Assert.Equal(9, record.Value.Mouse.Position.X);
        Assert.Equal(19, record.Value.Mouse.Position.Y);
        Assert.True(record.Value.Mouse.Released);
        Assert.True(record.Value.Mouse.IsRelease);
        Assert.False(record.Value.Mouse.Moved);
    }

    [Fact]
    public void Parse_MouseSGR_Hover_ReturnsCorrectRecord() {
        // \e[<35;15;25M -> Button 3 (Release/None) + motion (32), X=15, Y=25
        var parser = new MockAnsiParser("\e[<35;15;25M");
        var record = parser.Parse();

        Assert.NotNull(record);
        Assert.Equal(AnsiRecordType.MOUSE, record.Value.Type);
        Assert.Equal(MouseButton.RELEASE, record.Value.Mouse.Button);
        Assert.Equal(14, record.Value.Mouse.Position.X);
        Assert.Equal(24, record.Value.Mouse.Position.Y);
        Assert.True(record.Value.Mouse.Moved);
        Assert.False(record.Value.Mouse.Released);
        Assert.True(record.Value.Mouse.IsHover);
    }

    [Fact]
    public void Parse_MouseSGR_Drag_ReturnsCorrectRecord() {
        // \e[<32;12;22M -> Left Button (0) + motion (32), X=12, Y=22
        var parser = new MockAnsiParser("\e[<32;12;22M");
        var record = parser.Parse();

        Assert.NotNull(record);
        Assert.Equal(AnsiRecordType.MOUSE, record.Value.Type);
        Assert.Equal(MouseButton.LEFT, record.Value.Mouse.Button);
        Assert.Equal(11, record.Value.Mouse.Position.X);
        Assert.Equal(21, record.Value.Mouse.Position.Y);
        Assert.True(record.Value.Mouse.Moved);
        Assert.False(record.Value.Mouse.Released);
        Assert.False(record.Value.Mouse.IsHover);
    }

    [Fact]
    public void Parse_MouseSGR_Wheel_ReturnsCorrectRecord() {
        // \e[<64;10;20M -> Wheel Up (64)
        var parserUp = new MockAnsiParser("\e[<64;10;20M");
        var recordUp = parserUp.Parse();

        Assert.NotNull(recordUp);
        Assert.Equal(MouseButton.WHEEL_UP, recordUp.Value.Mouse.Button);

        // \e[<65;10;20M -> Wheel Down (65)
        var parserDown = new MockAnsiParser("\e[<65;10;20M");
        var recordDown = parserDown.Parse();

        Assert.NotNull(recordDown);
        Assert.Equal(MouseButton.WHEEL_DOWN, recordDown.Value.Mouse.Button);
    }

    [Fact]
    public void Parse_MouseX10_PressAndRelease_ReturnsCorrectRecord() {
        // \e[M + ' ' (32 -> Button 0 = Left) + '*' (42 -> col 10 => 9) + '4' (52 -> row 20 => 19)
        var parserPress = new MockAnsiParser("\e[M *4");
        var recordPress = parserPress.Parse();

        Assert.NotNull(recordPress);
        Assert.Equal(AnsiRecordType.MOUSE, recordPress.Value.Type);
        Assert.Equal(MouseButton.LEFT, recordPress.Value.Mouse.Button);
        Assert.Equal(9, recordPress.Value.Mouse.Position.X);
        Assert.Equal(19, recordPress.Value.Mouse.Position.Y);
        Assert.True(recordPress.Value.Mouse.IsPress);

        // \e[M + '#' (35 -> Button 3 = Release) + '*' (42) + '4' (52)
        var parserRel = new MockAnsiParser("\e[M#*4");
        var recordRel = parserRel.Parse();

        Assert.NotNull(recordRel);
        Assert.Equal(AnsiRecordType.MOUSE, recordRel.Value.Type);
        Assert.Equal(MouseButton.RELEASE, recordRel.Value.Mouse.Button);
        Assert.Equal(9, recordRel.Value.Mouse.Position.X);
        Assert.Equal(19, recordRel.Value.Mouse.Position.Y);
        Assert.True(recordRel.Value.Mouse.IsRelease);
    }

    [Fact]
    public void Parse_PrimaryDA_ReturnsCorrectRecord() {
        // \e[?64;1;2c -> VT mode 64, capabilities 1, 2
        var parser = new MockAnsiParser("\e[?64;1;2c");
        var record = parser.Parse();

        Assert.NotNull(record);
        Assert.Equal(AnsiRecordType.VT_QUERY, record.Value.Type);
        Assert.Equal(VTType.VT420, record.Value.Query.PrimaryDAResponse!.Value.Type);
        Assert.True(record.Value.Query.PrimaryDAResponse.Value.Capabilities!.Value.HasFlag(VTCapabilities.COL_132));
        Assert.True(record.Value.Query.PrimaryDAResponse.Value.Capabilities!.Value.HasFlag(VTCapabilities.PRINTER));
    }

    [Fact]
    public void Parse_DecReq_ReturnsCorrectRecord() {
        // \e[?1;2y -> mode 1, value 2 (DISABLED)
        var parser = new MockAnsiParser("\e[?1;2y");
        var record = parser.Parse();

        Assert.NotNull(record);
        Assert.Equal(AnsiRecordType.VT_QUERY, record.Value.Type);
        Assert.Equal(1, (int)record.Value.Query.DecMode);
        Assert.Equal(DecReqResponseType.DISABLED, record.Value.Query.DecResponse);
    }


    [Fact]
    public void Parse_NonEscapeChar_ReturnsKeyRecord() {
        var parser = new MockAnsiParser("A");
        var record = parser.Parse();

        Assert.NotNull(record);
        Assert.Equal(AnsiRecordType.KEY, record.Value.Type);
        Assert.Equal('A', record.Value.Key.Char);
        Assert.True(parser.Released);
        Assert.True(parser.ReleaseSuccess);
    }

    [Fact]
    public void Parse_ArrowKeys_ReturnsCorrectRecord() {
        var parserUp = new MockAnsiParser("\e[A");
        var recordUp = parserUp.Parse();
        Assert.NotNull(recordUp);
        Assert.Equal(AnsiRecordType.KEY, recordUp.Value.Type);
        Assert.Equal(KeyCode.ARROW_UP, recordUp.Value.Key.Key);
        Assert.Equal(KeyModifiers.NONE, recordUp.Value.Key.Modifiers);

        var parserDown = new MockAnsiParser("\e[B");
        var recordDown = parserDown.Parse();
        Assert.NotNull(recordDown);
        Assert.Equal(KeyCode.ARROW_DOWN, recordDown.Value.Key.Key);

        var parserRight = new MockAnsiParser("\e[C");
        var recordRight = parserRight.Parse();
        Assert.NotNull(recordRight);
        Assert.Equal(KeyCode.ARROW_RIGHT, recordRight.Value.Key.Key);

        var parserLeft = new MockAnsiParser("\e[D");
        var recordLeft = parserLeft.Parse();
        Assert.NotNull(recordLeft);
        Assert.Equal(KeyCode.ARROW_LEFT, recordLeft.Value.Key.Key);
    }

    [Fact]
    public void Parse_ModifiedArrowKeys_ReturnsCorrectRecord() {
        // \e[1;5A -> Ctrl + ArrowUp
        var parserCtrlUp = new MockAnsiParser("\e[1;5A");
        var recordCtrlUp = parserCtrlUp.Parse();
        Assert.NotNull(recordCtrlUp);
        Assert.Equal(AnsiRecordType.KEY, recordCtrlUp.Value.Type);
        Assert.Equal(KeyCode.ARROW_UP, recordCtrlUp.Value.Key.Key);
        Assert.Equal(KeyModifiers.LEFT_CTRL, recordCtrlUp.Value.Key.Modifiers);

        // \e[1;2B -> Shift + ArrowDown
        var parserShiftDown = new MockAnsiParser("\e[1;2B");
        var recordShiftDown = parserShiftDown.Parse();
        Assert.NotNull(recordShiftDown);
        Assert.Equal(KeyCode.ARROW_DOWN, recordShiftDown.Value.Key.Key);
        Assert.Equal(KeyModifiers.SHIFT, recordShiftDown.Value.Key.Modifiers);
    }

    [Fact]
    public void Parse_TildeSpecialKeys_ReturnsCorrectRecord() {
        // \e[3~ -> Delete
        var parserDel = new MockAnsiParser("\e[3~");
        var recordDel = parserDel.Parse();
        Assert.NotNull(recordDel);
        Assert.Equal(KeyCode.DELETE, recordDel.Value.Key.Key);

        // \e[5~ -> Page Up
        var parserPgUp = new MockAnsiParser("\e[5~");
        var recordPgUp = parserPgUp.Parse();
        Assert.NotNull(recordPgUp);
        Assert.Equal(KeyCode.PAGE_UP, recordPgUp.Value.Key.Key);

        // \e[6~ -> Page Down
        var parserPgDn = new MockAnsiParser("\e[6~");
        var recordPgDn = parserPgDn.Parse();
        Assert.NotNull(recordPgDn);
        Assert.Equal(KeyCode.PAGE_DOWN, recordPgDn.Value.Key.Key);
    }

    [Fact]
    public void Parse_Ss3Keys_ReturnsCorrectRecord() {
        // \eOP -> F1
        var parserF1 = new MockAnsiParser("\eOP");
        var recordF1 = parserF1.Parse();
        Assert.NotNull(recordF1);
        Assert.Equal(AnsiRecordType.KEY, recordF1.Value.Type);
        Assert.Equal(KeyCode.F1, recordF1.Value.Key.Key);
    }

    [Fact]
    public void Parse_FocusEvents_ReturnsCorrectRecord() {
        var parserIn = new MockAnsiParser("\e[I");
        var recordIn = parserIn.Parse();
        Assert.NotNull(recordIn);
        Assert.Equal(AnsiRecordType.FOCUS, recordIn.Value.Type);
        Assert.True(recordIn.Value.HasFocus);

        var parserOut = new MockAnsiParser("\e[O");
        var recordOut = parserOut.Parse();
        Assert.NotNull(recordOut);
        Assert.Equal(AnsiRecordType.FOCUS, recordOut.Value.Type);
        Assert.False(recordOut.Value.HasFocus);
    }

    [Fact]
    public void Parse_BracketedPaste_ReturnsCorrectRecord() {
        var parserPaste = new MockAnsiParser("\e[200~Hello NeoKolors!\e[201~");
        var recordPaste = parserPaste.Parse();
        Assert.NotNull(recordPaste);
        Assert.Equal(AnsiRecordType.PASTE, recordPaste.Value.Type);
        Assert.Equal("Hello NeoKolors!", recordPaste.Value.Pasted);
    }

    [Fact]
    public void Parse_AltKey_ReturnsCorrectRecord() {
        // \ea -> Alt+A
        var parserAlt = new MockAnsiParser("\ea");
        var recordAlt = parserAlt.Parse();
        Assert.NotNull(recordAlt);
        Assert.Equal(AnsiRecordType.KEY, recordAlt.Value.Type);
        Assert.Equal(KeyCode.A, recordAlt.Value.Key.Key);
        Assert.True(recordAlt.Value.Key.Modifiers.HasFlag(KeyModifiers.LEFT_ALT));
    }
}
