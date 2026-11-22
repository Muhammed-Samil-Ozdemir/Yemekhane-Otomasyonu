namespace Yemekhane.Backend.Application.Auth;

public sealed record SessionUser(
    int UserId,
    string FullName,
    string Email,
    string Role);
