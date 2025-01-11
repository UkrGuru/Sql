// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;

namespace UkrGuru.Sql;

public class DbHelper
{
    private static string? _connectionString;

    public static string ConnectionString { set => _connectionString = value; }

    public static SqlConnection CreateConnection() => new(_connectionString);

    public static int Exec(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();

        return connection.Exec(tsql, data, timeout);
    }

    public static T? Exec<T>(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();

        return connection.Exec<T?>(tsql, data, timeout);
    }

    public static IEnumerable<T> Read<T>(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();

        return connection.Read<T>(tsql, data, timeout);
    }

    public static async Task<int> ExecAsync(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        return await connection.ExecAsync(tsql, data, timeout, cancellationToken);
    }

    public static async Task<T?> ExecAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        return await connection.ExecAsync<T?>(tsql, data, timeout, cancellationToken);
    }

    public static async Task<IEnumerable<T>> ReadAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        return await connection.ReadAsync<T>(tsql, data, timeout, cancellationToken);
    }
}   