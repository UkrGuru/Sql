// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;

namespace UkrGuru.Sql;

/// <summary>
/// Represents a utility class for handling SQL parameters.
/// </summary>
public class Params
{
    /// <summary>
    /// Parses dynamic properties into an array of SqlParameters.
    /// </summary>
    /// <param name="props">The dynamic properties to parse.</param>
    /// <returns>An array of SqlParameters.</returns>
    public static SqlParameter[] Parse(dynamic props) =>
        (from prop in ((object)props).GetType().GetProperties()
         select new SqlParameter(prop.Name, prop.GetValue(props))).ToArray();
}
