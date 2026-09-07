namespace Tic_Tac_Toe.Models;

/// <summary>
/// Represents the current status of a game.
/// </summary>
public enum GameStatus
{
    InProgress,
    Won,
    Draw,
}

/// <summary>
/// Represents the game mode being played.
/// </summary>
public enum GameMode
{
    TwoPlayer,
    ComputerMode
}

/// <summary>
/// Represents a player in the game.
/// </summary>
public enum Player
{
    X,
    O
}
