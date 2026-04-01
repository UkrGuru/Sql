// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.Sql;

/// <summary>
/// Defines asynchronous database operations for executing queries and commands.
/// </summary>
public interface IDbService
{
    /// <summary>
    /// Executes a SQL command asynchronously and returns the number of affected rows.
    /// </summary>
    Task<int> ExecAsync(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a SQL query asynchronously and returns a single typed result.
    /// </summary>
    Task<T?> ExecAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a SQL query asynchronously and returns a sequence of typed results.
    /// </summary>
    Task<IEnumerable<T>> ReadAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of <see cref="IDbService"/> providing asynchronous database access.
/// </summary>
/// <param name="connectionString">The SQL Server connection string.</param>
public class DbService(string connectionString) : IDbService
{
    private readonly string _connectionString = connectionString;

    /// <inheritdoc />
    private async Task<SqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }

    /// <inheritdoc />
    public async Task<int> ExecAsync(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        return await connection.ExecAsync(tsql, data, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<T?> ExecAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        return await connection.ExecAsync<T?>(tsql, data, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> ReadAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        return await connection.ReadAsync<T>(tsql, data, timeout, cancellationToken).ConfigureAwait(false);
    }
}
