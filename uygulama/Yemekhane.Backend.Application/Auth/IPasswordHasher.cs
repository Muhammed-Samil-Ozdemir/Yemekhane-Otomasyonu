namespace Yemekhane.Backend.Application.Auth;

public interface IPasswordHasher
{
    bool Verify(string password, string passwordHash);
}
