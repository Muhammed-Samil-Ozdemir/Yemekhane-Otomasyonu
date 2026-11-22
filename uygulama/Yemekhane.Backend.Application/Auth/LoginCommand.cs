using System.Text.Json.Serialization;
using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Auth.Dtos;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Users;

namespace Yemekhane.Backend.Application.Auth;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<LoginCommandResponse>>;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ISessionService sessionService) : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<LoginCommandResponse>.BadRequest("E-posta ve şifre zorunludur.");

        var user = await userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);

        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginCommandResponse>.Unauthorized("E-posta ya da şifre hatalı.");

        var token = await sessionService.CreateAsync(
            new SessionUser(user.Id, user.FullName, user.Email, user.Role),
            cancellationToken);

        return Result<LoginCommandResponse>.Success(
            new LoginCommandResponse(token, user.Adapt<CurrentUserDto>()));
    }
}

public sealed record LoginCommandResponse(
    [property: JsonIgnore] string Token,
    CurrentUserDto User);
