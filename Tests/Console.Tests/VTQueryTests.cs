using NeoKolors.Common;
using NeoKolors.Console.Ansi;

namespace NeoKolors.Console.Tests;

public class VTQueryTests {

    [Fact]
    public void VTQuery_RequestFactories_SetCorrectTypes() {
        Assert.Equal(VTQueryType.DEC,        VTQuery.RequestDec(EscapeCodes.DecMode.REPORT_FOCUS).Type);
        Assert.Equal(VTQueryType.WIN_STATE,  VTQuery.RequestWinState.Type);
        Assert.Equal(VTQueryType.PRIMARY_DA, VTQuery.RequestPrimaryDA.Type);
        Assert.Equal(VTQueryType.OSC,        VTQuery.RequestOsc(EscapeCodes.OscMode.REDEFINE_COLOR, 1).Type);
    }

    [Fact]
    public void VTQuery_RequestOsc_Parameterless_EscSeqIsCorrect() {
        var query = VTQuery.RequestOsc(EscapeCodes.OscMode.CHANGE_TEXT_FORG_COLOR);
        Assert.Equal(VTQueryType.OSC, query.Type);
        Assert.Empty(query.Arguments);
        Assert.Equal("\e]10;?\a", query.GetEscSeq());
    }

    [Fact]
    public void VTQuery_RequestRedefineColor_SingleArg_EscSeqIsCorrect() {
        var query1 = VTQuery.RequestRedefineColor(1);
        Assert.Equal(VTQueryType.OSC, query1.Type);
        Assert.Equal(EscapeCodes.OscMode.REDEFINE_COLOR, query1.OscMode);
        Assert.Single(query1.Arguments);
        Assert.Equal(1, query1.Arguments[0]);
        Assert.Equal("\e]4;1;?\a", query1.GetEscSeq());

        var queryRed = VTQuery.RequestRedefineColor(NKConsoleColor.RED);
        Assert.Equal((int)NKConsoleColor.RED, queryRed.Arguments[0]);
        Assert.Equal($"\e]4;{(int)NKConsoleColor.RED};?\a", queryRed.GetEscSeq());
    }

    [Fact]
    public void VTQuery_RequestRedefineColors_MultiArg_EscSeqIsCorrect() {
        var query = VTQuery.RequestRedefineColors(0, 1, 2);
        Assert.Equal(VTQueryType.OSC, query.Type);
        Assert.Equal(3, query.Arguments.Count);
        Assert.Equal([0, 1, 2], query.Arguments);
        Assert.Equal("\e]4;0;?;1;?;2;?\a", query.GetEscSeq());
    }

    [Fact]
    public void VTQuery_RedefineColorResponse_StoresAndRetrievesData() {
        var redColor = NKColor.FromRgb(255, 0, 0);
        var query    = VTQuery.RedefineColor(1, redColor);

        Assert.Equal(VTQueryType.OSC, query.Type);
        Assert.Equal(EscapeCodes.OscMode.REDEFINE_COLOR, query.OscMode);
        Assert.Equal(1, query.RedefineColorResponse.Index);
        Assert.Equal(redColor, query.RedefineColorResponse.Color);
        Assert.Equal(redColor, query.ColorResponse);
        Assert.Equal((1, redColor), query.OscResponse);
    }

    [Fact]
    public void VTQuery_RedefineColorsResponse_StoresAndRetrievesMultipleColors() {
        var colors = new Dictionary<int, NKColor> {
            { 0, NKColor.FromRgb(0, 0, 0) },
            { 1, NKColor.FromRgb(255, 0, 0) }
        };
        var query = VTQuery.RedefineColors(colors);

        Assert.Equal(VTQueryType.OSC, query.Type);
        Assert.Equal(EscapeCodes.OscMode.REDEFINE_COLOR, query.OscMode);
        Assert.Equal(2, query.RedefineColorsResponse.Count);
        Assert.Equal(NKColor.FromRgb(0, 0, 0), query.RedefineColorsResponse[0]);
        Assert.Equal(NKColor.FromRgb(255, 0, 0), query.RedefineColorsResponse[1]);
    }

    [Fact]
    public void VTQuery_PrimaryDAResponse_StoresData() {
        var response = new PDAResponse(VTType.VT420, VTCapabilities.ANSI_COLOR | VTCapabilities.SIXEL);
        var query    = VTQuery.PrimaryDA(response);

        Assert.Equal(VTQueryType.PRIMARY_DA, query.Type);
        Assert.Equal(VTType.VT420,           query.PrimaryDAResponse!.Value.Type);
        Assert.True(query.PrimaryDAResponse.Value.Capabilities!.Value.HasFlag(VTCapabilities.ANSI_COLOR));
        Assert.True(query.PrimaryDAResponse.Value.Capabilities!.Value.HasFlag(VTCapabilities.SIXEL));
    }

    [Fact]
    public void PDAResponse_ConstructorWithInts_MapsCorrectly() {
        // 64 -> VT420
        // 4 -> SIXEL, 22 -> ANSI_COLOR
        var response = new PDAResponse(64, [4, 22]);

        Assert.Equal(VTType.VT420, response.Type);
        Assert.True(response.Capabilities!.Value.HasFlag(VTCapabilities.SIXEL));
        Assert.True(response.Capabilities.Value.HasFlag(VTCapabilities.ANSI_COLOR));
    }

    [Fact]
    public void VTQuery_WinStateResponse_StoresData() {
        var query = VTQuery.WinState(true);
        Assert.Equal(VTQueryType.WIN_STATE, query.Type);
        Assert.True(query.AsWinState);
    }

    [Fact]
    public void VTQuery_DecResponse_StoresData() {
        var query = VTQuery.Dec(1, DecReqResponseType.ENABLED);
        Assert.Equal(VTQueryType.DEC,            query.Type);
        Assert.Equal(1,                          (int)query.DecMode);
        Assert.Equal(DecReqResponseType.ENABLED, query.DecResponse);
    }
}
