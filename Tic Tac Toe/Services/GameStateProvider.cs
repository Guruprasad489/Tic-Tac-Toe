using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.Services;

/// <summary>
/// Provides in-memory storage for game states and scoreboard.
/// This singleton maintains all active games and the session-level scoreboard.
/// All data is lost when the application restarts.
/// </summary>
public class GameStateProvider
{
    private static readonly Dictionary<Guid, GameState> Games = new();
    private static readonly Scoreboard SessionScoreboard = new();
    private static readonly object LockObject = new();

    /// <summary>
    /// Saves or updates a game state.
    /// </summary>
    public void SaveGame(GameState game)
    {
        lock (LockObject)
        {
            Games[game.GameId] = game;
        }
    }

    /// <summary>
    /// Retrieves a game by ID, or null if not found.
    /// </summary>
    public GameState? GetGame(Guid gameId)
    {
        lock (LockObject)
        {
            Games.TryGetValue(gameId, out var game);
            return game;
        }
    }

    /// <summary>
    /// Deletes a game (rarely used, but available for cleanup).
    /// </summary>
    public void DeleteGame(Guid gameId)
    {
        lock (LockObject)
        {
            Games.Remove(gameId);
        }
    }

    /// <summary>
    /// Gets the current session scoreboard.
    /// </summary>
    public Scoreboard GetScoreboard()
    {
        lock (LockObject)
        {
            return new Scoreboard
            {
                XWins = SessionScoreboard.XWins,
                OWins = SessionScoreboard.OWins,
                Draws = SessionScoreboard.Draws
            };
        }
    }

    /// <summary>
    /// Updates the scoreboard (used after a game completes).
    /// Increments the appropriate win/draw counter based on the game result.
    /// </summary>
    public void UpdateScoreboard(GameStatus status, Player? winner)
    {
        lock (LockObject)
        {
            if (status == GameStatus.Won && winner.HasValue)
            {
                if (winner == Player.X)
                    SessionScoreboard.XWins++;
                else
                    SessionScoreboard.OWins++;
            }
            else if (status == GameStatus.Draw)
            {
                SessionScoreboard.Draws++;
            }
        }
    }

    /// <summary>
    /// Resets the scoreboard to 0-0-0.
    /// </summary>
    public void ResetScoreboard()
    {
        lock (LockObject)
        {
            SessionScoreboard.XWins = 0;
            SessionScoreboard.OWins = 0;
            SessionScoreboard.Draws = 0;
        }
    }
}
