// NeoKolors
// Copyright (c) krystof 2026

namespace NKChess;

public class Game {
    public bool IsWhiteCpu { get; }
    public string? WhiteEnginePath { get; }
    public TimeControl WhiteTime { get; }
    
    public bool IsBlackCpu { get; }
    public string? BlackEnginePath { get; }
    public TimeControl BlackTime { get; }
    
    public ChessBoard Board { get; }
    
    private readonly List<Move> _moves = [];
    public IReadOnlyList<Move> Moves => _moves.AsReadOnly();

    public Game(
        string? whiteEnginePath,
        TimeControl whiteTime,
        string? blackEnginePath,
        TimeControl blackTime)
    {
        IsWhiteCpu = true;
        WhiteEnginePath = whiteEnginePath;
        WhiteTime = whiteTime;
        IsBlackCpu = true;
        BlackEnginePath = blackEnginePath;
        BlackTime = blackTime;
        Board = new ChessBoard();
    }

    public Game(TimeControl whiteTime, TimeControl blackTime) {
        IsWhiteCpu = false;
        WhiteTime = whiteTime;
        IsBlackCpu = false;
        BlackTime = blackTime;
        Board = new ChessBoard();
    }
    
    public void AddMove(Move move) => _moves.Add(move);
}