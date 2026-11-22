using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Menus;
using Yemekhane.Backend.Domain.Users;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/admin/menus")]
[ApiController]
[Authorize(Roles = UserRoles.Admin)]
public sealed class AdminMenuController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetReport(int? cafeteriaId, DateOnly? startDate, DateOnly? endDate, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetMenuReportQuery(cafeteriaId, startDate, endDate), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetMenuDefinitionQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(CreateMenuCommand request, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(request, cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateMenuCommand request, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(request with { Id = id }, cancellationToken));
}
