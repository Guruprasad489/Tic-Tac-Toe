using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.Services;

/// <summary>
/// Service for core Tic Tac Toe game logic, including move validation, win/draw detection, and undo capability.
/// </summary>
public class GameService
{
    private readonly GameStateProvider _stateProvider;

    public GameService(GameStateProvider stateProvider)
    {
        _stateProvider = stateProvider;
    }

    /// <summary>
    /// Creates a new game with the specified mode.
    /// </summary>
    public GameState CreateGame(GameMode mode)
    {
        var game = new GameState { GameMode = mode };
        _stateProvider.SaveGame(game);
        return game;
    }

    /// <summary>
    /// Attempts to make a move. Returns (success, errorMessage).
    /// If successful, applies the move and updates game state.
    /// </summary>
    public (bool Success, string? ErrorMessage) TryMakeMove(GameState game, int row, int column, Player player)
    {
        // Validate game is in progress
        if (game.GameStatus != GameStatus.InProgress)
        {
            return (false, "Game is already finished. Cannot make moves after game completion.");
        }

        // Validate cell position
        if (row < 0 || row > 2 || column < 0 || column > 2)
        {
            return (false, "Cell position out of bounds. Row and column must be between 0 and 2.");
        }

        // Validate it's the correct player's turn
        if (player != game.CurrentPlayer)
        {
            return (false, $"It's {game.CurrentPlayer}'s turn, not {player}'s turn.");
        }

        // Validate the cell is empty
        int cellIndex = row * 3 + column;
        if (game.Board[cellIndex] != null)
        {
            return (false, "Cell is already occupied.");
        }

        // Apply the move
        game.Board[cellIndex] = player;
        var move = new Move { Player = player, Row = row, Column = column };
        game.MoveHistory.Add(move);

        // Check for win
        if (CheckWin(game.Board, player, out var winningCells))
        {
            game.GameStatus = GameStatus.Won;
            game.Winner = player;
            game.WinningCells = winningCells;
            _stateProvider.SaveGame(game);
            return (true, null);
        }

        // Check for draw
        if (IsBoardFull(game.Board))
        {
            game.GameStatus = GameStatus.Draw;
            _stateProvider.SaveGame(game);
            return (true, null);
        }

        // Switch player turn
        game.CurrentPlayer = game.CurrentPlayer == Player.X ? Player.O : Player.X;

        _stateProvider.SaveGame(game);
        return (true, null);
    }

    /// <summary>
    /// Undoes the last move(s) based on game mode.
    /// In two-player mode, undoes one move.
    /// In computer mode, undoes both the computer move and the human move.
    /// Returns (success, errorMessage).
    /// Undo is disabled if the game is already finished.
    /// </summary>
    public (bool Success, string? ErrorMessage) TryUndo(GameState game)
    {
        // Undo is disabled after game completion (Option A from requirements)
        if (game.GameStatus != GameStatus.InProgress)
        {
            return (false, "Cannot undo after game completion.");
        }

        if (game.MoveHistory.Count == 0)
        {
            return (false, "No moves to undo.");
        }

        if (game.GameMode == GameMode.TwoPlayer)
        {
            // Undo one move
            UndoLastMove(game);
        }
        else if (game.GameMode == GameMode.ComputerMode)
        {
            // Undo two moves (computer move + human move)
            if (game.MoveHistory.Count < 2)
            {
                return (false, "Not enough moves to undo in computer mode.");
            }
            UndoLastMove(game);
            UndoLastMove(game);
        }

        _stateProvider.SaveGame(game);
        return (true, null);
    }

    /// <summary>
    /// Resets the game board but keeps the scoreboard unchanged.
    /// </summary>
    public void ResetGame(GameState game)
    {
        game.Board = new Player?[9];
        game.CurrentPlayer = Player.X;
        game.GameStatus = GameStatus.InProgress;
        game.Winner = null;
        game.WinningCells = [];
        game.MoveHistory.Clear();

        _stateProvider.SaveGame(game);
    }

    // Helper Methods

    private bool CheckWin(Player?[] board, Player player, out int[] winningCells)
    {
        winningCells = [];

        // Check rows
        for (int row = 0; row < 3; row++)
        {
            int startIndex = row * 3;
            if (board[startIndex] == player && board[startIndex + 1] == player && board[startIndex + 2] == player)
            {
                winningCells = [startIndex, startIndex + 1, startIndex + 2];
                return true;
            }
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (board[col] == player && board[col + 3] == player && board[col + 6] == player)
            {
                winningCells = [col, col + 3, col + 6];
                return true;
            }
        }

        // Check top-left to bottom-right diagonal
        if (board[0] == player && board[4] == player && board[8] == player)
        {
            winningCells = [0, 4, 8];
            return true;
        }

        // Check top-right to bottom-left diagonal
        if (board[2] == player && board[4] == player && board[6] == player)
        {
            winningCells = [2, 4, 6];
            return true;
        }

        return false;
    }

    private bool IsBoardFull(Player?[] board)
    {
        return board.All(cell => cell != null);
    }

    private void UndoLastMove(GameState game)
    {
        if (game.MoveHistory.Count == 0)
            return;

        var lastMove = game.MoveHistory[^1];
        int cellIndex = lastMove.Row * 3 + lastMove.Column;
        game.Board[cellIndex] = null;
        game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);

        // Restore the previous player's turn
        game.CurrentPlayer = lastMove.Player;

        // Clear win/draw status if moving back into in-progress state
        game.GameStatus = GameStatus.InProgress;
        game.Winner = null;
        game.WinningCells = [];
    }
}
