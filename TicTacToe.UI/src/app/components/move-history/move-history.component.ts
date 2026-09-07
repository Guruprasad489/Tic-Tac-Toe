import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoveDto, Player } from '../../models/game.models';

@Component({
  selector: 'app-move-history',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="move-history-container">
      <h3>Move History</h3>
      <div class="history-list">
        <table *ngIf="moves.length > 0" class="moves-table">
          <thead>
            <tr>
              <th>Move #</th>
              <th>Player</th>
              <th>Position</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let move of moves">
              <td>{{ move.moveNumber }}</td>
              <td [class.x-player]="move.player === Player.X" [class.o-player]="move.player === Player.O">
                {{ move.player }}
              </td>
              <td>Row {{ move.row + 1 }}, Col {{ move.column + 1 }}</td>
            </tr>
          </tbody>
        </table>
        <p *ngIf="moves.length === 0" class="empty-message">No moves yet. Start a game!</p>
      </div>
    </div>
  `,
  styles: [`
    .move-history-container {
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
    }

    .history-list {
      max-height: 300px;
      overflow-y: auto;
    }

    .moves-table {
      width: 100%;
      border-collapse: collapse;
      font-size: 13px;
    }

    .moves-table thead {
      background-color: #ddd;
      position: sticky;
      top: 0;
    }

    .moves-table th {
      padding: 8px;
      text-align: left;
      font-weight: bold;
      border-bottom: 2px solid #999;
    }

    .moves-table td {
      padding: 8px;
      border-bottom: 1px solid #ddd;
    }

    .moves-table tbody tr:hover {
      background-color: #e8e8e8;
    }

    .x-player {
      color: #0066cc;
      font-weight: bold;
    }

    .o-player {
      color: #cc0000;
      font-weight: bold;
    }

    .empty-message {
      text-align: center;
      color: #999;
      font-style: italic;
    }
  `]
})
export class MoveHistoryComponent {
  @Input() moves: MoveDto[] = [];
  Player = Player;
}
