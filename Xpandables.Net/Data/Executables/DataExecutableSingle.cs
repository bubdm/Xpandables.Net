
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
using System.Linq;
using System.Threading.Tasks;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    ///  Executes a stored procedure or query and return a single result.
    /// </summary>
    public sealed class DataExecutableSingle<T> : DataExecutable<T>
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="component">The target component instance.</param>
        /// <param name="argument">The target argument instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="component" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="argument" /> is null.</exception>
        public override async Task<T> ExecuteAsync(DataComponent component, DataArgument argument)
        {
            component.Command.CommandType = component.CommandType;
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            component.Command.CommandText = component.CommandType == CommandType.StoredProcedure ? argument.Command : argument.Command.ParseSql();
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            DataParameterBuilder.Build(component.Command, argument.Parameters?.ToArray());

            var result = await component.Command.ExecuteScalarAsync(argument.Options.CancellationToken).ConfigureAwait(false);

            if (argument.Options.IsTransactionEnabled)
                component.Command.Transaction.Commit();

            return (T)result;
        }
    }
}
