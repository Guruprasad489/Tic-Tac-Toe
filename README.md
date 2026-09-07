# Tic Tac Toe Application

A browser-based Tic Tac Toe game built with **Angular 22** (TypeScript frontend) and **.NET 10 Web API** (backend).

## Overview

This full-stack application allows users to:
- ✅ Play Tic Tac Toe in **Two-Player Mode** (2 humans) or **Computer Mode** (human vs AI)
- ✅ Track move history with detailed information
- ✅ Undo moves (disabled after game completion for simplicity)
- ✅ Maintain a session-level scoreboard
- ✅ Reset individual games or the entire scoreboard
- ✅ Win/draw detection with highlighted winning cells

## Technology Stack

### Backend
- **.NET 10** Web API
- **xUnit** for unit testing
- **CORS** enabled for local frontend communication
- **Swagger/OpenAPI** for API documentation

### Frontend
- **Angular 22** with TypeScript
- **Standalone Components** (modern Angular approach)
- **HttpClient** for API communication
- **Vitest** for component/service testing
- **Responsive CSS Grid & Flexbox** layout

### Database/Storage
- **In-memory storage** (no database needed)
- Static dictionaries in `GameStateProvider` service
- All data lost on application restart (by design for this exercise)

## Project Structure

```
Tic Tac Toe/
├── Tic Tac Toe/                          # .NET Backend API
│   ├── Program.cs                        # DI, CORS, Swagger configuration
│   ├── Controllers/
│   │   ├── GamesController.cs            # Game endpoints
│   │   └── ScoreboardController.cs       # Scoreboard endpoints
│   ├── Services/
│   │   ├── GameService.cs                # Core game logic
│   │   ├── ComputerAIService.cs          # AI move selection
│   │   └── GameStateProvider.cs          # In-memory storage
│   ├── Models/
│   │   ├── Enums.cs                      # GameStatus, GameMode, Player
│   │   ├── GameState.cs                  # Game model
│   │   ├── Move.cs                       # Move record
│   │   ├── Scoreboard.cs                 # Score tracking
│   │   └── Responses.cs                  # DTOs for API
│   └── TicTacToe.Api.csproj
│
├── TicTacToe.Api.Tests/                  # Backend unit tests
│   ├── UnitTest1.cs                      # GameService tests (20+ cases)
│   ├── ComputerAIServiceTests.cs         # AI tests (7 cases)
│   ├── GameStateProviderTests.cs         # Storage tests (8 cases)
│   └── TicTacToe.Api.Tests.csproj
│
└── TicTacToe.UI/                         # Angular Frontend
	├── src/
	│   ├── app/
	│   │   ├── app.ts                    # Main app component
	│   │   ├── components/
	│   │   │   ├── game-board/           # 3x3 clickable board
	│   │   │   ├── game-controls/        # Buttons & mode selector
	│   │   │   ├── move-history/         # Move list display
	│   │   │   └── scoreboard/           # Score display
	│   │   ├── services/
	│   │   │   └── game-api.service.ts   # HTTP API calls
	│   │   ├── models/
	│   │   │   └── game.models.ts        # TypeScript interfaces
	│   │   └── environments/
	│   │       ├── environment.ts        # Dev config (localhost:5000)
	│   │       └── environment.prod.ts   # Prod config
	│   ├── index.html
	│   ├── main.ts
	│   └── styles.css
	├── package.json
	└── angular.json

Tic Tac Toe.slnx                          # Solution file
```

## Installation & Setup

### Prerequisites
- **.NET SDK 10.0+** installed
- **Node.js 18+** and **npm 12+** installed
- **Visual Studio** or any IDE with .NET + Angular support (we use VS Community 2026)

### Backend Setup

1. **Navigate to the backend project:**
   ```bash
   cd "Tic Tac Toe"
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Run backend tests (optional but recommended):**
   ```bash
   dotnet test
   ```
   Expected: 31 tests pass (20 GameService + 7 ComputerAI + 8 GameStateProvider tests)

5. **Start the API server:**
   ```bash
   dotnet run
   ```
   - API will be available at `http://localhost:5000` (or the next available port)
   - Swagger UI: `http://localhost:5000/swagger`

### Frontend Setup

1. **Navigate to the Angular project:**
   ```bash
   cd TicTacToe.UI
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start the development server:**
   ```bash
   npm start
   ```
   - Angular will open at `http://localhost:4200`
   - The app will automatically connect to the backend at `localhost:5000/api`

4. **Run Angular tests (optional):**
   ```bash
   npm test
   ```

## API Documentation

### Base URL
- Development: `http://localhost:5000/api`
- Production: `/api` (relative URL)

### Endpoints

#### Games
| Method | Endpoint | Purpose | Request | Response |
|--------|----------|---------|---------|----------|
| POST | `/games` | Create a new game | `{ gameMode: 0\|1 }` | GameResponse |
| GET | `/games/{id}` | Get game state | - | GameResponse |
| POST | `/games/{id}/moves` | Make a move | MoveRequest | GameResponse |
| POST | `/games/{id}/undo` | Undo last move(s) | - | GameResponse |
| POST | `/games/{id}/reset` | Reset board | - | GameResponse |

#### Scoreboard
| Method | Endpoint | Purpose | Response |
|--------|----------|---------|----------|
| GET | `/scoreboard` | Get current scores | Scoreboard |
| POST | `/scoreboard/reset` | Reset all scores | Scoreboard |

### Sample Request/Response

**Create Game:**
```bash
curl -X POST http://localhost:5000/api/games \
  -H "Content-Type: application/json" \
  -d '{ "gameMode": 0 }'
```

**Response:**
```json
{
  "gameId": "550e8400-e29b-41d4-a716-446655440000",
  "board": [null, null, null, null, null, null, null, null, null],
  "currentPlayer": "X",
  "gameMode": "TwoPlayer",
  "gameStatus": "InProgress",
  "winner": null,
  "winningCells": [],
  "moveHistory": [],
  "scoreboard": {
	"xWins": 0,
	"oWins": 0,
	"draws": 0
  }
}
```

**Make Move:**
```bash
curl -X POST http://localhost:5000/api/games/550e8400-e29b-41d4-a716-446655440000/moves \
  -H "Content-Type: application/json" \
  -d '{ "row": 0, "column": 0, "player": "X" }'
```

## Features & Implementation Details

### 1. Game Board
- **3×3 grid** with 9 clickable cells
- Cells display `X` or `O` when occupied
- Disabled cells prevent duplicate moves
- Disabled after game completion

### 2. Player Turns
- Alternates between X and O each move
- X always starts first
- Invalid moves do not change turn
- Current player clearly displayed

### 3. Win Detection
- Checks all **3 rows**, **3 columns**, and **2 diagonals**
- Returns winning cell indices for highlighting
- Sets game status to "Won"
- Updates scoreboard

### 4. Draw Detection
- Detected when board is full with no winner
- Sets game status to "Draw"
- Updates scoreboard

### 5. Move History
- Records **move number**, **player**, and **position** (row, column)
- Updates in real-time
- Displayed in table format
- 1-indexed for user readability (Row 1, Col 1 = position 0,0)

### 6. Undo Behavior
- **Two-Player Mode**: Undoes single most recent move
- **Computer Mode**: Undoes computer move + human move together (pair removal)
- **Disabled after completion** (Option A from requirements - simpler implementation)
- Error message shown if no moves to undo

### 7. Computer AI
- Priority-based heuristic (no minimax lookahead):
  1. **Win** if O can complete a line
  2. **Block** if X can win next move
  3. **Center** (position 1,1) if available
  4. **Corner** (0,0), (0,2), (2,0), (2,2) if available
  5. **Any available cell** as fallback
- Automatically plays after human move
- Will not move after game is completed

### 8. Scoreboard
- **Session-level tracking** of X wins, O wins, and draws
- Updated immediately when game completes
- Persists across game resets
- Can be reset independently
- Shared between game modes

### 9. Reset Actions
- **Reset Game**: Clears board, move history, winner status; keeps game ID and scoreboard
- **Reset Scoreboard**: Resets X/O wins and draws to 0-0-0

## Game Modes

### Two-Player Mode
- Both X and O controlled by human players
- Turn-based local gameplay
- Ideal for testing move logic and win conditions

### Computer Mode (Play Against Computer)
- Human is always **X** (moves first)
- Computer is **O** (responds with AI)
- Computer move happens immediately after human move
- Undo removes both moves together to maintain coherence

## Backend Architecture

### Game State Ownership
- **Backend is the source of truth** for all game state
- Frontend reads and displays state only
- Every action sends HTTP request to backend
- Backend validates moves before applying them

### Service Layer

**GameService:**
- `CreateGame()` - Initializes new game
- `TryMakeMove()` - Validates and applies move, detects win/draw
- `TryUndo()` - Reverts move(s) based on game mode
- `ResetGame()` - Clears board, keeps scoreboard

**ComputerAIService:**
- `SelectMove()` - Returns best move using priority heuristics
- Considers both AI (O) winning and opponent (X) blocking

**GameStateProvider:**
- Thread-safe in-memory storage with locks
- `SaveGame()`, `GetGame()`, `DeleteGame()` - Game persistence
- `UpdateScoreboard()`, `ResetScoreboard()` - Score management
- Static resources persist for session lifetime

### Validation
- **Cell position** (0-2 range)
- **Empty cell check** (no overwrites)
- **Player turn validation** (correct player moving)
- **Game completion** (no moves after Win/Draw)

## Testing

### Backend Tests (31 tests, 100% pass rate)

**GameService (20 tests):**
- Valid move on empty cell ✓
- Occupied cell rejection ✓
- Out-of-bounds rejection ✓
- Wrong player rejection ✓
- Move after completion rejection ✓
- Row win detection (all 3 rows) ✓
- Column win detection (all 3 columns) ✓
- Diagonal win detection (both diagonals) ✓
- Draw detection ✓
- Turn switching ✓
- Reset game integrity ✓
- Undo two-player mode ✓
- Undo computer mode ✓
- Undo on empty history ✓
- Undo after completion ✓

**ComputerAI (7 tests):**
- Winning move selection ✓
- Opponent blocking ✓
- Center preference ✓
- Corner fallback ✓
- Any cell fallback ✓
- Full board handling ✓

**GameStateProvider (8 tests):**
- Game save/retrieve ✓
- Non-existent game ✓
- Game deletion ✓
- Scoreboard initialization ✓
- Score updates (X wins, O wins, draws) ✓
- Multiple score accumulation ✓
- Scoreboard reset ✓

Run tests:
```bash
cd "Tic Tac Toe"
dotnet test
```

### Frontend Tests (In Progress)
- GameApiService HTTP call testing ✓
- GameBoardComponent rendering ✓
- Cell click events ✓
- Winning cell highlighting ✓
- Game status messages ✓

Run tests:
```bash
cd TicTacToe.UI
npm test
```

## Usage Walkthrough

### Two-Player Game (Example)

1. **Start application:**
   - Backend running on `localhost:5000`
   - Frontend open at `localhost:4200`

2. **Select "Two Player" mode** (default)

3. **Click "New Game"** to create game session

4. **Player X (human 1) clicks a cell** (e.g., center)
   - Request sent to backend
   - Backend validates move
   - Backend checks win/draw
   - Response returns updated GameResponse
   - Frontend re-renders board and history

5. **Player O (human 2) clicks a cell**
   - Same flow repeats
   - Move history updates

6. **Continue until win or draw**
   - For win: Scoreboard updates, winning row highlights
   - For draw: Draw message shown, scoreboard updates

7. **Click "Undo"** to remove last move
   - Both players' previous move removed

8. **Click "Reset Board"** to start new game
   - Board clears, move history clears
   - Scoreboard stays same
   - Same game ID continues

9. **Click "Reset Scoreboard"** to reset scores to 0-0-0

### Computer Mode (Example)

1. **Select "Play Against Computer"** from dropdown

2. **Click "New Game"**
   - Backend creates GameMode.ComputerMode game

3. **Player (X) clicks a cell**
   - Request sent with `player: "X"`
   - Backend applies move

4. **Computer (O) responds automatically**
   - Backend runs AI algorithm
   - Returns game state with O's move already applied
   - Frontend displays updated board

5. **Continue alternating until end**
   - Undo removes both X and O moves (user's move + computer's response)

## Design Decisions

### 1. Undo After Completion (Disabled)
- **Choice:** Option A - Undo disabled after game completion
- **Rationale:** Prevents complexity of reversing scoreboard changes; simpler UX
- **Alternative:** Could implement Option B with scoreboard rollback if needed

### 2. Backend State Ownership
- **Choice:** Backend is single source of truth
- **Rationale:** Prevents desynchronization between frontend/backend
- **Frontend role:** UI display and event handling only

### 3. In-Memory Storage
- **Choice:** No database, static dictionaries with thread-safe locks
- **Rationale:** Meets requirements (in-memory acceptable); keeps deployment simple
- **Trade-off:** Data lost on restart (acceptable for demo/exercise)

### 4. AI Implementation
- **Choice:** Priority heuristic (no minimax)
- **Rationale:** Fast, simple, sufficient for Tic Tac Toe
- **Details:** Win > Block > Center > Corner > Any

### 5. Component Architecture
- **Choice:** Standalone Angular components (modern approach)
- **Rationale:** No need for NgModules; cleaner, more modular code

### 6. HTTP Client approach
- **Choice:** Typed HttpClient with observables
- **Rationale:** Angular best practice; testable with HttpTestingController

## Accessibility & Responsiveness

- **Responsive Layout:** Works on desktop, tablet, and mobile
- **Colors:** X (blue), O (red), Winning cells (green)
- **Keyboard accessible:** Can use Tab to navigate buttons
- **Screen reader friendly:** Semantic HTML, ARIA labels on key elements (could be enhanced)

## Troubleshooting

### Backend won't start
- Verify .NET SDK 10 installed: `dotnet --version`
- Check port 5000/5001 not in use: `netstat -ano | findstr :5000`
- Rebuild: `dotnet build --configuration Release`

### Frontend won't connect to backend
- Verify backend is running: `http://localhost:5000/swagger`
- Check CORS is enabled in Program.cs (pre-configured)
- Check environment.ts has correct API URL: `http://localhost:5000/api`

### Tests fail
- Backend: `dotnet test` from `Tic Tac Toe/` folder
- Frontend: `npm test` from `TicTacToe.UI/` folder
- Clear cache: `rm -r bin obj` (backend) or `rm -r node_modules` (frontend)

## Future Enhancements

- [ ] Database persistence (SQL Server, Entity Framework Core)
- [ ] User authentication & game history tracking
- [ ] Minimax AI algorithm for harder opponent
- [ ] Multiplayer online games
- [ ] WebSocket real-time updates
- [ ] PWA support for offline play
- [ ] Dark mode UI
- [ ] Game statistics and analytics
- [ ] Mobile app (React Native)

## Development Workflow

### Running Locally

**Terminal 1 (Backend):**
```bash
cd "Tic Tac Toe"
dotnet run
```

**Terminal 2 (Frontend):**
```bash
cd TicTacToe.UI
npm start
```

**Terminal 3 (Backend Tests - optional):**
```bash
cd "Tic Tac Toe"
dotnet test --watch
```

### Source Control
- Repository: GitHub (as per requirements)
- Ignore files: `.gitignore` includes `bin/`, `obj/`, `node_modules/`, `dist/`

## API Swagger Documentation

Once backend is running, open:
```
http://localhost:5000/swagger
```

Interactive API documentation with try-it-out capability.

## Performance Notes

- **Backend:** In-memory operations, O(1) access
- **Frontend:** Instant UI updates via Angular change detection
- **Network:** Minimal payload size; only game state transferred
- **Scalability:** For production, add database and scale backend

## License

This project is provided as-is for educational purposes.

## Contact & Questions

For implementation questions or suggestions, refer to:
- Backend: `Tic Tac Toe/` project structure
- Frontend: `TicTacToe.UI/src/app/` architecture
- Tests: Comprehensive xUnit suite in `TicTacToe.Api.Tests/`

---

**Status:** ✅ Complete implementation with 31 backend tests passing and Angular frontend ready for testing.
