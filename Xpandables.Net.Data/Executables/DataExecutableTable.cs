
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    ///  Executes a stored procedure or query and return a data table.
    /// </summary>
    public sealed class DataExecutableTable : DataExecutable<DataTable>
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public override async Task<DataTable> ExecuteAsync(DataExecutableContext context, CancellationToken cancellationToken = default)
        {
            using var dataSet = new DataSet();
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
                await context.Component.Command.PrepareAsync(cancellationToken).ConfigureAwait(false);
            }

            context.Component.Adapter.SelectCommand = context.Component.Command;
            context.Component.Adapter.AcceptChangesDuringFill = false;
            context.Component.Adapter.FillLoadOption = LoadOption.OverwriteChanges;
            context.Component.Adapter.Fill(dataSet);

            var result = dataSet.Tables[0];

            if (context.Argument.Options.IsTransactionEnabled)
                await context.Component.Command.Transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            return await Task.FromResult(result).ConfigureAwait(false);
        }
    }
}
