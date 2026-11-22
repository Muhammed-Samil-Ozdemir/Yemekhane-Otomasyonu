using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Auth.Dtos;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Users;

namespace Yemekhane.Backend.Application.Auth;

public sealed record GetCurrentUserQuery() : IRequest<Result<CurrentUserDto>>;

internal sealed class GetCurrentUserQueryHandler(
    IUserRepository userRepository,
    IUserContext userContext) : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userContext.UserId, cancellationToken);

        return user is null
            ? Result<CurrentUserDto>.NotFound("Kullanıcı bulunamadı.")
            : Result<CurrentUserDto>.Success(user.Adapt<CurrentUserDto>());
    }
}
