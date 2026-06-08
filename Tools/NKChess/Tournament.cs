using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NKChess;

public class Tournament {
    public string EngineAPath { get; }
    public string EngineBPath { get; }
    public int TimeLimitMs { get; }
    public int TotalPairs { get; }
    
    public int CurrentPairIndex { get; private set; }
    public int CurrentGameIndex { get; private set; } // 1 or 2
    public bool IsEngineAPlayingWhite { get; private set; }
    
    public int[] PentanomialCounts { get; } = new int[5]; // Index 0: LL, 1: LD, 2: DD, 3: WD, 4: WW
    
    public ChessBoard CurrentBoard { get; } = new();
    public List<string> CurrentMoveHistory { get; } = [];
    
    public string EngineAName { get; private set; } = "Engine A";
    public string EngineBName { get; private set; } = "Engine B";
    
    public string EngineALastInfo { get; set; } = "";
    public string EngineBLastInfo { get; set; } = "";

    public bool IsRunning { get; private set; }
    
    // Configurable options according to Navigation.md
    public string OpeningFen { get; set; } = "";
    public int MaxMoves { get; set; } = 200;
    public string PgnOutputPath { get; set; } = "tournament.pgn";
    public string EpdOutputPath { get; set; } = "tournament.epd";
    public string TournamentType { get; set; } = "Round-robin";
    public bool PlayBothColors { get; set; } = true;
    
    private CancellationTokenSource? _cts;
    private Task? _runTask;

    public event Action<string>? LogMessage;
    public event Action? ProgressUpdated;
    public event Action? BoardUpdated;
    public event Action? InfoUpdated;

    public Tournament(string engineAPath, string engineBPath, int timeLimitMs, int totalPairs) {
        EngineAPath = engineAPath;
        EngineBPath = engineBPath;
        TimeLimitMs = timeLimitMs;
        TotalPairs = totalPairs;
        
        EngineAName = Path.GetFileNameWithoutExtension(engineAPath);
        EngineBName = Path.GetFileNameWithoutExtension(engineBPath);
    }

    public void Start() {
        if (IsRunning) return;
        IsRunning = true;
        _cts = new CancellationTokenSource();
        _runTask = Task.Run(() => RunTournamentAsync(_cts.Token));
    }

    public void Stop() {
        if (!IsRunning) return;
        _cts?.Cancel();
        try {
            _runTask?.Wait();
        }
        catch {
            // ignored
        }

        IsRunning = false;
    }

    private async Task RunTournamentAsync(CancellationToken token) {
        using var engineA = new UciEngine(EngineAPath);
        using var engineB = new UciEngine(EngineBPath);

        LogMessage?.Invoke("Initializing Engine A...");
        if (!engineA.Start()) {
            LogMessage?.Invoke("FAILED to start Engine A!");
            IsRunning = false;
            return;
        }

        LogMessage?.Invoke("Initializing Engine B...");
        if (!engineB.Start()) {
            LogMessage?.Invoke("FAILED to start Engine B!");
            IsRunning = false;
            return;
        }

        // Wait a small amount of time for engines to identify
        await Task.Delay(1000, token);
        EngineAName = engineA.Name;
        EngineBName = engineB.Name;
        
        // Listen to engine output for thinking logs
        engineA.InfoReceived += (info) => {
            EngineALastInfo = ParseInfo(info);
            InfoUpdated?.Invoke();
        };
        engineB.InfoReceived += (info) => {
            EngineBLastInfo = ParseInfo(info);
            InfoUpdated?.Invoke();
        };

        LogMessage?.Invoke($"Tournament started: {EngineAName} vs {EngineBName}");
        LogMessage?.Invoke($"Total pairings: {TotalPairs}, Time control: {TimeLimitMs}ms per move");

        for (int i = 0; i < TotalPairs; i++) {
            if (token.IsCancellationRequested) break;

            CurrentPairIndex = i + 1;
            LogMessage?.Invoke($"--- Starting Pair {CurrentPairIndex}/{TotalPairs} ---");

            // Game 1: Engine A (White) vs Engine B (Black)
            CurrentGameIndex = 1;
            IsEngineAPlayingWhite = true;
            CurrentMoveHistory.Clear();
            if (!string.IsNullOrEmpty(OpeningFen)) {
                CurrentBoard.ParseFen(OpeningFen);
            } else {
                CurrentBoard.Reset();
            }
            BoardUpdated?.Invoke();
            ProgressUpdated?.Invoke();

            LogMessage?.Invoke($"Game 1: {EngineAName} (White) vs {EngineBName} (Black)");
            string res1 = await PlayGameAsync(engineA, engineB, TimeLimitMs, CurrentMoveHistory, token);
            
            double scoreA1 = 0;
            if (res1 == "white_win") {
                scoreA1 = 1.0;
                LogMessage?.Invoke($"Result: {EngineAName} wins");
            } else if (res1 == "black_win") {
                scoreA1 = 0.0;
                LogMessage?.Invoke($"Result: {EngineBName} wins");
            } else {
                scoreA1 = 0.5;
                LogMessage?.Invoke("Result: Draw");
            }

            SaveGameToPgn(EngineAName, EngineBName, res1, CurrentMoveHistory);
            SaveGameToEpd(res1);

            double scoreA2 = 0;
            if (PlayBothColors) {
                if (token.IsCancellationRequested) break;

                // Game 2: Engine B (White) vs Engine A (Black)
                CurrentGameIndex = 2;
                IsEngineAPlayingWhite = false;
                CurrentMoveHistory.Clear();
                if (!string.IsNullOrEmpty(OpeningFen)) {
                    CurrentBoard.ParseFen(OpeningFen);
                } else {
                    CurrentBoard.Reset();
                }
                BoardUpdated?.Invoke();
                ProgressUpdated?.Invoke();

                LogMessage?.Invoke($"Game 2: {EngineBName} (White) vs {EngineAName} (Black)");
                string res2 = await PlayGameAsync(engineB, engineA, TimeLimitMs, CurrentMoveHistory, token);

                if (res2 == "white_win") {
                    scoreA2 = 0.0;
                    LogMessage?.Invoke($"Result: {EngineBName} wins");
                } else if (res2 == "black_win") {
                    scoreA2 = 1.0;
                    LogMessage?.Invoke($"Result: {EngineAName} wins");
                } else {
                    scoreA2 = 0.5;
                    LogMessage?.Invoke("Result: Draw");
                }

                SaveGameToPgn(EngineBName, EngineAName, res2, CurrentMoveHistory);
                SaveGameToEpd(res2);
            }

            // Record Pair Outcome
            double totalPairScore = PlayBothColors ? (scoreA1 + scoreA2) : (scoreA1 * 2.0);
            int outcomeIndex = -1;
            if (totalPairScore == 2.0) {
                outcomeIndex = 4; // WW
                LogMessage?.Invoke($"Pair Result: WW (+2.0 Elo points to {EngineAName})");
            } else if (totalPairScore == 1.5) {
                outcomeIndex = 3; // WD
                LogMessage?.Invoke($"Pair Result: WD (+1.5 Elo points to {EngineAName})");
            } else if (totalPairScore == 1.0) {
                outcomeIndex = 2; // DD / WL
                LogMessage?.Invoke($"Pair Result: DD/WL (+1.0 Elo points to {EngineAName})");
            } else if (totalPairScore == 0.5) {
                outcomeIndex = 1; // LD
                LogMessage?.Invoke($"Pair Result: LD (+0.5 Elo points to {EngineAName})");
            } else if (totalPairScore == 0.0) {
                outcomeIndex = 0; // LL (+0.0 Elo points to {EngineAName})
                LogMessage?.Invoke($"Pair Result: LL (+0.0 Elo points to {EngineAName})");
            }

            if (outcomeIndex != -1) {
                PentanomialCounts[outcomeIndex]++;
            }

            ProgressUpdated?.Invoke();
        }

        LogMessage?.Invoke("Tournament finished!");
        var eloResult = CalculateElo();
        LogMessage?.Invoke($"Final standing Elo diff: {eloResult.eloDiff:F1} +/- {eloResult.margin:F1}");
        IsRunning = false;
        ProgressUpdated?.Invoke();
    }

    private async Task<string> PlayGameAsync(UciEngine white, UciEngine black, int timeLimitMs, List<string> moveHistory, CancellationToken token) {
        var repetitions = new Dictionary<string, int>();
        repetitions[GetRepetitionKey(CurrentBoard)] = 1;

        int movesCount = 0;
        while (movesCount < MaxMoves && !token.IsCancellationRequested) {
            var legalMoves = CurrentBoard.GetLegalMoves();
            if (legalMoves.Count == 0) {
                if (CurrentBoard.IsInCheck(CurrentBoard.ActiveColor)) {
                    return CurrentBoard.ActiveColor == 'w' ? "black_win" : "white_win";
                } else {
                    return "draw"; // Stalemate
                }
            }

            if (CurrentBoard.HalfmoveClock >= 100) {
                return "draw"; // 50-move rule
            }

            UciEngine activeEngine = CurrentBoard.ActiveColor == 'w' ? white : black;
            string fen = CurrentBoard.ToFen();
            string uciMove = await activeEngine.GetBestMoveAsync(fen, timeLimitMs, token);

            if (string.IsNullOrEmpty(uciMove)) {
                // Engine crashed or timed out
                LogMessage?.Invoke($"Engine {(CurrentBoard.ActiveColor == 'w' ? "White" : "Black")} failed to play a move (timeout/crash).");
                return CurrentBoard.ActiveColor == 'w' ? "black_win" : "white_win";
            }

            Move selectedMove = Move.ParseUci(uciMove);
            bool isLegal = false;
            foreach (var lm in legalMoves) {
                if (lm.From == selectedMove.From && lm.To == selectedMove.To && (lm.Promotion == '\0' || lm.Promotion == selectedMove.Promotion)) {
                    selectedMove = lm;
                    isLegal = true;
                    break;
                }
            }

            if (!isLegal) {
                LogMessage?.Invoke($"Engine {(CurrentBoard.ActiveColor == 'w' ? "White" : "Black")} made an ILLEGAL move: {uciMove}");
                return CurrentBoard.ActiveColor == 'w' ? "black_win" : "white_win";
            }

            CurrentBoard.MakeMove(selectedMove);
            moveHistory.Add(selectedMove.ToUci());
            BoardUpdated?.Invoke();

            string repKey = GetRepetitionKey(CurrentBoard);
            if (repetitions.ContainsKey(repKey)) {
                repetitions[repKey]++;
                if (repetitions[repKey] >= 3) {
                    return "draw"; // Threefold repetition
                }
            } else {
                repetitions[repKey] = 1;
            }

            if (IsInsufficientMaterial(CurrentBoard)) {
                return "draw";
            }

            movesCount++;
            await Task.Delay(50, token); // Small delay to animate moves in real-time in the UI
        }

        return "draw"; // MaxMoves cap
    }

    private void SaveGameToPgn(string whiteName, string blackName, string resultStr, List<string> moveHistory) {
        if (string.IsNullOrEmpty(PgnOutputPath)) return;
        try {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[Event \"NKChess Tournament\"]");
            sb.AppendLine($"[White \"{whiteName}\"]");
            sb.AppendLine($"[Black \"{blackName}\"]");
            string resPgn = resultStr switch {
                "white_win" => "1-0",
                "black_win" => "0-1",
                "draw" => "1/2-1/2",
                _ => "*"
            };
            sb.AppendLine($"[Result \"{resPgn}\"]");
            sb.AppendLine();
            for (int i = 0; i < moveHistory.Count; i++) {
                if (i % 2 == 0) sb.Append($"{i / 2 + 1}. {moveHistory[i]}");
                else sb.Append($" {moveHistory[i]}  ");
            }
            sb.AppendLine();
            sb.AppendLine();
            File.AppendAllText(PgnOutputPath, sb.ToString());
        } catch {}
    }

    private void SaveGameToEpd(string resultStr) {
        if (string.IsNullOrEmpty(EpdOutputPath)) return;
        try {
            string fen = CurrentBoard.ToFen();
            string resEpd = resultStr switch {
                "white_win" => "1-0",
                "black_win" => "0-1",
                _ => "1/2-1/2"
            };
            File.AppendAllText(EpdOutputPath, $"{fen} result {resEpd};\n");
        } catch {}
    }

    private string GetRepetitionKey(ChessBoard board) {
        var parts = board.ToFen().Split(' ');
        return $"{parts[0]} {parts[1]} {parts[2]} {parts[3]}";
    }

    private bool IsInsufficientMaterial(ChessBoard board) {
        int pieces = 0;
        for (int i = 0; i < 64; i++) {
            if (board.Squares[i] != '.') pieces++;
        }
        return pieces <= 2; // only Kings left
    }

    public (double eloDiff, double margin) CalculateElo() {
        int ww = PentanomialCounts[4];
        int wd = PentanomialCounts[3];
        int dd = PentanomialCounts[2];
        int ld = PentanomialCounts[1];
        int ll = PentanomialCounts[0];

        int n = ww + wd + dd + ld + ll;
        if (n == 0) return (0, 0);

        double mu = (ww * 1.0 + wd * 0.75 + dd * 0.5 + ld * 0.25) / n;

        double var = (ww * Math.Pow(1.0 - mu, 2) +
                      wd * Math.Pow(0.75 - mu, 2) +
                      dd * Math.Pow(0.5 - mu, 2) +
                      ld * Math.Pow(0.25 - mu, 2) +
                      ll * Math.Pow(0.0 - mu, 2)) / n;

        double se = Math.Sqrt(var / n);

        double eloDiff = 0;
        if (mu <= 0) eloDiff = -999;
        else if (mu >= 1) eloDiff = 999;
        else eloDiff = 400 * Math.Log10(mu / (1 - mu));

        double margin = 0;
        if (mu is > 0 and < 1 && se > 0) {
            double derivative = 400.0 / (Math.Log(10.0) * mu * (1.0 - mu));
            margin = 1.96 * se * derivative;
        }

        return (eloDiff, margin);
    }

    private string ParseInfo(string infoLine) {
        if (string.IsNullOrEmpty(infoLine)) return "";
        var parts = infoLine.Split(' ');
        string depth = "";
        string score = "";
        string nps = "";
        string nodes = "";
        string hash = "";
        string tbhits = "";
        string time = "";
        string ponder = "";
        string pv = "";
        
        for (int i = 0; i < parts.Length - 1; i++) {
            if (parts[i] == "depth") depth = parts[i + 1];
            if (parts[i] == "score") {
                if (i + 2 < parts.Length) {
                    if (parts[i + 1] == "cp") score = parts[i + 2] + " cp";
                    else if (parts[i + 1] == "mate") score = "M" + parts[i + 2];
                }
            }
            if (parts[i] == "nps") {
                if (double.TryParse(parts[i + 1], out double nVal)) {
                    nps = (nVal / 1000.0).ToString("F0") + "k";
                } else {
                    nps = parts[i + 1];
                }
            }
            if (parts[i] == "nodes") nodes = parts[i + 1];
            if (parts[i] == "hashfull") hash = parts[i + 1];
            if (parts[i] == "tbhits") tbhits = parts[i + 1];
            if (parts[i] == "time") time = parts[i + 1];
            if (parts[i] == "ponder") ponder = parts[i + 1];
            if (parts[i] == "pv") {
                pv = string.Join(" ", parts[(i + 1)..Math.Min(parts.Length, i + 6)]);
            }
        }
        
        string res = "";
        if (!string.IsNullOrEmpty(depth)) res += $"D:{depth} ";
        if (!string.IsNullOrEmpty(score)) res += $"S:{score} ";
        if (!string.IsNullOrEmpty(nps)) res += $"NPS:{nps} ";
        if (!string.IsNullOrEmpty(nodes)) res += $"Nodes:{nodes} ";
        if (!string.IsNullOrEmpty(hash)) res += $"Hash:{hash} ";
        if (!string.IsNullOrEmpty(tbhits)) res += $"TB:{tbhits} ";
        if (!string.IsNullOrEmpty(time)) res += $"Time:{time} ";
        if (!string.IsNullOrEmpty(ponder)) res += $"Ponder:{ponder} ";
        if (!string.IsNullOrEmpty(pv)) res += $"PV: {pv}";
        
        return string.IsNullOrEmpty(res) ? infoLine : res.Trim();
    }
}
