// NeoKolors
// Copyright (c) 2026 KryKom

using System.Text;
using static NeoKolors.Common.NKTextStyles;

namespace NeoKolors.Common.Tests;

public class NKTextStylesTests {

    #region Flag and Getter Tests

    [Fact]
    public void GetIsFlags_SingleFlags_ReturnTrueOnlyForSpecified() {
        Assert.True(BOLD.GetIsBold());
        Assert.False(BOLD.GetIsItalic());

        Assert.True(FAINT.GetIsFaint());
        Assert.False(FAINT.GetIsBold());

        Assert.True(ITALIC.GetIsItalic());
        Assert.False(ITALIC.GetIsUnderline());

        Assert.True(UNDERLINE.GetIsUnderline());
        Assert.False(UNDERLINE.GetIsBlink());

        Assert.True(BLINK.GetIsBlink());
        Assert.False(BLINK.GetIsNegative());

        Assert.True(NEGATIVE.GetIsNegative());
        Assert.False(NEGATIVE.GetIsInvisible());

        Assert.True(INVISIBLE.GetIsInvisible());
        Assert.False(INVISIBLE.GetIsStrikethrough());

        Assert.True(STRIKETHROUGH.GetIsStrikethrough());
        Assert.False(STRIKETHROUGH.GetIsBold());
    }

    [Fact]
    public void GetIsFlags_CombinedFlags_ReturnTrueForAllComponents() {
        var styles = BOLD | ITALIC | STRIKETHROUGH;

        Assert.True(styles.GetIsBold());
        Assert.True(styles.GetIsItalic());
        Assert.True(styles.GetIsStrikethrough());
        Assert.False(styles.GetIsUnderline());
        Assert.False(styles.GetIsFaint());
    }

    #endregion

    #region Sequence Generation Tests

    [Fact]
    public void GetEscSeq_None_ReturnsEmpty() {
        Assert.Equal(string.Empty, NONE.GetEscSeq());
        Assert.Equal(string.Empty, NONE.GetOvrEscSeq());
        Assert.Equal(string.Empty, NONE.GetNegEscSeq());
    }

    [Theory]
    [InlineData(BOLD,          "\e[1m")]
    [InlineData(FAINT,         "\e[2m")]
    [InlineData(ITALIC,        "\e[3m")]
    [InlineData(UNDERLINE,     "\e[4m")]
    [InlineData(BLINK,         "\e[5m")]
    [InlineData(NEGATIVE,      "\e[7m")]
    [InlineData(INVISIBLE,     "\e[8m")]
    [InlineData(STRIKETHROUGH, "\e[9m")]
    public void GetEscSeq_SingleStyle_ReturnsCorrectAnsi(NKTextStyles style, string expected) {
        Assert.Equal(expected, style.GetEscSeq());
    }

    [Fact]
    public void GetEscSeq_CombinedStyles_AppendsAllCodes() {
        var seq = (BOLD | ITALIC).GetEscSeq();
        Assert.Equal("\e[1;3m", seq);
    }

    [Fact]
    public void GetOvrEscSeq_PrependsResetZero() {
        var seq = BOLD.GetOvrEscSeq();
        Assert.Equal("\e[0;1m", seq);

        var combined = (BOLD | ITALIC).GetOvrEscSeq();
        Assert.Equal("\e[0;1;3m", combined);
    }

    [Theory]
    [InlineData(BOLD,          "\e[22m")]
    [InlineData(FAINT,         "\e[22m")]
    [InlineData(ITALIC,        "\e[23m")]
    [InlineData(UNDERLINE,     "\e[24m")]
    [InlineData(BLINK,         "\e[25m")]
    [InlineData(NEGATIVE,      "\e[27m")]
    [InlineData(INVISIBLE,     "\e[28m")]
    [InlineData(STRIKETHROUGH, "\e[29m")]
    public void GetNegEscSeq_SingleStyle_ReturnsCorrectResetAnsi(NKTextStyles style, string expected) {
        Assert.Equal(expected, style.GetNegEscSeq());
    }

    [Fact]
    public void GetNegEscSeq_CombinedStyles_ResetsAll() {
        var seq = (BOLD | ITALIC).GetNegEscSeq();
        Assert.Equal("\e[22;23m", seq);
    }

    [Fact]
    public void GetEscSeq_PreviousToCurrent_EmitsDiff() {
        // Turning off BOLD (22) and turning on ITALIC (3)
        var seq = ITALIC.GetEscSeq(BOLD);
        Assert.Equal("\e[22;3m", seq);

        // Same styles -> no transition needed
        Assert.Equal(string.Empty, BOLD.GetEscSeq(BOLD));
    }

    [Fact]
    public void Static_GetEscSeq_WithInheritAndEsc() {
        // When next adds ITALIC but inherits BOLD
        var seqWithEsc = NKTextStyles.GetEscSeq(BOLD, BOLD | ITALIC, BOLD, addEsc: true);
        Assert.Equal("\e[3m", seqWithEsc);

        var seqWithoutEsc = NKTextStyles.GetEscSeq(BOLD, BOLD | ITALIC, BOLD, addEsc: false);
        Assert.Equal("3;", seqWithoutEsc);

        // No changes
        var noChange = NKTextStyles.GetEscSeq(BOLD, BOLD, NONE, addEsc: true);
        Assert.Equal(string.Empty, noChange);

        var noChangeNoEsc = NKTextStyles.GetEscSeq(BOLD, BOLD, NONE, addEsc: false);
        Assert.Equal(string.Empty, noChangeNoEsc);
    }

    [Fact]
    public void Static_GetEscSeq_Force_OverridesInherit() {
        // Without force, inherited style is ignored
        var seqNormal = NKTextStyles.GetEscSeq(BOLD | ITALIC, BOLD, force: false, addEsc: true);
        Assert.Equal("\e[3m", seqNormal);

        // With force, inherit is cleared to NONE so both BOLD and ITALIC are emitted
        var seqForced = NKTextStyles.GetEscSeq(BOLD | ITALIC, BOLD, force: true, addEsc: true);
        Assert.Equal("\e[1;3m", seqForced);
    }

    [Fact]
    public void GetEscSeq_BoldToNone_EmitsReset22WithoutFaint() {
        var seq = NONE.GetEscSeq(BOLD);
        Assert.Equal("\e[22m", seq);

        var staticSeq = NKTextStyles.GetEscSeq(BOLD, NONE, NONE, addEsc: true);
        Assert.Equal("\e[22m", staticSeq);
    }

    [Fact]
    public void GetEscSeq_FaintToNone_EmitsReset22WithoutBold() {
        var seq = NONE.GetEscSeq(FAINT);
        Assert.Equal("\e[22m", seq);

        var staticSeq = NKTextStyles.GetEscSeq(FAINT, NONE, NONE, addEsc: true);
        Assert.Equal("\e[22m", staticSeq);
    }

    [Fact]
    public void GetEscSeq_BoldToFaint_TransitionsCleanly() {
        var seq = FAINT.GetEscSeq(BOLD);
        Assert.Equal("\e[22;2m", seq);

        var staticSeq = NKTextStyles.GetEscSeq(BOLD, FAINT, NONE, addEsc: true);
        Assert.Equal("\e[22;2m", staticSeq);
    }

    [Fact]
    public void GetEscSeq_FaintToBold_TransitionsCleanly() {
        var seq = BOLD.GetEscSeq(FAINT);
        Assert.Equal("\e[22;1m", seq);

        var staticSeq = NKTextStyles.GetEscSeq(FAINT, BOLD, NONE, addEsc: true);
        Assert.Equal("\e[22;1m", staticSeq);
    }

    [Fact]
    public void GetEscSeq_CombinedBoldFaintTransitions() {
        // From BOLD|FAINT to BOLD (turning off faint, keeping bold)
        var toBold = BOLD.GetEscSeq(BOLD | FAINT);
        Assert.Equal("\e[22;1m", toBold);

        // From BOLD|FAINT to FAINT (turning off bold, keeping faint)
        var toFaint = FAINT.GetEscSeq(BOLD | FAINT);
        Assert.Equal("\e[22;2m", toFaint);

        // From BOLD|FAINT to NONE (turning off both)
        var toNone = NONE.GetEscSeq(BOLD | FAINT);
        Assert.Equal("\e[22m", toNone);

        // Single 22 emitted for negative escape sequence with both bold and faint
        var negSeq = (BOLD | FAINT).GetNegEscSeq();
        Assert.Equal("\e[22m", negSeq);
    }

    #endregion
}
