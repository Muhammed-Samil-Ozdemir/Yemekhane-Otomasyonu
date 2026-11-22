using MediatR;
using Yemekhane.Backend.Application.Common;

namespace Yemekhane.Backend.Application.Auth;

public sealed record LogoutCommand() : IRequest<Result<string>>;

internal sealed class LogoutCommandHandler(
    IUserContext userContext,
    ISessionService sessionService) : IRequestHandler<LogoutCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (userContext.SessionToken is not null)
            await sessionService.RemoveAsync(userContext.SessionToken, cancellationToken);

        return Result<string>.Success("Çıkış yapıldı.");
    }
}
