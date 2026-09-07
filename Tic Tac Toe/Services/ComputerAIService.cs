using Tic_Tac_Toe.Models;

namespace Tic_Tac_Toe.Services;

/// <summary>
/// AI service for the computer player (O) in computer mode.
/// Uses a priority-based heuristic: Win > Block > Center > Corner > Any Available
/// </summary>
public class ComputerAIService
{
    /// <summary>
    /// Selects the best move for the computer player (O).
    /// Priority:
    /// 1. If O can win, play the winning move
    /// 2. If X can win next, block X
    /// 3. Take center if available
    /// 4. Take a corner if available
    /// 5. Take any available cell
    /// Returns (row, column) or (-1, -1) if no moves available.
    /// </summary>
    public (int Row, int Column) SelectMove(Player?[] board)
    {
        var availableCells = GetAvailableCells(board);
        if (availableCells.Count == 0)
            return (-1, -1);

        // Priority 1: Can O win?
        foreach (var (row, col) in availableCells)
        {
            if (CanWin(board, Player.O, row, col))
                return (row, col);
        }

        // Priority 2: Can X win on next move? Block it.
        foreach (var (row, col) in availableCells)
        {
            if (CanWin(board, Player.X, row, col))
                return (row, col);
        }

        // Priority 3: Take center (index 4)
        if (availableCells.Contains((1, 1)))
            return (1, 1);

        // Priority 4: Take a corner
        var corners = new[] { (0, 0), (0, 2), (2, 0), (2, 2) };
        foreach (var corner in corners)
        {
            if (availableCells.Contains(corner))
                return corner;
        }

        // Priority 5: Take any available cell
        return availableCells[0];
    }

    // Helper Methods

    private List<(int Row, int Column)> GetAvailableCells(Player?[] board)
    {
        var available = new List<(int Row, int Column)>();
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == null)
            {
                int row = i / 3;
                int col = i % 3;
                available.Add((row, col));
            }
        }
        return available;
    }

    private bool CanWin(Player?[] board, Player player, int row, int column)
    {
        int cellIndex = row * 3 + column;

        // Temporarily place the player's mark
        board[cellIndex] = player;

        // Check if this move results in a win
        bool isWinning = CheckWin(board, player);

        // Undo the temporary placement
        board[cellIndex] = null;

        return isWinning;
    }

    private bool CheckWin(Player?[] board, Player player)
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            int startIndex = row * 3;
            if (board[startIndex] == player && board[startIndex + 1] == player && board[startIndex + 2] == player)
                return true;
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (board[col] == player && board[col + 3] == player && board[col + 6] == player)
                return true;
        }

        // Check top-left to bottom-right diagonal
        if (board[0] == player && board[4] == player && board[8] == player)
            return true;

        // Check top-right to bottom-left diagonal
        if (board[2] == player && board[4] == player && board[6] == player)
            return true;

        return false;
    }
}
