import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Scoreboard } from '../../models/game.models';

@Component({
  selector: 'app-scoreboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="scoreboard-container">
      <h3>Scoreboard</h3>
      <div class="score-grid">
        <div class="score-card">
          <div class="score-label">X Wins</div>
          <div class="score-value x-score">{{ scoreboard.xWins }}</div>
        </div>
        <div class="score-card">
          <div class="score-label">Draws</div>
          <div class="score-value draw-score">{{ scoreboard.draws }}</div>
        </div>
        <div class="score-card">
          <div class="score-label">O Wins</div>
          <div class="score-value o-score">{{ scoreboard.oWins }}</div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .scoreboard-container {
      display: flex;
      flex-direction: column;
      gap: 1rem;
      padding: 1rem;
      background-color: #f0f0f0;
      border-radius: 8px;
    }

    h3 {
      margin: 0;
      color: #333;
      text-align: center;
    }

    .score-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 10px;
    }

    .score-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 8px;
      padding: 12px;
      background-color: white;
      border-radius: 6px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }

    .score-label {
      font-size: 12px;
      font-weight: bold;
      color: #666;
      text-align: center;
    }

    .score-value {
      font-size: 28px;
      font-weight: bold;
    }

    .x-score {
      color: #0066cc;
    }

    .o-score {
      color: #cc0000;
    }

    .draw-score {
      color: #ff6600;
    }
  `]
})
export class ScoreboardComponent {
  @Input() scoreboard: Scoreboard = {
    xWins: 0,
    oWins: 0,
    draws: 0
  };
}
