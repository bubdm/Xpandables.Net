
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
using System.Threading.Tasks;

namespace System.Design.DataSource.SQL.Executables
{
    /// <summary>
    /// Executes a transaction and return number or records affected.
    /// </summary>
    public sealed class DataExecutableTransaction : DataExecutable<int>
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
            component.Command.CommandText = argument.Command.ParseSql();
            DataParameterBuilder.Build(component.Command, argument.Parameters);

            int result;
            if (argument.Options.IsIdentityRetrieved)
            {
                component.Command.CommandText += "; SELECT CAST(SCOPE_IDENTITY() AS int)";
                result = (int)await component.Command.ExecuteScalarAsync(argument.Options.CancellationToken).ConfigureAwait(false);
            }
            else
            {
                result = await component.Command.ExecuteNonQueryAsync(argument.Options.CancellationToken).ConfigureAwait(false);
            }

            if (argument.Options.IsTransactionEnabled)
                component.Command.Transaction?.Commit();

            return result;
        }
    }
}
