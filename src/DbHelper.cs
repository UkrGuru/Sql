// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Collections.Generic;

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
    /// <returns>An opened <see cref="SqlConnection"/> instance.</returns>
    public static SqlConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Executes a SQL command and returns the number of affected rows.
    /// </summary>
    /// <param name="tsql">The SQL command text.</param>
    /// <param name="data">The parameters object or anonymous object.</param>
    /// <param name="timeout">Optional command timeout.</param>
    /// <returns>Number of rows affected.</returns>
    public static int Exec(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();
        return connection.Exec(tsql, data, timeout);
    }

    /// <summary>
    /// Executes a SQL query and returns a single typed result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="tsql">The SQL command text.</param>
    /// <param name="data">The parameters object or anonymous object.</param>
    /// <param name="timeout">Optional command timeout.</param>
    /// <returns>A single result of type <typeparamref name="T"/>.</returns>
    public static T? Exec<T>(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();
        return connection.Exec<T?>(tsql, data, timeout);
    }

    /// <summary>
    /// Executes a SQL query and returns an enumerable sequence of typed results.
    /// </summary>
    /// <typeparam name="T">The type of the mapped result items.</typeparam>
    /// <param name="tsql">The SQL query text.</param>
    /// <param name="data">The parameters object or anonymous object.</param>
    /// <param name="timeout">Optional command timeout.</param>
    /// <returns>An enumerable collection of results.</returns>
    public static IEnumerable<T> Read<T>(string tsql, object? data = default, int? timeout = default)
    {
        using var connection = CreateConnection();
        return connection.Read<T>(tsql, data, timeout);
    }
}