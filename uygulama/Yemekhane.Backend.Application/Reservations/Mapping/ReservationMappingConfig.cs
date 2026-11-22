using Mapster;
using Yemekhane.Backend.Application.Reservations.Dtos;
using Yemekhane.Backend.Domain.Reservations;

namespace Yemekhane.Backend.Application.Reservations.Mapping;

public sealed class ReservationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Reservation, ReservationDto>()
            .Map(dest => dest.Date, src => DateOnly.FromDateTime(src.Date))
            .Map(dest => dest.StatusText, src => ReservationStatuses.GetText(src.Status))
            .Map(dest => dest.CanCancel, src => src.CanCancel(DateTime.Now));
    }
}
