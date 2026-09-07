using Microsoft.AspNetCore.Mvc;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly GameStateProvider _stateProvider;
    private readonly ComputerAIService _computerAI;

    public GamesController(
        GameService gameService,
        GameStateProvider stateProvider,
        ComputerAIService computerAI)
    {
        _gameService = gameService;
        _stateProvider = stateProvider;
        _computerAI = computerAI;
    }

    /// <summary>
    /// POST /api/games
    /// Creates a new game session.
    /// Request body: { "gameMode": 0 } (0 = TwoPlayer, 1 = ComputerMode)
    /// </summary>
    [HttpPost]
    public ActionResult<GameResponse> CreateGame([FromBody] CreateGameRequest request)
    {
        var mode = (GameMode)request.GameMode;
        var game = _gameService.CreateGame(mode);
        return Ok(MapToResponse(game));
    }

    /// <summary>
    /// GET /api/games/{id}
    /// Retrieves the current game state.
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<GameResponse> GetGame(Guid id)
    {
        var game = _stateProvider.GetGame(id);
        if (game == null)
            return NotFound(new ErrorResponse { Message = "Game not found.", StatusCode = 404 });

        return Ok(MapToResponse(game));
    }

    /// <summary>
    /// POST /api/games/{id}/moves
    /// Submits a player move.
    /// Request body: { "row": 0, "column": 0, "player": 0 } (0 = X, 1 = O)
    /// </summary>
    [HttpPost("{id}/moves")]
    public ActionResult<GameResponse> MakeMove(Guid id, [FromBody] MoveRequest request)
    {
        var game = _stateProvider.GetGame(id);
        if (game == null)
            return NotFound(new ErrorResponse { Message = "Game not found.", StatusCode = 404 });

        // Validate move
        var (success, errorMessage) = _gameService.TryMakeMove(game, request.Row, request.Column, request.Player);
        if (!success)
            return BadRequest(new ErrorResponse { Message = errorMessage!, StatusCode = 400 });

        // If the game is now complete, update scoreboard
        if (game.GameStatus != GameStatus.InProgress)
        {
            _stateProvider.UpdateScoreboard(game.GameStatus, game.Winner);
        }

        // In computer mode, if game is still in progress and it's now O's turn, make computer move
        if (game.GameMode == GameMode.ComputerMode && game.GameStatus == GameStatus.InProgress && game.CurrentPlayer == Player.O)
        {
            var (aiRow, aiCol) = _computerAI.SelectMove(game.Board);
            if (aiRow != -1 && aiCol != -1)
            {
                _gameService.TryMakeMove(game, aiRow, aiCol, Player.O);

                // Check if computer move completed the game
                if (game.GameStatus != GameStatus.InProgress)
                {
                    _stateProvider.UpdateScoreboard(game.GameStatus, game.Winner);
                }
            }
        }

        return Ok(MapToResponse(game));
    }

    /// <summary>
    /// POST /api/games/{id}/undo
    /// Undoes the last move(s) based on game mode.
    /// </summary>
    [HttpPost("{id}/undo")]
    public ActionResult<GameResponse> UndoMove(Guid id)
    {
        var game = _stateProvider.GetGame(id);
        if (game == null)
            return NotFound(new ErrorResponse { Message = "Game not found.", StatusCode = 404 });

        var (success, errorMessage) = _gameService.TryUndo(game);
        if (!success)
            return BadRequest(new ErrorResponse { Message = errorMessage!, StatusCode = 400 });

        return Ok(MapToResponse(game));
    }

    /// <summary>
    /// POST /api/games/{id}/reset
    /// Resets the current game (clears board, keeps game ID, keeps scoreboard).
    /// </summary>
    [HttpPost("{id}/reset")]
    public ActionResult<GameResponse> ResetGame(Guid id)
    {
        var game = _stateProvider.GetGame(id);
        if (game == null)
            return NotFound(new ErrorResponse { Message = "Game not found.", StatusCode = 404 });

        _gameService.ResetGame(game);
        return Ok(MapToResponse(game));
    }

    // Helper Methods

    private GameResponse MapToResponse(GameState game)
    {
        var scoreboard = _stateProvider.GetScoreboard();

        var moveHistory = game.MoveHistory
            .Select((move, index) => new MoveDto
            {
                MoveNumber = index + 1,
                Player = move.Player,
                Row = move.Row,
                Column = move.Column
            })
            .ToList();

        return new GameResponse
        {
            GameId = game.GameId,
            Board = game.Board,
            CurrentPlayer = game.CurrentPlayer,
            GameMode = game.GameMode,
            GameStatus = game.GameStatus,
            Winner = game.Winner,
            WinningCells = game.WinningCells,
            MoveHistory = moveHistory,
            Scoreboard = scoreboard
        };
    }
}

public class CreateGameRequest
{
    public int GameMode { get; set; } // 0 = TwoPlayer, 1 = ComputerMode
}
