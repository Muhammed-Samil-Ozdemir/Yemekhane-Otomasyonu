using BCrypt.Net;
using Yemekhane.Backend.Application.Auth;

namespace Yemekhane.Backend.Infrastructure.Auth;

public sealed class PasswordHasher : IPasswordHasher
{
    public bool Verify(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (SaltParseException)
        {
            return false;
        }
    }
}
