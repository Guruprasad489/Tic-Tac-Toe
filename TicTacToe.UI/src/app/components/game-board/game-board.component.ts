import { Component, Input, Output, EventEmitter, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameResponse, GameStatus, Player } from '../../models/game.models';

@Component({
  selector: 'app-game-board',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="board-container">
      <h2>Tic Tac Toe Board</h2>
      <div class="board">
        <button
          *ngFor="let cell of game.board; let i = index"
          class="cell"
          [class.x]="cell === Player.X"
          [class.o]="cell === Player.O"
          [class.winning]="isWinningCell(i)"
          [disabled]="cell !== null || game.gameStatus !== GameStatus.InProgress"
          (click)="onCellClick(i)"
        >
          {{ cell === Player.X ? 'X' : cell === Player.O ? 'O' : ''}}
        </button>
      </div>
      <div class="status">
        <p *ngIf="game.gameStatus === GameStatus.InProgress" class="current-player">
          Current Player: <strong>{{ game.currentPlayer === Player.X ? 'X' : game.currentPlayer === Player.O ? 'O' : ''}}</strong>
        </p>
        <p *ngIf="game.gameStatus === GameStatus.Won" class="winner">
          🎉 Player <strong>{{ game.winner === Player.X ? 'X' : game.winner === Player.O ? 'O' : ''}}</strong> Wins! 🎉
        </p>
        <p *ngIf="game.gameStatus === GameStatus.Draw" class="draw">
          It's a Draw! No winner this time.
        </p>
      </div>
    </div>
  `,
  styles: [`
    .board-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
    }

    h2 {
      margin: 0;
      color: #333;
    }

    .board {
      display: grid;
      grid-template-columns: repeat(3, 100px);
      grid-gap: 5px;
      background-color: #ddd;
      padding: 5px;
      border-radius: 5px;
    }

    .cell {
      width: 100px;
      height: 100px;
      font-size: 24px;
      font-weight: bold;
      background-color: white;
      border: 2px solid #999;
      cursor: pointer;
      transition: all 0.2s;
      border-radius: 3px;
    }

    .cell:hover:not(:disabled) {
      background-color: #f0f0f0;
      transform: scale(1.05);
    }

    .cell:disabled {
      cursor: not-allowed;
    }

    .cell.x {
      color: #0066cc;
    }

    .cell.o {
      color: #cc0000;
    }

    .cell.winning {
      background-color: #90EE90;
      font-weight: bolder;
    }

    .status {
      margin-top: 1rem;
      font-size: 18px;
      text-align: center;
    }

    .current-player {
      color: #0066cc;
    }

    .winner {
      color: #00aa00;
      font-size: 20px;
    }

    .draw {
      color: #ff6600;
      font-size: 20px;
    }
  `]
})
export class GameBoardComponent {
  @Input() game!: GameResponse;
  @Output() cellClicked = new EventEmitter<number>();
  GameStatus = GameStatus;
  Player = Player;

  private cdr = inject(ChangeDetectorRef);
  
  onCellClick(index: number): void {
    this.cellClicked.emit(index);
  }

  isWinningCell(index: number): boolean {
    return this.game.winningCells.includes(index);
  }
}
