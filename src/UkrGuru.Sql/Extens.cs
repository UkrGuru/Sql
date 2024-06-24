// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace UkrGuru.Sql;

public static class Extens
{
    public static bool IsName(string? tsql) => tsql is not null && tsql.Length <= 100
        && Regex.IsMatch(tsql, @"^([a-zA-Z_]\w*|\[.+?\])(\.([a-zA-Z_]\w*|\[.+?\]))?$");

    public static SqlCommand CreateCommand(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        SqlCommand command = new(tsql, connection);

        if (IsName(tsql)) command.CommandType = CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddData(data);

        if (timeout.HasValue) command.CommandTimeout = timeout.Value;

        return command;
    }

    public static SqlParameter AddData(this SqlParameterCollection parameters, object data) =>
        data is SqlParameter sqlParameter ? parameters.Add(sqlParameter) :
        data is SqlParameter[] sqlParameters ? parameters.AddParams(sqlParameters) :
        data.GetType().IsAnonymous() ? parameters.AddParams(Params.Parse(data)) :
        parameters.AddData(data, "@Data");

    public static SqlParameter AddData(this SqlParameterCollection parameters, object data, string name)
    {
        SqlParameter parameter = new(name, data);
        if (parameter.SqlValue is null && data is not Enum && data is not Stream && data is not TextReader && data is not XmlReader)
        {
            parameter.SqlValue = JsonSerializer.Serialize(data);
        }
        parameters.Add(parameter);
        return parameter;
    }

    public static SqlParameter AddParams(this SqlParameterCollection parameters, SqlParameter[] values)
    {
        parameters.AddRange(values);
        return values[0];
    }

    public static T? Exec<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        if (connection.State == ConnectionState.Closed) connection.Open();

        using var command = connection.CreateCommand(tsql, data, timeout);

        return Result.Parse<T?>(command.ExecuteScalar());
    }

    public static IEnumerable<T?> Read<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        if (connection.State == ConnectionState.Closed) connection.Open();

        using var command = connection.CreateCommand(tsql, data, timeout);

        return command.Read<T>().ToList();
    }

    public static IEnumerable<T?> Read<T>(this SqlCommand command)
    {
        Result? result = null;

        using SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess);

        while (reader.Read())
        {
            result ??= reader.Init<T>();

            yield return reader.Parse<T>(result);
        }
    }

    public static async Task<T?> ExecAsync<T>(this SqlConnection connection, 
        string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand(tsql, data, timeout);

        return Result.Parse<T?>(await command.ExecuteScalarAsync(cancellationToken));
    }

    public static async Task<IEnumerable<T?>> ReadAsync<T>(this SqlConnection connection,
        string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand(tsql, data, timeout);

        var items = new List<T?>();

        await foreach (var item in ReadAsync<T>(command, cancellationToken))
        {
            items.Add(item);
        }

        return items;
    }

    public static async IAsyncEnumerable<T?> ReadAsync<T>(this SqlCommand command, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Result? result = null;

        using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result ??= reader.Init<T>();
            yield return reader.Parse<T>(result);
        }
    }

    //public static async Task<IEnumerable<T?>> Read<T>(this SqlConnection connection, 
    //    string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    //{
    //    if (connection.State == ConnectionState.Closed) await connection.OpenAsync();

    //    using var command = connection.CreateCommand(tsql, data, timeout);

    //    return (await command.ReadAsync<T>()).ToL;
    //}

    //public static async Task<IEnumerator<T?>> ReadAsync<T>(this SqlCommand command)
    //{
    //    Result? result = null;

    //    using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

    //    while (await reader.ReadAsync())
    //    {
    //        result ??= reader.Init<T>();

    //        yield return reader.Parse<T>(result);
    //    }
    //}

    /// <summary>
    /// Initializes a SqlCross object based on the SqlDataReader.
    /// </summary>
    /// <typeparam name="T">The type of the mapped object.</typeparam>
    /// <param name="reader">The SqlDataReader containing the query result.</param>
    /// <returns>A SqlCross object initialized with column information from the SqlDataReader.</returns>
    public static Result Init<T>(this SqlDataReader reader)
    {
        Result result = new();

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

    /// <summary>
    /// Retrieves an object of type T from the SqlDataReader using the provided SqlCross object.
    /// </summary>
    /// <typeparam name="T">The type of the mapped object.</typeparam>
    /// <param name="reader">The SqlDataReader containing the query result.</param>
    /// <param name="result">The SqlCross object containing column information.</param>
    /// <returns>An object of type T with properties populated from the SqlDataReader.</returns>
    public static T Parse<T>(this SqlDataReader reader, Result result)
    {
        T item = (T)Activator.CreateInstance(typeof(T))!;

        int fieldCount = reader.GetValues(result.Values);

        for (int i = 0; i < fieldCount; i++)
        {
            if (result.Indexes[i] >= 0)
            {
                result.Props[result.Indexes[i]].SetValue(item, Result.Parse(result.Values[i]));
            }
        }

        return item;
    }

    public static bool IsAnonymous(this Type type)
    {
        return type.Name.Contains("AnonymousType");
    }
}


//public static T? Exec<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default,
//    CommandBehavior behavior = CommandBehavior.SingleResult)
//{
//    using var command = connection.CreateSqlCommand(tsql, data, timeout);

//    switch (behavior)
//    {
//        case CommandBehavior.Default:
//            return SqlResult.Parse<T?>(command.ExecuteNonQuery());

//        case CommandBehavior.SingleResult:
//            return SqlResult.Parse<T?>(command.ExecuteScalar());

//        case CommandBehavior.SingleRow:
//            return command.Read<T>().FirstOrDefault();

//        case CommandBehavior.SequentialAccess:
//        case CommandBehavior.CloseConnection:
//            using (SqlDataReader reader = command.ExecuteReader(behavior))
//            {
//                return (T?)reader.ReadAll<T>();
//            }

//        default:
//            return default;
//    }
//}

//public static object? ReadAll<T>(this SqlDataReader reader)
//    => reader.Read() && !reader.IsDBNull(0) ? reader.ReadObj<T>() : default(T);

//public static object? ReadObj<T>(this SqlDataReader reader) => typeof(T).Name switch
//{
//    "Byte[]" => reader[0],
//    "Char[]" => reader.GetSqlChars(0).Value,
//    nameof(Stream) => reader.GetStream(0),
//    nameof(SqlBinary) => reader.GetSqlBinary(0),
//    nameof(SqlBytes) => reader.GetSqlBytes(0),
//    nameof(SqlXml) => reader.GetSqlXml(0),
//    nameof(SqlChars) => reader.GetSqlChars(0),
//    nameof(TextReader) => reader.GetTextReader(0),
//    nameof(XmlReader) => reader.GetXmlReader(0),
//    _ => JsonSerializer.Deserialize<T?>(reader.ReadMore()),
//};

//public static string ReadMore(this SqlDataReader reader)
//{
//    StringBuilder sb = new();

//    do { sb.Append(reader.GetValue(0)); } while (reader.Read());

//    return sb.ToString();
//}

//public static Task<int> ExecAsync(this SqlConnection connection, string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
//{
//    using var command = connection.CreateCommand(tsql, data, timeout);

//    return command.ExecuteNonQueryAsync(cancellationToken);
//}

//public static Task<T?> ExecAsync<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
//{
//    using var command = connection.CreateCommand(tsql, data, timeout);

//    return Task.FromResult(Result.Parse<T?>(command.ExecuteScalarAsync(cancellationToken)));
//}

//public static Task<IEnumerable<T?>> ReadAsync<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
//{
//    using var command = connection.CreateCommand(tsql, data, timeout);

//    return Task.FromResult(command.ReadAsync<T?>());
//}

