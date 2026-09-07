using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace TicTacToe.Api.Tests;

public class ComputerAIServiceTests
{
    private ComputerAIService CreateAIService()
    {
        return new ComputerAIService();
    }

    #region AI Move Selection Tests

    [Fact]
    public void SelectMove_CanWin_PlaysWinningMove()
    {
        // Arrange
        var ai = CreateAIService();
        var board = new Player?[9];
        // Set up board where O can win with move to index 8
        board[2] = Player.O;  // Top-right
        board[4] = Player.O;  // Center
        // board[6] is empty - winning move

        // Act
        var (row, col) = ai.SelectMove(board);

        // Assert
        int moveIndex = row * 3 + col;
        Assert.Equal(6, moveIndex); // Top-right to bottom-left diagonal win
    }

    [Fact]
    public void SelectMove_CanBlockOpponentWin_BlocksX()
    {
        // Arrange
        var ai = CreateAIService();
        var board = new Player?[9];
        // Set up board where X can win on next move, O must block
        board[0] = Player.X;  // Top-left
        board[1] = Player.X;  // Top-middle
        // board[2] is empty - blocking move

        // Act
        var (row, col) = ai.SelectMove(board);

        // Assert
        int moveIndex = row * 3 + col;
        Assert.Equal(2, moveIndex); // Block the top row
    }

    [Fact]
    public void SelectMove_CenterAvailable_TakesCenter()
    {
        // Arrange
        var ai = CreateAIService();
        var board = new Player?[9];

        // Act
        var (row, col) = ai.SelectMove(board);

        // Assert
        Assert.Equal(1, row);
        Assert.Equal(1, col);
    }

    [Fact]
    public void SelectMove_CenterTaken_TakesCorner()
    {
        // Arrange
        var ai = CreateAIService();
        var board = new Player?[9];
        board[4] = Player.X; // Center taken

        // Act
        var (row, col) = ai.SelectMove(board);

        // Assert
        // Should be a corner: (0,0), (0,2), (2,0), or (2,2)
        var corners = new[] { (0, 0), (0, 2), (2, 0), (2, 2) };
        Assert.Contains((row, col), corners);
    }

    [Fact]
    public void SelectMove_NoPreference_TakesAnyAvailable()
    {
        // Arrange
        var ai = CreateAIService();
        var board = new Player?[9];
        board[0] = Player.X;
        board[1] = Player.O;
        board[2] = Player.X;
        board[3] = Player.O;
        board[4] = Player.X;
        board[5] = Player.O;
        board[6] = Player.X;
        board[7] = Player.O;
        // Only index 8 is available

        // Act
        var (row, col) = ai.SelectMove(board);

        // Assert
        Assert.Equal(2, row);
        Assert.Equal(2, col);
    }

    [Fact]
    public void SelectMove_BoardFull_ReturnsNoMove()
    {
        // Arrange
        var ai = CreateAIService();
        var board = new Player?[9];
        for (int i = 0; i < 9; i++)
        {
            board[i] = i % 2 == 0 ? Player.X : Player.O;
        }

        // Act
        var (row, col) = ai.SelectMove(board);

        // Assert
        Assert.Equal(-1, row);
        Assert.Equal(-1, col);
    }

    #endregion
}
