using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Foods.Dtos;
using Yemekhane.Backend.Domain.Foods;

namespace Yemekhane.Backend.Application.Foods;

public sealed record GetAllFoodsQuery() : IRequest<Result<List<FoodDto>>>;

internal sealed class GetAllFoodsQueryHandler(
    IFoodRepository foodRepository) : IRequestHandler<GetAllFoodsQuery, Result<List<FoodDto>>>
{
    public async Task<Result<List<FoodDto>>> Handle(GetAllFoodsQuery request, CancellationToken cancellationToken)
    {
        var foods = await foodRepository.GetAllAsync(cancellationToken);

        return Result<List<FoodDto>>.Success(foods.Adapt<List<FoodDto>>());
    }
}
