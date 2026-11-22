namespace Yemekhane.Backend.Application.Auth;

public interface IUserContext
{
    int UserId { get; }
    string Role { get; }
    string? SessionToken { get; }
}
