namespace Tic_Tac_Toe.Models;

/// <summary>
/// Represents a single move in the game.
/// </summary>
public class Move
{
    public Player Player { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
