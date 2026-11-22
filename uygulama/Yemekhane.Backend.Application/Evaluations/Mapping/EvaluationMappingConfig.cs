using Mapster;
using Yemekhane.Backend.Application.Evaluations.Dtos;
using Yemekhane.Backend.Domain.Evaluations;

namespace Yemekhane.Backend.Application.Evaluations.Mapping;

public sealed class EvaluationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<EvaluableMenu, EvaluationDto>()
            .Map(dest => dest.Date, src => DateOnly.FromDateTime(src.Date))
            .Map(dest => dest.CanEvaluate, src => !src.IsEvaluated);
    }
}
