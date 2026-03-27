using NeoKolors.Common;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Events;

namespace NeoKolors.Console.Tests.Ansi;

public class VTQueryTests {

    [Fact]
    public void VTQuery_RequestFactories_SetCorrectTypes() {
        Assert.Equal(VTQueryType.DEC, VTQuery.RequestDec(EscapeCodes.DecMode.REPORT_FOCUS).Type);
        Assert.Equal(VTQueryType.WIN_STATE, VTQuery.RequestWinState.Type);
        Assert.Equal(VTQueryType.PRIMARY_DA, VTQuery.RequestPrimaryDA.Type);
    }

    [Fact]
    public void VTQuery_PrimaryDAResponse_StoresData() {
        var response = new PDAResponse(VTType.VT420, VTCapabilities.ANSI_COLOR | VTCapabilities.SIXEL);
        var query = VTQuery.PrimaryDA(response);

        Assert.Equal(VTQueryType.PRIMARY_DA, query.Type);
        Assert.Equal(VTType.VT420, query.PrimaryDAResponse!.Value.Type);
        Assert.True(query.PrimaryDAResponse.Value.Capabilities!.Value.HasFlag(VTCapabilities.ANSI_COLOR));
        Assert.True(query.PrimaryDAResponse.Value.Capabilities!.Value.HasFlag(VTCapabilities.SIXEL));
    }

    [Fact]
    public void PDAResponse_ConstructorWithInts_MapsCorrectly() {
        // 64 -> VT420
        // 4 -> SIXEL, 22 -> ANSI_COLOR
        var response = new PDAResponse(64, new[] { 4, 22 });

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
        Assert.Equal(VTQueryType.DEC, query.Type);
        Assert.Equal(1, (int)query.DecMode);
        Assert.Equal(DecReqResponseType.ENABLED, query.DecResponse);
    }
}
