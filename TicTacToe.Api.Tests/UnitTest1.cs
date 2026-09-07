using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace TicTacToe.Api.Tests;

public class GameServiceTests
{
    private GameService CreateGameService()
    {
        var stateProvider = new GameStateProvider();
        return new GameService(stateProvider);
    }

    #region Valid Move Tests

    [Fact]
    public void MakeMove_OnEmptyCell_Succeeds()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Act
        var (success, error) = service.TryMakeMove(game, 0, 0, Player.X);

        // Assert
        Assert.True(success);
        Assert.Null(error);
        Assert.Equal(Player.X, game.Board[0]);
        Assert.Equal(Player.O, game.CurrentPlayer); // Turn switched
        Assert.Single(game.MoveHistory);
    }

    #endregion

    #region Invalid Move Tests

    [Fact]
    public void MakeMove_OnOccupiedCell_Fails()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        game.Board[0] = Player.X;
        game.CurrentPlayer = Player.O; // Set to O's turn so we can test occupied cell, not wrong player

        // Act
        var (success, error) = service.TryMakeMove(game, 0, 0, Player.O);

        // Assert
        Assert.False(success);
        Assert.Contains("occupied", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MakeMove_OutOfBounds_Fails()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Act
        var (success, error) = service.TryMakeMove(game, 3, 0, Player.X);

        // Assert
        Assert.False(success);
        Assert.Contains("bounds", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MakeMove_WrongPlayer_Fails()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Act
        var (success, error) = service.TryMakeMove(game, 0, 0, Player.O); // O's turn before X moves

        // Assert
        Assert.False(success);
        Assert.Contains("turn", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MakeMove_AfterGameWon_Fails()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        game.GameStatus = GameStatus.Won;
        game.Winner = Player.X;

        // Act
        var (success, error) = service.TryMakeMove(game, 0, 0, Player.O);

        // Assert
        Assert.False(success);
        Assert.Contains("finished", error, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Win Detection Tests

    [Fact]
    public void CheckWin_RowWin_DetectsWinner()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Play to create a row win for X (top row: 0, 1, 2)
        service.TryMakeMove(game, 0, 0, Player.X);
        service.TryMakeMove(game, 1, 0, Player.O);
        service.TryMakeMove(game, 0, 1, Player.X);
        service.TryMakeMove(game, 1, 1, Player.O);
        var (finalSuccess, _) = service.TryMakeMove(game, 0, 2, Player.X);

        // Assert
        Assert.True(finalSuccess);
        Assert.Equal(GameStatus.Won, game.GameStatus);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(3, game.WinningCells.Length);
    }

    [Fact]
    public void CheckWin_ColumnWin_DetectsWinner()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Play to create a column win for X (leftmost column)
        service.TryMakeMove(game, 0, 0, Player.X);
        service.TryMakeMove(game, 0, 1, Player.O);
        service.TryMakeMove(game, 1, 0, Player.X);
        service.TryMakeMove(game, 1, 1, Player.O);
        var (success, _) = service.TryMakeMove(game, 2, 0, Player.X);

        // Assert
        Assert.True(success);
        Assert.Equal(GameStatus.Won, game.GameStatus);
        Assert.Equal(Player.X, game.Winner);
    }

    [Fact]
    public void CheckWin_DiagonalWin_TopLeftToBottomRight()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Play to create diagonal win for X (0 -> 4 -> 8)
        service.TryMakeMove(game, 0, 0, Player.X);
        service.TryMakeMove(game, 0, 1, Player.O);
        service.TryMakeMove(game, 1, 1, Player.X);
        service.TryMakeMove(game, 0, 2, Player.O);
        var (success, _) = service.TryMakeMove(game, 2, 2, Player.X);

        // Assert
        Assert.True(success);
        Assert.Equal(GameStatus.Won, game.GameStatus);
        Assert.Equal(Player.X, game.Winner);
        Assert.Contains(0, game.WinningCells);
        Assert.Contains(4, game.WinningCells);
        Assert.Contains(8, game.WinningCells);
    }

    [Fact]
    public void CheckWin_DiagonalWin_TopRightToBottomLeft()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Play to create diagonal win for O (2 -> 4 -> 6)
        service.TryMakeMove(game, 0, 0, Player.X);
        service.TryMakeMove(game, 0, 2, Player.O);
        service.TryMakeMove(game, 1, 0, Player.X);
        service.TryMakeMove(game, 1, 1, Player.O);
        service.TryMakeMove(game, 0, 1, Player.X);
        var (success, _) = service.TryMakeMove(game, 2, 0, Player.O);

        // Assert
        Assert.True(success);
        Assert.Equal(GameStatus.Won, game.GameStatus);
        Assert.Equal(Player.O, game.Winner);
    }

    #endregion

    #region Draw Detection Tests

    [Fact]
    public void CheckDraw_FullBoardNoWinner_DetectsDraw()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Use a well-known draw pattern (verified to not have three-in-a-row)
        // X | X | O
        // O | O | X
        // X | O | X
        service.TryMakeMove(game, 0, 0, Player.X); // 1. X
        service.TryMakeMove(game, 0, 2, Player.O); // 2. O
        service.TryMakeMove(game, 0, 1, Player.X); // 3. X
        service.TryMakeMove(game, 1, 0, Player.O); // 4. O
        service.TryMakeMove(game, 2, 0, Player.X); // 5. X
        service.TryMakeMove(game, 1, 1, Player.O); // 6. O
        service.TryMakeMove(game, 1, 2, Player.X); // 7. X
        service.TryMakeMove(game, 2, 1, Player.O); // 8. O
        var (success, _) = service.TryMakeMove(game, 2, 2, Player.X); // 9. X (final move)

        // Assert
        Assert.True(success);
        Assert.Equal(GameStatus.Draw, game.GameStatus);
        Assert.Null(game.Winner);
    }

    #endregion

    #region Turn Switching Tests

    [Fact]
    public void MakeMove_ValidMove_SwitchesTurn()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        Assert.Equal(Player.X, game.CurrentPlayer);

        // Act & Assert - first move
        service.TryMakeMove(game, 0, 0, Player.X);
        Assert.Equal(Player.O, game.CurrentPlayer);

        // Act & Assert - second move
        service.TryMakeMove(game, 1, 1, Player.O);
        Assert.Equal(Player.X, game.CurrentPlayer);
    }

    #endregion

    #region Reset Game Tests

    [Fact]
    public void ResetGame_ClearsBoard()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.TryMakeMove(game, 0, 0, Player.X);
        service.TryMakeMove(game, 1, 1, Player.O);

        // Act
        service.ResetGame(game);

        // Assert
        Assert.All(game.Board, cell => Assert.Null(cell));
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, game.GameStatus);
        Assert.Empty(game.MoveHistory);
        Assert.Null(game.Winner);
    }

    #endregion

    #region Undo Tests

    [Fact]
    public void Undo_TwoPlayerMode_RemovesSingleMove()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.TryMakeMove(game, 0, 0, Player.X);
        service.TryMakeMove(game, 1, 1, Player.O);
        Assert.Equal(2, game.MoveHistory.Count);
        Assert.Equal(Player.X, game.CurrentPlayer); // O moved, now X's turn

        // Act
        var (success, _) = service.TryUndo(game);

        // Assert
        Assert.True(success);
        Assert.Single(game.MoveHistory);
        Assert.Null(game.Board[4]); // Center cell cleared (was O's move)
        Assert.Equal(Player.O, game.CurrentPlayer); // O's turn again
    }

    [Fact]
    public void Undo_ComputerMode_RemovesTwoMoves()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.ComputerMode);
        service.TryMakeMove(game, 0, 0, Player.X);
        service.TryMakeMove(game, 1, 1, Player.O);
        Assert.Equal(2, game.MoveHistory.Count);

        // Act
        var (success, _) = service.TryUndo(game);

        // Assert
        Assert.True(success);
        Assert.Empty(game.MoveHistory);
        Assert.Null(game.Board[0]); // X's initial move cleared
        Assert.Equal(Player.X, game.CurrentPlayer); // X's turn again
    }

    [Fact]
    public void Undo_NoMoves_Fails()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // Act
        var (success, error) = service.TryUndo(game);

        // Assert
        Assert.False(success);
        Assert.Contains("No moves", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Undo_AfterGameCompletion_Fails()
    {
        // Arrange
        var service = CreateGameService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.TryMakeMove(game, 0, 0, Player.X);
        service.TryMakeMove(game, 0, 1, Player.O);
        service.TryMakeMove(game, 1, 0, Player.X);
        service.TryMakeMove(game, 1, 1, Player.O);
        service.TryMakeMove(game, 2, 0, Player.X); // X wins
        Assert.Equal(GameStatus.Won, game.GameStatus);

        // Act
        var (success, error) = service.TryUndo(game);

        // Assert
        Assert.False(success);
        Assert.Contains("Cannot undo", error, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
