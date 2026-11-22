using Mapster;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Menus.Dtos;
using Yemekhane.Backend.Domain.Menus;

namespace Yemekhane.Backend.Application.Menus.Mapping;

public sealed class MenuMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MenuReport, MenuReportDto>()
            .Map(dest => dest.Date, src => DateOnly.FromDateTime(src.Date));

        config.NewConfig<MenuDefinition, MenuDefinitionDto>()
            .Map(dest => dest.Date, src => DateOnly.FromDateTime(src.Date));

        config.NewConfig<CreateMenuCommand, MenuDefinition>()
            .Ignore(dest => dest.Id)
            .Map(dest => dest.Date, src => src.Date.ToDateTime())
            .Map(dest => dest.FoodIds, src => src.FoodIds.Distinct().ToList());

        config.NewConfig<UpdateMenuCommand, MenuDefinition>()
            .Map(dest => dest.Date, src => src.Date.ToDateTime())
            .Map(dest => dest.FoodIds, src => src.FoodIds.Distinct().ToList());
    }
}
