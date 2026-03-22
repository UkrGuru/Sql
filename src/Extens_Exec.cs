// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.Sql;

/// <summary>
/// Provides execution helpers for SQL commands.
/// </summary>
public static partial class Extens
{
    /// <summary>
    /// Executes a SQL command and returns the number of affected rows.
    /// </summary>
    public static int Exec(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        using var command = connection.CreateCommand(tsql, data, timeout);
        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a SQL scalar query and returns a typed result.
    /// </summary>
    public static T? Exec<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        using var command = connection.CreateCommand(tsql, data, timeout);
        return Results.Parse<T?>(command.ExecuteScalar());
    }

    /// <summary>
    /// Executes a SQL command asynchronously.
    /// </summary>
    public static async Task<int> ExecAsync(
        this SqlConnection connection,
        string tsql,
        object? data = default,
        int? timeout = default,
        CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand(tsql, data, timeout);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Executes a SQL scalar query asynchronously.
    /// </summary>
    public static async Task<T?> ExecAsync<T>(
        this SqlConnection connection,
        string tsql,
        object? data = default,
        int? timeout = default,
        CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand(tsql, data, timeout);
        return Results.Parse<T?>(await command.ExecuteScalarAsync(cancellationToken));
    }
}