
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
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// Executes a transaction and return number or records affected.
    /// </summary>
    public sealed class DataExecutableTransaction : DataExecutable<int>
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public override async Task<Optional<int>> ExecuteAsync(DataExecutableContext context)
        {
            context.Component.Command.CommandText = context.Argument.CommandText.ParseSql();

            DataParameterBuilder.Build(context.Component.Command, context.Argument.Parameters?.ToArray());

            int result;
            if (context.Argument.Options.IsIdentityRetrieved)
            {
                context.Component.Command.CommandText += "; SELECT CAST(SCOPE_IDENTITY() AS int)";
                var execution = await context.Component.Command.ExecuteScalarAsync(context.Argument.Options.CancellationToken).ConfigureAwait(false);

                result = execution is DBNull || execution is null ? 0 : (int)execution;
            }
            else
            {
                result = await context.Component.Command.ExecuteNonQueryAsync(context.Argument.Options.CancellationToken).ConfigureAwait(false);
            }

            if (context.Argument.Options.IsTransactionEnabled)
                context.Component.Command.Transaction?.Commit();

            return result;
        }
    }
}
