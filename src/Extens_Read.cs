// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.Sql;

/// <summary>
/// Provides helpers for reading SQL results.
/// </summary>
public static partial class Extens
{
    /// <summary>
    /// Reads items from a SQL data reader into typed objects.
    /// </summary>
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

    /// <summary>
    /// Executes a SQL query and reads typed items.
    /// </summary>
    public static IEnumerable<T> Read<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        using var command = connection.CreateCommand(tsql, data, timeout);
        return [.. command.Read<T>()];
    }

    /// <summary>
    /// Reads items asynchronously from a SQL data reader.
    /// </summary>
    public static async IAsyncEnumerable<T> ReadAsync<T>(
        this SqlCommand command,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Results? result = null;
        await using SqlDataReader reader =
            await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result ??= reader.Init<T>();
            yield return reader.Parse<T>(result);
        }
    }

    /// <summary>
    /// Executes a SQL query asynchronously and returns typed items.
    /// </summary>
    public static async Task<IEnumerable<T>> ReadAsync<T>(
        this SqlConnection connection,
        string tsql,
        object? data = default,
        int? timeout = default,
        CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand(tsql, data, timeout);
        List<T> items = [];
        await foreach (var item in command.ReadAsync<T>(cancellationToken))
            items.Add(item);
        return items;
    }

    /// <summary>
    /// Initializes mapping metadata for the reader.
    /// </summary>
    public static Results Init<T>(this SqlDataReader reader)
    {
        Results result = new() { Props = typeof(T).GetProperties() };

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

    /// <summary>
    /// Parses a record into a typed object.
    /// </summary>
    public static T Parse<T>(this SqlDataReader reader, Results results)
    {
        T item = Activator.CreateInstance<T>();
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
}