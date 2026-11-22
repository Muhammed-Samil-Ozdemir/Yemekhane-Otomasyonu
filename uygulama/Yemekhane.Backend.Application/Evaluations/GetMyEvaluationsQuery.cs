using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Evaluations.Dtos;
using Yemekhane.Backend.Domain.Evaluations;

namespace Yemekhane.Backend.Application.Evaluations;

public sealed record GetMyEvaluationsQuery() : IRequest<Result<List<EvaluationDto>>>;

internal sealed class GetMyEvaluationsQueryHandler(
    IEvaluationRepository evaluationRepository,
    IUserContext userContext) : IRequestHandler<GetMyEvaluationsQuery, Result<List<EvaluationDto>>>
{
    public async Task<Result<List<EvaluationDto>>> Handle(GetMyEvaluationsQuery request, CancellationToken cancellationToken)
    {
        var menus = await evaluationRepository.GetByUserAsync(userContext.UserId, cancellationToken);

        return Result<List<EvaluationDto>>.Success(menus.Adapt<List<EvaluationDto>>());
    }
}
