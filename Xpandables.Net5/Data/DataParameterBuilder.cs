
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
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace Xpandables.Net5.Data
{
    /// <summary>
    /// Defines a method to add parameters to a command.
    /// </summary>
    public sealed class DataParameterBuilder
    {
        /// <summary>
        /// Adds the specified parameters to the current command instance.
        /// </summary>
        /// <param name="command">The command to act on.</param>
        /// <param name="parameters">The parameters to be added.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="parameters"/> can not be a mix of DbParameters and values.</exception>
        public static void Build(DbCommand command, params object[]? parameters)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            if (parameters is null || parameters.Length == 0)
                return;

            var dbParameters = new DbParameter[parameters.Length];
            if (parameters.All(p => p is DbParameter))
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    dbParameters[i] = (DbParameter)parameters[i];
                }
            }
            else
            if (!parameters.Any(p => p is DbParameter))
            {
                var commandSplit = command.CommandText.Split('@').ToList();
                var parameterSourceNames = Array.Empty<string>();
                if (commandSplit.Count > 1)
                {
                    commandSplit.RemoveAt(0);
                    parameterSourceNames = commandSplit.Select(param => param.Split(' ')[0].Replace(",", "").RemoveExtraChars().Trim()).Distinct().ToArray();
                    if (parameterSourceNames.Length != parameters.Length)
                        throw new ArgumentException($"Arguments provided ({parameters.Length}) must match in number expected ({parameterSourceNames.Length}).");
                }

                var parameterNames = new string[parameters.Length];
                var parameterSql = new string[parameters.Length];

                // We check for the friendly representation of the parameter.
                // We only manage SQL Server and Oracle.
                var friendlyRepresentation = command.Connection.GetType().Name.Contains("SqlConnection", StringComparison.OrdinalIgnoreCase) ? "@" : ":";

                for (var i = 0; i < parameters.Length; i++)
                {
                    parameterNames[i] = string.Format(CultureInfo.InvariantCulture, parameterSourceNames?[i] ?? "p{0}", i);
                    dbParameters[i] = command.CreateParameter();
                    dbParameters[i].ParameterName = parameterNames[i];
                    dbParameters[i].Value = parameters[i] ?? DBNull.Value;

                    parameterSql[i] = friendlyRepresentation + parameterNames[i];
                }

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                command.CommandText = string.Format(CultureInfo.InvariantCulture, command.CommandText, parameterSql);
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            }
            else
            {
                throw new ArgumentException("Arguments provided can not be a mix of DbParameters and values.");
            }

            command.Parameters.AddRange(dbParameters);
        }
    }

    /// <summary>
    /// Helper for builder.
    /// </summary>
    public static class DataParameterBuilderExtensions
    {
        /// <summary>
        /// Remove extra characters from the specified string.
        /// </summary>
        /// <param name="source">The string to act on.</param>
        public static string RemoveExtraChars(this string source)
            => new string(
                source
                .Where(c =>
                    char.IsLetterOrDigit(c)
                    || c == '_')
                .ToArray());
    }
}
