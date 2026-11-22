using Dapper;
using MySqlConnector;
using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Infrastructure.Data;

internal static class DbConnectionExtensions
{
    private const string BusinessRuleSqlState = "45000";

    public static async Task<T> CallProcedureAsync<T>(
        this MySqlConnection connection,
        string sql,
        object parameters,
        CancellationToken cancellationToken)
    {
        try
        {
            return await connection.QuerySingleAsync<T>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }
        catch (MySqlException exception) when (exception.SqlState == BusinessRuleSqlState)
        {
            throw new BusinessRuleException(exception.Message);
        }
    }
}
