using NeoKolors.Console.Ansi;
using NeoKolors.Console.Events;
using NeoKolors.Console.Input;
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
        Assert.Equal(10, record.Value.Mouse.Position.X);
        Assert.Equal(20, record.Value.Mouse.Position.Y);
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
}
