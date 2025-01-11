// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace UkrGuru.Sql;

public interface IDbService
{
    Task<int> ExecAsync(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);

    Task<T?> ExecAsync<T>(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> ReadAsync<T>(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default);
}

public class DbService : IDbService
{
    public virtual string ConnectionStringName => "DefaultConnection";

    private readonly string? _connectionString;

    public DbService(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString(ConnectionStringName);

    public SqlConnection CreateConnection() => new(_connectionString);

    public async Task<int> ExecAsync(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        return await connection.ExecAsync(tsql, data, timeout, cancellationToken);
    }

    public async Task<T?> ExecAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        return await connection.ExecAsync<T?>(tsql, data, timeout, cancellationToken);
    }

    public async Task<IEnumerable<T>> ReadAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        return await connection.ReadAsync<T>(tsql, data, timeout, cancellationToken);
    }
}