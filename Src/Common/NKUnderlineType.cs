// NeoKolors
// Copyright (c) 2025 KryKom

using System.Text;

namespace NeoKolors.Common;

public enum NKUnderlineType {
    NORMAL = 0,
    THICK  = 1,
    CURLY  = 2,
    DOTTED = 3,
    DASHED = 4
}

public static class NKUnderlineTypeExtensions {
    extension(NKUnderlineType) {
        public static string GetEscSeq(NKUnderlineType type, bool addEsc = true) {
            return EscapeCodes.GetUnderline(type, addEsc);
        }

        public static void AppendEscSeq(
            StringBuilder   sb,
            NKUnderlineType type,
            bool            addEsc = true
        ) {
            sb.Append(EscapeCodes.GetUnderline(type, addEsc));
        }

        public static string GetEscSeq(
            NKUnderlineType prev,
            NKUnderlineType next,
            bool            addEsc = true
        ) {
            return prev == next ? string.Empty : EscapeCodes.GetUnderline(next, addEsc);
        }

        public static void AppendEscSeq(
            StringBuilder   sb,
            NKUnderlineType prev,
            NKUnderlineType next,
            bool            addEsc = true
        ) {
            if (prev == next)
                return;

            sb.Append(EscapeCodes.GetUnderline(next, addEsc));
        }
    }
}