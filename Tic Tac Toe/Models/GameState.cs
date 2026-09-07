namespace Tic_Tac_Toe.Models;

/// <summary>
/// Represents the core game state.
/// The board is a 9-element array representing a 3x3 grid:
/// Index mapping: 0 1 2
///                3 4 5
///                6 7 8
/// </summary>
public class GameState
{
    public Guid GameId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 9-element array where null means empty, 'X' or 'O' represent players.
    /// </summary>
    public Player?[] Board { get; set; } = new Player?[9];

    public Player CurrentPlayer { get; set; } = Player.X;
    public GameMode GameMode { get; set; } = GameMode.TwoPlayer;
    public GameStatus GameStatus { get; set; } = GameStatus.InProgress;

    /// <summary>
    /// The player who won, or null if game is not won.
    /// </summary>
    public Player? Winner { get; set; } = null;

    /// <summary>
    /// Indices of cells that form the winning line (3 cells), or empty if no winner.
    /// </summary>
    public int[] WinningCells { get; set; } = [];

    public List<Move> MoveHistory { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
