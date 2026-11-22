using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Menus;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/menus")]
[ApiController]
[Authorize]
public sealed class MenuController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet("daily")]
    public async Task<IActionResult> GetDaily(int cafeteriaId, int mealId, DateOnly? date, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetDailyMenuQuery(cafeteriaId, mealId, date), cancellationToken));

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeekly(int cafeteriaId, DateOnly? startDate, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetWeeklyMenuQuery(cafeteriaId, startDate), cancellationToken));
}
