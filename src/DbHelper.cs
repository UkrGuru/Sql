// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.Sql;

/// <summary>
/// Provides helper methods for executing SQL commands using a shared static connection string.
/// </summary>
public partial class DbHelper
{
    private static string? _connectionString;

    /// <summary>
    /// Sets the connection string used by all static SQL operations.
    /// </summary>
    public static string ConnectionString
    {
        set => _connectionString = value;
    }

    /// <summary>
    /// Creates and opens a new <see cref="SqlConnection"/> using the configured connection string.
    /// </summary>
    public static SqlConnection CreateConnection(string? connectionString = default)
    {
        var connection = new SqlConnection(connectionString ?? _connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Creates and opens a new SQL connection asynchronously.
    /// </summary>
    public static async Task<SqlConnection> CreateConnectionAsync(string? connectionString = default, CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(connectionString ?? _connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }

    /// <summary>
    /// Creates and opens a <see cref="SqlCommand"/> asynchronously using the configured connection string.
    /// The returned command is associated with a connection that will be disposed when the command is disposed.
    /// </summary>
    public static async Task<SqlCommand> CreateCommandAsync(
        string tsql, object? data = default, string? connectionString = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        var connection = await CreateConnectionAsync(connectionString, cancellationToken).ConfigureAwait(false);
        var cmd = connection.CreateCommand(tsql, data, timeout);

        cmd.Disposed += (_, __) => connection.Dispose();

        return cmd;
    }

    /// <summary>
    /// Executes a SQL command and returns the number of affected rows.
    /// </summary>
    public static int Exec(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();
        return connection.Exec(tsql, data, timeout);
    }

    /// <summary>
    /// Executes a SQL query and returns a single typed result.
    /// </summary>
    public static T? Exec<T>(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();
        return connection.Exec<T?>(tsql, data, timeout);
    }

    /// <summary>
    /// Executes a SQL query and returns an enumerable sequence of typed results.
    /// </summary>
    public static IEnumerable<T> Read<T>(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();
        return connection.Read<T>(tsql, data, timeout);
    }
}