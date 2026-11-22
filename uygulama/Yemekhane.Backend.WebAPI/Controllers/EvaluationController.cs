using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Evaluations;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/evaluations")]
[ApiController]
[Authorize]
public sealed class EvaluationController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetMyEvaluationsQuery(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(CreateEvaluationCommand request, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(request, cancellationToken));
}
