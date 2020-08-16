
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
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using Xpandables.Net.Data.Elements;
using Xpandables.Net.Data.Mappers;
using Xpandables.Net.Extensions;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    ///  Executes a stored procedure or query and return the result mapped to specific type.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    public sealed class DataExecutableMapper<TResult> : IDataExecutableMapper<TResult>
         where TResult : class, new()
    {
        private readonly IDataMapper _dataMapper;

        /// <summary>
        /// Initializes a new instance of <see cref="DataExecutableMapper{T}"/>.
        /// </summary>
        /// <param name="dataMapper">the mapper to be used.</param>
        public DataExecutableMapper(IDataMapper dataMapper) => _dataMapper = dataMapper ?? throw new ArgumentNullException(nameof(dataMapper));

        /// <summary>
        /// Asynchronously executes an action to the database and returns the result mapped to the specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An asynchronous enumeration of <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public async IAsyncEnumerable<TResult> ExecuteMappedAsync(
            DataExecutableContext context, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            context.Component.Command.CommandText =
                context.Component.Command.CommandType == CommandType.StoredProcedure
                ? context.Argument.CommandText
                : context.Argument.CommandText.ParseSql();

            DataParameterBuilder.Build(context.Component.Command, context.Argument.Parameters?.ToArray());

            if (context.Component.Command.CommandType == CommandType.StoredProcedure)
            {
                context.Component.Command.CommandTimeout = 0;
                context.Component.Command.CommandText = context.Argument.CommandText.Split('@')[0].Trim();
            }

            if (context.Component.Command.CommandType == CommandType.Text
                && context.Argument.Parameters?.All(p => p is DbParameter) == true
                && context.Component.Command.Connection.IsSqlConnection())
            {
                await context.Component.Command.PrepareAsync(context.Argument.Options.CancellationToken).ConfigureAwait(false);
            }

            using (var reader = await context.Component.Command.ExecuteReaderAsync(context.Argument.Options.CancellationToken).ConfigureAwait(false))
            {
                await _dataMapper.MapAsync<TResult>(GetRecordsAsync(), context.Argument.Options).ConfigureAwait(false);
                async IAsyncEnumerable<IDataRecord> GetRecordsAsync()
                {
                    if (reader.HasRows)
                        while (await reader.ReadAsync(context.Argument.Options.CancellationToken).ConfigureAwait(false))
                            yield return reader;
                }
            }

            await foreach (var keyValue in _dataMapper.Entities)
                yield return (TResult)keyValue.Value.Entity!;
        }
    }
}
