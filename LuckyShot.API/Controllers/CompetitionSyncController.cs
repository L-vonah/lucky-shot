using LuckyShot.API.Services;
using LuckyShot.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuckyShot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/competition-sync")]
public class CompetitionSyncController(ICompetitionSyncService competitionSyncService) : ControllerBase
{
    [HttpPost("summary")]
    public async Task<IActionResult> SyncCompetitionSummary([FromBody] CompetitionSyncRequest request)
    {
        await competitionSyncService.SyncCompetitionsSummaryAsync(request.Category, request.Year);
        return NoContent();
    }

    [HttpPost("matches")]
    public async Task<IActionResult> SyncCompetitionMatches([FromBody] CompetitionSyncRequest request)
    {
        await competitionSyncService.SyncCompetitionSeasonMatchesAsync(request.Category, request.Year);
        return NoContent();
    }
}

public record CompetitionSyncRequest(CompetitionCategory Category, int Year);
