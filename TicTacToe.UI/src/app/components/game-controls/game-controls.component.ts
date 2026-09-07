import { Component, Input, Output, EventEmitter, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GameMode } from '../../models/game.models';

@Component({
  selector: 'app-game-controls',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="controls-container">
      <div class="mode-selector">
        <label>Game Mode:</label>
        <select [(ngModel)]="selectedMode" (change)="onModeChange()" [disabled]="isPlaying && !initialLoad">
          <option [ngValue]="0">Two Player</option>
          <option [ngValue]="1">Play Against Computer</option>
        </select>
      </div>

      <div class="button-group">
        <button class="btn btn-primary" (click)="onNewGame()">
          New Game
        </button>
        <button 
          class="btn btn-secondary" 
          (click)="onUndo()" 
          [disabled]="!canUndo"
        >
          ↶ Undo
        </button>
        <button 
          class="btn btn-secondary" 
          (click)="onReset()" 
          [disabled]="!isPlaying"
        >
          Reset Board
        </button>
      </div>

      <div class="scoreboard-reset">
        <button 
          class="btn btn-danger" 
          (click)="onResetScoreboard()"
        >
          Reset Scoreboard
        </button>
      </div>
    </div>
  `,
  styles: [`
    .controls-container {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
      padding: 1.5rem;
      background-color: #f9f9f9;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .mode-selector {
      display: flex;
      align-items: center;
      gap: 10px;
    }

    .mode-selector label {
      font-weight: bold;
      color: #333;
    }

    .mode-selector select {
      padding: 8px 12px;
      border: 2px solid #ddd;
      border-radius: 4px;
      font-size: 14px;
      cursor: pointer;
    }

    .mode-selector select:disabled {
      background-color: #eee;
      cursor: not-allowed;
    }

    .button-group {
      display: flex;
      flex-direction: column;
      gap: 10px;
    }

    .btn {
      padding: 10px 16px;
      font-size: 14px;
      font-weight: bold;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-primary {
      background-color: #0066cc;
      color: white;
    }

    .btn-primary:hover {
      background-color: #0052a3;
    }

    .btn-secondary {
      background-color: #666;
      color: white;
    }

    .btn-secondary:hover:not(:disabled) {
      background-color: #444;
    }

    .btn-secondary:disabled {
      background-color: #ccc;
      cursor: not-allowed;
    }

    .btn-danger {
      background-color: #cc4444;
      color: white;
    }

    .btn-danger:hover {
      background-color: #aa2222;
    }

    .scoreboard-reset {
      border-top: 1px solid #ddd;
      padding-top: 1rem;
    }
  `]
})
export class GameControlsComponent {
  @Input() isPlaying = false;
  @Input() canUndo = false;
  @Input() gameMode = GameMode.ComputerMode;
  @Input() initialLoad = true;

  private cdr = inject(ChangeDetectorRef);

  @Output() newGame = new EventEmitter<number>();
  @Output() undo = new EventEmitter<void>();
  @Output() reset = new EventEmitter<void>();
  @Output() resetScoreboard = new EventEmitter<void>();
  @Output() gameModeChanged = new EventEmitter<number>();

  selectedMode = GameMode.ComputerMode;

  onNewGame(): void {
    this.newGame.emit(this.selectedMode);
  }

  onUndo(): void {
    this.undo.emit();
  }

  onReset(): void {
    this.reset.emit();
  }

  onResetScoreboard(): void {
    this.resetScoreboard.emit();
  }

  onModeChange(): void {
    this.gameModeChanged.emit(this.selectedMode);
  }
}
