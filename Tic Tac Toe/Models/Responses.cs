namespace Tic_Tac_Toe.Models;

/// <summary>
/// Data Transfer Objects for API responses and requests.
/// </summary>

public class GameResponse
{
    public Guid GameId { get; set; }
    public Player?[] Board { get; set; } = null!;
    public Player CurrentPlayer { get; set; }
    public GameMode GameMode { get; set; }
    public GameStatus GameStatus { get; set; }
    public Player? Winner { get; set; }
    public int[] WinningCells { get; set; } = [];
    public List<MoveDto> MoveHistory { get; set; } = [];
    public Scoreboard Scoreboard { get; set; } = null!;
}

public class MoveDto
{
    public int MoveNumber { get; set; }
    public Player Player { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}

public class MoveRequest
{
    public int Row { get; set; }
    public int Column { get; set; }
    public Player Player { get; set; }
}

public class ErrorResponse
{
    public string Message { get; set; } = null!;
    public int StatusCode { get; set; }
}
