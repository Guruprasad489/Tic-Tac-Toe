using Microsoft.AspNetCore.Mvc;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController : ControllerBase
{
    private readonly GameStateProvider _stateProvider;

    public ScoreboardController(GameStateProvider stateProvider)
    {
        _stateProvider = stateProvider;
    }

    /// <summary>
    /// GET /api/scoreboard
    /// Returns the current session scoreboard.
    /// </summary>
    [HttpGet]
    public ActionResult<Scoreboard> GetScoreboard()
    {
        var scoreboard = _stateProvider.GetScoreboard();
        return Ok(scoreboard);
    }

    /// <summary>
    /// POST /api/scoreboard/reset
    /// Resets the scoreboard to 0-0-0.
    /// </summary>
    [HttpPost("reset")]
    public ActionResult<Scoreboard> ResetScoreboard()
    {
        _stateProvider.ResetScoreboard();
        var scoreboard = _stateProvider.GetScoreboard();
        return Ok(scoreboard);
    }
}
