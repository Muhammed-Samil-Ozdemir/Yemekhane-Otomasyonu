using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Menus.Dtos;
using Yemekhane.Backend.Domain.Menus;

namespace Yemekhane.Backend.Application.Menus;

public sealed record GetMenuReportQuery(
    int? CafeteriaId,
    DateOnly? StartDate,
    DateOnly? EndDate) : IRequest<Result<List<MenuReportDto>>>;

internal sealed class GetMenuReportQueryHandler(
    IMenuRepository menuRepository) : IRequestHandler<GetMenuReportQuery, Result<List<MenuReportDto>>>
{
    private const int DefaultDayRange = 7;

    public async Task<Result<List<MenuReportDto>>> Handle(GetMenuReportQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var startDate = request.StartDate ?? today.AddDays(-DefaultDayRange);
        var endDate = request.EndDate ?? today.AddDays(DefaultDayRange);

        if (startDate > endDate)
            return Result<List<MenuReportDto>>.BadRequest("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        var report = await menuRepository.GetReportAsync(
            request.CafeteriaId, startDate.ToDateTime(), endDate.ToDateTime(), cancellationToken);

        return Result<List<MenuReportDto>>.Success(report.Adapt<List<MenuReportDto>>());
    }
}
