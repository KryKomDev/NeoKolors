// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;

namespace NKChess;

public partial class MainView {
    
    // private static readonly NKColor FILE_BLACK_BG_COLOR = NKColor.FromRgb(0xcf4113); 
    private static readonly NKColor FILE_BLACK_BG_COLOR = NKColor.DarkRed; 
    // private static readonly NKColor FILE_WHITE_BG_COLOR = NKColor.FromRgb(0xf4f0ea);
    private static readonly NKColor FILE_WHITE_BG_COLOR = NKColor.White;
    private static readonly NKColor FILE_BLACK_BG_HOVER_COLOR = NKColor.Red;
    private static readonly NKColor FILE_WHITE_BG_HOVER_COLOR = NKColor.FromRgb(0xf4f0ea + 0x0a0a0a);

    private Button[,] _board = new Button[8, 8];
    private NKApplication? _app;
    
    private bool _useLetterPieces;
    private bool _useCompactBoard;
    private bool _enableSound;
    private bool _enableAnimations;
    private string? _whiteEnginePath = null;
    private string? _blackEnginePath = null;
    private List<EngineConfig> _addedEngines = [];
    private string? _tourneyEngineA = null;
    private string? _tourneyEngineB = null;
    
    private Game _currentGame;

    public MainView() {
        
        // load preferences
        LoadUserPreferences();

        _currentGame = new Game(
            new TimeControl(new TimeSpan(0, 10, 0), TimeSpan.FromSeconds(1), 1), 
            new TimeControl(new TimeSpan(0, 10, 0), TimeSpan.FromSeconds(1), 1)
        );
        
        InitializeComponent();

        InitializeUi();
    }

    public void SetApp(NKApplication? app) {
        _app = app;
    }

    private void InitializeUi() {
        SetupMainGrid();
        SetupTopNavbar();
        SetupGamePanel();
        SetupMovesList();
        SetupWhiteInfo();
        SetupBlackInfo();
    }

    private void LoadUserPreferences() {
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
    }

    private void SetupBlackInfo() {
        BlackInfoPanel.Style.Border = BorderStyle.GetNormal();
        BlackInfoPanel.RowDefinitions.AddRange(
            GridLength.Chars(1),
            GridLength.Chars(1), 
            GridLength.Chars(1), 
            GridLength.Chars(1),
            GridLength.Chars(1),
            GridLength.Star()
        );
        
        BlackInfoPanel.ColumnDefinitions.AddRange(
            GridLength.Star(), 
            GridLength.Star(), 
            GridLength.Star(),
            GridLength.Star()
        );
    }

    private void SetupWhiteInfo() {
        WhiteInfoPanel.Style.Border = BorderStyle.GetNormal();
        WhiteInfoPanel.RowDefinitions.AddRange(
            GridLength.Chars(1),
            GridLength.Chars(1), 
            GridLength.Chars(1), 
            GridLength.Chars(1),
            GridLength.Chars(1),
            GridLength.Star()
        );
        
        WhiteInfoPanel.ColumnDefinitions.AddRange(
            GridLength.Star(), 
            GridLength.Star(), 
            GridLength.Star(),
            GridLength.Star()
        );
    }

    private void SetupMovesList() {
        MovesPanel.Style.Border = BorderStyle.GetNormal();
    }

    private void SetupGamePanel() {
        GameGrid.ColumnDefinitions.AddRange(GridLength.Star(), GridLength.Star(), GridLength.Star());
        GameGrid.RowDefinitions.AddRange(GridLength.Chars(1), GridLength.Chars(1), GridLength.Star());
        
        // chessboard
        for (int i = 0; i < 9; i++) ChessBoard.ColumnDefinitions.Add(GridLength.Chars(4));
        for (int i = 0; i < 9; i++) ChessBoard.RowDefinitions.Add(GridLength.Chars(2));
        
        for (int i = 0; i < 8; i++) {
            ChessBoard.AddChild(
                new TextBlock($"{8 - i}") {
                    Style = {
                        TextAlign = new Align(HorizontalAlign.CENTER, VerticalAlign.CENTER),
                    }
                }, 
                i, 
                0
            );
        }

        for (int i = 0; i < 8; i++) {
            ChessBoard.AddChild(
                new TextBlock($"{"abcdefgh"[i]}") {
                    Style = {
                        TextAlign = new Align(HorizontalAlign.CENTER, VerticalAlign.CENTER),
                    }
                }, 
                8, 
                i + 1
            );
        }

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                
                var fb = new Button("") {
                    Style = new StyleCollection {
                        Padding = Spacing.Zero,
                        Margin = Spacing.Zero,
                    }
                };

                var bgColor = (x + y) % 2 == 1 ? FILE_BLACK_BG_COLOR : FILE_WHITE_BG_COLOR;
                var bgHover = FILE_BLACK_BG_HOVER_COLOR;
                
                fb.Style.BackgroundColor = bgColor;
                fb.OnHover += () => fb.Style.BackgroundColor = bgHover;
                fb.OnHoverOut += () => fb.Style.BackgroundColor = bgColor;
                
                ChessBoard.AddChild(fb, y, x + 1);
                _board[x, 7 - y] = fb;
            }
        }
    }

    private void SetupTopNavbar() {
        TopNav.Style.BackgroundColor = NKColor.DarkGray;
        GameButton.Content = new TextBlock(AnsiString.Parse("{:u}G{:!u}ame"));
        TournamentButton.Content = new TextBlock(AnsiString.Parse("{:u}T{:!u}ournament"));
        EditButton.Content = new TextBlock(AnsiString.Parse("{:u}E{:!u}dit"));
        HelpButton.Content = new TextBlock(AnsiString.Parse("{:u}H{:!u}elp"));

        GameButton.OnClick += _ => OpenGameMenu();
    }

    private void OpenGameMenu() {
        
    }

    private void SetupMainGrid() {
        MainGrid.RowDefinitions.Add(GridLength.Chars(1));
        MainGrid.RowDefinitions.Add(GridLength.Star());
        MainGrid.RowDefinitions.Add(GridLength.Chars(1));

        MainGrid.ColumnDefinitions.Add(GridLength.Chars(40));
        MainGrid.ColumnDefinitions.Add(GridLength.Chars(22));
        MainGrid.ColumnDefinitions.Add(GridLength.Star());
        MainGrid.ColumnDefinitions.Add(GridLength.Star());
    }
}