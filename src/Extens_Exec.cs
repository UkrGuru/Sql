// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.Sql;

public static partial class Extens
{
    public static int Exec(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        using var command = connection.CreateCommand(tsql, data, timeout);

        return command.ExecuteNonQuery();
    }

    public static T? Exec<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        using var command = connection.CreateCommand(tsql, data, timeout);

        return Results.Parse<T?>(command.ExecuteScalar());
    }

    public static async Task<int> ExecAsync(this SqlConnection connection,
        string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand(tsql, data, timeout);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public static async Task<T?> ExecAsync<T>(this SqlConnection connection,
        string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand(tsql, data, timeout);

        return Results.Parse<T?>(await command.ExecuteScalarAsync(cancellationToken));
    }
}