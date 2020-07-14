
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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Xpandables.Net5.Data.Executables
{
    /// <summary>
    /// Executes a stored procedure and return number or records affected.
    /// </summary>
    public sealed class DataExecutableProcedure : DataExecutable<int>
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="component">The target component instance.</param>
        /// <param name="argument">The target argument instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="component" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="argument" /> is null.</exception>
        public override async Task<int> ExecuteAsync(DataComponent component, DataArgument argument)
        {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            component.Command.CommandText = component.CommandType == CommandType.StoredProcedure ? argument.Command : argument.Command.ParseSql();
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            component.Command.CommandType = component.CommandType;
            DataParameterBuilder.Build(component.Command, argument.Parameters?.ToArray());

#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            component.Command.CommandText = argument.Command.Split('@')[0].Trim();
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            component.Command.CommandTimeout = 0;

            var returnValParameter = component.Command.CreateParameter();
            returnValParameter.Direction = ParameterDirection.ReturnValue;
            component.Command.Parameters.Add(returnValParameter);

            if (component.Command.Connection.IsSqlConnection() && argument.Parameters?.All(p => p is DbParameter) == true)
                component.Command.Prepare();

            int result;
            if (argument.Options.IsIdentityRetrieved)
            {
                component.Command.CommandText += "; SELECT CAST(SCOPE_IDENTITY() AS int);";
                result = (int)await component.Command.ExecuteScalarAsync(argument.Options.CancellationToken).ConfigureAwait(false);
            }
            else
            {
                _ = await component.Command.ExecuteNonQueryAsync(argument.Options.CancellationToken).ConfigureAwait(false);
                result = int.Parse(returnValParameter.Value.ToString() ?? "0", CultureInfo.InvariantCulture);
            }

            if (argument.Options.IsTransactionEnabled)
                component.Command.Transaction.Commit();

            return result;
        }
    }
}
