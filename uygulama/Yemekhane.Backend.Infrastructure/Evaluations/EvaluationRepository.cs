using Dapper;
using MySqlConnector;
using Yemekhane.Backend.Domain.Abstractions;
using Yemekhane.Backend.Domain.Evaluations;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Evaluations;

public sealed class EvaluationRepository(
    IDbConnectionFactory connectionFactory) : IEvaluationRepository
{
    public async Task<List<EvaluableMenu>> GetByUserAsync(int userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT r.menu_id          AS MenuId,
                   m.tarih            AS Date,
                   o.ad               AS MealName,
                   yh.ad              AS CafeteriaName,
                   (SELECT GROUP_CONCAT(md.yemek ORDER BY md.kategori_sira SEPARATOR ', ')
                    FROM v_menu_detay md
                    WHERE md.menu_id = r.menu_id) AS Foods,
                   d.puan             AS Score,
                   d.yorum            AS Comment,
                   d.olusturma        AS EvaluatedAt
            FROM rezervasyonlar r
                     JOIN menuler m ON m.id = r.menu_id
                     JOIN ogunler o ON o.id = m.ogun_id
                     JOIN yemekhaneler yh ON yh.id = m.yemekhane_id
                     LEFT JOIN degerlendirmeler d ON d.kullanici_id = r.kullanici_id AND d.menu_id = r.menu_id
            WHERE r.kullanici_id = @UserId
              AND r.durum = 'kullanildi'
            ORDER BY m.tarih DESC, o.sira DESC
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var menus = await connection.QueryAsync<EvaluableMenu>(new CommandDefinition(
            sql,
            new { UserId = userId },
            cancellationToken: cancellationToken));

        return menus.ToList();
    }

    public async Task CreateAsync(int userId, int menuId, int score, string? comment, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO degerlendirmeler (kullanici_id, menu_id, puan, yorum)
            SELECT r.kullanici_id, r.menu_id, @Score, @Comment
            FROM rezervasyonlar r
            WHERE r.kullanici_id = @UserId
              AND r.menu_id = @MenuId
              AND r.durum = 'kullanildi'
            """;

        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);

        try
        {
            var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
                sql,
                new { UserId = userId, MenuId = menuId, Score = score, Comment = comment },
                cancellationToken: cancellationToken));

            if (affectedRows == 0)
                throw new BusinessRuleException("Yalnızca kullandığınız menüleri değerlendirebilirsiniz.");
        }
        catch (MySqlException exception) when (exception.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            throw new BusinessRuleException("Bu menüyü zaten değerlendirdiniz.");
        }
    }
}
