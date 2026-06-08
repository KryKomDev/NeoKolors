# NKChess: TUI Chess Engine Controller & Tournament Director

NKChess is a Terminal User Interface (TUI) application built using the `NeoKolors.Tui` framework in C# .NET 10. It serves as a testing ground for chess engines, supporting play against UCI engines, automated head-to-head tournaments, and detailed pentanomial statistics.

---

## 🚀 Key Features

### 1. Play Against a UCI Engine
* **Universal Compatibility**: Load any UCI-compliant chess engine by specifying the path to its executable (e.g. `C:\engines\stockfish.exe`).
* **Interactive Settings**: Set custom time limits (in milliseconds) per move for the engine search.
* **Real-time Engine Statistics**: Displays search statistics like depth (D), score in centipawns or mate-in-N (S), nodes-per-second (NPS), and the Principal Variation (PV).
* **Live Logs**: Displays the engine's stdout line-by-line.
* **Game Controls**: "New Game" resets the board, and "Flip Board" rotates the rank/file perspective.

### 2. Engine vs Engine Tournaments
* **Automated Pairings**: Engines play head-to-head in pairs of 2 games (each engine plays once as White and once as Black) to eliminate color bias.
* **Live Animation**: Watch the games progress on the main chess board in real-time as the engines exchange moves.
* **Progress Panel**: Displays current pairing counts, search statistics for both active engines, and a scrolling tournament log.
* **Thread-Safe Background Execution**: Tournaments run on a background thread so the user interface remains responsive.

### 3. Pentanomial Result Tracking
NKChess tracks tournament pairs using the standard **pentanomial system** (common in advanced engine testing like Stockfish's Fishtest):
* **WW (Index 4)**: Engine A wins both games of the pair (+2.0 points).
* **WD (Index 3)**: Engine A wins one and draws one (+1.5 points).
* **DD / WL (Index 2)**: Engine A draws both, or wins one and loses one (+1.0 points).
* **LD (Index 1)**: Engine A draws one and loses one (+0.5 points).
* **LL (Index 0)**: Engine A loses both games of the pair (+0.0 points).

### 4. Mathematical Elo & Margin Estimation
The tournament director computes real-time statistics to assess engine strength:
* **Percentage Score ($\mu$)**: Normalized score representing the fraction of total points obtained.
* **Elo Difference**: Calculated as:
  $$\Delta\text{Elo} = 400 \log_{10}\left(\frac{\mu}{1 - \mu}\right)$$
* **95% Confidence Interval (Margin)**: Computed using the empirical variance of the pentanomial distributions:
  $$\sigma^2 = \sum p_i \left( x_i - \mu \right)^2 \quad\Longrightarrow\quad \text{Margin} = 1.96 \times \text{SE} \times \frac{400}{\ln(10) \mu (1 - \mu)}$$

---

## 🛠️ How to Build and Run

To compile and launch the TUI, make sure you have the [.NET SDK](https://dotnet.microsoft.com/) installed, then execute:

```bash
# Navigate to the project folder
cd NKChess

# Restore packages and build
dotnet build

# Run the TUI application
dotnet run
```

---

## 🎨 Rich Terminal Aesthetics & Cute Chess-Inspired Design

NKChess features a premium dark-themed aesthetic inspired by **Cute Chess** layouts, utilizing HSL-tailored colors and rounded borders:
* **Deep Slate Background**: Main page styled with deep charcoal/blue (`#0F172A`).
* **Interactive Menu Bar & Actionable Toolbar**: Top-level interactive `MenuBar` component with hover effects, supporting dropdown submenus: **Game** (New, Save, Copy FEN, Copy PGN, Adjudicate, Close), **Tournament** (New, Results), **Edit** (Preferences, Engines), and **Help** (Shortcuts, About). Actionable toolbar buttons provide quick shortcuts.
* **Player Cards**: Structured information cards displaying active players/engines along with their live metrics (depth, nodes-per-second, search speed).
* **Live Visual Evaluation Gauge**: A 12-block evaluation bar (`[██████░░░░░░]`) and centipawn/mate tracker representing current board advantage.
* **Moves History Log**: Clean, formatted PGN-style log of the current game's moves.
* **Curated Chess Board**: Muted cream (`#F0D9B5`) and warm brown (`#B58863`) square backgrounds.
* **Contrasting Unicode Pieces**: Styled pieces (♚ ♛ ♜ ♝ ♞ ♟) drawn in stark white for White and dark slate for Black.
* **Dynamic Highlight System**:
  * **Selected Piece**: Bright gold (`#EAB308`).
  * **Legal Destinations**: Vibrant green (`#22C55E`).
  * **King in Check**: Alert red (`#EF4444`).
  * **Active Keyboard Cursor**: Sleek blue (`#3B82F6`).
* **Responsive Micro-Animations**: Buttons transition to brighter shades when hovered.

---

## 🕹️ Interactive Controls

NKChess supports hybrid navigation so that users can control the TUI easily:

### 🖱️ Mouse Controls
* **Square Selection**: Left-click a square containing your piece to select it.
* **Move Execution**: Left-click any green-highlighted legal square to move the piece.
* **Sidebar Inputs**: Click textboxes to focus and type; click buttons to trigger loading, switching tabs, or commencing actions.

### ⌨️ Keyboard Controls
* **Arrow Keys**: Navigate the blue cursor (up, down, left, right) across the board.
* **Space / Enter**: Select the piece under the cursor, or execute a move to the selected destination.
* **Tab / Shift+Tab**: Cycle focus between board squares, buttons, and textboxes.
* **Escape**: Cancel active selections or halt running tournaments.

---

## 📂 Project Structure

* **[NKChess/ChessBoard.cs](NKChess/ChessBoard.cs)**: Full chess game state model (FEN parser, castling, en passant, promotions, pseudo-legal and legal move generators, check/mate/stalemate detectors).
* **[NKChess/UciEngine.cs](NKChess/UciEngine.cs)**: Asynchronous UCI engine process wrapper managing stderr/stdout redirection and search query communications.
* **[NKChess/Tournament.cs](NKChess/Tournament.cs)**: Tournament director logic managing automated matches, pairings, pentanomial counts, and Elo margins.
* **[NKChess/ChessTuiApp.cs](NKChess/ChessTuiApp.cs)**: Visual element tree combining the chess board grid, mouse triggers, keyboard key events, and sidebar tabs.
* **[NKChess/Program.cs](NKChess/Program.cs)**: Application bootstrapper initializing the configuration and launching the TUI.
