// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;

namespace UkrGuru.Sql;

public static class Extens
{
    public static void AddData(this SqlParameterCollection parameters, object? data)
    {
        switch (data)
        {
            case null:
                break;

            case SqlParameter sqlParameter:
                parameters.Add(sqlParameter);
                break;

            case SqlParameter[] sqlParameters:
                parameters.AddRange(sqlParameters);
                break;

            default:
                {
                    SqlParameter parameter = new("@Data", data);
                    if (parameter.SqlValue is null && data is not Enum && data is not Stream && data is not TextReader && data is not XmlReader)
                    {
                        parameters.AddRange((from prop in data.GetType().GetProperties()
                                             select new SqlParameter("@" + prop.Name, prop.GetValue(data) ?? DBNull.Value)).ToArray());
                    }
                    else
                    {
                        parameters.Add(parameter);
                    }
                    break;
                }
        }
    }

    public static SqlCommand CreateCommand(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        SqlCommand command = new(tsql, connection);

        if (IsName(tsql)) command.CommandType = CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddData(data);

        if (timeout.HasValue) command.CommandTimeout = timeout.Value;

        return command;

        static bool IsName(string? tsql) => tsql is not null && tsql.Length <= 100 &&
            Regex.IsMatch(tsql, @"^([a-zA-Z_]\w*|\[.+?\])(\.([a-zA-Z_]\w*|\[.+?\]))?$");
    }

    public static int Exec(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        if (connection.State == ConnectionState.Closed) connection.Open();

        using var command = connection.CreateCommand(tsql, data, timeout);

        return command.ExecuteNonQuery();
    }

    public static T? Exec<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        if (connection.State == ConnectionState.Closed) connection.Open();

        using var command = connection.CreateCommand(tsql, data, timeout);

        return Results.Parse<T?>(command.ExecuteScalar());

        //if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>))
        //    return (T?)(object?)command.Read<T>().ToList();
    }

    public static IEnumerable<T> Read<T>(this SqlCommand command)
    {
        Results? result = null;

        using SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);

        while (reader.Read())
        {
            result ??= reader.Init<T>();

            yield return reader.Parse<T>(result);
        }
    }

    public static IEnumerable<T> Read<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        if (connection.State == ConnectionState.Closed) connection.Open();

        using var command = connection.CreateCommand(tsql, data, timeout);

        return command.Read<T>().ToList();
    }

    public static async Task<int> ExecAsync(this SqlConnection connection,
        string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand(tsql, data, timeout);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public static async Task<T?> ExecAsync<T>(this SqlConnection connection,
        string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand(tsql, data, timeout);
        return Results.Parse<T?>(await command.ExecuteScalarAsync(cancellationToken));
    }

    public static async IAsyncEnumerable<T> ReadAsync<T>(this SqlCommand command, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Results? result = null;

        using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result ??= reader.Init<T>();
            yield return reader.Parse<T>(result);
        }
    }

    public static async Task<IEnumerable<T>> ReadAsync<T>(this SqlConnection connection,
        string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand(tsql, data, timeout);

        var items = new List<T>();

        await foreach (var item in command.ReadAsync<T>(cancellationToken))
        {
            items.Add(item);
        }

        return items;
    }

    public static Results Init<T>(this SqlDataReader reader)
    {
        Results result = new();

        result.Props = typeof(T).GetProperties();

        int fieldCount = reader.FieldCount;
        var names = new string[fieldCount];

        result.Indexes = new int[fieldCount];
        result.Values = new object[fieldCount];

        for (int i = 0; i < fieldCount; i++)
        {
            names[i] = reader.GetName(i);
            result.Indexes[i] = Array.FindIndex(result.Props, p => p.Name == names[i] && p.CanWrite);
        }

        return result;
    }

    public static T Parse<T>(this SqlDataReader reader, Results results)
    {
        T item = (T)Activator.CreateInstance(typeof(T))!;

        int fieldCount = reader.GetValues(results.Values);

        for (int i = 0; i < fieldCount; i++)
        {
            if (results.Indexes[i] >= 0)
            {
                results.Props[results.Indexes[i]].SetValue(item, Results.Parse(results.Values[i]));
            }
        }

        return item;
    }

    public static string ToJson(this object value) => JsonSerializer.Serialize(value);
}