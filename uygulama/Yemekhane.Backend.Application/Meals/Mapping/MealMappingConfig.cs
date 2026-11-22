using Mapster;
using Yemekhane.Backend.Application.Meals.Dtos;
using Yemekhane.Backend.Domain.Meals;

namespace Yemekhane.Backend.Application.Meals.Mapping;

public sealed class MealMappingConfig : IRegister
{
    private const string TimeFormat = @"hh\:mm";

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Meal, MealDto>()
            .Map(dest => dest.StartTime, src => src.StartTime.ToString(TimeFormat))
            .Map(dest => dest.EndTime, src => src.EndTime.ToString(TimeFormat))
            .Map(dest => dest.ReservationDeadline, src => src.ReservationDeadline.ToString(TimeFormat));
    }
}
