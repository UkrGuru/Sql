// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;

namespace UkrGuru.Sql;

/// <summary>
/// Provides helpers for adding parameter data to SQL commands.
/// </summary>
public static partial class Extens
{
    /// <summary>
    /// Adds parameter data to the collection based on the provided object.
    /// </summary>
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
                SqlParameter parameter = new("@Data", data);

                if (parameter.SqlValue is null &&
                    data is not Enum &&
                    data is not Stream &&
                    data is not TextReader &&
                    data is not XmlReader)
                {
                    parameters.AddRange(
                        [.. (from prop in data.GetType().GetProperties()
                         select new SqlParameter("@" + prop.Name,
                             prop.GetValue(data) ?? DBNull.Value))]);
                }
                else
                {
                    parameters.Add(parameter);
                }
                break;
        }
    }

    /// <summary>
    /// Serializes an object to JSON.
    /// </summary>
    public static string ToJson(this object value, JsonSerializerOptions? options = default)
        => JsonSerializer.Serialize(value, options);
}