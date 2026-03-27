using Metriks;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Tests.Ansi;

public class AnsiRecordTests {

    [Fact]
    public void KeyRecord_StoresAndRetrievesCorrectly() {
        var key = new KeyEventArgs(KeyCode.A, KeyModifiers.SHIFT, 'A');
        var record = new AnsiRecord(key);

        Assert.Equal(AnsiRecordType.KEY, record.Type);
        Assert.Equal(key, record.Key);
        Assert.Throws<InvalidOperationException>(() => record.Mouse);
    }

    [Fact]
    public void MouseRecord_StoresAndRetrievesCorrectly() {
        var mouse = new MouseEventArgs(MouseButton.LEFT, KeyModifiers.NONE, new Point2D(5, 5), false, false);
        var record = new AnsiRecord(mouse);

        Assert.Equal(AnsiRecordType.MOUSE, record.Type);
        Assert.Equal(mouse, record.Mouse);
        Assert.Throws<InvalidOperationException>(() => record.Key);
    }

    [Fact]
    public void FocusRecord_StoresAndRetrievesCorrectly() {
        var record = new AnsiRecord(true);

        Assert.Equal(AnsiRecordType.FOCUS, record.Type);
        Assert.True(record.HasFocus);
        Assert.Throws<InvalidOperationException>(() => record.Key);
    }

    [Fact]
    public void PasteRecord_StoresAndRetrievesCorrectly() {
        var text = "Pasted Text";
        var record = new AnsiRecord(text);

        Assert.Equal(AnsiRecordType.PASTE, record.Type);
        Assert.Equal(text, record.Pasted);
    }

    [Fact]
    public void QueryRecord_StoresAndRetrievesCorrectly() {
        var query = VTQuery.WinState(true);
        var record = new AnsiRecord(query);

        Assert.Equal(AnsiRecordType.VT_QUERY, record.Type);
        Assert.Equal(query.Type, record.Query.Type);
        Assert.True(record.Query.AsWinState);
    }

    [Fact]
    public void NoneRecord_IsCorrect() {
        var record = AnsiRecord.None;
        Assert.Equal(AnsiRecordType.NONE, record.Type);
        Assert.Equal("None", record.ToString());
    }
}
