using System.Data;
using Dapper;
using MySqlConnector;
using Yemekhane.Backend.Domain.Abstractions;
using Yemekhane.Backend.Domain.Menus;
using Yemekhane.Backend.Infrastructure.Data;

namespace Yemekhane.Backend.Infrastructure.Menus;

public sealed class MenuRepository(
    IDbConnectionFactory connectionFactory) : IMenuRepository
{
    public async Task<Menu?> GetAsync(int cafeteriaId, DateTime date, int mealId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT menu_id                AS MenuId,
                   yemekhane_id           AS CafeteriaId,
                   yemekhane              AS CafeteriaName,
                   tarih                  AS Date,
                   ogun_id                AS MealId,
                   ogun                   AS MealName,
                   rezervasyon_son        AS ReservationDeadline,
                   ogrenci_fiyati         AS StudentPrice,
                   personel_fiyati        AS StaffPrice,
                   yemek_id               AS FoodId,
                   yemek                  AS FoodName,
                   kategori               AS CategoryName,
                   kategori_sira          AS CategoryOrder,
                   kalori                 AS Calories,
                   vejetaryen             AS IsVegetarian,
                   alerjenler             AS Allergens
            FROM v_menu_detay
            WHERE yemekhane_id = @CafeteriaId
              AND tarih = @Date
              AND ogun_id = @MealId
            ORDER BY kategori_sira, yemek
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var rows = (await connection.QueryAsync<MenuDetailRow>(new CommandDefinition(
            sql,
            new { CafeteriaId = cafeteriaId, Date = date, MealId = mealId },
            cancellationToken: cancellationToken))).ToList();

        if (rows.Count == 0)
            return null;

        var first = rows[0];

        return new Menu
        {
            Id = first.MenuId,
            CafeteriaId = first.CafeteriaId,
            CafeteriaName = first.CafeteriaName,
            Date = first.Date,
            MealId = first.MealId,
            MealName = first.MealName,
            ReservationDeadline = first.ReservationDeadline,
            StudentPrice = first.StudentPrice,
            StaffPrice = first.StaffPrice,
            Foods = rows.Select(row => new MenuFood
            {
                FoodId = row.FoodId,
                Name = row.FoodName,
                CategoryName = row.CategoryName,
                CategoryOrder = row.CategoryOrder,
                Calories = row.Calories,
                IsVegetarian = row.IsVegetarian,
                Allergens = SplitNames(row.Allergens)
            }).ToList()
        };
    }

    public async Task<List<MenuOccupancy>> GetOccupanciesAsync(int cafeteriaId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT menu_id            AS MenuId,
                   kapasite           AS Capacity,
                   rezerve            AS ReservationCount,
                   bos_yer            AS FreeSeats
            FROM v_menu_doluluk
            WHERE yemekhane_id = @CafeteriaId
              AND tarih BETWEEN @StartDate AND @EndDate
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var occupancies = await connection.QueryAsync<MenuOccupancy>(new CommandDefinition(
            sql,
            new { CafeteriaId = cafeteriaId, StartDate = startDate, EndDate = endDate },
            cancellationToken: cancellationToken));

        return occupancies.ToList();
    }

    public async Task<List<MenuReport>> GetReportAsync(int? cafeteriaId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT d.menu_id              AS MenuId,
                   d.yemekhane_id         AS CafeteriaId,
                   d.yemekhane            AS CafeteriaName,
                   d.tarih                AS Date,
                   d.ogun_id              AS MealId,
                   d.ogun                 AS MealName,
                   d.kapasite             AS Capacity,
                   d.rezerve              AS ReservationCount,
                   d.bos_yer              AS FreeSeats,
                   d.doluluk_yuzde        AS OccupancyRate,
                   p.oy_sayisi            AS EvaluationCount,
                   p.ortalama_puan        AS AverageScore
            FROM v_menu_doluluk d
                     JOIN v_menu_puan p ON p.menu_id = d.menu_id
            WHERE (@CafeteriaId IS NULL OR d.yemekhane_id = @CafeteriaId)
              AND d.tarih BETWEEN @StartDate AND @EndDate
            ORDER BY d.tarih, d.ogun_id, d.yemekhane_id
            """;

        await using var connection = await connectionFactory.OpenReplicaAsync(cancellationToken);

        var report = await connection.QueryAsync<MenuReport>(new CommandDefinition(
            sql,
            new { CafeteriaId = cafeteriaId, StartDate = startDate, EndDate = endDate },
            cancellationToken: cancellationToken));

        return report.ToList();
    }

    public async Task<MenuDefinition?> GetDefinitionAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id              AS Id,
                   yemekhane_id    AS CafeteriaId,
                   tarih           AS Date,
                   ogun_id         AS MealId,
                   ogrenci_fiyati  AS StudentPrice,
                   personel_fiyati AS StaffPrice
            FROM menuler
            WHERE id = @Id;

            SELECT yemek_id
            FROM menu_yemekleri
            WHERE menu_id = @Id
            ORDER BY yemek_id;
            """;

        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);
        await using var results = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken));

        var menu = await results.ReadSingleOrDefaultAsync<MenuDefinition>();
        if (menu is null)
            return null;

        menu.FoodIds = (await results.ReadAsync<int>()).ToList();

        return menu;
    }

    public async Task<int> CreateAsync(MenuDefinition menu, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO menuler (yemekhane_id, tarih, ogun_id, ogrenci_fiyati, personel_fiyati)
            VALUES (@CafeteriaId, @Date, @MealId, @StudentPrice, @StaffPrice);

            SELECT LAST_INSERT_ID();
            """;

        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var menuId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                sql, menu, transaction, cancellationToken: cancellationToken));

            await InsertFoodsAsync(connection, transaction, menuId, menu.FoodIds, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return menuId;
        }
        catch (MySqlException exception) when (IsKnownWriteError(exception))
        {
            throw ToBusinessRuleException(exception);
        }
    }

    public async Task UpdateAsync(MenuDefinition menu, CancellationToken cancellationToken)
    {
        const string currentSql = """
            SELECT id           AS Id,
                   yemekhane_id AS CafeteriaId,
                   tarih        AS Date,
                   ogun_id      AS MealId
            FROM menuler
            WHERE id = @Id
                FOR UPDATE
            """;

        const string reservationCountSql = """
            SELECT COUNT(*)
            FROM rezervasyonlar
            WHERE menu_id = @Id
              AND durum IN ('aktif', 'kullanildi')
            """;

        const string updateSql = """
            UPDATE menuler
            SET yemekhane_id    = @CafeteriaId,
                tarih           = @Date,
                ogun_id         = @MealId,
                ogrenci_fiyati  = @StudentPrice,
                personel_fiyati = @StaffPrice
            WHERE id = @Id;

            DELETE FROM menu_yemekleri
            WHERE menu_id = @Id;
            """;

        await using var connection = await connectionFactory.OpenPrimaryAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var current = await connection.QuerySingleOrDefaultAsync<MenuDefinition>(new CommandDefinition(
                currentSql, new { menu.Id }, transaction, cancellationToken: cancellationToken))
            ?? throw new BusinessRuleException("Menü bulunamadı.");

        var isSlotChanged = current.CafeteriaId != menu.CafeteriaId
                            || current.Date != menu.Date
                            || current.MealId != menu.MealId;

        if (isSlotChanged)
        {
            var reservationCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                reservationCountSql, new { menu.Id }, transaction, cancellationToken: cancellationToken));

            if (reservationCount > 0)
                throw new BusinessRuleException("Rezervasyonu olan bir menünün yemekhanesi, tarihi ya da öğünü değiştirilemez.");
        }

        try
        {
            await connection.ExecuteAsync(new CommandDefinition(
                updateSql, menu, transaction, cancellationToken: cancellationToken));

            await InsertFoodsAsync(connection, transaction, menu.Id, menu.FoodIds, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (MySqlException exception) when (IsKnownWriteError(exception))
        {
            throw ToBusinessRuleException(exception);
        }
    }

    private static Task InsertFoodsAsync(
        MySqlConnection connection,
        IDbTransaction transaction,
        int menuId,
        IEnumerable<int> foodIds,
        CancellationToken cancellationToken) =>
        connection.ExecuteAsync(new CommandDefinition(
            "INSERT INTO menu_yemekleri (menu_id, yemek_id) VALUES (@MenuId, @FoodId)",
            foodIds.Select(foodId => new { MenuId = menuId, FoodId = foodId }),
            transaction,
            cancellationToken: cancellationToken));

    private static bool IsKnownWriteError(MySqlException exception) =>
        exception.ErrorCode is MySqlErrorCode.DuplicateKeyEntry or MySqlErrorCode.NoReferencedRow2;

    private static BusinessRuleException ToBusinessRuleException(MySqlException exception) =>
        exception.ErrorCode == MySqlErrorCode.DuplicateKeyEntry
            ? new BusinessRuleException("Bu yemekhanede seçilen gün ve öğün için zaten bir menü var.")
            : new BusinessRuleException("Seçilen yemekhane, öğün ya da yemek bulunamadı.");

    private static List<string> SplitNames(string? value) =>
        string.IsNullOrEmpty(value)
            ? []
            : value.Split(", ").ToList();
}
