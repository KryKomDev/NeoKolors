//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text;
using HasFlagExtension;
using static NeoKolors.Common.NKTextStyles;

namespace NeoKolors.Common;

/// <summary>
/// style types
/// </summary>
[Flags]
[HasFlagPrefix("Is")]
public enum NKTextStyles : byte {

    [ExcludeFlag] NONE = 0,

    BOLD          = 1 << 0,
    FAINT         = 1 << 1,
    ITALIC        = 1 << 2,
    UNDERLINE     = 1 << 3,
    BLINK         = 1 << 4,
    NEGATIVE      = 1 << 5,
    INVISIBLE     = 1 << 6,
    STRIKETHROUGH = 1 << 7,

    [ExcludeFlag] ALL = BOLD | FAINT | ITALIC | UNDERLINE | BLINK | NEGATIVE | INVISIBLE | STRIKETHROUGH,
}

public static partial class NKTextStylesExtensions {

    /// <summary>
    /// Applies the specified text style to the given string.
    /// </summary>
    /// <param name="c">The string to which the text style will be applied.</param>
    /// <param name="style">The <see cref="NKTextStyles"/> value specifying the styles
    /// to apply, such as bold, italic, or underline.</param>
    /// <returns>A new string with the specified text styles applied.</returns>
    public static string AddCStyle(this char c, NKTextStyles style) => c.ToString().AddCStyle(style);

    /// <param name="styles">The combination of <see cref="NKTextStyles"/> specifying the styles to include, such as
    /// bold, italic, or underline.</param>
    extension(NKTextStyles styles) {

        /// <summary>
        /// Generates the ANSI escape sequence string representing the specified text styles.
        /// </summary>
        /// <returns>A string containing the ANSI escape sequence for the specified styles, or an empty string if no
        /// styles are provided.</returns>
        public string GetEscSeq() {
            if (styles == NONE)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append("\e[");
            styles.AppendPosModes(sb);

            sb.Remove(sb.Length - 1, 1);
            sb.Append('m');

            return sb.ToString();
        }

        /// <summary>
        /// Generates an escape sequence string based on the provided text styles.
        /// The escape sequence can be used to apply formatting (e.g., bold, italic, underline) to text in terminal
        /// environments.
        /// </summary>
        /// <returns>A string containing the escape sequence corresponding to the specified styles, or an empty string
        /// if no styles are provided.</returns>
        public string GetOvrEscSeq() {
            if (styles == NONE)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append("\e[0;");
            styles.AppendPosModes(sb);

            sb.Remove(sb.Length - 1, 1);
            sb.Append('m');

            return sb.ToString();
        }

        /// <summary>
        /// Generates the ANSI escape sequence to reset the specified text styles.
        /// </summary>
        /// <returns>A string containing the ANSI escape sequence to reset the specified styles, or an empty string
        /// if no styles are specified.</returns>
        public string GetNegEscSeq() {
            if (styles == NONE)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append("\e[");
            styles.AppendNegModes(sb);

            sb.Remove(sb.Length - 1, 1);
            sb.Append('m');

            return sb.ToString();
        }

        /// <summary>
        /// Generates an escape sequence representing the difference between the two styles.
        /// </summary>
        /// <returns>A string containing the formatted ANSI escape sequence for the specified text styles,
        /// or an empty string if no styles are applied.</returns>
        public string GetEscSeq(NKTextStyles previous) {
            var off = previous  & ~styles;
            var on  = ~previous & styles;

            if (off == NONE && on == NONE)
                return string.Empty;

            var sb = new StringBuilder("\e[");

            off.AppendNegModes(sb);
            on.AppendPosModes(sb);

            sb.Remove(sb.Length - 1, 1);
            sb.Append('m');

            return sb.ToString();
        }

        /// <summary>
        /// Appends the activating ANSI escape sequence for the specified active text styles.
        /// </summary>
        internal void AppendPosModes(StringBuilder sb) {
            sb.Append(styles.GetIsBold() ? "1;" : "");
            sb.Append(styles.GetIsFaint() ? "2;" : "");
            sb.Append(styles.GetIsItalic() ? "3;" : "");
            sb.Append(styles.GetIsUnderline() ? "4;" : "");
            sb.Append(styles.GetIsBlink() ? "5;" : "");
            sb.Append(styles.GetIsNegative() ? "7;" : "");
            sb.Append(styles.GetIsInvisible() ? "8;" : "");
            sb.Append(styles.GetIsStrikethrough() ? "9;" : "");
        }

        /// <summary>
        /// Appends the terminating ANSI escape sequence for the specified active text styles.
        /// </summary>
        internal void AppendNegModes(StringBuilder sb) {
            sb.Append(styles.GetIsBold() ? "22;" : "");
            sb.Append(styles.GetIsFaint() ? "22;" : "");
            sb.Append(styles.GetIsItalic() ? "23;" : "");
            sb.Append(styles.GetIsUnderline() ? "24;" : "");
            sb.Append(styles.GetIsBlink() ? "25;" : "");
            sb.Append(styles.GetIsNegative() ? "27;" : "");
            sb.Append(styles.GetIsInvisible() ? "28;" : "");
            sb.Append(styles.GetIsStrikethrough() ? "29;" : "");
        }
    }

    extension(NKTextStyles) {

        /// <summary>
        /// Generates an escape sequence string for transitioning between two sets of text styles.
        /// </summary>
        /// <param name="prev">The previously applied text styles.</param>
        /// <param name="next">The new text styles to be applied.</param>
        /// <param name="inherit">The inherited text styles that remain unchanged.</param>
        /// <param name="addEsc">
        /// A boolean indicating whether the escape sequence prefix and terminator should be added
        /// to the output string.
        /// </param>
        /// <returns>
        /// A string representing the escape sequence for applying the specified style changes.
        /// </returns>
        public static string GetEscSeq(
            NKTextStyles prev,
            NKTextStyles next,
            NKTextStyles inherit,
            bool         addEsc = true
        ) {
            var sb = new StringBuilder();

            if (addEsc)
                sb.Append("\e[");

            var on  = next  & ~inherit & ~prev;
            var off = ~next & ~inherit & prev;

            // This mess had to be made because of the way VTs handle bold/faint
            // Screw you who designed the codes.
            if (off.GetIsBold() ^ off.GetIsFaint()) {
                sb.Append("22;");

                if (on.GetIsBold() || on.GetIsFaint()) {
                    sb.Append(on.GetIsBold() ? "1;" : "2;");
                }
                else {
                    sb.Append(prev.GetIsBold() ? "2;" : "1;");
                }
            }
            else if (off.GetIsBold() && off.GetIsFaint()) {
                sb.Append("22;");
                if (on.GetIsBold())  sb.Append("1;");
                if (on.GetIsFaint()) sb.Append("2;");
            }
            else {
                if (on.GetIsBold())  sb.Append("1;");
                if (on.GetIsFaint()) sb.Append("2;");
            }

            if (off.GetIsItalic())        sb.Append("23;");
            if (off.GetIsUnderline())     sb.Append("24;");
            if (off.GetIsBlink())         sb.Append("25;");
            if (off.GetIsNegative())      sb.Append("27;");
            if (off.GetIsInvisible())     sb.Append("28;");
            if (off.GetIsStrikethrough()) sb.Append("29;");
            
            if (on.GetIsItalic())        sb.Append("3;");
            if (on.GetIsUnderline())     sb.Append("4;");
            if (on.GetIsBlink())         sb.Append("5;");
            if (on.GetIsNegative())      sb.Append("7;");
            if (on.GetIsInvisible())     sb.Append("8;");
            if (on.GetIsStrikethrough()) sb.Append("9;");

            if (sb.Length == 0 || (addEsc && sb.Length == 2))
                return string.Empty;

            if (sb[^1] != ';')
                return string.Empty;

            if (addEsc) {
                sb.Remove(sb.Length - 1, 1);
                sb.Append('m');
            }
                
            return sb.ToString();
        }

        public static string GetEscSeq(
            NKTextStyles next,
            NKTextStyles inherit,
            bool         force  = false,
            bool         addEsc = true
        ) {
            if (force)
                inherit = NONE;

            return NKTextStyles.GetEscSeq(NONE, next, inherit, addEsc);
        }
    }
}