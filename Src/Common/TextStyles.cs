//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text;
using HasFlagExtension;
using static NeoKolors.Common.TextStyles;

namespace NeoKolors.Common;

/// <summary>
/// style types
/// </summary>
[Flags]
[HasFlagPrefix("Is")]
public enum TextStyles : byte {
    
    [ExcludeFlag]
    NONE          = 0,
    
    BOLD          = 1 << 0,
    FAINT         = 1 << 1,
    ITALIC        = 1 << 2,
    UNDERLINE     = 1 << 3,
    BLINK         = 1 << 4,
    NEGATIVE      = 1 << 5,
    INVISIBLE     = 1 << 6,
    STRIKETHROUGH = 1 << 7,
    
    [ExcludeFlag]
    ALL           = BOLD | FAINT | ITALIC | UNDERLINE | BLINK | NEGATIVE | INVISIBLE | STRIKETHROUGH,
}

public static partial class TextStylesExtensions {

    /// <summary>
    /// Applies the specified text style to the given string.
    /// </summary>
    /// <param name="c">The string to which the text style will be applied.</param>
    /// <param name="style">The <see cref="TextStyles"/> value specifying the styles
    /// to apply, such as bold, italic, or underline.</param>
    /// <returns>A new string with the specified text styles applied.</returns>
    public static string AddCStyle(this char c, TextStyles style) => 
        c.ToString().AddCStyle(style);

    /// <param name="styles">The combination of <see cref="TextStyles"/> specifying the styles to include, such as
    /// bold, italic, or underline.</param>
    extension(TextStyles styles) {
        
        /// <summary>
        /// Generates the ANSI escape sequence string representing the specified text styles.
        /// </summary>
        /// <returns>A string containing the ANSI escape sequence for the specified styles, or an empty string if no
        /// styles are provided.</returns>
        public string GetEscSeq() {
            if (styles == NONE) return string.Empty;
        
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
            if (styles == NONE) return string.Empty;
        
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
            if (styles == NONE) return string.Empty;
        
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
        public string GetEscSeq(TextStyles previous) {
            var off =  previous & ~styles;
            var on  = ~previous &  styles;

            if (off == NONE && on == NONE) return string.Empty;
            
            var sb = new StringBuilder("\e[");
            
            off.AppendNegModes(sb);
            on .AppendPosModes(sb);

            sb.Remove(sb.Length - 1, 1);
            sb.Append('m');
            
            return sb.ToString();
        }

        internal void AppendPosModes(StringBuilder sb) {
            sb.Append(styles.GetIsBold()          ? "1;" : "");
            sb.Append(styles.GetIsFaint()         ? "2;" : "");
            sb.Append(styles.GetIsItalic()        ? "3;" : "");
            sb.Append(styles.GetIsUnderline()     ? "4;" : "");
            sb.Append(styles.GetIsBlink()         ? "5;" : "");
            sb.Append(styles.GetIsNegative()      ? "7;" : "");
            sb.Append(styles.GetIsInvisible()     ? "8;" : "");
            sb.Append(styles.GetIsStrikethrough() ? "9;" : "");
        }

        internal void AppendNegModes(StringBuilder sb) {
            sb.Append(styles.GetIsBold()          ? "22;" : "");
            sb.Append(styles.GetIsFaint()         ? "22;" : "");
            sb.Append(styles.GetIsItalic()        ? "23;" : "");
            sb.Append(styles.GetIsUnderline()     ? "24;" : "");
            sb.Append(styles.GetIsBlink()         ? "25;" : "");
            sb.Append(styles.GetIsNegative()      ? "27;" : "");
            sb.Append(styles.GetIsInvisible()     ? "28;" : "");
            sb.Append(styles.GetIsStrikethrough() ? "29;" : "");
        }
    }
}