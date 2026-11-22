using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Meals.Dtos;
using Yemekhane.Backend.Domain.Meals;

namespace Yemekhane.Backend.Application.Meals;

public sealed record GetAllMealsQuery() : IRequest<Result<List<MealDto>>>;

internal sealed class GetAllMealsQueryHandler(
    IMealRepository mealRepository) : IRequestHandler<GetAllMealsQuery, Result<List<MealDto>>>
{
    public async Task<Result<List<MealDto>>> Handle(GetAllMealsQuery request, CancellationToken cancellationToken)
    {
        var meals = await mealRepository.GetAllAsync(cancellationToken);

        return Result<List<MealDto>>.Success(meals.Adapt<List<MealDto>>());
    }
}
