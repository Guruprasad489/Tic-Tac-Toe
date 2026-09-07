// TypeScript interfaces matching backend DTOs

export enum GameStatus {
  // InProgress = 'InProgress',
  // Won = 'Won',
  // Draw = 'Draw'
  InProgress = 0,
  Won = 1,
  Draw = 2
}

export enum GameMode {
  // TwoPlayer = 'TwoPlayer',
  // ComputerMode = 'ComputerMode'
  TwoPlayer = 0,
  ComputerMode = 1
}

export enum Player {
  // X = 'X',
  // O = 'O'
  X = 0,
  O = 1
}

export interface GameResponse {
  gameId: string;
  board: (Player | null)[];
  currentPlayer: Player;
  gameMode: GameMode;
  gameStatus: GameStatus;
  winner: Player | null;
  winningCells: number[];
  moveHistory: MoveDto[];
  scoreboard: Scoreboard;
}

export interface MoveDto {
  moveNumber: number;
  player: Player;
  row: number;
  column: number;
}

export interface MoveRequest {
  row: number;
  column: number;
  player: Player;
}

export interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface CreateGameRequest {
  gameMode: number; // 0 = TwoPlayer, 1 = ComputerMode
}

export interface ErrorResponse {
  message: string;
  statusCode: number;
}
