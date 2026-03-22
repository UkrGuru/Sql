// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace UkrGuru.Sql;

public partial class DbHelper
{
    private static string? _connectionString;

    public static string ConnectionString
    {
        set => _connectionString = value;
    }

    public static SqlConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

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
}