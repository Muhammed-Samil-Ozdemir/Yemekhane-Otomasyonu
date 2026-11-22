using Mapster;
using Yemekhane.Backend.Application.Balances.Dtos;
using Yemekhane.Backend.Domain.Balances;

namespace Yemekhane.Backend.Application.Balances.Mapping;

public sealed class BalanceMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BalanceMovement, BalanceMovementDto>()
            .Map(dest => dest.TypeText, src => BalanceMovementTypes.GetText(src.Type));
    }
}
