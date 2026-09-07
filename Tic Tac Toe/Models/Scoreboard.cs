namespace Tic_Tac_Toe.Models;

/// <summary>
/// Represents the scoreboard tracking wins and draws for a session.
/// </summary>
public class Scoreboard
{
    public int XWins { get; set; } = 0;
    public int OWins { get; set; } = 0;
    public int Draws { get; set; } = 0;
}
