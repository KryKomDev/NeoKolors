using NeoKolors.Tui;
using NeoKolors.Console;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;
using NeoKolors.Tui.Core;
using NeoKolors.Common;

namespace NKChess;

public class ChessTuiApp : Page {
    private NKApplication? _app;

    // Game State model
    public class GameState {
        public ChessBoard Board { get; } = new();
        public List<string> PlayHistory { get; } = new();
        public string WhiteName { get; set; } = "Player";
        public string BlackName { get; set; } = "Engine";
        public string WhiteDetails { get; set; } = "  [Human]";
        public string BlackDetails { get; set; } = "  [Idle]";
        public List<double> EvalHistory { get; } = new(); // Centipawn scores from White's perspective
        public bool IsWhiteCpu { get; set; } = false;
        public bool IsBlackCpu { get; set; } = true;
        public string WhiteEnginePath { get; set; } = "stockfish.exe";
        public string BlackEnginePath { get; set; } = "stockfish.exe";
        public int MoveIndex { get; set; } = -1; // For moves slider! -1 means latest move.
        public string StartingFen { get; set; } = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public int WhiteTimeLeft { get; set; } = 1000;
        public int BlackTimeLeft { get; set; } = 1000;
    }

    private readonly List<GameState> _games = new() {
        new GameState { WhiteName = "Player", BlackName = "Engine", IsWhiteCpu = false, IsBlackCpu = true },
        new GameState { WhiteName = "Engine A", BlackName = "Engine B", IsWhiteCpu = true, IsBlackCpu = true }
    };

    private int _activeGameIdx = 0;

    private GameState GetActiveGame() => _games[_activeGameIdx];

    // Selected Square for movement
    private int _selectedSquare = -1;
    private int _cursorRow = 7;
    private int _cursorCol = 0;
    private bool _isFlipped = false;
    private bool _isEngineThinking = false;
    private bool _useLetterPieces = false;
    private bool _useCompactBoard = false;
    private bool _enableSound = true;
    private bool _enableAnimations = true;

    // UCI Engines
    private UciEngine? _whiteEngine;
    private UciEngine? _blackEngine;
    private Tournament? _activeTournament;
    private CancellationTokenSource? _timerCts;

    // UI Elements - Top Level Layout
    private readonly Grid _topLevelGrid = new();
    private readonly MenuBar _menuBar = new();
    private readonly Grid _mainContentGrid = new();
    private readonly Button _btnMenuGame = new(" ≡ Game ");
    private readonly Button _btnMenuTournament = new(" ≡ Tournament ");
    private readonly Button _btnMenuEdit = new(" ≡ Edit ");
    private readonly Button _btnMenuHelp = new(" ≡ Help ");
    private readonly StackPanel _toolbar = new(Orientation.HORIZONTAL, 1);
    private readonly Button _btnToolbarNewGame = new(" New Game ");
    private readonly Button _btnToolbarNewTourney = new(" New Tournament ");
    private readonly Button _btnToolbarStop = new(" Abort/Stop ");
    private readonly Button _btnToolbarFlip = new(" Flip Board ");

    // UI Elements - Board
    private readonly Grid _boardGrid = new();
    private readonly Button[,] _boardButtons = new Button[8, 8];
    private readonly TextBlock[] _rankLabels = new TextBlock[8];
    private readonly TextBlock[] _fileLabels = new TextBlock[8];

    // UI Elements - Main Content Tabs
    private readonly Button _btnTabSingle = new(" [Single Game] ");
    private readonly Button _btnTabTourney = new(" [Tournament] ");

    // UI Elements - Moves List
    private readonly TextBlock _lblMovesList = new("");

    // UI Elements - White Info
    private readonly TextBlock _lblWhiteInfoTitle = new("=== White Info ===");
    private readonly TextBlock _lblWhiteInfoLog = new("Log: Idle");
    private readonly TextBlock _lblWhiteInfoNps = new("NPS: -");
    private readonly TextBlock _lblWhiteInfoHash = new("Hash: -");
    private readonly TextBlock _lblWhiteInfoPonder = new("Ponder: -");
    private readonly TextBlock _lblWhiteInfoTb = new("TB: -");
    private readonly TextBlock _lblWhiteInfoDepth = new("Depth: -");
    private readonly TextBlock _lblWhiteInfoTime = new("Time: -");
    private readonly TextBlock _lblWhiteInfoNodes = new("Nodes: -");
    private readonly TextBlock _lblWhiteInfoScore = new("Score: -");
    private readonly TextBlock _lblWhiteInfoPv = new("PV: -");

    // UI Elements - Black Info
    private readonly TextBlock _lblBlackInfoTitle = new("=== Black Info ===");
    private readonly TextBlock _lblBlackInfoLog = new("Log: Idle");
    private readonly TextBlock _lblBlackInfoNps = new("NPS: -");
    private readonly TextBlock _lblBlackInfoHash = new("Hash: -");
    private readonly TextBlock _lblBlackInfoPonder = new("Ponder: -");
    private readonly TextBlock _lblBlackInfoTb = new("TB: -");
    private readonly TextBlock _lblBlackInfoDepth = new("Depth: -");
    private readonly TextBlock _lblBlackInfoTime = new("Time: -");
    private readonly TextBlock _lblBlackInfoNodes = new("Nodes: -");
    private readonly TextBlock _lblBlackInfoScore = new("Score: -");
    private readonly TextBlock _lblBlackInfoPv = new("PV: -");

    // Slider, Graph, dialogue overlay, and extra labels
    private readonly Slider _movesSlider = new();
    private readonly TextBlock _lblEvalGraph = new("Evaluation Graph: [No Data]");
    private Grid? _activeDialogOverlay;

    // Backward compatible status labels
    private readonly TextBlock _lblWhitePlayer = new("White: Player");
    private readonly TextBlock _lblWhiteDetails = new("  [Human]");
    private readonly TextBlock _lblBlackPlayer = new("Black: Engine");
    private readonly TextBlock _lblBlackDetails = new("  [Idle]");
    private readonly TextBlock _lblEvalText = new("Eval: +0.00");
    private readonly TextBlock _lblEvalBar = new(" [██████░░░░░░] ");

    // Dialogue configuration values
    private bool _whiteIsCpu = false;
    private bool _blackIsCpu = true;
    private string _whiteEnginePath;
    private string _blackEnginePath;
    private readonly List<EngineConfig> _addedEngines;
    private int _whiteTimeLimit;
    private int _whiteIncrement = 0;
    private int _blackTimeLimit;
    private int _blackIncrement = 0;

    private string _tourneyName = "NKChess Cup";
    private string _tourneyPgnPath = "tournament.pgn";
    private string _tourneyEpdPath = "tournament.epd";
    private string _tourneyType = "Round-robin";
    private int _tourneyRounds = 5;
    private int _tourneyGamesPerEncounter = 2;
    private bool _tourneyPlayBothColors = true;
    private int _tourneyMaxMoves = 100;
    private string _tourneyOpeningFen = "";

    private string _tourneyEngineA = "stockfish.exe";
    private string _tourneyEngineB = "stockfish.exe";
    private readonly TextBlock _lblTourneyProgress = new("Pairing: 0/0");
    private readonly TextBlock _lblTourneyPentanomial = new("WW: 0 | WD: 0 | DD: 0 | LD: 0 | LL: 0");
    private readonly TextBlock _lblTourneyElo = new("Elo Diff: 0.0 +/- 0.0");
    private readonly TextBlock _lblTourneyStats = new("");
    private readonly TextBlock _lblTourneyLog = new("");
    private readonly TextBlock _lblPlayStatus = new("Status: Idle");
    private readonly TextBlock _lblWhiteTimeRemaining = new("W-Time: -");
    private readonly TextBlock _lblBlackTimeRemaining = new("B-Time: -");

    // Curated sleek palettes
    private static readonly NKColor COLOR_BG = NKColor.Default; // Tailwind slate-900
    private static readonly NKColor COLOR_FG = NKColor.White; // Tailwind slate-100
    private static readonly NKColor COLOR_PANEL_BG = NKColor.Default; // Tailwind slate-800

    private static readonly NKColor COLOR_LIGHT_SQ = NKColor.FromRgb(0xC2B8B2); // Lichess light wood
    private static readonly NKColor COLOR_DARK_SQ = NKColor.FromRgb(0x197BBD); // Lichess dark wood

    private static readonly NKColor COLOR_SELECTED = NKColor.FromRgb(234, 179, 8); // Tailwind yellow-500
    private static readonly NKColor COLOR_VALID_MOVE = NKColor.FromRgb(34, 197, 94); // Tailwind green-500
    private static readonly NKColor COLOR_CURSOR = NKColor.FromRgb(59, 130, 246); // Tailwind blue-500
    private static readonly NKColor COLOR_CHECK = NKColor.FromRgb(239, 68, 68); // Tailwind red-500

    private static readonly NKColor COLOR_BTN_BG = NKColor.FromRgb(71, 85, 105); // Tailwind slate-600
    private static readonly NKColor COLOR_BTN_ACTIVE_BG = NKColor.FromRgb(99, 102, 241); // Tailwind indigo-500

    public ChessTuiApp() {
        // Load preferences
        var prefs = AppPreferences.Load();
        _useLetterPieces = prefs.UseLetterPieces;
        _useCompactBoard = prefs.UseCompactBoard;
        _enableSound = prefs.EnableSound;
        _enableAnimations = prefs.EnableAnimations;
        _whiteEnginePath = prefs.WhiteEnginePath;
        _blackEnginePath = prefs.BlackEnginePath;
        _addedEngines = prefs.AddedEngines;

        if (_addedEngines.Count > 0) {
            if (_addedEngines.All(e => e.Path != _tourneyEngineA)) _tourneyEngineA = _addedEngines[0].Path;
            if (_addedEngines.All(e => e.Path != _tourneyEngineB)) _tourneyEngineB = _addedEngines[0].Path;
        }

        _whiteTimeLimit = prefs.WhiteTimeLimit;
        _blackTimeLimit = prefs.BlackTimeLimit;

        // Sync loaded preferences to game states
        foreach (var game in _games) {
            game.WhiteEnginePath = _whiteEnginePath;
            game.BlackEnginePath = _blackEnginePath;
            game.WhiteTimeLeft = _whiteTimeLimit * 1000;
            game.BlackTimeLeft = _blackTimeLimit * 1000;
        }

        // Apply main page theme styles
        Style.Set(new BackgroundColorProperty(COLOR_BG));
        Style.Set(new TextColorProperty(COLOR_FG));

        BuildUi();
        UpdateBoardUi();

        // Register keys for keyboard controls
        AppEventBus.SubscribeToKeyEvent(OnKeyEvent);
    }

    public void SetApplication(NKApplication app) {
        _app = app;
    }

    private void BuildUi() {
        // 1. Top Level Grid Layout (Rows: Menu Bar [1], Toolbar [2], Body [Star])
        _topLevelGrid.RowDefinitions.Add(GridLength.Chars(1));
        _topLevelGrid.RowDefinitions.Add(GridLength.Chars(2));
        _topLevelGrid.RowDefinitions.Add(GridLength.Star());
        AddChild(_topLevelGrid);

        // Menu Bar Styling
        _menuBar.Style.Set(new BackgroundColorProperty(NKColor.FromRgb(30, 41, 59))); // slate-800

        StyleMenuButton(_btnMenuGame);
        StyleMenuButton(_btnMenuTournament);
        StyleMenuButton(_btnMenuEdit);
        StyleMenuButton(_btnMenuHelp);

        _btnMenuGame.OnClick += _ => OpenGameMenu();
        _btnMenuTournament.OnClick += _ => OpenTournamentMenu();
        _btnMenuEdit.OnClick += _ => OpenEditMenu();
        _btnMenuHelp.OnClick += _ => OpenHelpMenu();

        _menuBar.AddChild(_btnMenuGame);
        _menuBar.AddChild(_btnMenuTournament);
        _menuBar.AddChild(_btnMenuEdit);
        _menuBar.AddChild(_btnMenuHelp);

        _topLevelGrid.AddChild(_menuBar, 0, 0);

        // Toolbar Styling & Buttons
        _toolbar.Style.Set(new BackgroundColorProperty(NKColor.FromRgb(51, 65, 85))); // slate-700
        _toolbar.Style.Set(new PaddingProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));

        StyleButton(_btnToolbarNewGame, COLOR_BTN_BG, COLOR_FG);
        StyleButton(_btnToolbarNewTourney, COLOR_BTN_BG, COLOR_FG);
        StyleButton(_btnToolbarStop, COLOR_BTN_BG, COLOR_FG);
        StyleButton(_btnToolbarFlip, COLOR_BTN_BG, COLOR_FG);

        _btnToolbarNewGame.OnClick += _ => OpenNewGameDialog();
        _btnToolbarNewTourney.OnClick += _ => OpenNewTournamentDialog();

        _btnToolbarStop.OnClick += _ => {
            _selectedSquare = -1;
            var active = GetActiveGame();
            active.Board.Reset();
            active.PlayHistory.Clear();
            active.EvalHistory.Clear();
            active.MoveIndex = -1;
            active.StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            UpdateMovesListUi();
            UpdateEvalGraphUi();
            UpdateBoardUi();
            UpdateTimeLabels();
        };

        _btnToolbarFlip.OnClick += _ => FlipBoard();

        _toolbar.AddChild(_btnToolbarNewGame);
        _toolbar.AddChild(_btnToolbarNewTourney);
        _toolbar.AddChild(_btnToolbarStop);
        _toolbar.AddChild(_btnToolbarFlip);
        _topLevelGrid.AddChild(_toolbar, 1, 0);

        // 2. Body Grid Layout (Rows: Tabs [2], Time/Status [1], Content [Star], Slider [2], Graph [7])
        var bodyGrid = new Grid();
        bodyGrid.RowDefinitions.Add(GridLength.Chars(2));
        bodyGrid.RowDefinitions.Add(GridLength.Chars(1));
        bodyGrid.RowDefinitions.Add(GridLength.Star());
        bodyGrid.RowDefinitions.Add(GridLength.Chars(2));
        bodyGrid.RowDefinitions.Add(GridLength.Chars(7));
        _topLevelGrid.AddChild(bodyGrid, 2, 0);

        // Tabs Row (Row 0)
        var tabsPanel = new StackPanel(Orientation.HORIZONTAL, 1);
        tabsPanel.Style.Set(new MarginProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));
        StyleButton(_btnTabSingle, COLOR_BTN_ACTIVE_BG, COLOR_FG);
        StyleButton(_btnTabTourney, COLOR_BTN_BG, COLOR_FG);
        _btnTabSingle.OnClick += _ => SwitchTab(0);
        _btnTabTourney.OnClick += _ => SwitchTab(1);
        tabsPanel.AddChild(_btnTabSingle);
        tabsPanel.AddChild(_btnTabTourney);
        bodyGrid.AddChild(tabsPanel, 0, 0);

        // Time & Result Row (Row 1)
        var timeResultGrid = new Grid();
        timeResultGrid.ColumnDefinitions.Add(GridLength.Star());
        timeResultGrid.ColumnDefinitions.Add(GridLength.Star());
        timeResultGrid.ColumnDefinitions.Add(GridLength.Star());
        timeResultGrid.Style.Set(new MarginProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));

        _lblWhiteTimeRemaining.Style.Set(new TextAlignProperty(new Align(HorizontalAlign.LEFT, VerticalAlign.CENTER)));
        _lblWhiteTimeRemaining.Style.Set(new TextColorProperty(NKColor.FromRgb(248, 250, 252))); // slate-50
        _lblWhiteTimeRemaining.Style.Set(new TextStyleProperty(TextStyles.BOLD));

        _lblPlayStatus.Style.Set(new TextAlignProperty(new Align(HorizontalAlign.CENTER, VerticalAlign.CENTER)));
        _lblPlayStatus.Style.Set(new TextColorProperty(NKColor.FromRgb(251, 146, 60))); // orange-400
        _lblPlayStatus.Style.Set(new TextStyleProperty(TextStyles.BOLD));

        _lblBlackTimeRemaining.Style.Set(new TextAlignProperty(new Align(HorizontalAlign.RIGHT, VerticalAlign.CENTER)));
        _lblBlackTimeRemaining.Style.Set(new TextColorProperty(NKColor.FromRgb(248, 250, 252))); // slate-50
        _lblBlackTimeRemaining.Style.Set(new TextStyleProperty(TextStyles.BOLD));

        timeResultGrid.AddChild(_lblWhiteTimeRemaining, 0, 0);
        timeResultGrid.AddChild(_lblPlayStatus, 0, 1);
        timeResultGrid.AddChild(_lblBlackTimeRemaining, 0, 2);
        bodyGrid.AddChild(timeResultGrid, 1, 0);

        UpdateTimeLabels();

        // Main Content (Row 2) -> nested columns (Board [36], Moves [24], White Info [Star(1)], Black Info [Star(1)])
        _mainContentGrid.ColumnDefinitions.Add(GridLength.Chars(36));
        _mainContentGrid.ColumnDefinitions.Add(GridLength.Chars(24));
        _mainContentGrid.ColumnDefinitions.Add(GridLength.Star());
        _mainContentGrid.ColumnDefinitions.Add(GridLength.Star());
        bodyGrid.AddChild(_mainContentGrid, 2, 0);

        // Column 0: Game Board Grid
        _boardGrid.Style.Set(new MarginProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));

        for (int r = 0; r < 8; r++) {
            _rankLabels[r] = new TextBlock("");
            _rankLabels[r].Style.Set(new TextColorProperty(NKColor.FromRgb(148, 163, 184)));
            _rankLabels[r].Style.Set(new TextStyleProperty(TextStyles.BOLD));
            _boardGrid.AddChild(_rankLabels[r], r, 0);
        }

        for (int c = 0; c < 8; c++) {
            _fileLabels[c] = new TextBlock("");
            _fileLabels[c].Style.Set(new TextColorProperty(NKColor.FromRgb(148, 163, 184)));
            _fileLabels[c].Style.Set(new TextStyleProperty(TextStyles.BOLD));
            _boardGrid.AddChild(_fileLabels[c], 8, c + 1);
        }

        UpdateBoardLayout();
        UpdateLabels();

        for (int r = 0; r < 8; r++) {
            for (int c = 0; c < 8; c++) {
                int row = r;
                int col = c;
                var btn = new Button("");
                btn.Style.Set(new BorderProperty(BorderStyle.GetBorderless()));
                btn.Style.Set(new MarginProperty(Spacing.Zero));
                btn.Style.Set(new PaddingProperty(Spacing.Zero));
                btn.IsTabStop = true;

                btn.OnClick += _ => {
                    if (_activeTournament is { IsRunning: true }) return;

                    HandleSquareSelect(row, col);
                    UpdateBoardUi();
                };

                btn.OnHover += UpdateBoardUi;
                btn.OnHoverOut += UpdateBoardUi;

                _boardButtons[r, c] = btn;
                _boardGrid.AddChild(btn, r, c + 1);
            }
        }

        _mainContentGrid.AddChild(_boardGrid, 0, 0);

        // Column 1: Moves History ScrollViewer
        var movesHistoryScroll = new ScrollViewer();
        movesHistoryScroll.Style.Set(new BackgroundColorProperty(COLOR_PANEL_BG));
        movesHistoryScroll.Style.Set(new BorderProperty(BorderStyle.GetRounded(NKColor.FromRgb(71, 85, 105), COLOR_PANEL_BG)));
        movesHistoryScroll.Style.Set(new MarginProperty(new Spacing(Dimension.Zero, Dimension.Zero, Dimension.Chars(1), Dimension.Zero)));
        _lblMovesList.Style.Set(new TextColorProperty(NKColor.FromRgb(148, 163, 184)));
        movesHistoryScroll.Content = _lblMovesList;
        _mainContentGrid.AddChild(movesHistoryScroll, 0, 1);

        // Column 2: White Info panel
        var whiteFrame = new StackPanel(Orientation.VERTICAL);
        whiteFrame.Style.Set(new BackgroundColorProperty(COLOR_PANEL_BG));
        whiteFrame.Style.Set(new BorderProperty(BorderStyle.GetRounded(NKColor.FromRgb(99, 102, 241), COLOR_PANEL_BG)));
        whiteFrame.Style.Set(new PaddingProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));
        whiteFrame.Style.Set(new MarginProperty(new Spacing(Dimension.Zero, Dimension.Zero, Dimension.Chars(1), Dimension.Zero)));

        _lblWhiteInfoTitle.Style.Set(new TextStyleProperty(TextStyles.BOLD));
        _lblWhiteInfoTitle.Style.Set(new TextColorProperty(NKColor.FromRgb(255, 255, 255)));
        whiteFrame.AddChild(_lblWhiteInfoTitle);

        var whiteGrid = new Grid();

        for (int r = 0; r < 5; r++) {
            whiteGrid.RowDefinitions.Add(GridLength.Chars(1));
        }

        whiteGrid.ColumnDefinitions.Add(GridLength.Star());
        whiteGrid.ColumnDefinitions.Add(GridLength.Star());
        whiteGrid.AddChild(_lblWhiteInfoNps, 0, 0);
        whiteGrid.AddChild(_lblWhiteInfoDepth, 0, 1);
        whiteGrid.AddChild(_lblWhiteInfoHash, 1, 0);
        whiteGrid.AddChild(_lblWhiteInfoTime, 1, 1);
        whiteGrid.AddChild(_lblWhiteInfoPonder, 2, 0);
        whiteGrid.AddChild(_lblWhiteInfoNodes, 2, 1);
        whiteGrid.AddChild(_lblWhiteInfoTb, 3, 0);
        whiteGrid.AddChild(_lblWhiteInfoScore, 3, 1);
        whiteGrid.AddChild(_lblWhiteInfoPv, 4, 0, 1, 2);
        whiteFrame.AddChild(whiteGrid);

        _lblWhiteInfoLog.Style.Set(new MarginProperty(new Spacing(Dimension.Zero, Dimension.Chars(1), Dimension.Zero, Dimension.Zero)));
        whiteFrame.AddChild(_lblWhiteInfoLog);
        _mainContentGrid.AddChild(whiteFrame, 0, 2);

        // Column 3: Black Info panel
        var blackFrame = new StackPanel(Orientation.VERTICAL);
        blackFrame.Style.Set(new BackgroundColorProperty(COLOR_PANEL_BG));
        blackFrame.Style.Set(new BorderProperty(BorderStyle.GetRounded(NKColor.FromRgb(99, 102, 241), COLOR_PANEL_BG)));
        blackFrame.Style.Set(new PaddingProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));
        blackFrame.Style.Set(new MarginProperty(new Spacing(Dimension.Zero, Dimension.Zero, Dimension.Chars(1), Dimension.Zero)));

        _lblBlackInfoTitle.Style.Set(new TextStyleProperty(TextStyles.BOLD));
        _lblBlackInfoTitle.Style.Set(new TextColorProperty(NKColor.FromRgb(203, 213, 225)));
        blackFrame.AddChild(_lblBlackInfoTitle);

        var blackGrid = new Grid();

        for (int r = 0; r < 5; r++) {
            blackGrid.RowDefinitions.Add(GridLength.Chars(1));
        }

        blackGrid.ColumnDefinitions.Add(GridLength.Star());
        blackGrid.ColumnDefinitions.Add(GridLength.Star());
        blackGrid.AddChild(_lblBlackInfoNps, 0, 0);
        blackGrid.AddChild(_lblBlackInfoDepth, 0, 1);
        blackGrid.AddChild(_lblBlackInfoHash, 1, 0);
        blackGrid.AddChild(_lblBlackInfoTime, 1, 1);
        blackGrid.AddChild(_lblBlackInfoPonder, 2, 0);
        blackGrid.AddChild(_lblBlackInfoNodes, 2, 1);
        blackGrid.AddChild(_lblBlackInfoTb, 3, 0);
        blackGrid.AddChild(_lblBlackInfoScore, 3, 1);
        blackGrid.AddChild(_lblBlackInfoPv, 4, 0, 1, 2);
        blackFrame.AddChild(blackGrid);

        _lblBlackInfoLog.Style.Set(new MarginProperty(new Spacing(Dimension.Zero, Dimension.Chars(1), Dimension.Zero, Dimension.Zero)));
        blackFrame.AddChild(_lblBlackInfoLog);
        _mainContentGrid.AddChild(blackFrame, 0, 3);

        // Slider Row (Row 3)
        _movesSlider.Style.Set(new MarginProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));
        _movesSlider.Minimum = 0;
        _movesSlider.Maximum = 0;
        _movesSlider.Value = 0;
        _movesSlider.IsEnabled = false;

        _movesSlider.ValueChanged += (_, _, newVal) => {
            var active = GetActiveGame();
            int step = (int)Math.Round(newVal);

            if (step >= 0 && step < active.PlayHistory.Count - 1) {
                active.MoveIndex = step;
            }
            else {
                active.MoveIndex = -1;
            }

            UpdateBoardUi();
        };

        bodyGrid.AddChild(_movesSlider, 3, 0);

        // Graph Row (Row 4)
        _lblEvalGraph.Style.Set(new MarginProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));
        _lblEvalGraph.Style.Set(new TextColorProperty(NKColor.FromRgb(56, 189, 248))); // sky-400
        bodyGrid.AddChild(_lblEvalGraph, 4, 0);
    }

    private void StyleButton(Button btn, NKColor bg, NKColor fg) {
        btn.Style.Set(new BackgroundColorProperty(bg));
        btn.Style.Set(new TextColorProperty(fg));
        btn.Style.Set(new BorderProperty(BorderStyle.GetBorderless()));
        btn.Style.Set(new PaddingProperty(new Spacing(Dimension.Chars(1), Dimension.Chars(1), Dimension.Zero, Dimension.Zero)));
        btn.IsTabStop = true;

        var hoverBg = NKColor.FromRgb(
            (byte)Math.Min(255, (bg.AsRgb >> 16 & 0xFF) + 20),
            (byte)Math.Min(255, (bg.AsRgb >> 8 & 0xFF) + 20),
            (byte)Math.Min(255, (bg.AsRgb & 0xFF) + 20)
        );

        btn.OnHover += () => {
            btn.Style.Set(new BackgroundColorProperty(hoverBg));
            _app?.Screen?.Render();
        };

        btn.OnHoverOut += () => {
            btn.Style.Set(new BackgroundColorProperty(bg));
            _app?.Screen?.Render();
        };
    }

    private void StyleTextBox(TextBox tb) {
        tb.Style.Set(new BackgroundColorProperty(NKColor.FromRgb(15, 23, 42))); // slate-900
        tb.Style.Set(new TextColorProperty(NKColor.FromRgb(241, 245, 249))); // slate-100
        tb.Style.Set(new BorderProperty(BorderStyle.GetNormal(NKColor.FromRgb(71, 85, 105), NKColor.FromRgb(15, 23, 42))));
        tb.Style.Set(new WidthProperty(Dimension.Chars(30)));
        tb.IsTabStop = true;
    }

    private void StyleComboBox(ComboBox cb) {
        cb.Style.Set(new BackgroundColorProperty(NKColor.FromRgb(15, 23, 42))); // slate-900
        cb.Style.Set(new TextColorProperty(NKColor.FromRgb(241, 245, 249))); // slate-100
        cb.Style.Set(new BorderProperty(BorderStyle.GetNormal(NKColor.FromRgb(71, 85, 105), NKColor.FromRgb(15, 23, 42))));
        cb.Style.Set(new WidthProperty(Dimension.Chars(30)));
        cb.IsTabStop = true;
    }

    // Modal dialog overlay helper
    private void ShowDialog(string title, IElement content, Action onOk, Action? onCancel = null) {
        if (_activeDialogOverlay != null) {
            _topLevelGrid.RemoveChild(_activeDialogOverlay);
        }

        var overlay = new Grid();
        overlay.Style.Set(new BackgroundColorProperty(COLOR_BG));

        var dialogFrame = new StackPanel(Orientation.VERTICAL, 1);
        dialogFrame.Style.Set(new MarginProperty(new Spacing(Dimension.Chars(4), Dimension.Chars(2))));
        dialogFrame.Style.Set(new PaddingProperty(new Spacing(Dimension.Chars(2))));
        dialogFrame.Style.Set(new BackgroundColorProperty(COLOR_PANEL_BG));
        dialogFrame.Style.Set(new BorderProperty(BorderStyle.GetRounded(NKColor.FromRgb(99, 102, 241), COLOR_PANEL_BG)));

        var lblTitle = new TextBlock($"=== {title} ===");
        lblTitle.Style.Set(new TextStyleProperty(TextStyles.BOLD));
        lblTitle.Style.Set(new TextColorProperty(NKColor.FromRgb(99, 102, 241)));
        dialogFrame.AddChild(lblTitle);

        dialogFrame.AddChild(content);

        var buttonRow = new StackPanel(Orientation.HORIZONTAL, 2);
        var btnOk = new Button(" OK ");
        StyleButton(btnOk, COLOR_BTN_ACTIVE_BG, COLOR_FG);

        btnOk.OnClick += _ => {
            _topLevelGrid.RemoveChild(overlay);
            _activeDialogOverlay = null;
            onOk();
            _app?.Screen?.Render();
        };

        buttonRow.AddChild(btnOk);

        var btnCancel = new Button(" Cancel ");
        StyleButton(btnCancel, COLOR_BTN_BG, COLOR_FG);

        btnCancel.OnClick += _ => {
            _topLevelGrid.RemoveChild(overlay);
            _activeDialogOverlay = null;
            onCancel?.Invoke();
            _app?.Screen?.Render();
        };

        buttonRow.AddChild(btnCancel);

        dialogFrame.AddChild(buttonRow);
        overlay.AddChild(dialogFrame);

        _activeDialogOverlay = overlay;
        _topLevelGrid.AddChild(overlay, 0, 0, 3);
        _app?.Screen?.Render();
    }

    private void OpenNewGameDialog() {
        var content = new StackPanel(Orientation.VERTICAL, 1);

        var rowWhite = new StackPanel(Orientation.HORIZONTAL, 2);
        var cbWhiteCpu = new CheckBox("White is CPU");
        cbWhiteCpu.IsChecked = _whiteIsCpu;
        var comboWhite = new ComboBox();
        comboWhite.ItemsSource = _addedEngines;
        StyleComboBox(comboWhite);
        var whiteConfig = _addedEngines.FirstOrDefault(e => e.Path == _whiteEnginePath);

        if (whiteConfig != null) {
            comboWhite.SelectedItem = whiteConfig;
        }
        else if (_addedEngines.Count > 0) {
            comboWhite.SelectedIndex = 0;
        }

        rowWhite.AddChild(cbWhiteCpu);
        rowWhite.AddChild(comboWhite);
        content.AddChild(rowWhite);

        var rowBlack = new StackPanel(Orientation.HORIZONTAL, 2);

        var cbBlackCpu = new CheckBox("Black is CPU") {
            IsChecked = _blackIsCpu
        };

        var comboBlack = new ComboBox();
        comboBlack.ItemsSource = _addedEngines;
        StyleComboBox(comboBlack);
        var blackConfig = _addedEngines.FirstOrDefault(e => e.Path == _blackEnginePath);

        if (blackConfig != null) {
            comboBlack.SelectedItem = blackConfig;
        }
        else if (_addedEngines.Count > 0) {
            comboBlack.SelectedIndex = 0;
        }

        rowBlack.AddChild(cbBlackCpu);
        rowBlack.AddChild(comboBlack);
        content.AddChild(rowBlack);

        var btnTime = new Button(" Configure Time Control ");
        StyleButton(btnTime, COLOR_BTN_BG, COLOR_FG);
        btnTime.OnClick += _ => OpenTimeControlDialog();
        content.AddChild(btnTime);

        content.AddChild(new TextBlock("Opening position (FEN):"));
        var txtFen = new TextBox("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        StyleTextBox(txtFen);
        content.AddChild(txtFen);

        ShowDialog("New Game", content, () => {
            _whiteIsCpu = cbWhiteCpu.IsChecked ?? false;
            _whiteEnginePath = (comboWhite.SelectedItem as EngineConfig)?.Path ?? "stockfish.exe";
            _blackIsCpu = cbBlackCpu.IsChecked ?? false;
            _blackEnginePath = (comboBlack.SelectedItem as EngineConfig)?.Path ?? "stockfish.exe";

            SavePreferences();
            UpdateTimeLabels();

            var active = GetActiveGame();
            active.IsWhiteCpu = _whiteIsCpu;
            active.IsBlackCpu = _blackIsCpu;
            active.WhiteEnginePath = _whiteEnginePath;
            active.BlackEnginePath = _blackEnginePath;

            active.WhiteName = _whiteIsCpu ? Path.GetFileNameWithoutExtension(_whiteEnginePath) : "Player";
            active.BlackName = _blackIsCpu ? Path.GetFileNameWithoutExtension(_blackEnginePath) : "Engine";

            _lblWhitePlayer.Content = $"White: {active.WhiteName}";
            _lblBlackPlayer.Content = $"Black: {active.BlackName}";

            _selectedSquare = -1;
            string fen = txtFen.Text.Trim();
            active.StartingFen = fen;
            active.Board.ParseFen(fen);
            active.PlayHistory.Clear();
            active.EvalHistory.Clear();
            active.MoveIndex = -1;
            active.WhiteTimeLeft = _whiteTimeLimit * 1000;
            active.BlackTimeLeft = _blackTimeLimit * 1000;

            _lblPlayStatus.Content = "Status: Game in progress.";

            UpdateMovesListUi();
            UpdateEvalGraphUi();
            UpdateBoardUi();
            UpdateTimeLabels();

            _lblWhiteInfoLog.Content = "Log: New game initialized";
            _lblBlackInfoLog.Content = "Log: New game initialized";

            StartGameTimer();

            if (_whiteIsCpu) {
                _lblPlayStatus.Content = "Status: Engine thinking...";
                TriggerEngineMove();
            }
        });
    }

    private void OpenTimeControlDialog() {
        var content = new StackPanel(Orientation.VERTICAL, 1);

        content.AddChild(new TextBlock("White total time (s):"));
        var txtWhiteTime = new TextBox(_whiteTimeLimit.ToString());
        StyleTextBox(txtWhiteTime);
        content.AddChild(txtWhiteTime);

        content.AddChild(new TextBlock("White increment per move (s):"));
        var txtWhiteInc = new TextBox(_whiteIncrement.ToString());
        StyleTextBox(txtWhiteInc);
        content.AddChild(txtWhiteInc);

        content.AddChild(new TextBlock("Black total time (s):"));
        var txtBlackTime = new TextBox(_blackTimeLimit.ToString());
        StyleTextBox(txtBlackTime);
        content.AddChild(txtBlackTime);

        content.AddChild(new TextBlock("Black increment per move (s):"));
        var txtBlackInc = new TextBox(_blackIncrement.ToString());
        StyleTextBox(txtBlackInc);
        content.AddChild(txtBlackInc);

        ShowDialog("Time Control", content, () => {
            int.TryParse(txtWhiteTime.Text, out _whiteTimeLimit);
            int.TryParse(txtWhiteInc.Text, out _whiteIncrement);
            int.TryParse(txtBlackTime.Text, out _blackTimeLimit);
            int.TryParse(txtBlackInc.Text, out _blackIncrement);

            SavePreferences();
            UpdateTimeLabels();
        });
    }

    private void OpenSaveDialog() {
        var content = new StackPanel(Orientation.VERTICAL, 1);
        content.AddChild(new TextBlock("PGN output file path:"));
        var txtPath = new TextBox("game.pgn");
        StyleTextBox(txtPath);
        content.AddChild(txtPath);

        ShowDialog("Save Game (PGN)", content, () => {
            string path = txtPath.Text.Trim();

            try {
                var active = GetActiveGame();
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("[Event \"NKChess Session\"]");
                sb.AppendLine($"[White \"{active.WhiteName}\"]");
                sb.AppendLine($"[Black \"{active.BlackName}\"]");
                sb.AppendLine($"[Result \"*\"]");
                sb.AppendLine();

                for (int i = 0; i < active.PlayHistory.Count; i++) {
                    if (i % 2 == 0) sb.Append($"{i / 2 + 1}. {active.PlayHistory[i]}");
                    else sb.Append($" {active.PlayHistory[i]}  ");
                }

                File.WriteAllText(path, sb.ToString());
                _lblPlayStatus.Content = $"Status: Game saved to {path}";
            }
            catch (Exception ex) {
                _lblPlayStatus.Content = $"Status: Error saving game: {ex.Message}";
            }
        });
    }

    private void OpenAdjudicateDialog() {
        var content = new StackPanel(Orientation.VERTICAL, 1);
        content.AddChild(new TextBlock("Choose adjudicated result:"));

        var cbWhiteWin = new CheckBox("White Wins");
        var cbBlackWin = new CheckBox("Black Wins");
        var cbDraw = new CheckBox("Draw Game");

        content.AddChild(cbWhiteWin);
        content.AddChild(cbBlackWin);
        content.AddChild(cbDraw);

        ShowDialog("Adjudicate Result", content, () => {
            if (cbWhiteWin.IsChecked == true) {
                _lblPlayStatus.Content = "Status: Game adjudicated - White Wins.";
            }
            else if (cbBlackWin.IsChecked == true) {
                _lblPlayStatus.Content = "Status: Game adjudicated - Black Wins.";
            }
            else if (cbDraw.IsChecked == true) {
                _lblPlayStatus.Content = "Status: Game adjudicated - Draw.";
            }
        });
    }

    private void OpenNewTournamentDialog() {
        var content = new StackPanel(Orientation.VERTICAL, 1);

        content.AddChild(new TextBlock("--- General Info ---"));
        content.AddChild(new TextBlock("Name:"));
        var txtName = new TextBox(_tourneyName);
        StyleTextBox(txtName);
        content.AddChild(txtName);
        content.AddChild(new TextBlock("PGN output:"));
        var txtPgn = new TextBox(_tourneyPgnPath);
        StyleTextBox(txtPgn);
        content.AddChild(txtPgn);
        content.AddChild(new TextBlock("EPD output:"));
        var txtEpd = new TextBox(_tourneyEpdPath);
        StyleTextBox(txtEpd);
        content.AddChild(txtEpd);

        content.AddChild(new TextBlock("--- Engines ---"));
        content.AddChild(new TextBlock("Engine A Path:"));
        var cbEngA = new ComboBox();
        cbEngA.ItemsSource = _addedEngines;
        StyleComboBox(cbEngA);
        var engAConfig = _addedEngines.FirstOrDefault(e => e.Path == _tourneyEngineA);

        if (engAConfig != null) {
            cbEngA.SelectedItem = engAConfig;
        }
        else if (_addedEngines.Count > 0) {
            cbEngA.SelectedIndex = 0;
        }

        content.AddChild(cbEngA);

        content.AddChild(new TextBlock("Engine B Path:"));
        var cbEngB = new ComboBox();
        cbEngB.ItemsSource = _addedEngines;
        StyleComboBox(cbEngB);
        var engBConfig = _addedEngines.FirstOrDefault(e => e.Path == _tourneyEngineB);

        if (engBConfig != null) {
            cbEngB.SelectedItem = engBConfig;
        }
        else if (_addedEngines.Count > 0) {
            cbEngB.SelectedIndex = 0;
        }

        content.AddChild(cbEngB);

        content.AddChild(new TextBlock("--- Tournament Config ---"));
        content.AddChild(new TextBlock("Type (Round-robin/Gauntlet/Knockout/Pyramid):"));
        var txtType = new TextBox(_tourneyType);
        StyleTextBox(txtType);
        content.AddChild(txtType);
        content.AddChild(new TextBlock("Rounds:"));
        var txtRounds = new TextBox(_tourneyRounds.ToString());
        StyleTextBox(txtRounds);
        content.AddChild(txtRounds);
        content.AddChild(new TextBlock("Games per encounter:"));
        var txtGpe = new TextBox(_tourneyGamesPerEncounter.ToString());
        StyleTextBox(txtGpe);
        content.AddChild(txtGpe);
        var cbPlayBoth = new CheckBox("Play both colors");
        cbPlayBoth.IsChecked = _tourneyPlayBothColors;
        content.AddChild(cbPlayBoth);

        content.AddChild(new TextBlock("--- Game Config ---"));
        content.AddChild(new TextBlock("Max Moves (Game Length):"));
        var txtMaxMoves = new TextBox(_tourneyMaxMoves.ToString());
        StyleTextBox(txtMaxMoves);
        content.AddChild(txtMaxMoves);
        content.AddChild(new TextBlock("Opening Position (FEN):"));
        var txtOpening = new TextBox(_tourneyOpeningFen);
        StyleTextBox(txtOpening);
        content.AddChild(txtOpening);

        ShowDialog("New Tournament", content, () => {
            _tourneyName = txtName.Text.Trim();
            _tourneyPgnPath = txtPgn.Text.Trim();
            _tourneyEpdPath = txtEpd.Text.Trim();
            _tourneyEngineA = (cbEngA.SelectedItem as EngineConfig)?.Path ?? "stockfish.exe";
            _tourneyEngineB = (cbEngB.SelectedItem as EngineConfig)?.Path ?? "stockfish.exe";
            _tourneyType = txtType.Text.Trim();
            int.TryParse(txtRounds.Text, out _tourneyRounds);
            int.TryParse(txtGpe.Text, out _tourneyGamesPerEncounter);
            _tourneyPlayBothColors = cbPlayBoth.IsChecked ?? true;
            int.TryParse(txtMaxMoves.Text, out _tourneyMaxMoves);
            _tourneyOpeningFen = txtOpening.Text.Trim();

            StartTournament();
        });
    }

    private void OpenPreferencesDialog() {
        var content = new StackPanel(Orientation.VERTICAL, 1);
        content.AddChild(new TextBlock("Preferences:"));

        var cbSound = new CheckBox("Enable Sound Effects");
        cbSound.IsChecked = _enableSound;

        var cbAnimations = new CheckBox("Enable Board Animations");
        cbAnimations.IsChecked = _enableAnimations;

        var cbLetters = new CheckBox("Use Letter Pieces (K, Q, B, N, R, P)");
        cbLetters.IsChecked = _useLetterPieces;

        var cbCompact = new CheckBox("Compact Board (1x2 fields)");
        cbCompact.IsChecked = _useCompactBoard;

        content.AddChild(cbSound);
        content.AddChild(cbAnimations);
        content.AddChild(cbLetters);
        content.AddChild(cbCompact);

        ShowDialog("Preferences", content, () => {
            _enableSound = cbSound.IsChecked ?? true;
            _enableAnimations = cbAnimations.IsChecked ?? true;
            _useLetterPieces = cbLetters.IsChecked ?? false;
            _useCompactBoard = cbCompact.IsChecked ?? false;

            SavePreferences();

            UpdateBoardLayout();
            UpdateLabels();
            UpdateBoardUi();
        });
    }

    private void OpenEnginesDialog() {
        var content = new StackPanel(Orientation.VERTICAL, 1);
        content.AddChild(new TextBlock("Added Engines:"));

        for (int i = 0; i < _addedEngines.Count; i++) {
            var engine = _addedEngines[i];
            var row = new StackPanel(Orientation.HORIZONTAL, 2);
            row.AddChild(new TextBlock($"{i + 1}. {engine.Name} ({engine.Path})"));

            var btnRemove = new Button(" Remove ");
            StyleButton(btnRemove, COLOR_BTN_BG, COLOR_FG);

            btnRemove.OnClick += _ => {
                _addedEngines.Remove(engine);

                if (_whiteEnginePath == engine.Path) {
                    _whiteEnginePath = _addedEngines.Count > 0 ? _addedEngines[0].Path : "stockfish.exe";
                }

                if (_blackEnginePath == engine.Path) {
                    _blackEnginePath = _addedEngines.Count > 0 ? _addedEngines[0].Path : "stockfish.exe";
                }

                if (_tourneyEngineA == engine.Path) {
                    _tourneyEngineA = _addedEngines.Count > 0 ? _addedEngines[0].Path : "stockfish.exe";
                }

                if (_tourneyEngineB == engine.Path) {
                    _tourneyEngineB = _addedEngines.Count > 0 ? _addedEngines[0].Path : "stockfish.exe";
                }

                SavePreferences();
                OpenEnginesDialog();
            };

            row.AddChild(btnRemove);
            content.AddChild(row);
        }

        var btnAddEngine = new Button(" Add Engine... ");
        StyleButton(btnAddEngine, COLOR_BTN_ACTIVE_BG, COLOR_FG);
        btnAddEngine.OnClick += _ => OpenAddEngineDialog();
        content.AddChild(btnAddEngine);

        ShowDialog("Manage Engines", content, () => {
            // Selections and removals are saved immediately
        });
    }

    private void ShowWarningDialog(string message, Action onClose) {
        var content = new StackPanel(Orientation.VERTICAL, 1);
        content.AddChild(new TextBlock(message));
        ShowDialog("Warning", content, onClose, onClose);
    }

    private void OpenAddEngineDialog(string defaultName = "", string defaultPath = "") {
        var content = new StackPanel(Orientation.VERTICAL, 1);
        content.AddChild(new TextBlock("Engine Name:"));
        var txtName = new TextBox(defaultName);
        StyleTextBox(txtName);
        content.AddChild(txtName);

        content.AddChild(new TextBlock("Engine Path:"));
        var txtPath = new TextBox(defaultPath);
        StyleTextBox(txtPath);
        content.AddChild(txtPath);

        ShowDialog("Add Engine", content, () => {
            string name = txtName.Text.Trim();
            string path = txtPath.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path)) {
                ShowWarningDialog("Name and path cannot be empty.", () => OpenAddEngineDialog(name, path));

                return;
            }

            if (_addedEngines.Any(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) {
                ShowWarningDialog($"An engine with name '{name}' already exists.", () => OpenAddEngineDialog(name, path));

                return;
            }

            if (_addedEngines.Any(e => e.Path.Equals(path, StringComparison.OrdinalIgnoreCase))) {
                ShowWarningDialog($"An engine with path '{path}' already exists.", () => OpenAddEngineDialog(name, path));

                return;
            }

            _addedEngines.Add(new EngineConfig { Name = name, Path = path });
            SavePreferences();
            OpenEnginesDialog();
        }, OpenEnginesDialog);
    }

    private void OpenResultsDialog() {
        var content = new StackPanel(Orientation.VERTICAL, 1);
        content.AddChild(new TextBlock("Finished Tournaments Standings:"));
        content.AddChild(new TextBlock($"{_tourneyName}: Engine A vs Engine B"));
        content.AddChild(new TextBlock(_lblTourneyPentanomial.Content.ToString()));
        content.AddChild(new TextBlock(_lblTourneyElo.Content.ToString()));

        ShowDialog("Tournament Results", content, () => { });
    }

    private void OpenGameMenu() {
        ShowMenuDropdown("Game", [
            ("New (ctrl+n)", OpenNewGameDialog),
            ("Save (ctrl+s)", OpenSaveDialog),
            ("Copy FEN", () => CopyToClipboard(GetActiveGame().Board.ToFen())),
            ("Copy PGN", () => CopyToClipboard(GetPgnString())),
            ("Adjudicate result (ctrl+a)", OpenAdjudicateDialog),
            ("Close (ctrl+q)", () => _app?.Stop())
        ], 1);
    }

    private void OpenTournamentMenu() {
        ShowMenuDropdown("Tournament", [
            ("New (ctrl+t)", OpenNewTournamentDialog),
            ("Results", OpenResultsDialog)
        ], 12);
    }

    private void OpenEditMenu() {
        ShowMenuDropdown("Edit", [
            ("Preferences (ctrl+.)", OpenPreferencesDialog),
            ("Engines (ctrl+e)", OpenEnginesDialog)
        ], 28);
    }

    private void OpenHelpMenu() {
        ShowMenuDropdown("Help", [
            ("Keyboard Shortcuts", () => {
                var content = new StackPanel(Orientation.VERTICAL, 1);
                content.AddChild(new TextBlock("Keyboard Shortcuts:"));
                content.AddChild(new TextBlock("  Ctrl+N : New Game Dialogue"));
                content.AddChild(new TextBlock("  Ctrl+Q : Exit Program"));
                content.AddChild(new TextBlock("  Ctrl+S : Save Game PGN"));
                content.AddChild(new TextBlock("  Ctrl+A : Adjudicate Result"));
                content.AddChild(new TextBlock("  Ctrl+T : New Tournament"));
                content.AddChild(new TextBlock("  Ctrl+E : Manage Engines"));
                content.AddChild(new TextBlock("  Ctrl+. : Preferences"));
                ShowDialog("Help", content, () => { });
            }),
            ("About", () => {
                var content = new StackPanel(Orientation.VERTICAL, 1);
                content.AddChild(new TextBlock("NKChess TUI v1.0"));
                content.AddChild(new TextBlock("A modern tournament and game controls client for UCI chess engines."));
                ShowDialog("About", content, () => { });
            })
        ], 38);
    }

    private string GetPgnString() {
        var active = GetActiveGame();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("[Event \"NKChess Session\"]");
        sb.AppendLine($"[White \"{active.WhiteName}\"]");
        sb.AppendLine($"[Black \"{active.BlackName}\"]");
        sb.AppendLine($"[Result \"*\"]");
        sb.AppendLine();

        for (int i = 0; i < active.PlayHistory.Count; i++) {
            if (i % 2 == 0) sb.Append($"{i / 2 + 1}. {active.PlayHistory[i]}");
            else sb.Append($" {active.PlayHistory[i]}  ");
        }

        return sb.ToString();
    }

    private void CopyToClipboard(string text) {
        try {
            var psi = new System.Diagnostics.ProcessStartInfo {
                FileName = "powershell",
                Arguments = $"-Command \"Set-Clipboard -Value '{text.Replace("'", "''")}'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = System.Diagnostics.Process.Start(psi);
            proc?.WaitForExit();
            _lblPlayStatus.Content = "Status: Copied to clipboard successfully.";
        }
        catch (Exception ex) {
            _lblPlayStatus.Content = $"Status: Clipboard copy failed: {ex.Message}";
        }
    }

    private void ShowMenuDropdown(string title, (string Label, Action Action)[] items, int leftOffsetChar) {
        if (_activeDialogOverlay != null) {
            _topLevelGrid.RemoveChild(_activeDialogOverlay);
            _activeDialogOverlay = null;
        }

        var overlay = new Grid();

        var dismissPanel = new Button("");
        dismissPanel.Style.Set(new BackgroundColorProperty(NKColor.Inherit));
        dismissPanel.Style.Set(new BorderProperty(BorderStyle.GetBorderless()));
        dismissPanel.OnHover += () => { };
        dismissPanel.OnHoverOut += () => { };

        dismissPanel.OnClick += _ => {
            _topLevelGrid.RemoveChild(overlay);
            _activeDialogOverlay = null;
            _app?.Screen?.Render();
        };

        overlay.AddChild(dismissPanel, 0, 0, 3);

        var menuBox = new StackPanel(Orientation.VERTICAL);
        menuBox.Style.Set(new BackgroundColorProperty(COLOR_PANEL_BG));
        menuBox.Style.Set(new BorderProperty(BorderStyle.GetNormal(NKColor.FromRgb(99, 102, 241), COLOR_PANEL_BG)));
        menuBox.Style.Set(new MarginProperty(new Spacing(Dimension.Chars(leftOffsetChar), Dimension.Zero)));
        menuBox.Style.Set(new WidthProperty(Dimension.Chars(30)));

        foreach (var item in items) {
            var btn = new Button($"  {item.Label} ");
            StyleMenuDropdownItemButton(btn);

            btn.OnClick += _ => {
                _topLevelGrid.RemoveChild(overlay);
                _activeDialogOverlay = null;
                item.Action();
                _app?.Screen?.Render();
            };

            menuBox.AddChild(btn);
        }

        overlay.AddChild(menuBox, 0, 0, 3);

        _activeDialogOverlay = overlay;
        _topLevelGrid.AddChild(overlay, 1, 0, 2);
        _app?.Screen?.Render();
    }

    private void StyleMenuDropdownItemButton(Button btn) {
        btn.Style.Set(new BackgroundColorProperty(COLOR_PANEL_BG));
        btn.Style.Set(new TextColorProperty(COLOR_FG));
        btn.Style.Set(new BorderProperty(BorderStyle.GetBorderless()));
        btn.Style.Set(new PaddingProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));
        btn.IsTabStop = true;

        btn.OnHover += () => {
            btn.Style.Set(new BackgroundColorProperty(COLOR_BTN_ACTIVE_BG));
            _app?.Screen?.Render();
        };

        btn.OnHoverOut += () => {
            btn.Style.Set(new BackgroundColorProperty(COLOR_PANEL_BG));
            _app?.Screen?.Render();
        };
    }

    private void SwitchTab(int idx) {
        _activeGameIdx = idx;

        StyleButton(_btnTabSingle, _activeGameIdx == 0 ? COLOR_BTN_ACTIVE_BG : COLOR_BTN_BG, COLOR_FG);
        StyleButton(_btnTabTourney, _activeGameIdx == 1 ? COLOR_BTN_ACTIVE_BG : COLOR_BTN_BG, COLOR_FG);

        var active = GetActiveGame();

        _lblWhitePlayer.Content = $"White: {active.WhiteName}";
        _lblBlackPlayer.Content = $"Black: {active.BlackName}";
        _lblWhiteDetails.Content = active.WhiteDetails;
        _lblBlackDetails.Content = active.BlackDetails;

        UpdateBoardUi();
        UpdateMovesListUi();
        UpdateEvalGraphUi();
        UpdateSliderRange();
    }

    private void UpdateSliderRange() {
        var active = GetActiveGame();
        int count = active.PlayHistory.Count;

        if (count > 0) {
            _movesSlider.IsEnabled = true;
            _movesSlider.Minimum = 0;
            _movesSlider.Maximum = count - 1;

            if (active.MoveIndex == -1 || active.MoveIndex >= count) {
                _movesSlider.Value = count - 1;
            }
            else {
                _movesSlider.Value = active.MoveIndex;
            }
        }
        else {
            _movesSlider.IsEnabled = false;
            _movesSlider.Minimum = 0;
            _movesSlider.Maximum = 0;
            _movesSlider.Value = 0;
        }
    }

    private void UpdateEvalGraphUi() {
        var active = GetActiveGame();
        var history = active.EvalHistory;

        if (history.Count == 0) {
            _lblEvalGraph.Content = "Evaluation Graph: [No Data]";

            return;
        }

        int width = 40;
        int startIdx = Math.Max(0, history.Count - width);
        int count = Math.Min(width, history.Count - startIdx);

        char[,] canvas = new char[5, width];

        for (int r = 0; r < 5; r++) {
            for (int c = 0; c < width; c++) {
                canvas[r, c] = r == 2 ? '-' : ' ';
            }
        }

        for (int i = 0; i < count; i++) {
            double score = history[startIdx + i];
            double clamped = Math.Clamp(score, -3.0, 3.0);
            int r = (int)Math.Round(2.0 - (clamped * 2.0 / 3.0));
            canvas[r, i] = 'o';
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Evaluation Graph (Last 40 moves):");
        string[] labels = { " +3.0 | ", " +1.5 | ", "  0.0 | ", " -1.5 | ", " -3.0 | " };

        for (int r = 0; r < 5; r++) {
            sb.Append(labels[r]);

            for (int c = 0; c < count; c++) {
                sb.Append(canvas[r, c]);
            }

            sb.AppendLine();
        }

        _lblEvalGraph.Content = sb.ToString();
        _app?.Screen?.Render();
    }

    private void SavePreferences() {
        var prefs = new AppPreferences {
            EnableSound = _enableSound,
            EnableAnimations = _enableAnimations,
            UseLetterPieces = _useLetterPieces,
            UseCompactBoard = _useCompactBoard,
            WhiteEnginePath = _whiteEnginePath,
            BlackEnginePath = _blackEnginePath,
            AddedEngines = _addedEngines,
            WhiteTimeLimit = _whiteTimeLimit,
            BlackTimeLimit = _blackTimeLimit
        };

        prefs.Save();
    }

    private void UpdateTimeLabels() {
        var active = GetActiveGame();
        _lblWhiteTimeRemaining.Content = $"W-Time: {FormatTime(active.WhiteTimeLeft)}";
        _lblBlackTimeRemaining.Content = $"B-Time: {FormatTime(active.BlackTimeLeft)}";
    }

    private string FormatTime(int ms) {
        if (ms <= 0) return "0.0s";

        double sec = ms / 1000.0;

        return $"{sec:F1}s";
    }

    private void StartGameTimer() {
        _timerCts?.Cancel();
        _timerCts = new CancellationTokenSource();
        var token = _timerCts.Token;

        Task.Run(async () => {
            while (!token.IsCancellationRequested) {
                await Task.Delay(100);

                if (token.IsCancellationRequested) break;

                var active = GetActiveGame();
                string status = _lblPlayStatus.Content.ToString();

                if (status.Contains("in progress") || status.Contains("thinking")) {
                    bool isWhiteTurn = active.Board.ActiveColor == 'w';

                    if (isWhiteTurn) {
                        active.WhiteTimeLeft = Math.Max(0, active.WhiteTimeLeft - 100);
                    }
                    else {
                        active.BlackTimeLeft = Math.Max(0, active.BlackTimeLeft - 100);
                    }

                    if (_app != null) {
                        lock (_app.SyncRoot) {
                            UpdateTimeLabels();
                            _app.Screen.Render();
                        }
                    }
                }
            }
        });
    }

    private void StopGameTimer() {
        _timerCts?.Cancel();
        _timerCts = null;
    }

    private void UpdateBoardLayout() {
        _boardGrid.RowDefinitions.Clear();
        int rowHeight = _useCompactBoard ? 1 : 2;

        for (int r = 0; r < 8; r++) {
            _boardGrid.RowDefinitions.Add(GridLength.Chars(rowHeight));
        }

        _boardGrid.RowDefinitions.Add(GridLength.Chars(1));

        _boardGrid.ColumnDefinitions.Clear();
        _boardGrid.ColumnDefinitions.Add(GridLength.Chars(2));
        int colWidth = _useCompactBoard ? 2 : 4;

        for (int c = 0; c < 8; c++) {
            _boardGrid.ColumnDefinitions.Add(GridLength.Chars(colWidth));
        }

        if (_mainContentGrid != null && _mainContentGrid.ColumnDefinitions.Count > 0) {
            int boardGridWidth = 2 + (8 * colWidth); // Row label (2) + 8 columns * colWidth
            _mainContentGrid.ColumnDefinitions[0] = GridLength.Chars(boardGridWidth + 2); // boardGridWidth + Margin (2 chars spacing)
        }
    }

    private void UpdateLabels() {
        for (int r = 0; r < 8; r++) {
            _rankLabels[r].Content = $" {(_isFlipped ? r + 1 : 8 - r)}";
        }

        for (int c = 0; c < 8; c++) {
            char fileChar = (char)(_isFlipped ? 'H' - c : 'A' + c);

            if (_useCompactBoard) {
                _fileLabels[c].Content = $"{fileChar} ";
            }
            else {
                _fileLabels[c].Content = $"  {fileChar} ";
            }
        }
    }

    private void FlipBoard() {
        _isFlipped = !_isFlipped;
        UpdateLabels();
        UpdateBoardUi();
    }

    private void TriggerEngineMove() {
        var active = GetActiveGame();
        bool isWhiteTurn = active.Board.ActiveColor == 'w';
        bool isCpu = (isWhiteTurn && active.IsWhiteCpu) || (!isWhiteTurn && active.IsBlackCpu);

        if (!isCpu || _isEngineThinking) return;

        string enginePath = isWhiteTurn ? active.WhiteEnginePath : active.BlackEnginePath;

        if (!File.Exists(enginePath)) {
            _lblPlayStatus.Content = $"Status: Engine not found at {enginePath}!";

            return;
        }

        _isEngineThinking = true;
        _lblPlayStatus.Content = "Status: Engine thinking...";

        var uciEngine = new UciEngine(enginePath);

        if (isWhiteTurn) _whiteEngine = uciEngine;
        else _blackEngine = uciEngine;

        uciEngine.InfoReceived += info => { UpdateEngineInfo(active, isWhiteTurn ? 'w' : 'b', info); };

        Task.Run(async () => {
            bool moveMade = false;

            try {
                if (uciEngine.Start()) {
                    string fen = active.Board.ToFen();

                    string uciMove = await uciEngine.GetBestMoveAsync(
                        fen,
                        active.WhiteTimeLeft,
                        active.BlackTimeLeft,
                        _whiteIncrement * 1000,
                        _blackIncrement * 1000,
                        CancellationToken.None);

                    if (!string.IsNullOrEmpty(uciMove)) {
                        Move m = Move.ParseUci(uciMove);
                        var legal = active.Board.GetLegalMoves();
                        bool isLegal = false;

                        foreach (var lm in legal) {
                            if (lm.From == m.From && lm.To == m.To && (lm.Promotion == '\0' || lm.Promotion == m.Promotion)) {
                                m = lm;
                                isLegal = true;

                                break;
                            }
                        }

                        if (isLegal) {
                            active.PlayHistory.Add(m.ToUci());
                            active.Board.MakeMove(m);

                            if (isWhiteTurn) {
                                active.WhiteTimeLeft += _whiteIncrement * 1000;
                            }
                            else {
                                active.BlackTimeLeft += _blackIncrement * 1000;
                            }

                            active.MoveIndex = -1;
                            moveMade = true;

                            // Check endgame status
                            var nextLegal = active.Board.GetLegalMoves();

                            if (nextLegal.Count == 0) {
                                if (active.Board.IsInCheck(active.Board.ActiveColor)) {
                                    _lblPlayStatus.Content = $"Status: Checkmate! {(isWhiteTurn ? active.WhiteName : active.BlackName)} wins.";
                                }
                                else {
                                    _lblPlayStatus.Content = "Status: Draw by Stalemate.";
                                }

                                StopGameTimer();
                            }
                            else if (active.Board.HalfmoveClock >= 100) {
                                _lblPlayStatus.Content = "Status: Draw by 50-move rule.";
                                StopGameTimer();
                            }
                            else {
                                _lblPlayStatus.Content = "Status: Game in progress.";
                            }
                        }
                        else {
                            _lblPlayStatus.Content = $"Status: Engine returned illegal move {uciMove}!";
                            StopGameTimer();
                        }
                    }
                    else {
                        _lblPlayStatus.Content = "Status: Engine failed to return a move!";
                        StopGameTimer();
                    }
                }
                else {
                    _lblPlayStatus.Content = "Status: Failed to start engine process!";
                    StopGameTimer();
                }
            }
            catch (Exception ex) {
                _lblPlayStatus.Content = $"Status: Engine error: {ex.Message}";
                StopGameTimer();
            }
            finally {
                uciEngine.Dispose();

                if (_app != null) {
                    lock (_app.SyncRoot) {
                        _isEngineThinking = false;

                        if (GetActiveGame() == active) {
                            UpdateBoardUi();
                            UpdateMovesListUi();
                            UpdateSliderRange();
                        }

                        if (moveMade) {
                            var nextTurnWhite = active.Board.ActiveColor == 'w';
                            var nextCpu = (nextTurnWhite && active.IsWhiteCpu) || (!nextTurnWhite && active.IsBlackCpu);

                            if (nextCpu) {
                                TriggerEngineMove();
                            }
                        }
                    }
                }
                else {
                    _isEngineThinking = false;
                }
            }
        });
    }

    private void UpdateEngineInfo(GameState game, char color, string infoLine) {
        if (_app == null) return;

        lock (_app.SyncRoot) {
            var parts = infoLine.Split(' ');
            string depth = "";
            string score = "";
            string nps = "";
            string nodes = "";
            string hash = "";
            string tbhits = "";
            string time = "";
            string pv = "";
            string ponder = "";

            for (int i = 0; i < parts.Length - 1; i++) {
                if (parts[i] == "depth") depth = parts[i + 1];
                if (parts[i] == "nps") nps = parts[i + 1];
                if (parts[i] == "nodes") nodes = parts[i + 1];
                if (parts[i] == "hashfull") hash = parts[i + 1];
                if (parts[i] == "tbhits") tbhits = parts[i + 1];
                if (parts[i] == "time") time = parts[i + 1];
                if (parts[i] == "ponder") ponder = parts[i + 1];

                if (parts[i] == "score") {
                    if (parts[i + 1] == "cp") score = parts[i + 2];
                    else if (parts[i + 1] == "mate") score = "M" + parts[i + 2];
                }

                if (parts[i] == "pv") {
                    pv = string.Join(" ", parts[(i + 1)..Math.Min(parts.Length, i + 6)]);
                }
            }

            var logLbl = color == 'w' ? _lblWhiteInfoLog : _lblBlackInfoLog;
            var npsLbl = color == 'w' ? _lblWhiteInfoNps : _lblBlackInfoNps;
            var hashLbl = color == 'w' ? _lblWhiteInfoHash : _lblBlackInfoHash;
            var ponderLbl = color == 'w' ? _lblWhiteInfoPonder : _lblBlackInfoPonder;
            var tbLbl = color == 'w' ? _lblWhiteInfoTb : _lblBlackInfoTb;
            var depthLbl = color == 'w' ? _lblWhiteInfoDepth : _lblBlackInfoDepth;
            var timeLbl = color == 'w' ? _lblWhiteInfoTime : _lblBlackInfoTime;
            var nodesLbl = color == 'w' ? _lblWhiteInfoNodes : _lblBlackInfoNodes;
            var scoreLbl = color == 'w' ? _lblWhiteInfoScore : _lblBlackInfoScore;
            var pvLbl = color == 'w' ? _lblWhiteInfoPv : _lblBlackInfoPv;

            if (!string.IsNullOrEmpty(infoLine)) logLbl.Content = $"Log: {(infoLine.Length > 35 ? infoLine.Substring(0, 35) + "..." : infoLine)}";
            if (!string.IsNullOrEmpty(depth)) depthLbl.Content = $"Depth: {depth}";

            if (!string.IsNullOrEmpty(nps)) {
                if (double.TryParse(nps, out double nVal)) {
                    npsLbl.Content = $"NPS: {(nVal / 1000.0):F0}k";
                }
                else {
                    npsLbl.Content = $"NPS: {nps}";
                }
            }

            if (!string.IsNullOrEmpty(nodes)) nodesLbl.Content = $"Nodes: {nodes}";
            if (!string.IsNullOrEmpty(hash)) hashLbl.Content = $"Hash: {hash}‰";
            if (!string.IsNullOrEmpty(tbhits)) tbLbl.Content = $"TB: {tbhits}";

            if (!string.IsNullOrEmpty(time)) {
                timeLbl.Content = $"Time: {time}ms";
            }

            if (!string.IsNullOrEmpty(ponder)) ponderLbl.Content = $"Ponder: {ponder}";

            if (!string.IsNullOrEmpty(score)) {
                if (score.StartsWith("M")) {
                    scoreLbl.Content = $"Score: {score}";
                }
                else {
                    if (double.TryParse(score, out double cp)) {
                        scoreLbl.Content = $"Score: {(cp >= 0 ? "+" : "")}{(cp / 100.0):F2}";
                        double whiteScore = color == 'w' ? cp / 100.0 : -cp / 100.0;
                        game.EvalHistory.Add(whiteScore);
                        UpdateEvalGraphUi();
                        UpdateEvaluationBar(cp, false, 0, color);
                    }
                }
            }

            if (!string.IsNullOrEmpty(pv)) pvLbl.Content = $"PV: {pv}";

            _app?.Screen?.Render();
        }
    }

    private void UpdateEngineInfoFromParsedString(GameState game, char color, string parsedStr) {
        if (string.IsNullOrEmpty(parsedStr)) return;

        var parts = parsedStr.Split(' ');
        string depth = "";
        string score = "";
        string nps = "";
        string nodes = "";
        string hash = "";
        string tbhits = "";
        string time = "";
        string ponder = "";
        string pv = "";

        for (int i = 0; i < parts.Length; i++) {
            if (parts[i].StartsWith("D:")) depth = parts[i].Substring(2);
            if (parts[i].StartsWith("NPS:")) nps = parts[i].Substring(4);
            if (parts[i].StartsWith("Nodes:")) nodes = parts[i].Substring(6);
            if (parts[i].StartsWith("Hash:")) hash = parts[i].Substring(5);
            if (parts[i].StartsWith("TB:")) tbhits = parts[i].Substring(3);
            if (parts[i].StartsWith("Time:")) time = parts[i].Substring(5);
            if (parts[i].StartsWith("Ponder:")) ponder = parts[i].Substring(7);

            if (parts[i].StartsWith("S:")) {
                score = parts[i].Substring(2);
                if (i + 1 < parts.Length && parts[i + 1] == "cp") score += " cp";
            }

            if (parts[i] == "PV:") {
                pv = string.Join(" ", parts[(i + 1)..Math.Min(parts.Length, i + 6)]);
            }
        }

        var logLbl = color == 'w' ? _lblWhiteInfoLog : _lblBlackInfoLog;
        var npsLbl = color == 'w' ? _lblWhiteInfoNps : _lblBlackInfoNps;
        var hashLbl = color == 'w' ? _lblWhiteInfoHash : _lblBlackInfoHash;
        var ponderLbl = color == 'w' ? _lblWhiteInfoPonder : _lblBlackInfoPonder;
        var tbLbl = color == 'w' ? _lblWhiteInfoTb : _lblBlackInfoTb;
        var depthLbl = color == 'w' ? _lblWhiteInfoDepth : _lblBlackInfoDepth;
        var timeLbl = color == 'w' ? _lblWhiteInfoTime : _lblBlackInfoTime;
        var nodesLbl = color == 'w' ? _lblWhiteInfoNodes : _lblBlackInfoNodes;
        var scoreLbl = color == 'w' ? _lblWhiteInfoScore : _lblBlackInfoScore;
        var pvLbl = color == 'w' ? _lblWhiteInfoPv : _lblBlackInfoPv;

        logLbl.Content = $"Log: {parsedStr}";
        if (!string.IsNullOrEmpty(depth)) depthLbl.Content = $"Depth: {depth}";
        if (!string.IsNullOrEmpty(nps)) npsLbl.Content = $"NPS: {nps}";
        if (!string.IsNullOrEmpty(nodes)) nodesLbl.Content = $"Nodes: {nodes}";
        if (!string.IsNullOrEmpty(hash)) hashLbl.Content = $"Hash: {hash}‰";
        if (!string.IsNullOrEmpty(tbhits)) tbLbl.Content = $"TB: {tbhits}";
        if (!string.IsNullOrEmpty(time)) timeLbl.Content = $"Time: {time}ms";
        if (!string.IsNullOrEmpty(ponder)) ponderLbl.Content = $"Ponder: {ponder}";
        if (!string.IsNullOrEmpty(pv)) pvLbl.Content = $"PV: {pv}";

        if (!string.IsNullOrEmpty(score)) {
            scoreLbl.Content = $"Score: {score}";

            try {
                if (score.Contains("cp")) {
                    double cp = double.Parse(score.Replace("cp", "").Trim());
                    double whiteScore = color == 'w' ? cp / 100.0 : -cp / 100.0;
                    game.EvalHistory.Add(whiteScore);
                    UpdateEvalGraphUi();
                    UpdateEvaluationBar(cp, false, 0, color);
                }
            }
            catch {
                // ignored
            }
        }
    }

    private void StartTournament() {
        if (_activeTournament is { IsRunning: true }) return;

        _lblTourneyProgress.Content = "Starting tournament...";

        string pathA = _tourneyEngineA.Trim();
        string pathB = _tourneyEngineB.Trim();

        _activeTournament = new Tournament(pathA, pathB, _whiteTimeLimit * 1000, _tourneyRounds) {
            PgnOutputPath = _tourneyPgnPath,
            EpdOutputPath = _tourneyEpdPath,
            TournamentType = _tourneyType,
            PlayBothColors = _tourneyPlayBothColors,
            MaxMoves = _tourneyMaxMoves,
            OpeningFen = _tourneyOpeningFen
        };

        _activeTournament.LogMessage += msg => {
            if (_app == null) return;

            lock (_app.SyncRoot) {
                _lblTourneyLog.Content = msg;
                _app?.Screen?.Render();
            }
        };

        _activeTournament.BoardUpdated += () => {
            if (_app == null) return;

            lock (_app.SyncRoot) {
                var tourneyGame = _games[1];
                Array.Copy(_activeTournament.CurrentBoard.Squares, tourneyGame.Board.Squares, 64);
                tourneyGame.Board.ActiveColor = _activeTournament.CurrentBoard.ActiveColor;
                tourneyGame.Board.CastlingRights = _activeTournament.CurrentBoard.CastlingRights;
                tourneyGame.Board.EnPassantSquare = _activeTournament.CurrentBoard.EnPassantSquare;
                tourneyGame.Board.HalfmoveClock = _activeTournament.CurrentBoard.HalfmoveClock;
                tourneyGame.Board.FullmoveNumber = _activeTournament.CurrentBoard.FullmoveNumber;

                tourneyGame.StartingFen = string.IsNullOrEmpty(_activeTournament.OpeningFen)
                    ? "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
                    : _activeTournament.OpeningFen;

                tourneyGame.PlayHistory.Clear();
                tourneyGame.PlayHistory.AddRange(_activeTournament.CurrentMoveHistory);
                tourneyGame.MoveIndex = -1;

                if (_activeGameIdx == 1) {
                    UpdateBoardUi();
                    UpdateMovesListUi();
                    UpdateSliderRange();
                }
            }
        };

        _activeTournament.ProgressUpdated += () => {
            if (_app == null) return;

            lock (_app.SyncRoot) {
                var tourneyGame = _games[1];
                tourneyGame.WhiteName = _activeTournament.IsEngineAPlayingWhite ? _activeTournament.EngineAName : _activeTournament.EngineBName;
                tourneyGame.BlackName = _activeTournament.IsEngineAPlayingWhite ? _activeTournament.EngineBName : _activeTournament.EngineAName;

                _lblTourneyProgress.Content = $"Pairing: {_activeTournament.CurrentPairIndex}/{_activeTournament.TotalPairs} | Game: {_activeTournament.CurrentGameIndex}";

                string whiteInfo = _activeTournament.IsEngineAPlayingWhite ? _activeTournament.EngineALastInfo : _activeTournament.EngineBLastInfo;
                string blackInfo = _activeTournament.IsEngineAPlayingWhite ? _activeTournament.EngineBLastInfo : _activeTournament.EngineALastInfo;

                tourneyGame.WhiteDetails = ParseEngineDetails(whiteInfo);
                tourneyGame.BlackDetails = ParseEngineDetails(blackInfo);

                if (_activeGameIdx == 1) {
                    _lblWhitePlayer.Content = $"White: {tourneyGame.WhiteName}";
                    _lblBlackPlayer.Content = $"Black: {tourneyGame.BlackName}";
                    _lblWhiteDetails.Content = tourneyGame.WhiteDetails;
                    _lblBlackDetails.Content = tourneyGame.BlackDetails;

                    UpdateEngineInfoFromParsedString(tourneyGame, 'w', whiteInfo);
                    UpdateEngineInfoFromParsedString(tourneyGame, 'b', blackInfo);
                }

                var counts = _activeTournament.PentanomialCounts;
                _lblTourneyPentanomial.Content = $"WW: {counts[4]} | WD: {counts[3]} | DD: {counts[2]} | LD: {counts[1]} | LL: {counts[0]}";

                var (elo, margin) = _activeTournament.CalculateElo();
                _lblTourneyElo.Content = $"Elo Diff: {elo:F1} +/- {margin:F1} Elo";

                _lblTourneyStats.Content = $"A ({_activeTournament.EngineAName}): {_activeTournament.EngineALastInfo}\nB ({_activeTournament.EngineBName}): {_activeTournament.EngineBLastInfo}";
                _app?.Screen?.Render();
            }
        };

        _activeTournament.InfoUpdated += () => {
            if (_app == null) return;

            lock (_app.SyncRoot) {
                var tourneyGame = _games[1];
                string whiteInfo = _activeTournament.IsEngineAPlayingWhite ? _activeTournament.EngineALastInfo : _activeTournament.EngineBLastInfo;
                string blackInfo = _activeTournament.IsEngineAPlayingWhite ? _activeTournament.EngineBLastInfo : _activeTournament.EngineALastInfo;

                tourneyGame.WhiteDetails = ParseEngineDetails(whiteInfo);
                tourneyGame.BlackDetails = ParseEngineDetails(blackInfo);

                if (_activeGameIdx == 1) {
                    _lblWhiteDetails.Content = tourneyGame.WhiteDetails;
                    _lblBlackDetails.Content = tourneyGame.BlackDetails;

                    UpdateEngineInfoFromParsedString(tourneyGame, 'w', whiteInfo);
                    UpdateEngineInfoFromParsedString(tourneyGame, 'b', blackInfo);
                }

                _lblTourneyStats.Content = $"A ({_activeTournament.EngineAName}): {_activeTournament.EngineALastInfo}\nB ({_activeTournament.EngineBName}): {_activeTournament.EngineBLastInfo}";
                _app?.Screen?.Render();
            }
        };

        _activeTournament.Start();
        UpdateTimeLabels();
    }

    private void StopTournament() {
        if (_activeTournament == null || !_activeTournament.IsRunning) return;

        _activeTournament.Stop();
        _lblTourneyProgress.Content = "Tournament stopped.";
        _app?.Screen?.Render();
    }

    private void HandleSquareSelect(int row, int col) {
        var active = GetActiveGame();
        bool isWhiteTurn = active.Board.ActiveColor == 'w';
        bool isCpu = (isWhiteTurn && active.IsWhiteCpu) || (!isWhiteTurn && active.IsBlackCpu);

        if (isCpu) return;

        int idx = GetSquareIndex(row, col);

        if (_selectedSquare == -1) {
            char piece = active.Board.Squares[idx];

            if (piece != '.') {
                bool isWhite = char.IsUpper(piece);
                bool myTurn = (active.Board.ActiveColor == 'w' && isWhite) || (active.Board.ActiveColor == 'b' && !isWhite);

                if (myTurn) {
                    _selectedSquare = idx;
                }
            }
        }
        else {
            if (_selectedSquare == idx) {
                _selectedSquare = -1; // Deselect
            }
            else {
                char piece = active.Board.Squares[idx];
                bool isWhite = char.IsUpper(piece);
                bool ownPiece = piece != '.' && ((active.Board.ActiveColor == 'w' && isWhite) || (active.Board.ActiveColor == 'b' && !isWhite));

                if (ownPiece) {
                    _selectedSquare = idx;
                }
                else {
                    var legal = active.Board.GetLegalMoves();
                    Move selectedMove = default;
                    bool isLegal = false;

                    foreach (var m in legal) {
                        if (m.From == _selectedSquare && m.To == idx) {
                            selectedMove = m;
                            isLegal = true;

                            break;
                        }
                    }

                    if (isLegal) {
                        active.PlayHistory.Add(selectedMove.ToUci());
                        active.Board.MakeMove(selectedMove);

                        if (isWhiteTurn) {
                            active.WhiteTimeLeft += _whiteIncrement * 1000;
                        }
                        else {
                            active.BlackTimeLeft += _blackIncrement * 1000;
                        }

                        active.MoveIndex = -1;
                        _selectedSquare = -1;

                        UpdateMovesListUi();
                        UpdateSliderRange();

                        var nextLegal = active.Board.GetLegalMoves();

                        if (nextLegal.Count == 0) {
                            if (active.Board.IsInCheck(active.Board.ActiveColor)) {
                                _lblPlayStatus.Content = "Status: Checkmate! You win.";
                            }
                            else {
                                _lblPlayStatus.Content = "Status: Draw by Stalemate.";
                            }

                            StopGameTimer();
                        }
                        else if (active.Board.HalfmoveClock >= 100) {
                            _lblPlayStatus.Content = "Status: Draw by 50-move rule.";
                            StopGameTimer();
                        }
                        else {
                            _lblPlayStatus.Content = "Status: Engine thinking...";
                            TriggerEngineMove();
                        }
                    }
                    else {
                        _selectedSquare = -1;
                    }
                }
            }
        }
    }

    private int GetSquareIndex(int row, int col) {
        if (_isFlipped) {
            return (7 - row) * 8 + (7 - col);
        }
        else {
            return row * 8 + col;
        }
    }

    private ChessBoard GetActiveRenderBoard() {
        var active = GetActiveGame();

        if (active.MoveIndex == -1 || active.MoveIndex >= active.PlayHistory.Count) {
            return active.Board;
        }

        var tempBoard = new ChessBoard();
        tempBoard.ParseFen(active.StartingFen);

        for (int i = 0; i <= active.MoveIndex; i++) {
            Move m = Move.ParseUci(active.PlayHistory[i]);
            tempBoard.MakeMove(m);
        }

        return tempBoard;
    }

    private void UpdateBoardUi() {
        var active = GetActiveGame();
        var renderBoard = GetActiveRenderBoard();
        var legal = renderBoard.GetLegalMoves();
        var dests = new HashSet<int>();

        if (_selectedSquare != -1) {
            foreach (var m in legal) {
                if (m.From == _selectedSquare) {
                    dests.Add(m.To);
                }
            }
        }

        bool inCheck = renderBoard.IsInCheck(renderBoard.ActiveColor);
        int kingSq = -1;

        if (inCheck) {
            char kingChar = renderBoard.ActiveColor == 'w' ? 'K' : 'k';

            for (int i = 0; i < 64; i++) {
                if (renderBoard.Squares[i] == kingChar) {
                    kingSq = i;

                    break;
                }
            }
        }

        for (int r = 0; r < 8; r++) {
            for (int c = 0; c < 8; c++) {
                int sqIdx = GetSquareIndex(r, c);
                char piece = renderBoard.Squares[sqIdx];
                var btn = _boardButtons[r, c];

                btn.Content = MapPieceToChar(piece);

                if (piece != '.') {
                    bool isWhite = char.IsUpper(piece);
                    btn.Style.Set(new TextColorProperty(isWhite ? NKColor.FromRgb(255, 255, 255) : NKColor.FromRgb(30, 30, 30)));
                    btn.Style.Set(new TextStyleProperty(TextStyles.BOLD));
                }
                else {
                    btn.Style.Set(new TextColorProperty(NKColor.Inherit));
                    btn.Style.Set(new TextStyleProperty(TextStyles.NONE));
                }

                NKColor bg = ((r + c) % 2 == 0) ? COLOR_LIGHT_SQ : COLOR_DARK_SQ;

                if (sqIdx == kingSq) {
                    bg = COLOR_CHECK;
                }
                else if (sqIdx == _selectedSquare) {
                    bg = COLOR_SELECTED;
                }
                else if (dests.Contains(sqIdx)) {
                    bg = COLOR_VALID_MOVE;
                }
                else if (r == _cursorRow && c == _cursorCol) {
                    bg = COLOR_CURSOR;
                }

                if (btn.IsHovered) {
                    bg = NKColor.FromRgb(
                        (byte)Math.Min(255, (bg.AsRgb >> 16 & 0xFF) + 30),
                        (byte)Math.Min(255, (bg.AsRgb >> 8 & 0xFF) + 30),
                        (byte)Math.Min(255, (bg.AsRgb & 0xFF) + 30)
                    );
                }

                btn.Style.Set(new BackgroundColorProperty(bg));
            }
        }

        _app?.Screen?.Render();
    }

    private string MapPieceToChar(char p) {
        if (p == '.') {
            return _useCompactBoard ? "  " : "   ";
        }

        if (_useLetterPieces) {
            if (_useCompactBoard) {
                return $" {p}";
            }
            else {
                return $"  {p} ";
            }
        }
        else {
            string unicode = p switch {
                'K' => "♔",
                'Q' => "♕",
                'R' => "♖",
                'B' => "♗",
                'N' => "♘",
                'P' => "♙",
                'k' => "♚",
                'q' => "♛",
                'r' => "♜",
                'b' => "♝",
                'n' => "♞",
                'p' => "♟",
                _   => "  "
            };

            return _useCompactBoard ? $"{unicode}" : $" {unicode} ";
        }
    }

    private void OnKeyEvent(NeoKolors.Console.Input.KeyEventArgs e) {
        if (!e.Down) return;

        bool ctrl = (e.Modifiers & (NeoKolors.Console.Input.KeyModifiers.LEFT_CTRL | NeoKolors.Console.Input.KeyModifiers.RIGHT_CTRL)) != 0;

        if (ctrl) {
            switch (e.Key) {
                case KeyCode.N:
                    OpenNewGameDialog();

                    return;
                case KeyCode.Q:
                    _app?.Stop();

                    return;
                case KeyCode.S:
                    OpenSaveDialog();

                    return;
                case KeyCode.A:
                    OpenAdjudicateDialog();

                    return;
                case KeyCode.T:
                    OpenNewTournamentDialog();

                    return;
                case KeyCode.E:
                    OpenEnginesDialog();

                    return;
                case KeyCode.OEM_PERIOD:
                    OpenPreferencesDialog();

                    return;
            }
        }

        if (_activeTournament is { IsRunning: true }) {
            if (e.Key == KeyCode.ESCAPE) {
                StopTournament();
            }

            return;
        }

        bool updated = false;

        switch (e.Key) {
            case KeyCode.ARROW_UP:
                _cursorRow = Math.Max(0, _cursorRow - 1);
                updated = true;

                break;
            case KeyCode.ARROW_DOWN:
                _cursorRow = Math.Min(7, _cursorRow + 1);
                updated = true;

                break;
            case KeyCode.ARROW_LEFT:
                _cursorCol = Math.Max(0, _cursorCol - 1);
                updated = true;

                break;
            case KeyCode.ARROW_RIGHT:
                _cursorCol = Math.Min(7, _cursorCol + 1);
                updated = true;

                break;
            case KeyCode.SPACE:
            case KeyCode.RETURN:
                HandleSquareSelect(_cursorRow, _cursorCol);
                updated = true;

                break;
            case KeyCode.ESCAPE:
                _selectedSquare = -1;
                updated = true;

                break;
        }

        if (updated) {
            UpdateBoardUi();
        }
    }

    // Cleanup assets
    protected override void RenderCore(ICharCanvas canvas) {
        base.RenderCore(canvas);
    }

    private void UpdateMovesListUi() {
        var history = _activeTournament is { IsRunning: true } ? _activeTournament.CurrentMoveHistory : GetActiveGame().PlayHistory;
        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < history.Count; i++) {
            if (i % 2 == 0) {
                sb.Append($"{i / 2 + 1}. {history[i]}");
            }
            else {
                sb.AppendLine($" {history[i]}  ");
            }
        }

        _lblMovesList.Content = sb.ToString();
        _app?.Screen?.Render();
    }

    private void UpdateEvaluationBar(double scoreCp, bool isMate, int mateIn, char activeColor) {
        double whiteScore = activeColor == 'w' ? scoreCp : -scoreCp;

        if (isMate) {
            int whiteMate = activeColor == 'w' ? mateIn : -mateIn;

            if (whiteMate > 0) {
                _lblEvalBar.Content = " [████████████] ";
                _lblEvalText.Content = $"Eval: M+{whiteMate}";
            }
            else {
                _lblEvalBar.Content = " [░░░░░░░░░░░░] ";
                _lblEvalText.Content = $"Eval: M-{-whiteMate}";
            }
        }
        else {
            int filled = (int)Math.Clamp((whiteScore + 300) / 50.0, 0, 12);
            string barStr = new string('█', filled) + new string('░', 12 - filled);
            _lblEvalBar.Content = $" [{barStr}] ";
            double scorePawn = whiteScore / 100.0;
            _lblEvalText.Content = $"Eval: {(scorePawn >= 0 ? "+" : "")}{scorePawn:F2}";
        }

        _app?.Screen?.Render();
    }

    private void ParseAndUpdateEval(string info, char activeColor) {
        if (string.IsNullOrEmpty(info)) return;

        try {
            var parts = info.Split(' ');

            foreach (var p in parts) {
                if (p.StartsWith("S:")) {
                    var val = p.Substring(2);

                    if (val.StartsWith("M")) {
                        if (int.TryParse(val.Substring(1), out int mateVal)) {
                            UpdateEvaluationBar(0, true, mateVal, activeColor);
                        }
                    }
                    else {
                        if (double.TryParse(val, out double cp)) {
                            UpdateEvaluationBar(cp, false, 0, activeColor);
                        }
                    }

                    break;
                }
            }
        }
        catch { }
    }

    private string ParseEngineDetails(string info) {
        if (string.IsNullOrEmpty(info)) return "Idle";

        string depth = "";
        string nps = "";
        var parts = info.Split(' ');

        foreach (var p in parts) {
            if (p.StartsWith("D:")) depth = p.Substring(2);
            if (p.StartsWith("NPS:")) nps = p.Substring(4);
        }

        string details = "";
        if (!string.IsNullOrEmpty(depth)) details += $"Depth: {depth}  ";
        if (!string.IsNullOrEmpty(nps)) details += $"NPS: {nps}";

        return string.IsNullOrEmpty(details) ? "Searching..." : details;
    }

    private void StyleMenuButton(Button btn) {
        btn.Style.Set(new BackgroundColorProperty(NKColor.FromRgb(30, 41, 59))); // slate-800
        btn.Style.Set(new TextColorProperty(NKColor.FromRgb(241, 245, 249))); // slate-100
        btn.Style.Set(new BorderProperty(BorderStyle.GetBorderless()));
        btn.Style.Set(new PaddingProperty(new Spacing(Dimension.Chars(1), Dimension.Zero)));
        btn.Style.Set(new TextStyleProperty(TextStyles.BOLD));
        btn.IsTabStop = true;

        btn.OnHover += () => {
            btn.Style.Set(new BackgroundColorProperty(NKColor.FromRgb(51, 65, 85))); // slate-700
            _app?.Screen?.Render();
        };

        btn.OnHoverOut += () => {
            btn.Style.Set(new BackgroundColorProperty(NKColor.FromRgb(30, 41, 59))); // slate-800
            _app?.Screen?.Render();
        };
    }
}