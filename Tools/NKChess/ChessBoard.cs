using System.Text;

namespace NKChess;

public readonly struct Move {
    public readonly int From;
    public readonly int To;
    public readonly char Promotion; // 'q', 'r', 'b', 'n' or '\0'

    public Move(int from, int to, char promotion = '\0') {
        From = from;
        To = to;
        Promotion = promotion;
    }

    public string ToUci() {
        const string files = "abcdefgh";
        const string ranks = "87654321";
        string fromStr = $"{files[From % 8]}{ranks[From / 8]}";
        string toStr = $"{files[To % 8]}{ranks[To / 8]}";
        string promStr = Promotion != '\0' ? Promotion.ToString() : "";

        return fromStr + toStr + promStr;
    }

    public static Move ParseUci(string uci) {
        if (string.IsNullOrEmpty(uci) || uci.Length < 4) return default;

        int fromFile = uci[0] - 'a';
        int fromRank = '8' - uci[1];
        int toFile = uci[2] - 'a';
        int toRank = '8' - uci[3];
        char prom = uci.Length > 4 ? char.ToLower(uci[4]) : '\0';

        return new Move(fromRank * 8 + fromFile, toRank * 8 + toFile, prom);
    }
}

public class ChessBoard {
    public readonly char[] Squares = new char[64];
    public char ActiveColor = 'w';
    public string CastlingRights = "KQkq";
    public string EnPassantSquare = "-";
    public int HalfmoveClock = 0;
    public int FullmoveNumber = 1;

    public ChessBoard() {
        Reset();
    }

    public void Reset() {
        ParseFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
    }

    public void ParseFen(string fen) {
        var parts = fen.Split(' ');

        // 1. Board positions
        var ranks = parts[0].Split('/');

        for (int r = 0; r < 8; r++) {
            int col = 0;

            foreach (char c in ranks[r]) {
                if (char.IsDigit(c)) {
                    int emptySquares = c - '0';

                    for (int i = 0; i < emptySquares; i++) {
                        Squares[r * 8 + col] = '.';
                        col++;
                    }
                }
                else {
                    Squares[r * 8 + col] = c;
                    col++;
                }
            }
        }

        // 2. Active color
        ActiveColor = parts.Length > 1 ? parts[1][0] : 'w';

        // 3. Castling rights
        CastlingRights = parts.Length > 2 ? parts[2] : "KQkq";

        // 4. En passant square
        EnPassantSquare = parts.Length > 3 ? parts[3] : "-";

        // 5. Halfmove clock
        HalfmoveClock = parts.Length > 4 && int.TryParse(parts[4], out int hm) ? hm : 0;

        // 6. Fullmove number
        FullmoveNumber = parts.Length > 5 && int.TryParse(parts[5], out int fm) ? fm : 1;
    }

    public string ToFen() {
        var sb = new StringBuilder();

        for (int r = 0; r < 8; r++) {
            int emptyCount = 0;

            for (int c = 0; c < 8; c++) {
                char piece = Squares[r * 8 + c];

                if (piece == '.') {
                    emptyCount++;
                }
                else {
                    if (emptyCount > 0) {
                        sb.Append(emptyCount);
                        emptyCount = 0;
                    }

                    sb.Append(piece);
                }
            }

            if (emptyCount > 0) {
                sb.Append(emptyCount);
            }

            if (r < 7) sb.Append('/');
        }

        sb.Append(' ').Append(ActiveColor);
        sb.Append(' ').Append(CastlingRights);
        sb.Append(' ').Append(EnPassantSquare);
        sb.Append(' ').Append(HalfmoveClock);
        sb.Append(' ').Append(FullmoveNumber);

        return sb.ToString();
    }

    public bool IsInCheck(char color) {
        char kingChar = color == 'w' ? 'K' : 'k';
        int kingIdx = -1;

        for (int i = 0; i < 64; i++) {
            if (Squares[i] == kingChar) {
                kingIdx = i;

                break;
            }
        }

        if (kingIdx == -1) return false;

        char oldActive = ActiveColor;
        ActiveColor = color == 'w' ? 'b' : 'w';
        var oppMoves = GetPseudoLegalMoves();
        ActiveColor = oldActive;

        foreach (var move in oppMoves) {
            if (move.To == kingIdx) return true;
        }

        return false;
    }

    public List<Move> GetLegalMoves() {
        var pseudo = GetPseudoLegalMoves();
        var legal = new List<Move>();

        foreach (var move in pseudo) {
            var temp = new ChessBoard();
            Array.Copy(Squares, temp.Squares, 64);
            temp.ActiveColor = ActiveColor;
            temp.CastlingRights = CastlingRights;
            temp.EnPassantSquare = EnPassantSquare;

            temp.MakeMoveInternal(move);

            // Special castling validation: King cannot start, cross, or end in check
            if (char.ToLower(Squares[move.From]) == 'k' && Math.Abs(move.From % 8 - move.To % 8) == 2) {
                if (IsInCheck(ActiveColor)) continue;

                int step = (move.To % 8 - move.From % 8) > 0 ? 1 : -1;
                int midIdx = move.From + step;

                var tempMid = new ChessBoard();
                Array.Copy(Squares, tempMid.Squares, 64);
                tempMid.ActiveColor = ActiveColor;
                tempMid.CastlingRights = CastlingRights;
                tempMid.EnPassantSquare = EnPassantSquare;

                tempMid.Squares[midIdx] = tempMid.Squares[move.From];
                tempMid.Squares[move.From] = '.';

                if (tempMid.IsInCheck(ActiveColor)) continue;
            }

            if (!temp.IsInCheck(ActiveColor)) {
                legal.Add(move);
            }
        }

        return legal;
    }

    public void MakeMove(Move move) {
        char piece = Squares[move.From];
        char targetPiece = Squares[move.To];

        if (char.ToLower(piece) == 'p' || targetPiece != '.') {
            HalfmoveClock = 0;
        }
        else {
            HalfmoveClock++;
        }

        string nextEnPassant = "-";

        if (char.ToLower(piece) == 'p' && Math.Abs(move.From / 8 - move.To / 8) == 2) {
            int midRank = (move.From / 8 + move.To / 8) / 2;
            int file = move.From % 8;
            nextEnPassant = $"{(char)('a' + file)}{(char)('8' - midRank)}";
        }

        EnPassantSquare = nextEnPassant;

        if (piece == 'K') {
            CastlingRights = CastlingRights.Replace("K", "").Replace("Q", "");
        }
        else if (piece == 'k') {
            CastlingRights = CastlingRights.Replace("k", "").Replace("q", "");
        }

        if (move.From == 63 || move.To == 63) CastlingRights = CastlingRights.Replace("K", "");
        if (move.From == 56 || move.To == 56) CastlingRights = CastlingRights.Replace("Q", "");
        if (move.From == 7 || move.To == 7) CastlingRights = CastlingRights.Replace("k", "");
        if (move.From == 0 || move.To == 0) CastlingRights = CastlingRights.Replace("q", "");

        if (string.IsNullOrEmpty(CastlingRights)) CastlingRights = "-";

        MakeMoveInternal(move);

        ActiveColor = ActiveColor == 'w' ? 'b' : 'w';

        if (ActiveColor == 'w') {
            FullmoveNumber++;
        }
    }

    public void MakeMoveInternal(Move move) {
        char piece = Squares[move.From];

        if (char.ToLower(piece) == 'p' && (move.From % 8 != move.To % 8) && Squares[move.To] == '.') {
            int capturedIdx = (move.From / 8) * 8 + (move.To % 8);
            Squares[capturedIdx] = '.';
        }

        if (char.ToLower(piece) == 'k') {
            int diff = move.To - move.From;

            if (diff == 2) {
                int rookFrom = move.From + 3;
                int rookTo = move.From + 1;
                Squares[rookTo] = Squares[rookFrom];
                Squares[rookFrom] = '.';
            }
            else if (diff == -2) {
                int rookFrom = move.From - 4;
                int rookTo = move.From - 1;
                Squares[rookTo] = Squares[rookFrom];
                Squares[rookFrom] = '.';
            }
        }

        char finalPiece = piece;

        if (move.Promotion != '\0') {
            finalPiece = ActiveColor == 'w' ? char.ToUpper(move.Promotion) : char.ToLower(move.Promotion);
        }

        Squares[move.To] = finalPiece;
        Squares[move.From] = '.';
    }

    private List<Move> GetPseudoLegalMoves() {
        var moves = new List<Move>();
        char color = ActiveColor;

        for (int idx = 0; idx < 64; idx++) {
            char piece = Squares[idx];

            if (piece == '.') continue;

            bool isWhite = char.IsUpper(piece);

            if ((color == 'w' && !isWhite) || (color == 'b' && isWhite)) continue;

            char type = char.ToLower(piece);
            int r = idx / 8;
            int c = idx % 8;

            if (type == 'p') {
                int dir = color == 'w' ? -1 : 1;
                int startRow = color == 'w' ? 6 : 1;
                int promoRow = color == 'w' ? 0 : 7;

                int nextIdx = idx + dir * 8;

                if (nextIdx is >= 0 and < 64 && Squares[nextIdx] == '.') {
                    if (nextIdx / 8 == promoRow) {
                        moves.Add(new Move(idx, nextIdx, 'q'));
                        moves.Add(new Move(idx, nextIdx, 'r'));
                        moves.Add(new Move(idx, nextIdx, 'b'));
                        moves.Add(new Move(idx, nextIdx, 'n'));
                    }
                    else {
                        moves.Add(new Move(idx, nextIdx));

                        if (r == startRow) {
                            int doubleIdx = idx + dir * 16;

                            if (Squares[doubleIdx] == '.') {
                                moves.Add(new Move(idx, doubleIdx));
                            }
                        }
                    }
                }

                int[] captureCols = [c - 1, c + 1];

                foreach (var cc in captureCols) {
                    if (cc is < 0 or > 7) continue;

                    int targetIdx = (r + dir) * 8 + cc;

                    if (targetIdx is < 0 or >= 64) continue;

                    char targetPiece = Squares[targetIdx];
                    bool isOpponent = targetPiece != '.' && (color == 'w' ? char.IsLower(targetPiece) : char.IsUpper(targetPiece));

                    if (isOpponent) {
                        if (targetIdx / 8 == promoRow) {
                            moves.Add(new Move(idx, targetIdx, 'q'));
                            moves.Add(new Move(idx, targetIdx, 'r'));
                            moves.Add(new Move(idx, targetIdx, 'b'));
                            moves.Add(new Move(idx, targetIdx, 'n'));
                        }
                        else {
                            moves.Add(new Move(idx, targetIdx));
                        }
                    }

                    string squareName = $"{(char)('a' + cc)}{(char)('8' - (r + dir))}";

                    if (EnPassantSquare == squareName) {
                        moves.Add(new Move(idx, targetIdx));
                    }
                }
            }
            else if (type == 'n') {
                int[] dr = [-2, -2, -1, -1, 1, 1, 2, 2];
                int[] dc = [-1, 1, -2, 2, -2, 2, -1, 1];

                for (int i = 0; i < 8; i++) {
                    int nr = r + dr[i];
                    int nc = c + dc[i];

                    if (nr is >= 0 and < 8 && nc is >= 0 and < 8) {
                        int targetIdx = nr * 8 + nc;
                        char targetPiece = Squares[targetIdx];

                        if (targetPiece == '.' || (color == 'w' ? char.IsLower(targetPiece) : char.IsUpper(targetPiece))) {
                            moves.Add(new Move(idx, targetIdx));
                        }
                    }
                }
            }
            else if (type == 'k') {
                int[] dr = [-1, -1, -1, 0, 0, 1, 1, 1];
                int[] dc = [-1, 0, 1, -1, 1, -1, 0, 1];

                for (int i = 0; i < 8; i++) {
                    int nr = r + dr[i];
                    int nc = c + dc[i];

                    if (nr is < 0 or >= 8 || nc is < 0 or >= 8) continue;

                    int targetIdx = nr * 8 + nc;
                    char targetPiece = Squares[targetIdx];

                    if (targetPiece == '.' || (color == 'w' ? char.IsLower(targetPiece) : char.IsUpper(targetPiece))) {
                        moves.Add(new Move(idx, targetIdx));
                    }
                }

                if (color == 'w') {
                    if (r != 7 || c != 4) continue;

                    if (CastlingRights.Contains('K') && Squares[61] == '.' && Squares[62] == '.') {
                        moves.Add(new Move(60, 62));
                    }

                    if (CastlingRights.Contains('Q') && Squares[59] == '.' && Squares[58] == '.' && Squares[57] == '.') {
                        moves.Add(new Move(60, 58));
                    }
                }
                else {
                    if (r != 0 || c != 4) continue;

                    if (CastlingRights.Contains('k') && Squares[5] == '.' && Squares[6] == '.') {
                        moves.Add(new Move(4, 6));
                    }

                    if (CastlingRights.Contains('q') && Squares[3] == '.' && Squares[2] == '.' && Squares[1] == '.') {
                        moves.Add(new Move(4, 2));
                    }
                }
            }
            else {
                int[][]? dirs = type switch {
                    'b' => [[-1, -1], [-1, 1], [1, -1], [1, 1]],
                    'r' => [[-1, 0], [1, 0], [0, -1], [0, 1]],
                    'q' => [[-1, -1], [-1, 1], [1, -1], [1, 1], [-1, 0], [1, 0], [0, -1], [0, 1]],
                    _   => null
                };

                if (dirs == null) continue;

                foreach (var dir in dirs) {
                    int nr = r;
                    int nc = c;

                    while (true) {
                        nr += dir[0];
                        nc += dir[1];

                        if (nr < 0 || nr >= 8 || nc < 0 || nc >= 8) break;

                        int targetIdx = nr * 8 + nc;
                        char targetPiece = Squares[targetIdx];

                        if (targetPiece == '.') {
                            moves.Add(new Move(idx, targetIdx));
                        }
                        else {
                            if (color == 'w' ? char.IsLower(targetPiece) : char.IsUpper(targetPiece)) {
                                moves.Add(new Move(idx, targetIdx));
                            }

                            break;
                        }
                    }
                }
            }
        }

        return moves;
    }
}