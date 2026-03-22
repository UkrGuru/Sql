// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.Sql;

public interface IDbService
{
    Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);

    Task<int> ExecAsync(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);

    Task<T?> ExecAsync<T>(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> ReadAsync<T>(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);
}

public class DbService(string connectionString) : IDbService
{
    private readonly string _connectionString = connectionString;

    public async Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    public async Task<int> ExecAsync(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var connection = await CreateConnectionAsync(cancellationToken);

        return await connection.ExecAsync(tsql, data, timeout, cancellationToken);
    }

    public async Task<T?> ExecAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var connection = await CreateConnectionAsync(cancellationToken);

        return await connection.ExecAsync<T?>(tsql, data, timeout, cancellationToken);
    }

    public async Task<IEnumerable<T>> ReadAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var connection = await CreateConnectionAsync(cancellationToken);

        return await connection.ReadAsync<T>(tsql, data, timeout, cancellationToken);
    }
}