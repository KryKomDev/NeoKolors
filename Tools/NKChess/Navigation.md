# Navigation

## Main top navbar

### Game

#### New (ctrl+n)

Opens a dialogue for creating a new game.

The dialogue has the following options:
- White - Human x CPU switch for White player (when CPU is selected unlock an input for the engine executable)
- Black - Human x CPU switch for Black player (when CPU is selected unlock an input for the engine executable)
- Time control - opens a dialogue for configuring the time control for players individually
- Opening suite - controls the opening position using FEN or PGN

#### Close (ctr+q)

Closes the program.

#### Save (ctrl+s)

Opens a dialogue for saving the game (in PGN).

#### Copy FEN

Copies the FEN of the game to clipboard.

#### Copy PGN

Copies the PGN of the game to clipboard.

#### Adjudicate result (ctrl+a)

Opens a dialogue for adjudicating a result of the game (win, loss, draw)

### Tournament

#### New (ctrl+t)

Opens a dialogue for creating a new tournament

##### General info

- Name - the name of the tournament
- PGN output - the output PGN file of the tournament
- EPD output - the output EPD file of the tournament

##### Tournament config

- Type - Round-robin / Gauntlet / Knockout / Pyramid
- Rounds - the number of rounds of the tournament
- Games per encounter - the number of games per encounter
- Play both colors - whether the engines should play both colors on a single encounter

##### Game config

- Time control - opens a dialogue with move count before time reset, total time, and increment per move
- Opening suite - controls the opening position using FEN or PGN
- Game length - controls the maximum move count for a game

#### Results

Opens the list of finished tournaments and their results.

### Edit 

#### Preferences (ctrl+.)

Opens a dialogue for configuring the application.

#### Engines (ctrl+e)

Opens a dialogue for adding and removing engines.

## Main content

```
+----------------------------------------------------+
| Running games tabs                                 |
| W-Time Result B-Time                               |
|    +------------+  +---------+ +-------+ +-------+ |
|    | game       |  | moves   | | White | | Black | |
|    | board      |  | history | | info  | | info  | |
|    |            |  |         | |       | |       | |
|    |            |  |         | |       | |       | |
|    +------------+  +---------+ +-------+ +-------+ |
| x--- moves slider -------------------------------x |
| +------------------------------------------------+ |
| | evaluation graph                               | |
| +------------------------------------------------+ |
+----------------------------------------------------+
```

### Running games tab

A tab selector with all of the currently running games.

### Game board

The visual representation of the game board.

### Moves history 

Shows the history of moves, line per round.

### White/Black info

```
+- Color ---------------------+
| NPS:    24k   Depth: 5      |
| Hash:   12    Time:  4ms    |
| Ponder: -     Nodes: 9      |
| TB:     -     Score: +0,1   |
| PV:     0                   |
| Log:                        |
| info string                 |
| info log                    |
| info log                    |
+-----------------------------+
```

Shows the log, NPS, hash, pondermove, ponderhit, TB, depth, time, nodes, 
score and pv, of the engines.

### Moves slider

Lets the user slide through the history of the game.

### Evaluation graph

A graph of evaluation scores for the individual positions with scores from both engines. 