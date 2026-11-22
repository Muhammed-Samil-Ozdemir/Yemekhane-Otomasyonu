using Dapper;
using Yemekhane.Backend.Domain.Foods;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Foods;

public sealed class FoodRepository(
    IDbConnectionFactory connectionFactory) : IFoodRepository
{
    public async Task<List<Food>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT y.id         AS Id,
                   y.ad         AS Name,
                   k.id         AS CategoryId,
                   k.ad         AS CategoryName,
                   k.sira       AS CategoryOrder,
                   y.kalori     AS Calories,
                   y.vejetaryen AS IsVegetarian,
                   (SELECT GROUP_CONCAT(a.ad ORDER BY a.ad SEPARATOR ', ')
                    FROM yemek_alerjenleri ya
                             JOIN alerjenler a ON a.id = ya.alerjen_id
                    WHERE ya.yemek_id = y.id) AS Allergens
            FROM yemekler y
                     JOIN yemek_kategorileri k ON k.id = y.kategori_id
            ORDER BY k.sira, y.ad
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var foods = await connection.QueryAsync<Food>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return foods.ToList();
    }
}
