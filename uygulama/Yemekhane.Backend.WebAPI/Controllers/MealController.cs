using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Meals;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/meals")]
[ApiController]
[Authorize]
public sealed class MealController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetAllMealsQuery(), cancellationToken));
}
