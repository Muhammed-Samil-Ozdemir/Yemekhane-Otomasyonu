using Mapster;
using Yemekhane.Backend.Application.Auth.Dtos;
using Yemekhane.Backend.Domain.Users;

namespace Yemekhane.Backend.Application.Auth.Mapping;

public sealed class AuthMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, CurrentUserDto>()
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.RoleText, src => UserRoles.GetText(src.Role));
    }
}
