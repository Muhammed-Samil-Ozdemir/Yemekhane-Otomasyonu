using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Evaluations;

namespace Yemekhane.Backend.Application.Evaluations;

public sealed record CreateEvaluationCommand(
    int MenuId,
    int Score,
    string? Comment) : IRequest<Result<string>>;

internal sealed class CreateEvaluationCommandHandler(
    IEvaluationRepository evaluationRepository,
    IUserContext userContext) : IRequestHandler<CreateEvaluationCommand, Result<string>>
{
    private const int MaxCommentLength = 500;

    public async Task<Result<string>> Handle(CreateEvaluationCommand request, CancellationToken cancellationToken)
    {
        if (request.Score is < 1 or > 5)
            return Result<string>.BadRequest("Puan 1 ile 5 arasında olmalıdır.");

        var comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
        if (comment?.Length > MaxCommentLength)
            return Result<string>.BadRequest($"Yorum en fazla {MaxCommentLength} karakter olabilir.");

        await evaluationRepository.CreateAsync(userContext.UserId, request.MenuId, request.Score, comment, cancellationToken);

        return Result<string>.Created("Değerlendirmeniz kaydedildi. Teşekkürler!");
    }
}
