import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { GameApiService } from './game-api.service';
import { GameMode, GameStatus, Player } from '../models/game.models';
import { environment } from '../../environments/environment';

describe('GameApiService', () => {
  let service: GameApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [GameApiService]
    });
    service = TestBed.inject(GameApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('createGame', () => {
    it('should POST to /api/games and return game response', () => {
      const gameId = '123e4567-e89b-12d3-a456-426614174000';
      const expectedResponse = {
        gameId,
        board: Array(9).fill(null),
        currentPlayer: Player.X,
        gameMode: GameMode.TwoPlayer,
        gameStatus: GameStatus.InProgress,
        winner: null,
        winningCells: [],
        moveHistory: [],
        scoreboard: { xWins: 0, oWins: 0, draws: 0 }
      };

      service.createGame(GameMode.TwoPlayer).subscribe(response => {
        expect(response.gameId).toBe(gameId);
        expect(response.gameStatus).toBe(GameStatus.InProgress);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/games`);
      expect(req.request.method).toBe('POST');
      req.flush(expectedResponse);
    });
  });

  describe('getGame', () => {
    it('should GET /api/games/{id} and return game response', () => {
      const gameId = '123e4567-e89b-12d3-a456-426614174000';
      const expectedResponse = {
        gameId,
        board: Array(9).fill(null),
        currentPlayer: Player.X,
        gameMode: GameMode.TwoPlayer,
        gameStatus: GameStatus.InProgress,
        winner: null,
        winningCells: [],
        moveHistory: [],
        scoreboard: { xWins: 0, oWins: 0, draws: 0 }
      };

      service.getGame(gameId).subscribe(response => {
        expect(response.gameId).toBe(gameId);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/games/${gameId}`);
      expect(req.request.method).toBe('GET');
      req.flush(expectedResponse);
    });
  });

  describe('makeMove', () => {
    it('should POST move to /api/games/{id}/moves', () => {
      const gameId = '123e4567-e89b-12d3-a456-426614174000';
      const moveRequest = { row: 0, column: 0, player: Player.X };
      const expectedResponse = {
        gameId,
        board: [Player.X, null, null, null, null, null, null, null, null],
        currentPlayer: Player.O,
        gameMode: GameMode.TwoPlayer,
        gameStatus: GameStatus.InProgress,
        winner: null,
        winningCells: [],
        moveHistory: [{ moveNumber: 1, player: Player.X, row: 0, column: 0 }],
        scoreboard: { xWins: 0, oWins: 0, draws: 0 }
      };

      service.makeMove(gameId, moveRequest).subscribe(response => {
        expect(response.currentPlayer).toBe(Player.O);
        expect(response.moveHistory.length).toBe(1);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/games/${gameId}/moves`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(moveRequest);
      req.flush(expectedResponse);
    });
  });

  describe('undoMove', () => {
    it('should POST to /api/games/{id}/undo', () => {
      const gameId = '123e4567-e89b-12d3-a456-426614174000';
      const expectedResponse = {
        gameId,
        board: Array(9).fill(null),
        currentPlayer: Player.X,
        gameMode: GameMode.TwoPlayer,
        gameStatus: GameStatus.InProgress,
        winner: null,
        winningCells: [],
        moveHistory: [],
        scoreboard: { xWins: 0, oWins: 0, draws: 0 }
      };

      service.undoMove(gameId).subscribe(response => {
        expect(response.moveHistory.length).toBe(0);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/games/${gameId}/undo`);
      expect(req.request.method).toBe('POST');
      req.flush(expectedResponse);
    });
  });

  describe('getScoreboard', () => {
    it('should GET /api/scoreboard', () => {
      const expectedResponse = { xWins: 2, oWins: 1, draws: 0 };

      service.getScoreboard().subscribe(response => {
        expect(response.xWins).toBe(2);
        expect(response.oWins).toBe(1);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/scoreboard`);
      expect(req.request.method).toBe('GET');
      req.flush(expectedResponse);
    });
  });

  describe('resetScoreboard', () => {
    it('should POST to /api/scoreboard/reset', () => {
      const expectedResponse = { xWins: 0, oWins: 0, draws: 0 };

      service.resetScoreboard().subscribe(response => {
        expect(response.xWins).toBe(0);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/scoreboard/reset`);
      expect(req.request.method).toBe('POST');
      req.flush(expectedResponse);
    });
  });
});
