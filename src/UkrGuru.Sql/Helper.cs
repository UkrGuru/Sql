// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;

namespace UkrGuru.Sql;

public class Helper
{
    /// <summary>
    /// The connection used to open the SQL Server database.
    /// </summary>
    private static string? _connectionString;

    /// <summary>
    /// Specifies the string used to open a SQL Server database.
    /// </summary>
    public static string ConnectionString { set => _connectionString = value; }

    /// <summary>
    /// Initializes a new instance of the SqlConnection class.
    /// </summary>
    /// <returns>New instance of the SqlConnection class with initialize parameters</returns>
    public static SqlConnection CreateConnection() => new(_connectionString);

    public static int Exec(string tsql, object? data = default, int? timeout = default)
    {
        using var cnn = CreateConnection();
        cnn.Open();

        return cnn.Exec(tsql, data, timeout);
    }

    public static T? Exec<T>(string tsql, object? data = default, int? timeout = default)
    {
        using var cnn = CreateConnection();
        cnn.Open();

        return cnn.Exec<T?>(tsql, data, timeout);
    }

    public static IEnumerable<T?> Read<T>(string tsql, object? data = default, int? timeout = default)
    {
        using var cnn = CreateConnection();
        cnn.Open();

        return cnn.Read<T?>(tsql, data, timeout);
    }
}

//public static async Task<int> ExecAsync(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
//{
//    using var cnn = CreateConnection();
//    await cnn.OpenAsync(cancellationToken);

//    return await cnn.ExecAsync(tsql, data, timeout, cancellationToken);
//}

//public static async Task<T?> ExecAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
//{
//    using var cnn = CreateConnection();
//    await cnn.OpenAsync(cancellationToken);

//    return await cnn.ExecAsync<T?>(tsql, data, timeout, cancellationToken);
//}

//public static async Task<IEnumerable<T?>> ReadAsync<T>(string tsql, object? data = default, int? timeout = default)
//{
//    using var cnn = CreateConnection();

//    return await cnn.ReadAsync<T?>(tsql, data, timeout);
//}

///// <summary>
///// 
///// </summary>
///// <typeparam name="T"></typeparam>
///// <param name="tsql"></param>
///// <param name="data"></param>
///// <param name="timeout"></param>
///// <returns></returns>
//public static T? Create<T>(string tsql, object? data = default, int? timeout = default)
//{
//    return default;
//}

///// <summary>
///// 
///// </summary>
///// <typeparam name="T"></typeparam>
///// <param name="tsql"></param>
///// <param name="data"></param>
///// <param name="timeout"></param>
///// <returns></returns>
//public static IEnumerable<T> Read<T>(string tsql, object? data = default, int? timeout = default)
//{
//    return Array.Empty<T>();
//}

///// <summary>
///// 
///// </summary>
///// <typeparam name="T"></typeparam>
///// <param name="tsql"></param>
///// <param name="data"></param>
///// <param name="timeout"></param>
///// <returns></returns>
//public static T? Update<T>(string tsql, object? data = default, int? timeout = default)
//{
//    return default;
//}

///// <summary>
///// 
///// </summary>
///// <param name="tsql"></param>
///// <param name="data"></param>
///// <param name="timeout"></param>
///// <returns></returns>
//public static int Delete(string tsql, object? data = default, int? timeout = default)
//{
//    return 0;
//}
