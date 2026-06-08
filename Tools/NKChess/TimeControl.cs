// NeoKolors
// Copyright (c) krystof 2026

using System.Diagnostics;

namespace NKChess;

public class TimeControl {
    public Stopwatch TimePlayed { get; }
    public TimeSpan? TimeLimit { get; }
    public TimeSpan Increment { get; }
    public int IncrementMoves { get; }
    
    public TimeSpan? TimeLeft => TimeLimit != null ? TimeLimit - TimePlayed.Elapsed : null;

    public TimeControl(TimeSpan? timeLimit, TimeSpan increment, int incrementMoves) {
        TimePlayed = new Stopwatch();
        TimeLimit = timeLimit;
        Increment = increment;
        IncrementMoves = incrementMoves;
    }
}