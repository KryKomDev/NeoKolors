//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Common;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Exceptions;

namespace NeoKolors.Console.Tests;

[Collection("ConsoleTests")]
public class VTQueryTimeoutTests : IDisposable {

    private class NullOutputDriver : IOutputDriver {
        public void Write(ReadOnlySpan<char> value) { }
        public void Write(string value)             { }
        public void Clear()                         { }
        public void Dispose()                       { }
    }

    private readonly IOutputDriver _prevOutput;

    public VTQueryTimeoutTests() {
        _prevOutput            = NKConsole.OutputDriver;
        NKConsole.OutputDriver = new NullOutputDriver();
    }

    public void Dispose() {
        NKConsole.OutputDriver = _prevOutput;
    }

    [Fact]
    public void VTQueryTimeoutException_StoresQueryAndTimeout() {
        var query   = VTQuery.RequestRedefineColor(1);
        var timeout = TimeSpan.FromMilliseconds(250);
        var ex      = new VTQueryTimeoutException(in query, timeout);

        Assert.Equal(query, ex.Query);
        Assert.Equal(timeout, ex.Timeout);
        Assert.Contains("250ms", ex.Message);
        Assert.IsAssignableFrom<TimeoutException>(ex);
    }

    [Fact]
    public void VTQueryTimeoutException_CustomMessageAndInnerException() {
        var inner = new InvalidOperationException("Inner error");
        var ex1   = new VTQueryTimeoutException("Custom timeout message");
        var ex2   = new VTQueryTimeoutException("Custom with inner", inner);

        Assert.Equal("Custom timeout message", ex1.Message);
        Assert.Equal("Custom with inner", ex2.Message);
        Assert.Same(inner, ex2.InnerException);
    }

    [Fact]
    public void VTQuery_Matches_ExactTypes() {
        var winStateReq = VTQuery.RequestWinState;
        var primaryReq  = VTQuery.RequestPrimaryDA;
        var focusReq    = VTQuery.RequestDec(EscapeCodes.DecMode.REPORT_FOCUS);
        var altScrReq   = VTQuery.RequestDec(EscapeCodes.DecMode.ALTERNATE_BUFFER);

        Assert.True(winStateReq.Matches(VTQuery.WinState(true)));
        Assert.False(winStateReq.Matches(primaryReq));
        Assert.True(primaryReq.Matches(VTQuery.PrimaryDA(new PDAResponse(1, []))));
        Assert.True(focusReq.Matches(focusReq));
        Assert.False(focusReq.Matches(altScrReq));
    }

    [Fact]
    public void VTQuery_Matches_OscModesAndArguments() {
        var fgReq = VTQuery.RequestOsc(EscapeCodes.OscMode.CHANGE_TEXT_FORG_COLOR);
        var bgReq = VTQuery.RequestOsc(EscapeCodes.OscMode.CHANGE_TEXT_BCKG_COLOR);

        Assert.True(fgReq.Matches(fgReq));
        Assert.False(fgReq.Matches(bgReq));

        var singleReq   = VTQuery.RequestRedefineColor(1);
        var response1   = VTQuery.RedefineColor(1, NKColor.FromRgb(255, 0, 0));
        var response2   = VTQuery.RedefineColor(2, NKColor.FromRgb(0, 255, 0));

        Assert.True(singleReq.Matches(response1));
        Assert.False(singleReq.Matches(response2));

        var batchReq = VTQuery.RequestRedefineColors(0, 1, 2);
        Assert.True(batchReq.Matches(response1));
        Assert.True(batchReq.Matches(response2));

        var response3 = VTQuery.RedefineColor(3, NKColor.FromRgb(0, 0, 255));
        Assert.False(batchReq.Matches(response3));

        var multiResponseMatch = VTQuery.RedefineColors(new Dictionary<int, NKColor> {
            [1] = NKColor.FromRgb(255, 0, 0),
            [5] = NKColor.FromRgb(0, 0, 255)
        });
        
        var multiResponseNoMatch = VTQuery.RedefineColors(new Dictionary<int, NKColor> {
            [4] = NKColor.FromRgb(255, 255, 0),
            [5] = NKColor.FromRgb(0, 0, 255)
        });

        Assert.True(batchReq.Matches(multiResponseMatch));
        Assert.False(batchReq.Matches(multiResponseNoMatch));
    }

    [Fact]
    public void VTQuery_EqualityAndHashCode() {
        var q1 = VTQuery.RequestRedefineColors(0, 1, 2);
        var q2 = VTQuery.RequestRedefineColors(0, 1, 2);
        var q3 = VTQuery.RequestRedefineColors(0, 1, 3);
        var q4 = VTQuery.RequestRedefineColor(0);

        Assert.True(q1 == q2);
        Assert.False(q1 != q2);
        Assert.False(q1 == q3);
        Assert.True(q1 != q3);
        Assert.True(q1.Equals(q2));
        Assert.False(q1.Equals(q3));
        Assert.False(q1.Equals(q4));
        Assert.Equal(q1.GetHashCode(), q2.GetHashCode());
    }

    [Fact]
    public void Driver_CancelVTQuery_CancelsPendingRequest() {
        var driver = new DotnetInputDriver();
        var q1     = VTQuery.RequestRedefineColor(1);
        var q2     = VTQuery.RequestRedefineColor(2);
        var q3     = VTQuery.RequestRedefineColor(3);

        driver.RequestVTQuery(q1);
        driver.RequestVTQuery(q2);

        Assert.True(driver.CancelVTQuery(q1));
        Assert.False(driver.CancelVTQuery(q1)); // Already cancelled
        Assert.False(driver.CancelVTQuery(q3)); // Not in queue
        Assert.True(driver.CancelVTQuery(q2));
        Assert.False(driver.CancelVTQuery(q2));
    }

    [Fact]
    public void InputDriverConfig_VTRequestTimeout_HasCorrectDefaults() {
        var config = new DotnetInputDriverConfig();
        Assert.Equal(TimeSpan.FromMilliseconds(100), config.VTRequestTimeout);
        Assert.Equal(1, config.VTRequestMaxRetries);

        var customConfig = new DotnetInputDriverConfig(vtRequestTimeout: TimeSpan.FromMilliseconds(250), vtRequestMaxRetries: 3);
        Assert.Equal(TimeSpan.FromMilliseconds(250), customConfig.VTRequestTimeout);
        Assert.Equal(3, customConfig.VTRequestMaxRetries);
    }

    [Fact]
    public void DirectMode_TryReadUntil_ReturnsFalseOnTimeout() {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var success = NKConsole.TryReadUntil(
            out var result,
            out var last,
            TimeSpan.FromMilliseconds(30),
            true,
            '\a', '\\'
        );

        Assert.False(success);
        Assert.Null(result);
        Assert.Equal('\0', last);
        Assert.True(sw.ElapsedMilliseconds >= 25);
    }

    [Fact]
    public void DirectMode_ReadUntil_ThrowsVTQueryTimeoutException() {
        var ex = Assert.Throws<VTQueryTimeoutException>(() =>
            NKConsole.ReadUntil(out _, TimeSpan.FromMilliseconds(30), true, '\a')
        );
        Assert.Contains("Timed out after 30ms", ex.Message);
    }

    [Fact]
    public async Task NKConsole_GetColorAsync_ThrowsVTQueryTimeoutException() {
        var ex = await Assert.ThrowsAsync<VTQueryTimeoutException>(async () =>
            await NKConsole.GetColorAsync(NKConsoleColor.RED, TimeSpan.FromMilliseconds(30))
        );

        Assert.Equal(VTQueryType.OSC, ex.Query.Type);
        Assert.Equal(EscapeCodes.OscMode.REDEFINE_COLOR, ex.Query.OscMode);
        Assert.Equal(TimeSpan.FromMilliseconds(30), ex.Timeout);
    }

    [Fact]
    public async Task NKConsole_TryGetColorAsync_ReturnsFalseOnTimeout() {
        var (success, color) = await NKConsole.TryGetColorAsync(NKConsoleColor.RED, TimeSpan.FromMilliseconds(30));
        Assert.False(success);
        Assert.Equal(default, color);
    }

    [Fact]
    public async Task NKConsole_TryGetColorsAsync_ReturnsFalseOnTimeout() {
        var (success, colors) = await NKConsole.TryGetColorsAsync([0, 1, 2], TimeSpan.FromMilliseconds(30));
        Assert.False(success);
        Assert.Empty(colors);
    }

    [Fact]
    public async Task NKConsole_GetColorAsync_ThrowsOperationCanceledException_WhenCancelled() {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await NKConsole.GetColorAsync(NKConsoleColor.RED, TimeSpan.FromSeconds(5), cts.Token)
        );
    }
}
