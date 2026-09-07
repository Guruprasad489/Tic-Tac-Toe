import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  GameResponse,
  MoveRequest,
  CreateGameRequest,
  Scoreboard,
  GameMode
} from '../models/game.models';

@Injectable({
  providedIn: 'root'
})
export class GameApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * Creates a new game session.
   * @param gameMode The game mode (0 = TwoPlayer, 1 = ComputerMode)
   */
  createGame(gameMode: GameMode | number): Observable<GameResponse> {
    const request: CreateGameRequest = {
      gameMode: typeof gameMode === 'number' ? gameMode : (gameMode === GameMode.TwoPlayer ? 0 : 1)
    };
    return this.http.post<GameResponse>(`${this.apiUrl}/games`, request);
  }

  /**
   * Retrieves the current game state.
   * @param gameId The ID of the game
   */
  getGame(gameId: string): Observable<GameResponse> {
    return this.http.get<GameResponse>(`${this.apiUrl}/games/${gameId}`);
  }

  /**
   * Submits a move to the backend.
   * @param gameId The ID of the game
   * @param moveRequest The move to submit
   */
  makeMove(gameId: string, moveRequest: MoveRequest): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.apiUrl}/games/${gameId}/moves`, moveRequest);
  }

  /**
   * Undoes the last move(s).
   * @param gameId The ID of the game
   */
  undoMove(gameId: string): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.apiUrl}/games/${gameId}/undo`, {});
  }

  /**
   * Resets the current game board.
   * @param gameId The ID of the game
   */
  resetGame(gameId: string): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.apiUrl}/games/${gameId}/reset`, {});
  }

  /**
   * Retrieves the current scoreboard.
   */
  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.apiUrl}/scoreboard`);
  }

  /**
   * Resets the scoreboard.
   */
  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(`${this.apiUrl}/scoreboard/reset`, {});
  }
}
