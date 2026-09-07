import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { GameBoardComponent } from './components/game-board/game-board.component';
import { GameControlsComponent } from './components/game-controls/game-controls.component';
import { MoveHistoryComponent } from './components/move-history/move-history.component';
import { ScoreboardComponent } from './components/scoreboard/scoreboard.component';
import { GameApiService } from './services/game-api.service';
import { GameResponse, Player, GameMode, GameStatus, MoveRequest } from './models/game.models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    GameBoardComponent,
    GameControlsComponent,
    MoveHistoryComponent,
    ScoreboardComponent
  ],
  template: `
    <div class="app-container">
      <header class="app-header">
        <h4>🎮 Tic Tac Toe</h4>
        <p class="subtitle">Play against a friend or the computer!</p>
      </header>

      <main class="app-main">
        <div class="game-section">
          <app-game-board
            [game]="currentGame"
            (cellClicked)="onCellClick($event)"
          ></app-game-board>
        </div>

        <aside class="sidebar">
          <app-game-controls
            [initialLoad]="!currentGame.moveHistory.length"
            [isPlaying]="currentGame.gameStatus === GameStatus.InProgress"
            [canUndo]="currentGame.moveHistory.length > 0 && currentGame.gameStatus === GameStatus.InProgress && !disableUndo"
            [gameMode]="currentGameMode"
            (newGame)="onNewGame($event)"
            (undo)="onUndo()"
            (reset)="onResetGame()"
            (resetScoreboard)="onResetScoreboard()"
          ></app-game-controls>

          <app-move-history
            [moves]="currentGame.moveHistory"
          ></app-move-history>

          <app-scoreboard
            [scoreboard]="currentGame.scoreboard"
          ></app-scoreboard>
        </aside>
      </main>

      <footer class="app-footer">
        <p>Built with Angular + .NET Web API</p>
      </footer>
    </div>
  `,
  styles: [`
    .app-container {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: #333;
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    }

    .app-header {
      padding: 1rem;
      text-align: center;
      color: white;
      background-color: rgba(0, 0, 0, 0.1);
      border-bottom: 3px solid rgba(255, 255, 255, 0.2);
    }

    .app-header h4 {
      margin: 0;
      font-size: 24px;
      font-weight: bold;
    }

    .subtitle {
      margin: 0.5rem 0 0 0;
      font-size: 14px;
      opacity: 0.9;
    }

    .app-main {
      flex: 1;
      display: flex;
      gap: 2rem;
      padding: 1rem;
      max-width: 1200px;
      margin: 0 auto;
      width: 100%;
    }

    .game-section {
      flex: 1;
      background-color: white;
      padding: 2rem;
      border-radius: 10px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .sidebar {
      width: 350px;
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .sidebar > * {
      background-color: white;
      border-radius: 10px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
    }

    .app-footer {
      text-align: center;
      padding: 1rem;
      background-color: rgba(0, 0, 0, 0.2);
      color: rgba(255, 255, 255, 0.8);
      border-top: 1px solid rgba(255, 255, 255, 0.2);
    }

    .app-footer p {
      margin: 0;
      font-size: 13px;
    }

    @media (max-width: 1024px) {
      .app-main {
        flex-direction: column;
        padding: 1rem;
        gap: 1rem;
      }

      .sidebar {
        width: 100%;
      }

      .game-section {
        padding: 1rem;
      }
    }
  `]
})
export class App implements OnInit {
  currentGame: GameResponse = this.getEmptyGame();
  defaultGameMode = GameMode.ComputerMode;
  currentGameMode: number = 1;
  isLoading = false;
  errorMessage: string | null = null;
  GameStatus = GameStatus;
  disableUndo = true;

  private cdr = inject(ChangeDetectorRef);

  constructor(private gameApi: GameApiService) {}

  ngOnInit(): void {
    this.createNewGame(this.defaultGameMode);
  }

  onNewGame(gameMode: number): void {
    this.createNewGame(gameMode);
  }

  onCellClick(cellIndex: number): void {
    if (this.currentGame.gameStatus !== GameStatus.InProgress) {
      return;
    }

    const row = Math.floor(cellIndex / 3);
    const column = cellIndex % 3;
    const moveRequest: MoveRequest = {
      row,
      column,
      player: this.currentGame.currentPlayer
    };

    this.isLoading = true;
    this.errorMessage = null;

    this.gameApi.makeMove(this.currentGame.gameId, moveRequest).subscribe({
      next: (response) => {
        this.currentGame = response;
        this.isLoading = false;
        this.disableUndo = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Failed to make move';
        console.error('Error making move:', error);
      }
    });
  }

  onUndo(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.gameApi.undoMove(this.currentGame.gameId).subscribe({
      next: (response) => {
        this.currentGame = response;
        this.isLoading = false;
        this.disableUndo = true;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Cannot undo';
        console.error('Error undoing move:', error);
      }
    });
  }

  onResetGame(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.gameApi.resetGame(this.currentGame.gameId).subscribe({
      next: (response) => {
        this.currentGame = response;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to reset game';
        console.error('Error resetting game:', error);
      }
    });
  }

  onResetScoreboard(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.gameApi.resetScoreboard().subscribe({
      next: (scoreboard) => {
        this.currentGame.scoreboard = scoreboard;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to reset scoreboard';
        console.error('Error resetting scoreboard:', error);
      }
    });
  }

  private createNewGame(gameMode: number): void {
    this.currentGameMode = gameMode;
    this.isLoading = true;
    this.errorMessage = null;

    this.gameApi.createGame(gameMode).subscribe({
      next: (response) => {
        this.currentGame = response;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to create game';
        console.error('Error creating game:', error);
      }
    });
  }

  private getEmptyGame(): GameResponse {
    return {
      gameId: '',
      board: Array(9).fill(null),
      currentPlayer: Player.X,
      gameMode: GameMode.ComputerMode,
      gameStatus: GameStatus.InProgress,
      winner: null,
      winningCells: [],
      moveHistory: [],
      scoreboard: { xWins: 0, oWins: 0, draws: 0 }
    };
  }
}

