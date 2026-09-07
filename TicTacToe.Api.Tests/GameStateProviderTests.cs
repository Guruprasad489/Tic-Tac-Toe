using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace TicTacToe.Api.Tests;

public class GameStateProviderTests
{
    private GameStateProvider CreateProvider()
    {
        return new GameStateProvider();
    }

    #region Game Storage Tests

    [Fact]
    public void SaveAndGetGame_StoresGameCorrectly()
    {
        // Arrange
        var provider = CreateProvider();
        var game = new GameState { GameId = Guid.NewGuid() };
        game.Board[0] = Player.X;

        // Act
        provider.SaveGame(game);
        var retrieved = provider.GetGame(game.GameId);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(game.GameId, retrieved.GameId);
        Assert.Equal(Player.X, retrieved.Board[0]);
    }

    [Fact]
    public void GetGame_NonexistentGame_ReturnsNull()
    {
        // Arrange
        var provider = CreateProvider();
        var fakeId = Guid.NewGuid();

        // Act
        var retrieved = provider.GetGame(fakeId);

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public void DeleteGame_RemovesGame()
    {
        // Arrange
        var provider = CreateProvider();
        var game = new GameState { GameId = Guid.NewGuid() };
        provider.SaveGame(game);

        // Act
        provider.DeleteGame(game.GameId);
        var retrieved = provider.GetGame(game.GameId);

        // Assert
        Assert.Null(retrieved);
    }

    #endregion

    #region Scoreboard Tests
    // Note: Each test creates a new provider instance, but GameStateProvider uses static fields.
    // To properly test scoreboard isolation, we call ResetScoreboard at start of each test.

    [Fact]
    public void GetScoreboard_InitiallyZero()
    {
        // Arrange
        var provider = CreateProvider();
        provider.ResetScoreboard(); // Ensure clean state

        // Act
        var scoreboard = provider.GetScoreboard();

        // Assert
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void UpdateScoreboard_XWins_IncrementXCount()
    {
        // Arrange
        var provider = CreateProvider();
        provider.ResetScoreboard(); // Ensure clean state

        // Act
        provider.UpdateScoreboard(GameStatus.Won, Player.X);
        var scoreboard = provider.GetScoreboard();

        // Assert
        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void UpdateScoreboard_OWins_IncrementOCount()
    {
        // Arrange
        var provider = CreateProvider();
        provider.ResetScoreboard(); // Ensure clean state

        // Act
        provider.UpdateScoreboard(GameStatus.Won, Player.O);
        var scoreboard = provider.GetScoreboard();

        // Assert
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(1, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void UpdateScoreboard_Draw_IncrementDrawCount()
    {
        // Arrange
        var provider = CreateProvider();
        provider.ResetScoreboard(); // Ensure clean state

        // Act
        provider.UpdateScoreboard(GameStatus.Draw, null);
        var scoreboard = provider.GetScoreboard();

        // Assert
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public void UpdateScoreboard_Multiple_AccumulatesCorrectly()
    {
        // Arrange
        var provider = CreateProvider();
        provider.ResetScoreboard(); // Ensure clean state

        // Act
        provider.UpdateScoreboard(GameStatus.Won, Player.X);
        provider.UpdateScoreboard(GameStatus.Won, Player.X);
        provider.UpdateScoreboard(GameStatus.Won, Player.O);
        provider.UpdateScoreboard(GameStatus.Draw, null);

        var scoreboard = provider.GetScoreboard();

        // Assert
        Assert.Equal(2, scoreboard.XWins);
        Assert.Equal(1, scoreboard.OWins);
        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public void ResetScoreboard_ClearsAllCounts()
    {
        // Arrange
        var provider = CreateProvider();
        provider.UpdateScoreboard(GameStatus.Won, Player.X);
        provider.UpdateScoreboard(GameStatus.Won, Player.O);
        provider.UpdateScoreboard(GameStatus.Draw, null);

        // Act
        provider.ResetScoreboard();
        var scoreboard = provider.GetScoreboard();

        // Assert
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    #endregion
}
