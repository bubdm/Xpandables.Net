
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with extension methods for <see cref="DataBase"/>.
    /// </summary>
    public static partial class DataBaseExtensions
    {
        /// <summary>
        /// Determines whether or not the underlying data reader contains the specified column name.
        /// If so, returns <see langword="true"/> otherwise returns <see langword="false"/>.
        /// </summary>
        /// <param name="source">The source of data reader to act on.</param>
        /// <param name="columName">The column to find.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="columName"/> is null.</exception>
        public static bool Contains(this IDataRecord source, string columName)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(columName)) throw new ArgumentNullException(nameof(columName));

            for (int index = 0; index < source.FieldCount; index++)
            {
                if (source.GetName(index).Equals(columName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether or not the connection is an MSSQL Connection.
        /// </summary>
        /// <param name="connection">The connection to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connection"/> is null.</exception>
        public static bool IsSqlConnection(this IDbConnection connection)
        {
            if (connection is null) throw new ArgumentNullException(nameof(connection));
            return connection.GetType().Name.Contains("SqlConnection", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses query that uses old format.
        /// </summary>
        /// <param name="query">The query to be formatted.</param>
        /// <returns>A parsed query.</returns>
        public static string ParseSql(this string query)
        {
            string[] parts = query.Split('?');
            if (parts.Length > 0)
            {
                var output = new StringBuilder();
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    output.Append(parts[i]);
                    output.Append("@P").Append(i);
                }

                output.Append(parts[^1]);
                query = output.ToString();
            }

            return query;
        }    
    }
}
